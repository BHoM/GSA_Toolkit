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
        public static bool GetBars(ComAuto gsa, out List<BHE.Bar> barList, List<string> barNumbers = null)
        {
            barList = new List<BHE.Bar>();


            int[] potentialBeamRefs = GeneratePotentialBeamRef(gsa, barNumbers);

            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            gsa.Elements(potentialBeamRefs, out gsaElements);

            Dictionary<string, BHP.SectionProperty> secProps = PropertyIO.GetSections(gsa, false);
            Dictionary<string, BHE.Node> nodes = NodeIO.GetNodes(gsa);


            for (int i = 0; i < gsaElements.Length; i++)
            {
                GsaElement gsaBar = gsaElements[i]; //TODO: filter elements based on topology


                if (!(gsaBar.eType == 1 || gsaBar.eType == 2 || gsaBar.eType == 3 || gsaBar.eType == 10 || gsaBar.eType == 20 || gsaBar.eType == 21))
                    continue;

                //GsaNode[] gsaNodes;
                //gsa.Nodes(gsaBar.Topo, out gsaNodes);
                //BHE.Node n1 = new BHE.Node(gsaNodes[0].Coor[0], gsaNodes[0].Coor[1], gsaNodes[0].Coor[2]);
                //BHE.Node n2 = new BHE.Node(gsaNodes[1].Coor[0], gsaNodes[1].Coor[1], gsaNodes[1].Coor[2]);

                BHE.Bar bar = new BHE.Bar(nodes[gsaBar.Topo[0].ToString()], nodes[gsaBar.Topo[1].ToString()], gsaBar.Name);



                bar.OrientationAngle = gsaBar.Beta;

                bar.SectionProperty = secProps[gsaBar.Property.ToString()];

                bar.CustomData[Utils.ID] = gsaBar.Ref;

                barList.Add(bar);

            }

            //barList = bars.ToList();
            return true;
        }

        private static int[] GeneratePotentialBeamRef(ComAuto gsa, List<string> barNumbers)
        {
            if (barNumbers == null)
            {
                int maxIndex = gsa.GwaCommand("HIGHEST, EL");
                int[] potentialBeamRefs = new int[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                    potentialBeamRefs[i] = i + 1;

                return potentialBeamRefs;
            }

            return barNumbers.Select(x => int.Parse(x)).ToArray();
        }

        internal static bool GetOrCreateBars(ComAuto gsa, List<BHE.Bar> bars, out List<string> ids)
        {

            List<BHE.Bar> idBars = bars.Where(x => x.CustomData.ContainsKey(Utils.ID)).ToList();
            List<BHE.Bar> nonIdBars = bars.Where(x => !x.CustomData.ContainsKey(Utils.ID)).ToList();

            ids = idBars.Select(x => x.CustomData[Utils.ID].ToString()).ToList();

            if (nonIdBars.Count < 1)
                return true;

            //TODO: Should bars without an GSA_id be added here...??
            Utils.SendErrorMessage("Please only provide bars with an GSA_id when creating bar loads or bar groups");
            return false;
        }


        /// <summary>
        /// Create GSA bars
        /// </summary>
        /// <returns></returns>
        public static bool CreateBars(ComAuto gsa, List<BHE.Bar> bars, out List<string> ids)
        {
            //Shallowclone the bars and their custom data
            bars.ForEach(x => x = (BHE.Bar)x.ShallowClone());
            bars.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            ids = new List<string>();

            //Get unique section properties and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHP.SectionProperty> sectionProperties = bars.Select(x => x.SectionProperty).Distinct().ToDictionary(x => x.BHoM_Guid);
            Dictionary<Guid, BHP.SectionProperty> clonedSecProps = Utils.CloneSectionProperties(sectionProperties);


            //Create the section properties
            PropertyIO.CreateSectionProperties(gsa, clonedSecProps.Values.ToList());

            //Get unique nodes and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHE.Node> nodes = bars.SelectMany(x => new List<BHE.Node> { x.StartNode, x.EndNode }).Distinct().ToDictionary(x => x.BHoM_Guid);
            //Dictionary<Guid, BHE.Node> clonedNodes = nodes.Select(x => x.Value.CustomData.ContainsKey(Utils.ID) ? x : new KeyValuePair<Guid, BHE.Node>(x.Key, (BHE.Node)x.Value.ShallowClone())).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<Guid, BHE.Node> clonedNodes = Utils.CloneObjects(nodes);


            //Assign the clones section properties to the abrs
            bars.ForEach(x => x.SectionProperty = clonedSecProps[x.SectionProperty.BHoM_Guid]);

            //Assign the cloned nodes to the bars
            bars.ForEach(x => x.StartNode = clonedNodes[x.StartNode.BHoM_Guid]);
            bars.ForEach(x => x.EndNode = clonedNodes[x.EndNode.BHoM_Guid]);

            //bars = CloneBars(bars, clonedSecProps, clonedNodes);

            

            //Create nodes
            NodeIO.CreateNodes(gsa, clonedNodes.Values.ToList());

            int highestIndex = gsa.GwaCommand("HIGHEST, EL") + 1;

            foreach (BHE.Bar bar in bars)
            {
                string command = "EL.2";
                string index;

                if(!Utils.CheckAndGetGsaId(bar, out index))
                {
                    index = highestIndex.ToString();
                    highestIndex++;
                }

                string name = bar.Name;
                string type = GetElementTypeString(bar);
                //string materialId = MaterialIO.GetOrCreateMasterialGSA_id(gsa, bar, materials);
                string sectionPropertyIndex = bar.SectionProperty.CustomData[Utils.ID].ToString();// PropertyIO.GetOrCreateGSA_id(gsa, bar, sections, materialId);
                int group = 0;

                //NodeIO.CreateNodes(gsa, new List<BHoME.Node>() { bar.StartNode, bar.EndNode }, out nodeIds);
                string startIndex = bar.StartNode.CustomData[Utils.ID].ToString();// nodeIds[0];
                string endIndex = bar.EndNode.CustomData[Utils.ID].ToString();// nodeIds[1];

                string orientationAngle = bar.OrientationAngle.ToString();
                // TODO: Make sure that these are doing the correct thing. Release vs restraint corresponding to true vs false
                string startR = bar.Release != null ? CreateReleaseString(bar.Release.StartConstraint) : "FFFFFF";
                string endR = bar.Release != null ? CreateReleaseString(bar.Release.EndConstraint) : "FFFFFF";
                string dummy = Utils.CheckDummy(bar);

                string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
                dynamic commandResult = gsa.GwaCommand(str); //"EL.2, 1,, NO_RGB , BEAM , 1, 1, 1, 2 , 0 ,0, RLS, FFFFFF , FFFFFF, NO_OFFSET, "

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    bar.CustomData[Utils.ID] = index;
                    continue;
                }
                else
                {
                    return Utils.CommandFailed(command);
                }
            }

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

            if (nodeConstraint != null)
            {
                UX = ((nodeConstraint.UX == BHP.DOFType.Fixed) ? "R" : "F").ToString();
                UY = ((nodeConstraint.UY == BHP.DOFType.Fixed) ? "R" : "F").ToString();
                UZ = ((nodeConstraint.UZ == BHP.DOFType.Fixed) ? "R" : "F").ToString();
                RX = ((nodeConstraint.RX == BHP.DOFType.Fixed) ? "R" : "F").ToString();
                RY = ((nodeConstraint.RY == BHP.DOFType.Fixed) ? "R" : "F").ToString();
                RZ = ((nodeConstraint.RZ == BHP.DOFType.Fixed) ? "R" : "F").ToString();
            }
            return UX + UY + UZ + RX + RY + RZ;
        }


    }
}
