using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoM.Structural.SectionProperties;
using BHoM.Structural;
using BHoM.Materials;

namespace GSAToolkit
{
    public class PropertyIO
    {
        static public bool SetSectionProperty(ComAuto GSA, SectionProperty secProp, Material material, out string id)
        {
            id = "";
            int index = GSA.GwaCommand("HIGHEST, PROP_SEC") + 1;
            string num = index.ToString();
            string name = secProp.Name;
            string mat = material.Name;
            string desc = secProp.Description;

            string area = secProp.Area.ToString();
            string Iyy = "";
            string Izz = "";
            string J = "";
            string Avy = "";
            string Avz = "";
            double ModA; double ModIyy; double ModIzz; double ModJ;

            string command = "PROP_SEC";
            string colour = "NO_RGB";
            string principle = "NO";
            string type = "NA";
            string cost = "0";
            string plate_type = "FLAME_CUT";
            string calc_J = "NO_J";

            string props = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;
            //string props = "NO_PROP";
            //string mods = "MOD_PROP, BY," + ModA + ",BY," + ModIyy + ",BY," + ModIzz + ",BY," + ModJ + ",BY, 1, BY, 1, NO, NO_MOD";
            string mods = "NO_MOD_PROP";

            string str = command + "," + num + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;

            dynamic commandResult = GSA.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                id = num;
            }
            else
            {
                GSAUtils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            GSA.UpdateViews();
            return true;
        }

        /// <summary>
        /// Gets a section property in the format [SEC_PROP | num | prop | name | mat | desc | area | Iyy | Izz | J | Ky | Kz]
        /// </summary>
        /// <param name="gsa"></param>
        /// <param name="index">Section property index</param>
        /// <returns></returns>
        static public string GetSectionPropertyString(ComAuto GSA, int index)
        {
            string sResult = GSA.GwaCommand("GET, SEC_PROP," + index).ToString();
            return sResult;
        }

        static public string GetSectionPropertyTypeFromGSAString(string gsaString)
        {
            string[] parts = gsaString.Split(' ');

            if (parts.Length < 2)
            {
                if (parts[0] == "EXP")
                    return "EXP";
                else
                    return "unknown";
            }
            else if (parts[1].Contains("CHS"))
                return "CHS";
            else if (parts[1].Contains("RHS"))
                return "RHS";
            else if (parts[1].Contains("UB") || parts[1].Contains("UKB"))
                return "UB";
            else if (parts[1].Contains("UC") || parts[1].Contains("UKC"))
                return "UC";
            else if (parts[1].Contains("IT"))
                return "T";
            else if (parts[1] == "I")
                return "IBEAM";
            else if (parts[1] == "R")
                return "R";
            else
                return "unknown";
        }


        /// <summary></summary>
        public static BHoM.Structural.SectionProperties.SectionProperty GetSection(string gsaString)
        {
            BHoM.Structural.SectionProperties.SectionProperty secProp = new SectionProperty();

            return secProp;
        }

        /// <summary>
        /// Gets all section properties in a list of the format [SEC_PROP | num | prop | name | mat | desc | area | Iyy | Izz | J | Ky | Kz]
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all section properties</returns>
        static public List<string> GetSectionPropertyStringList(ComAuto GSA)
        {
            List<string> sResult = new List<string>();
            bool b = true;
            int i = 1;
            int chkNum = 0;
            int abortNum = 1000; //Amount of "" rows after which to abort

            while (b)
            {
                string iResult = GSA.GwaCommand("GET, SEC_PROP," + i).ToString();

                if (iResult != "") //This check is to count the number of consecutive null rows and later abort at a certain number
                {
                    sResult.Add(iResult);
                    chkNum = 0;
                }
                else
                    chkNum++;

                if (chkNum >= abortNum)
                    b = false;

                i++;
            }

            return sResult;
        }

        /// <summary>
        /// Gets a specific section property from a string of section properties
        /// </summary>
        /// <param name="sectionProperties"></param>
        /// <param name="index">0 = SEC_PROP, 1 = Section index, 2 = Properties, 3 = Name, 4 = Material, 5 = Description, 6 = Area, 7 = Iyy, 8 = Izz, 9 = J, 10 = Ky, 11 = Kz</param>
        /// <returns></returns>
        static public string GetDataStringFromSecPropStr(string sectionProperties, int index)
        {
            if (sectionProperties == "")
            {
                return "";
            }
            else
            {
                string[] secProps = sectionProperties.Split(',');
                string secProp = secProps[index];
                return secProp;
            }
        }

        public static string GetOrCreateSectionPropertyIndex(ComAuto GSA, Bar bar, List<string> secProps, List<string> materials)
        {
            string sectionPropertyIndex = "";
            string materialIndex = "";
            foreach (string secPropString in secProps)
                if (PropertyIO.GetDataStringFromSecPropStr(secPropString, 3) == bar.SectionProperty.Name)
                    sectionPropertyIndex = PropertyIO.GetDataStringFromSecPropStr(secPropString, 1);

            foreach (string matString in materials)
                if (MaterialIO.GetDataStringFromMatStr(matString, 3) == bar.Material.Name)
                    materialIndex = MaterialIO.GetDataStringFromMatStr(matString, 1);

            if(materialIndex =="")
            {
                MaterialIO.SetMaterial(GSA, bar.Material, out materialIndex);
                materials.Add(MaterialIO.GetMaterialString(GSA, int.Parse(materialIndex)));

            }

            if (sectionPropertyIndex == "")
            {

                PropertyIO.SetSectionProperty(GSA, bar.SectionProperty, bar.Material, out sectionPropertyIndex);
                secProps.Add(PropertyIO.GetSectionPropertyString(GSA, int.Parse(sectionPropertyIndex)));
            }
            return sectionPropertyIndex;
        }

        //static public bool Set2DProperty(ComAuto gsa, ThicknessProperty thickProp, out string num)
        //{

        //    string name = thickProp.Name;
        //    string mat = secProp.SectionMaterial.ToString();
        //    string desc = secProp.Description;

        //    double area; double Iyy; double Izz; double J; double Avy; double Avz;
        //    double ModA; double ModIyy; double ModIzz; double ModJ;

        //    string command = "PROP_2D";
        //    string index = gsa.GwaCommand("HIGHEST, PROP_2D") + 1;
        //    string colour = "NO_RGB";
        //    string principle = "NO";
        //    string type = "NA";
        //    string cost = "0";
        //    string plate_type = "FLAME_CUT";
        //    string calc_J = "NO_J";

        //    //string props = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;
        //    string props = "NO_PROP";
        //    //string mods = "MOD_PROP, BY," + ModA + ",BY," + ModIyy + ",BY," + ModIzz + ",BY," + ModJ + ",BY, 1, BY, 1, NO, NO_MOD";
        //    string mods = "NO_MOD_PROP";

        //    string str = command + "," + num + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;

        //    dynamic commandResult = gsa.GwaCommand(str);

        //    if (1 == (int)commandResult) return true;

        //    return GSAUtils.CommandFailed(command);

        //}

        /// <summary>
        /// Gets all 2D properties in a list of the format [PROP_2D | num | prop | name | mat | desc | area | Iyy | Izz | J | Ky | Kz]
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all section properties</returns>
        static public List<string> Get2DPropertyStringList(ComAuto GSA)
        {
            List<string> sResult = new List<string>();
            bool b = true;
            int i = 1;
            int chkNum = 0;
            int abortNum = 1000; //Amount of "" rows after which to abort

            while (b)
            {
                string iResult = GSA.GwaCommand("GET, PROP_2D," + i).ToString();

                if (iResult != "") //This check is to count the number of consecutive null rows and later abort at a certain number
                {
                    sResult.Add(iResult);
                    chkNum = 0;
                }
                else
                    chkNum++;

                if (chkNum >= abortNum)
                    b = false;

                i++;
            }

            return sResult;
        }

        /// <summary>
        /// Gets a specific 2D property from a string of 2D properties
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="index">0 = SEC_PROP, 1 = Section index, 2 = Properties, 3 = Name, 4 = Material, 5 = Description, 6 = Area, 7 = Iyy, 8 = Izz, 9 = J, 10 = Ky, 11 = Kz</param>
        /// <returns></returns>
        static public string GetDataStringFrom2DPropStr(string properties, int index)
        {
            if (properties == "")
            {
                return "";
            }
            else
            {
                string[] secProps = properties.Split(',');
                string secProp = secProps[index];
                return secProp;
            }
        }

        public static string GetOrCreate2DPropertyIndex(ComAuto GSA, Panel panel, List<string> props)
        {
            string propertyIndex = "";

            if (panel.ThicknessProperty == null)
                return "1";

            foreach (string propString in props)
                if (PropertyIO.GetDataStringFromSecPropStr(propString, 3) == panel.ThicknessProperty.Name)
                    propertyIndex = PropertyIO.GetDataStringFromSecPropStr(propString, 1);

            //if (propertyIndex == "")
            //{
            //    PropertyIO.Set2DProperty(GSA, panel.ThicknessProperty, out propertyIndex);
            //    secProps.Add(PropertyIO.GetSectionPropertyString(GSA, int.Parse(propertyIndex)));
            //}
            return propertyIndex;
        }

    }
}
