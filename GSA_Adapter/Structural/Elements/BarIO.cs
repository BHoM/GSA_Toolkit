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


            //sections = PropertyIO.GetSections(gsa); //name as key
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
                BHoME.Node n2 = new BHoME.Node(gsaNodes[1].Coor[0], gsaNodes[1].Coor[1], gsaNodes[1].Coor[2]);

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

            //BHoMB.ObjectManager<BHoMP.SectionProperty> sections = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);
            //BHoMB.ObjectManager<BHoMM.Material> materials = new BHoMB.ObjectManager<BHoMM.Material>(BHoMG.Project.ActiveProject);

            
            Dictionary<string, BHoMP.SectionProperty> sections = PropertyIO.GetSections(gsa, true);

            List<BHoMP.SectionProperty> sectionProperties = bars.Select(x => x.SectionProperty).Distinct().ToList();

            PropertyIO.CreateSectionProperties(gsa, sectionProperties);

            //TODO: Create dictionary of properties and materials - do this at higher level for repeat use
            //List<string> secProps = PropertyIO.GetGsaSectionPropertyStrings(gsa);
            //List<string> materialList = MaterialIO.GetMaterialStringList(gsa);
            List<string> nodeIds = new List<string>();

            List<BHoME.Node> nodes = bars.SelectMany(x => new List<BHoME.Node> { x.StartNode, x.EndNode }).Distinct().ToList();

            NodeIO.CreateNodes(gsa, nodes);

            int highestIndex = gsa.GwaCommand("HIGHEST, EL") + 1;

            foreach (BHoME.Bar bar in bars)
            {
                string command = "EL.2";
                string index;
                if (bar.CustomData.ContainsKey("GSA_id"))
                    index = bar.CustomData["GSA_id"].ToString();
                else
                {
                    index = highestIndex.ToString();
                    highestIndex++;
                }
                string name = bar.Name;
                string type = GetElementTypeString(bar);
                //string materialId = MaterialIO.GetOrCreateMasterialGSA_id(gsa, bar, materials);
                string sectionPropertyIndex = bar.SectionProperty.CustomData["GSA_id"].ToString();// PropertyIO.GetOrCreateGSA_id(gsa, bar, sections, materialId);
                int group = 0;

                //NodeIO.CreateNodes(gsa, new List<BHoME.Node>() { bar.StartNode, bar.EndNode }, out nodeIds);
                string startIndex = bar.StartNode.CustomData["GSA_id"].ToString();// nodeIds[0];
                string endIndex = bar.EndNode.CustomData["GSA_id"].ToString();// nodeIds[1];

                string orientationAngle = bar.OrientationAngle.ToString();
                // TODO: Make sure that these are doing the correct thing. Release vs restraint corresponding to true vs false
                string startR = CreateReleaseString(bar.Release.StartConstraint);
                string endR = CreateReleaseString(bar.Release.EndConstraint);
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

        private static string GetElementTypeString(BHoME.Bar bar)
        {
            switch (bar.FEAType)
            {
                case BHoME.BarFEAType.Bar:
                    return "BAR";
                case BHoME.BarFEAType.Beam:
                    return "BEAM";
                case BHoME.BarFEAType.Tie:
                    return "TIE";
                case BHoME.BarFEAType.Strut:
                    return "STRUT";
                default:
                    return "BEAM";
                    //Returning beam by default as it is the most generic type.
                    //Might be better flagging this as an error
            }

        }

        public static string CreateReleaseString(BHoMP.NodeConstraint nodeConstraint)
        {
            string UX = "F";
            string UY = "F";
            string UZ = "F";
            string RX = "F";
            string RY = "F";
            string RZ = "F";

            UX = ((nodeConstraint.UX == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
            UY = ((nodeConstraint.UY == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
            UZ = ((nodeConstraint.UZ == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
            RX = ((nodeConstraint.RX == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
            RY = ((nodeConstraint.RY == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();
            RZ = ((nodeConstraint.RZ == BHoMP.DOFType.Fixed) ? "R" : "F").ToString();

            return UX + UY + UZ + RX + RY + RZ;
        }


    }
}
