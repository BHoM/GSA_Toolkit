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

        public static List<string> ToGsaString(this Load<Node> nodeLoad, double[] unitFactors)
        {
            string command;
            Vector force, moment;
            List<string> forceStrings = new List<string>();
            double factor;

            force = nodeLoad.ITranslationVector()[0];
            command = nodeLoad.IForceTypeString();
            moment = nodeLoad.IRotationVector()[0];
            factor = nodeLoad.IFactor(unitFactors);

            string name = nodeLoad.Name;
            string str;
            string appliedTo = CreateIdListOrGroupName();
            string caseNo = nodeLoad.Loadcase.Number.ToString();
            string axis = GetAxis(nodeLoad);

            str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis;

            AddVectorDataToStringSingle(str, force, ref forceStrings, factor, true);
            AddVectorDataToStringSingle(str, moment, ref forceStrings, factor, false);

            return forceStrings;
        }

        /***************************************************/

        public static List<string> ToGsaString(this Load<Bar> barLoad, double[] unitFactors)
        {
            Vector force1, force2,  moment1, moment2;
            List<string> forceStrings = new List<string>();

            string name = barLoad.Name;
            string projection = barLoad.IsProjectedString();
            string axis = barLoad.IsGlobal();
            double factor = barLoad.IFactor(unitFactors);
            string appliedTo = CreateIdListOrGroupName();
            force1 = barLoad.ITranslationVector()[0];
            moment1 = barLoad.IRotationVector()[0];
            force2 = barLoad.ITranslationVector()[1];
            moment2 = barLoad.IRotationVector()[1];
            string caseNo = barLoad.Loadcase.Number.ToString();
            string command = barLoad.IForceTypeString();
            string pos1 = "," + barLoad.ILoadPosition()[0];
            string pos2 = "," + barLoad.ILoadPosition()[1];

            if (appliedTo == null)
                return null;

            string str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis + "," + projection;

            AddVectorDataToStringSingle(str, force1, ref forceStrings, factor, true, pos1);
            AddVectorDataToStringSingle(str, moment1, ref forceStrings, factor, false, pos1);
            AddVectorDataToStringSingle(str, force2, ref forceStrings, factor, true, pos2);
            AddVectorDataToStringSingle(str, moment2, ref forceStrings, factor, false, pos2);

            return forceStrings;
        }

        /***************************************************/

        public static List<string> IToGsaString(this ILoad load, double[] unitFactors)
        {
            return ToGsaString(load as dynamic, unitFactors);
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
