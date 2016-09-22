using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHG = BHoM.Global;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;
using BHM = BHoM.Materials;
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
        public static bool GetBars(ComAuto gsa, out List<BHE.Bar> barList, string barNumbers = "all")
        {
            BHB.ObjectManager<int, BHE.Bar> bars = new BHB.ObjectManager<int, BHE.Bar>(BHG.Project.ActiveProject, Utils.NUM_KEY, BHB.FilterOption.UserData);
            BHB.ObjectManager<BHP.SectionProperty> sections = new BHB.ObjectManager<BHP.SectionProperty>(BHG.Project.ActiveProject);
            BHB.ObjectManager<BHP.BarRelease> releases = new BHB.ObjectManager<BHP.BarRelease>(BHG.Project.ActiveProject);
            BHB.ObjectManager<BHP.BarConstraint> constraints = new BHB.ObjectManager<BHP.BarConstraint>(BHG.Project.ActiveProject);
            BHB.ObjectManager<BHM.Material> materials = new BHB.ObjectManager<BHM.Material>(BHG.Project.ActiveProject);
            BHB.ObjectManager<int, BHE.Node> nodes = new BHB.ObjectManager<int, BHE.Node>(BHG.Project.ActiveProject, Utils.NUM_KEY, BHB.FilterOption.UserData);


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
                BHE.Node n1 = new BHE.Node(gsaNodes[0].Coor[0], gsaNodes[0].Coor[1], gsaNodes[0].Coor[2]);
                BHE.Node n2 = new BHE.Node(gsaNodes[1].Coor[0], gsaNodes[1].Coor[1], gsaNodes[1].Coor[2]);

                BHE.Bar bar = bars.Add(gsaBar.Ref, new BHE.Bar(n1, n2, gsaBar.Ref.ToString()));
               
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

        internal static bool GetOrCreateBars(ComAuto gsa, List<BHE.Bar> bars, out List<string> ids)
        {

            List<BHE.Bar> idBars = bars.Where(x => x.CustomData.ContainsKey("GSA_id")).ToList();
            List<BHE.Bar> nonIdBars = bars.Where(x => !x.CustomData.ContainsKey("GSA_id")).ToList();

            ids = idBars.Select(x => x.CustomData["GSA_id"].ToString()).ToList();

            

            if (nonIdBars.Count < 1)
                return true;

            //TODO: Should bars without an GSA_id be added here...??
            Utils.SendErrorMessage("Please only provide bars with an GSA_id when creating bar loads");
            return false;
        }


        /// <summary>
        /// Create GSA bars
        /// </summary>
        /// <returns></returns>
        public static bool CreateBars(ComAuto gsa, List<BHE.Bar> bars, out List<string> ids)
        {
            ids = new List<string>();

            //BHoMB.ObjectManager<BHoMP.SectionProperty> sections = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);
            //BHoMB.ObjectManager<BHoMM.Material> materials = new BHoMB.ObjectManager<BHoMM.Material>(BHoMG.Project.ActiveProject);

            
            //Dictionary<string, BHP.SectionProperty> sections = PropertyIO.GetSections(gsa, true);

            List<BHP.SectionProperty> sectionProperties = bars.Select(x => x.SectionProperty).Distinct().ToList();

            PropertyIO.CreateSectionProperties(gsa, sectionProperties);

            //TODO: Create dictionary of properties and materials - do this at higher level for repeat use
            //List<string> secProps = PropertyIO.GetGsaSectionPropertyStrings(gsa);
            //List<string> materialList = MaterialIO.GetMaterialStringList(gsa);
            List<string> nodeIds = new List<string>();

            List<BHE.Node> nodes = bars.SelectMany(x => new List<BHE.Node> { x.StartNode, x.EndNode }).Distinct().ToList();

            NodeIO.CreateNodes(gsa, nodes);

            int highestIndex = gsa.GwaCommand("HIGHEST, EL") + 1;

            foreach (BHE.Bar bar in bars)
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

        private static string GetElementTypeString(BHE.Bar bar)
        {
            switch (bar.FEAType)
            {
                case BHE.BarFEAType.Axial:
                    return "BAR";
                case BHE.BarFEAType.Flexural:
                    return "BEAM";
                case BHE.BarFEAType.TensionOnly:
                    return "TIE";
                case BHE.BarFEAType.CompressionOnly:
                    return "STRUT";
                default:
                    return "BEAM";
                    //Returning beam by default as it is the most generic type.
                    //Might be better flagging this as an error
            }

        }

        public static string CreateReleaseString(BHP.NodeConstraint nodeConstraint)
        {
            string UX = "F";
            string UY = "F";
            string UZ = "F";
            string RX = "F";
            string RY = "F";
            string RZ = "F";

            UX = ((nodeConstraint.UX == BHP.DOFType.Fixed) ? "R" : "F").ToString();
            UY = ((nodeConstraint.UY == BHP.DOFType.Fixed) ? "R" : "F").ToString();
            UZ = ((nodeConstraint.UZ == BHP.DOFType.Fixed) ? "R" : "F").ToString();
            RX = ((nodeConstraint.RX == BHP.DOFType.Fixed) ? "R" : "F").ToString();
            RY = ((nodeConstraint.RY == BHP.DOFType.Fixed) ? "R" : "F").ToString();
            RZ = ((nodeConstraint.RZ == BHP.DOFType.Fixed) ? "R" : "F").ToString();

            return UX + UY + UZ + RX + RY + RZ;
        }


    }
}
