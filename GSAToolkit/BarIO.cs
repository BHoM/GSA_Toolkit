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
            ObjectManager<int, Bar> bars = new ObjectManager<int, Bar>(Utils.NUM_KEY, FilterOption.UserData);
            ObjectManager<SectionProperty> sections = new ObjectManager<SectionProperty>();
            ObjectManager<BarRelease> releases = new ObjectManager<BarRelease>();
            ObjectManager<BarConstraint> constraints = new ObjectManager<BarConstraint>();
            ObjectManager<Material> materials = new ObjectManager<Material>();
            ObjectManager<int, Node> nodes = new ObjectManager<int, Node>(Utils.NUM_KEY, FilterOption.UserData);

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
            List<string> nodeIDs = new List<string>();
            List<string> secProps = PropertyIO.GetSectionPropertyStringList(GSA);

            Project.ActiveProject.MergeWithin(0.00000000001);

            foreach (Bar bar in str_bars)
            {
                int index = int.Parse(bar.Name);
                string name = bar.Name;
                string type = "BEAM";
                string sectionPropertyIndex = "";

                foreach (string secPropString in secProps)
                    if (PropertyIO.GetDataStringFromSecPropStr(secPropString, 3) == bar.SectionProperty.Name)
                        sectionPropertyIndex = PropertyIO.GetDataStringFromSecPropStr(secPropString, 1);

                if (sectionPropertyIndex == "")
                {
                    PropertyIO.SetSectionProperty(GSA, bar.SectionProperty, out sectionPropertyIndex);
                    secProps.Add(PropertyIO.GetSectionPropertyString(GSA, int.Parse(sectionPropertyIndex)));
                }

                int group = 0;
                int startIndex = int.Parse(bar.StartNode.Name);
                int endIndex = int.Parse(bar.EndNode.Name);
                string orientationAngle = bar.OrientationAngle.ToString();
                string startR = "FFFFFF";
                string endR = "FFFFFF";
                string dummy = "";

                ids.Add(index.ToString());

                if (GSA.GwaCommand("EXIST, NODE, " + startIndex)==0)
                    NodeIO.CreateNodes(GSA, new List<Node>() { bar.StartNode }, out nodeIDs);

                if (GSA.GwaCommand("EXIST, NODE, " + endIndex)==0)
                    NodeIO.CreateNodes(GSA, new List<Node>() { bar.EndNode }, out nodeIDs);

                string str = "EL.2, " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;

                //string str = "EL.2, 1,, NO_RGB , BEAM , 1, 1, 1, 2 , 0 ,0, RLS, FFFFFF , FFFFFF, NO_OFFSET, ";
                dynamic commandResult = GSA.GwaCommand(str);

                if (1 == (int)commandResult) continue;
                else
                {
                    //SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }

            GSA.UpdateViews();
            return true;
        }

       

        public static string CreateReleaseString(NodeConstraint bhomData)
        {
            return "FFFFFF";
        }

        ///// <summary>
        ///// Get the  robot end release value from the degree of freedom type
        ///// </summary>
        ///// <param name="endRelease"></param>
        ///// <returns></returns>
        //public static string GetReleaseType(DOFType endRelease)
        //{
        //    switch (endRelease)
        //    {
        //        case DOFType.Spring:
        //            return IRobotBarEndReleaseValue.I_BERV_ELASTIC;
        //        case DOFType.SpringNegative:
        //            return IRobotBarEndReleaseValue.I_BERV_ELASTIC_MINUS;
        //        case DOFType.SpringPositive:
        //            return IRobotBarEndReleaseValue.I_BERV_ELASTIC_PLUS;
        //        case DOFType.Free:
        //            return IRobotBarEndReleaseValue.I_BERV_NONE;
        //        case DOFType.FixedNegative:
        //            return IRobotBarEndReleaseValue.I_BERV_MINUS;
        //        case DOFType.FixedPositive:
        //            return IRobotBarEndReleaseValue.I_BERV_PLUS;
        //        case DOFType.Fixed:
        //            return IRobotBarEndReleaseValue.I_BERV_STD;
        //        case DOFType.NonLinear:
        //            return IRobotBarEndReleaseValue.I_BERV_NONLINEAR;
        //        case DOFType.SpringRelative:
        //            return IRobotBarEndReleaseValue.I_BERV_ELASTIC_REDUCED;
        //        case DOFType.SpringRelativeNegative:
        //            return IRobotBarEndReleaseValue.I_BERV_ELASTIC_REDUCED_MINUS;
        //        case DOFType.SpringRelativePositive:
        //            return IRobotBarEndReleaseValue.I_BERV_ELASTIC_REDUCED_PLUS;
        //        default:
        //            return IRobotBarEndReleaseValue.I_BERV_NONE;
        //    }
        //}
    }
}
