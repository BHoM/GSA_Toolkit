using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMB = BHoM.Base;
using BHoMG = BHoM.Global;
using BHoME = BHoM.Structural.Elements;
using BHoMP = BHoM.Structural.Properties;
using BHoMM = BHoM.Materials;
using GSA_Adapter.Structural.Properties;
using GSA_Adapter.Utility;


namespace GSA_Adapter.Structural.Elements
{
    /// <summary>
    /// GSA bar class, for all bar objects and operations
    /// </summary>
    public class BarIO
    {
        /// <summary>
        /// Get bars method, gets bars from a GSA model and all associated data. 
        /// </summary>
        /// <returns></returns>
        public static bool GetBars(ComAuto gsa, out List<BHoME.Bar> barList, string barNumbers = "all")
        {
            BHoMB.ObjectManager<int, BHoME.Bar> bars = new BHoMB.ObjectManager<int, BHoME.Bar>(BHoMG.Project.ActiveProject, Utils.NUM_KEY, BHoMB.FilterOption.UserData);
            BHoMB.ObjectManager<BHoMP.SectionProperty> sections = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMP.BarRelease> releases = new BHoMB.ObjectManager<BHoMP.BarRelease>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMP.BarConstraint> constraints = new BHoMB.ObjectManager<BHoMP.BarConstraint>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMM.Material> materials = new BHoMB.ObjectManager<BHoMM.Material>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<int, BHoME.Node> nodes = new BHoMB.ObjectManager<int, BHoME.Node>(BHoMG.Project.ActiveProject, Utils.NUM_KEY, BHoMB.FilterOption.UserData);


            sections = PropertyIO.GetSections(gsa); //name as key
                                                    //need to ref by GSA_id too

            int maxIndex = gsa.GwaCommand("HIGHEST, EL");
            int[] potentialBeamRefs = new int[maxIndex];
            for (int i = 0; i < maxIndex; i++)
                potentialBeamRefs[i] = i + 1;

            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            gsa.Elements(potentialBeamRefs, out gsaElements);

            for (int i = 0; i < gsaElements.Length; i++)
            {
                GsaElement gsaBar = gsaElements[i]; //TODO: filter elements based on topology

                GsaNode[] gsaNodes;
                gsa.Nodes(gsaBar.Topo, out gsaNodes);
                BHoME.Node n1 = new BHoME.Node(gsaNodes[0].Coor[0], gsaNodes[0].Coor[1], gsaNodes[0].Coor[2]);
                BHoME.Node  n2 = new BHoME.Node(gsaNodes[1].Coor[0], gsaNodes[1].Coor[1], gsaNodes[1].Coor[2]);

                BHoME.Bar bar = bars.Add(gsaBar.Ref, new BHoME.Bar(n1, n2, gsaBar.Ref.ToString()));
               
                bar.OrientationAngle = gsaBar.Beta;

                //bar.Release

                //bar.SectionProperty = gsaBar.Property;
                //bar.SectionProperty = sections

                //bar.Material

                //TODO: implement property and material setting
            }

            barList = bars.ToList();
            return true;
        }


        /// <summary>
        /// Create GSA bars
        /// </summary>
        /// <returns></returns>
        public static bool CreateBars(ComAuto gsa, List<BHoME.Bar> bars, out List<string> ids)
        {
            ids = new List<string>();

            BHoMB.ObjectManager<BHoMP.SectionProperty> sections = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMM.Material> materials = new BHoMB.ObjectManager<BHoMM.Material>(BHoMG.Project.ActiveProject);

            sections = PropertyIO.GetSections(gsa, true);

            //TODO: Create dictionary of properties and materials - do this at higher level for repeat use
            List<string> secProps = PropertyIO.GetGsaSectionPropertyStrings(gsa);
            List<string> materialList = MaterialIO.GetMaterialStringList(gsa);
            List<string> nodeIds = new List<string>(); 

            foreach (BHoME.Bar bar in bars)
            {
                string command = "EL.2";
                string index = bar.Name;
                string name = bar.Name;
                string type = "BEAM";
                string sectionPropertyIndex = PropertyIO.GetOrCreateGSA_id(gsa, bar, sections);
                int group = 0;

                NodeIO.CreateNodes(gsa, new List<BHoME.Node>() { bar.StartNode, bar.EndNode }, out nodeIds);
                string startIndex = nodeIds[0];
                string endIndex = nodeIds[1];

                string orientationAngle = bar.OrientationAngle.ToString();
                string startR = CreateReleaseString(bar.StartNode);
                string endR = CreateReleaseString(bar.EndNode);
                string dummy = "";

                string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
                dynamic commandResult = gsa.GwaCommand(str); //"EL.2, 1,, NO_RGB , BEAM , 1, 1, 1, 2 , 0 ,0, RLS, FFFFFF , FFFFFF, NO_OFFSET, "

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    continue;
                }
                else
                {
                    return Utils.CommandFailed(command);
                }
            }

            //PropertyIO.SetSections(sections);

            gsa.UpdateViews();
            return true;
        }



        public static string CreateReleaseString(BHoME.Node node)
        {
            string UX = "F";
            string UY = "F";
            string UZ = "F";
            string RX = "F";
            string RY = "F";
            string RZ = "F";

            if (node.IsConstrained)
            {
                UX = ((node.Constraint.UX == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
                UY = ((node.Constraint.UY == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
                UZ = ((node.Constraint.UZ == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
                RX = ((node.Constraint.RX == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
                RY = ((node.Constraint.RY == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
                RZ = ((node.Constraint.RZ == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
            }
            return UX + UY + UZ + RX + RY + RZ;
        }

    }
}
