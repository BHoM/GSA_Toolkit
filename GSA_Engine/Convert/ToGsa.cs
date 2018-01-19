using BH.Engine.Serialiser;
using BH.oM.Common.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Structural.Loads;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;

using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this Type type)
        {
            if (type == typeof(Node))
                return "NODE";
            else if (type == typeof(Bar))
                return "EL";
            else if (type == typeof(Material))
                return "MAT";
            else if (type == typeof(ISectionProperty))
                return "PROP_SEC";

            return null;
        }

        /***************************************************/

        public static List<string> ToGsaString(this LoadCombination loadComb, string combNo, string desc)
        {
            List<string> gsaStrings = new List<string>();
            gsaStrings.Add(GetAnalysisCase(combNo, loadComb.Name, combNo, desc));
            string type = GetTaskType(loadComb);
            gsaStrings.Add(GetAnalysisTask(combNo, loadComb.Name, type, "0", combNo));

            return gsaStrings;
        }

        /***************************************************/

        public static string ToGsaString(this Load<Node> nodeLoad, double loadFactor, double lengthFactor)
        {
            string command;
            Vector trans, rot;

            if (nodeLoad is PointDisplacement)
            {
                trans = ((PointDisplacement)nodeLoad).Translation;
                rot = ((PointDisplacement)nodeLoad).Rotation;
                command = "DISP_NODE";
                return command;
            }

            else if (nodeLoad is PointForce)
            {
                trans = ((PointForce)nodeLoad).Force;
                rot = ((PointForce)nodeLoad).Moment;
                command = "LOAD_NODE";
                return command;
            }

            else
                return null;
        }

        /***************************************************/

        public static List<string> ToGsaString(this Load<Bar> barLoad, double loadFactor, double lengthFactor)
        {
            return null;
        }

        /***************************************************/

        public static List<string> IToGsaString(this ILoad load, double loadFactor = 1 , double lengthFactor = 1)
        {
            return ToGsaString(load as dynamic, loadFactor, lengthFactor);
        }

        /***************************************************/

        static public string ToGsaString(this Loadcase loadCase)
        {
            string title = loadCase.Name; ;
            string type = GetGsaLoadType(loadCase.Nature);

            string str;
            string command = "LOAD_TITLE.1";
            string bridge = "BRIDGE_NO";

            if (type == "SUPERDEAD") type = "DEAD";

            str = command + "," + loadCase.Number + "," + title + "," + type + "," + bridge;
            return str;
        }

        /***************************************************/

        private static string ToGsaString(this Material material, string index)
        {
            string command = "MAT";
            string num = index;//(GSA.GwaCommand("HIGHEST, PROP_SEC") + 1).ToString();
            string mModel = "MAT_ELAS_ISO";
            string name = material.TaggedName();
            string colour = "NO_RGB";
            string type = GetMaterialType(material).ToString();
            string E = material.YoungsModulus.ToString();
            string nu = material.PoissonsRatio.ToString();
            string rho = material.Density.ToString();
            string alpha = material.CoeffThermalExpansion.ToString();
            string G = material.ShearModulus.ToString();
            string damp = material.DampingRatio.ToString();

            string str = command + "," + num + "," + mModel + "," + name + "," + colour + "," + type + ",6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
            return str;
        }

        /***************************************/

        private static string ToGsaString(this Bar bar, string index)
        {
            string command = "EL.2";
            string name = bar.TaggedName();
            string type = GetElementTypeString(bar);

            string sectionPropertyIndex = bar.SectionProperty.CustomData[AdapterID].ToString();
            int group = 0;

            string startIndex = bar.StartNode.CustomData[AdapterID].ToString();
            string endIndex = bar.EndNode.CustomData[AdapterID].ToString();

            string orientationAngle = bar.OrientationAngle.ToString();
            // TODO: Make sure that these are doing the correct thing. Release vs restraint corresponding to true vs false
            string startR = bar.Release != null ? CreateReleaseString(bar.Release.StartRelease) : "FFFFFF";
            string endR = bar.Release != null ? CreateReleaseString(bar.Release.EndRelease) : "FFFFFF";
            string dummy = CheckDummy(bar);

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************/

        private static string ToGsaString(this Node node, string index)
        {
            string command = "NODE.2";
            string name = node.TaggedName();

            string restraint = GetRestraintString(node);

            string str = command + ", " + index + ", " + name + " , NO_RGB, " + node.Position.X + " , " + node.Position.Y + " , " + node.Position.Z + ", NO_GRID, " + 0 + "," + restraint;
            return str;
        }

        /***************************************/

        private static string ToGsaString(this ISectionProperty prop, string index)
        {
            string name = prop.TaggedName();

            string mat = prop.Material.CustomData[AdapterID].ToString();// materialId;  //"STEEL";// material.Name;

            string desc;
            string props;

            ICreateDescAndPropString(prop, out desc, out props);

            string command = "PROP_SEC";
            string colour = "NO_RGB";
            string principle = "NO";
            string type = "NA";
            string cost = "0";
            string plate_type = "FLAME_CUT";
            string calc_J = "NO_J";
            string mods = "NO_MOD_PROP";

            object modifiers;

            if (prop.CustomData.TryGetValue("Modifiers", out modifiers) && modifiers is List<double>)
            {
                List<double> modList = modifiers as List<double>;
                if (modList.Count == 6)
                {
                    mods = "MOD_PROP";
                    for (int i = 0; i < 6; i++)
                    {
                        mods += ",BY," + modList[i];
                    }
                    mods += ",NO,NO_MOD";
                }

            }

            //PROP_SEC    2   Section 2   NO_RGB  1   CAT % UB % UB914x419x388 % 19990407   NO NA  0   NO_PROP NO_MOD_PROP FLAME_CUT NO_J

            string str = command + "," + index + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;
            return str;
        }

        /***************************************************/
        /**** Public Interface Methods                  ****/
        /***************************************************/

        public static string IToGsaString(this object obj, string index)
        {
            return ToGsaString(obj as dynamic, index);
        }


        /***************************************/
    }
}
