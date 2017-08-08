using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************/
        public static string GetGsaString(this object obj, string index)
        {
            return _GetGsaString(obj as dynamic, index);
        }

        /***************************************/

        private static string _GetGsaString(this Material material, string index)
        {
            string command = "MAT";
            string num = index;//(GSA.GwaCommand("HIGHEST, PROP_SEC") + 1).ToString();
            string mModel = "MAT_ELAS_ISO";
            string name = material.GetNameAndTagString();
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

        private static string _GetGsaString(this Bar bar, string index)
        {
            string command = "EL.2";
            string name = bar.GetNameAndTagString();
            string type = GetElementTypeString(bar);

            string sectionPropertyIndex = bar.SectionProperty.CustomData[GSAAdapter.ID].ToString();
            int group = 0;

            string startIndex = bar.StartNode.CustomData[GSAAdapter.ID].ToString();
            string endIndex = bar.EndNode.CustomData[GSAAdapter.ID].ToString();

            string orientationAngle = bar.OrientationAngle.ToString();
            // TODO: Make sure that these are doing the correct thing. Release vs restraint corresponding to true vs false
            string startR = bar.Release != null ? CreateReleaseString(bar.Release.StartConstraint) : "FFFFFF";
            string endR = bar.Release != null ? CreateReleaseString(bar.Release.EndConstraint) : "FFFFFF";
            string dummy = CheckDummy(bar);

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************/

        private static string _GetGsaString(this Node node, string index)
        {
            string command = "NODE.2";
            string name = node.GetNameAndTagString();

            string restraint = GetRestraintString(node);

            string str = command + ", " + index + ", " + name + " , NO_RGB, " + node.Point.X + " , " + node.Point.Y + " , " + node.Point.Z + ", NO_GRID, " + 0 + "," + restraint;
            return str;
        }

        /***************************************/

        private static string _GetGsaString(this SectionProperty prop, string index)
        {
            string name = prop.GetNameAndTagString();

            string mat = prop.Material.CustomData[GSAAdapter.ID].ToString();// materialId;  //"STEEL";// material.Name;

            string desc;
            string props;

            CreateDescAndPropString(prop, out desc, out props);

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

        /***************************************/
    }
}
