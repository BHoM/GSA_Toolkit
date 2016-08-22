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
        public static bool GetBars(ComAuto GSA, out List<BHoME.Bar> outputBars, string barNumbers = "all")
        {
            BHoMB.ObjectManager<int, BHoME.Bar> bars = new BHoMB.ObjectManager<int, BHoME.Bar>(BHoMG.Project.ActiveProject, Utils.NUM_KEY, BHoMB.FilterOption.UserData);
            BHoMB.ObjectManager<BHoMP.SectionProperty> sections = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMP.BarRelease> releases = new BHoMB.ObjectManager<BHoMP.BarRelease>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMP.BarConstraint> constraints = new BHoMB.ObjectManager<BHoMP.BarConstraint>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<BHoMM.Material> materials = new BHoMB.ObjectManager<BHoMM.Material>(BHoMG.Project.ActiveProject);
            BHoMB.ObjectManager<int, BHoME.Node> nodes = new BHoMB.ObjectManager<int, BHoME.Node>(BHoMG.Project.ActiveProject, Utils.NUM_KEY, BHoMB.FilterOption.UserData);

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
                BHoME.Node n1 = new BHoME.Node(endNodes[0].Coor[0], endNodes[0].Coor[1], endNodes[0].Coor[2]);
                BHoME.Node  n2 = new BHoME.Node(endNodes[1].Coor[0], endNodes[1].Coor[1], endNodes[1].Coor[2]);

                BHoME.Bar str_bar = bars.Add(gbar.Ref, new BHoME.Bar(n1, n2, gbar.Ref.ToString()));

                str_bar.OrientationAngle = gbar.Beta;
            }

            outputBars = bars.ToList();
            return true;
        }


        /// <summary>
        /// Create GSA bars
        /// </summary>
        /// <returns></returns>
        public static bool CreateBars(ComAuto GSA, List<BHoME.Bar> str_bars, out List<string> ids)
        {
            ids = new List<string>();
            List<string> secProps = PropertyIO.GetSectionPropertyStringList(GSA);
            List<string> materials = MaterialIO.GetMaterialStringList(GSA);
            List<string> nodeIds = new List<string>();

            foreach (BHoME.Bar bar in str_bars)
            {
                string command = "EL.2";
                string index = bar.Name;
                string name = bar.Name;
                string type = "BEAM";
                string sectionPropertyIndex = PropertyIO.GetOrCreateSectionPropertyIndex(GSA, bar, secProps, materials);
                int group = 0;

                NodeIO.CreateNodes(GSA, new List<BHoME.Node>() { bar.StartNode, bar.EndNode }, out nodeIds);
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
                    Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }

            GSA.UpdateViews();
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
