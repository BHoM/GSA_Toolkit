using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Structural;
using BHoM.Global;
using BHoM.Structural.SectionProperties;
using BHoM.Materials;
using Interop.gsa_8_7;


namespace GSAToolkit
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
        public static bool GetBars(ComAuto GSA, out List<Bar> outputBars, string barNumbers = "all")
        {
            ObjectManager<int, Bar> bars = new ObjectManager<int, Bar>(Project.ActiveProject, GSAUtils.NUM_KEY, FilterOption.UserData);
            ObjectManager<SectionProperty> sections = new ObjectManager<SectionProperty>(Project.ActiveProject);
            ObjectManager<BarRelease> releases = new ObjectManager<BarRelease>(Project.ActiveProject);
            ObjectManager<BarConstraint> constraints = new ObjectManager<BarConstraint>(Project.ActiveProject);
            ObjectManager<Material> materials = new ObjectManager<Material>(Project.ActiveProject);
            ObjectManager<int, Node> nodes = new ObjectManager<int, Node>(Project.ActiveProject, GSAUtils.NUM_KEY, FilterOption.UserData);

            int maxIndex = GSA.GwaCommand("HIGHEST, EL");
            int[] potentialBeamRefs = new int[maxIndex];
            for (int i = 0; i < maxIndex; i++)
                potentialBeamRefs[i] = i + 1;

            GsaElement[] elements = new GsaElement[potentialBeamRefs.Length];
            GSA.Elements(potentialBeamRefs, out elements);

            for (int i = 0; i < elements.Length; i++)
            {
                GsaElement gbar = elements[i];

                GsaNode[] endNodes;
                GSA.Nodes(gbar.Topo, out endNodes);
                BHoM.Structural.Node n1 = new Node(endNodes[0].Coor[0], endNodes[0].Coor[1], endNodes[0].Coor[2]);
                BHoM.Structural.Node  n2 = new Node(endNodes[1].Coor[0], endNodes[1].Coor[1], endNodes[1].Coor[2]);
            
                BHoM.Structural.Bar str_bar = bars.Add(gbar.Ref, new Bar(n1, n2, gbar.Ref.ToString()));

                str_bar.OrientationAngle = gbar.Beta;
            }

            outputBars = bars.ToList();
            return true;
        }


        /// <summary>
        /// Create GSA bars
        /// </summary>
        /// <returns></returns>
        public static bool CreateBars(ComAuto GSA, List<Bar> str_bars, out List<string> ids)
        {
            ids = new List<string>();
            List<string> secProps = PropertyIO.GetSectionPropertyStringList(GSA);
            List<string> materials = MaterialIO.GetMaterialStringList(GSA);
            List<string> nodeIds = new List<string>();

            foreach (Bar bar in str_bars)
            {
                string command = "EL.2";
                string index = bar.Name;
                string name = bar.Name;
                string type = "BEAM";
                string sectionPropertyIndex = PropertyIO.GetOrCreateSectionPropertyIndex(GSA, bar, secProps, materials);
                int group = 0;

                NodeIO.CreateNodes(GSA, new List<Node>() { bar.StartNode, bar.EndNode }, out nodeIds);
                string startIndex = nodeIds[0];
                string endIndex = nodeIds[1];

                string orientationAngle = bar.OrientationAngle.ToString();
                string startR = CreateReleaseString(bar.StartNode);
                string endR = CreateReleaseString(bar.EndNode);
                string dummy = "";

                string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
                dynamic commandResult = GSA.GwaCommand(str); //"EL.2, 1,, NO_RGB , BEAM , 1, 1, 1, 2 , 0 ,0, RLS, FFFFFF , FFFFFF, NO_OFFSET, "

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    continue;
                }
                else
                {
                    GSAUtils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }

            GSA.UpdateViews();
            return true;
        }



        public static string CreateReleaseString(Node node)
        {
            string UX = "F";
            string UY = "F";
            string UZ = "F";
            string RX = "F";
            string RY = "F";
            string RZ = "F";

            if (node.IsConstrained)
            {
                UX = ((node.Constraint.UX.Type == DOFType.Fixed) ? "R" : "F").ToString();
                UY = ((node.Constraint.UY.Type == DOFType.Fixed) ? "R" : "F").ToString();
                UZ = ((node.Constraint.UZ.Type == DOFType.Fixed) ? "R" : "F").ToString();
                RX = ((node.Constraint.RX.Type == DOFType.Fixed) ? "R" : "F").ToString();
                RY = ((node.Constraint.RY.Type == DOFType.Fixed) ? "R" : "F").ToString();
                RZ = ((node.Constraint.RZ.Type == DOFType.Fixed) ? "R" : "F").ToString();
            }
            return UX + UY + UZ + RX + RY + RZ;
        }

    }
}
