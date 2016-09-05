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
using GSA_Adapter.Utility;


namespace GSA_Adapter.Structural.Properties
{
    public class PropertyIO
    {
        public static BHoMB.ObjectManager<BHoMP.SectionProperty> GetSections(ComAuto gsa, bool nameAsKey = true)
        {
            BHoMB.ObjectManager<BHoMP.SectionProperty> secProps = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);

            List<string> gsaProps = GetGsaSectionPropertyStrings(gsa);

            foreach (string gsaProp in gsaProps)
            {
                BHoMP.SectionProperty secProp = GetSection(gsaProp);
                if(nameAsKey) secProps.Add(secProp.Name, secProp);
                else secProps.Add(secProp.CustomData["GSA_id"].ToString() , secProp);
            }

            return secProps;
        }


        /// <summary></summary>
        public static BHoMP.SectionProperty GetSection(string gsaString)
        {
            BHoMP.SectionProperty secProp = null;

            if (gsaString == "")
            {
                return null;
            }
                       
            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);
            string name = gsaStrings[3];
            string material = gsaStrings[4];
            string description = gsaStrings[5];

            if (description == "EXP")
            {
                secProp = new BHoMP.SectionProperty();
                double a, iyy, izz, j;
                double.TryParse(gsaStrings[6], out a);
                double.TryParse(gsaStrings[7], out iyy);
                double.TryParse(gsaStrings[8], out izz);
                double.TryParse(gsaStrings[9], out j);
            }

            if (description.StartsWith("STD") /*|| description.StartsWith("CAT") */)
            {   
                double D, W, T, t, Wt, Wb, Tt, Tb;
                string[] desc = description.Split('%');
                switch (desc[1])
                {
                    case "UC":
                    case "UB":
                    case "I":
                        D = double.Parse(desc[2]);
                        W = double.Parse(desc[3]);
                        T = double.Parse(desc[4]);
                        t = double.Parse(desc[5]);
                        secProp = new BHoMP.SectionProperty(BHoMP.ShapeType.ISection, BHoMP.SectionType.Steel, D, W, T, t, 0, 0);
                        break;
                    case "GI":
                        D = double.Parse(desc[2]);
                        Wt = double.Parse(desc[3]);
                        Wb = double.Parse(desc[4]);
                        Tt = double.Parse(desc[5]);
                        Tb = double.Parse(desc[6]);
                        t = double.Parse(desc[7]);
                        secProp = BHoMP.SectionProperty.CreateISection(BHoMP.SectionType.Steel, Wt, Wb, D, Tt, Tb, t, 0, 0);
                        break;
                    case "CHS":
                        D = double.Parse(desc[2]);
                        t = double.Parse(desc[3]);
                        secProp = new BHoMP.SectionProperty(BHoMP.ShapeType.Tube, BHoMP.SectionType.Steel, D, D, t, t, 0, 0);
                        break;
                    case "RHS":
                        D = double.Parse(desc[2]);
                        W = double.Parse(desc[3]);
                        T = double.Parse(desc[4]);
                        t = double.Parse(desc[5]);
                        secProp = new BHoMP.SectionProperty(BHoMP.ShapeType.Rectangle, BHoMP.SectionType.Steel, D, W, T, t, 0, 0);
                        break;

                    default:
                        break;
                }

            }
                
            secProp.CustomData.Add("GSA_id", id);
            secProp.Name = name;
            secProp.Description = description;
            return secProp;
        }

        /// <summary>
        /// Gets all section properties in a list of the format [SEC_PROP | num | prop | name | mat | desc | area | Iyy | Izz | J | Ky | Kz]
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all section properties</returns>
        static public List<string> GetGsaSectionPropertyStrings(ComAuto gsa)
        {
            List<string> gsaProps = new List<string>();

            int i = 1;
            int chkCount = 0;
            int abortNum = 1000; //Amount of "" rows after which to abort

            while (chkCount<abortNum)
            {
                string gsaProp = gsa.GwaCommand("GET, SEC_PROP," + i).ToString();
                chkCount++;
                i++;

                if (gsaProp != "") //This check is to count the number of consecutive null rows and later abort at a certain number
                {
                    gsaProps.Add(gsaProp);
                    chkCount = 0;
                }               
            }

            return gsaProps;
        }

        static public bool SetSectionProperty(ComAuto GSA, BHoMP.SectionProperty secProp, BHoMM.Material material, out string id)
        {
            id = "";
            //Check to see if "GSA_id" Custom data
            int index = GSA.GwaCommand("HIGHEST, PROP_SEC") + 1;
            string num = index.ToString();
            string name = secProp.Name;
            string mat = "2";// material.Name;
            string desc = secProp.Description;

            string area = ""; // secProp.GrossArea.ToString();
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
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            GSA.UpdateViews();
            return true;
        }




        static public bool SetSectionProperty(ComAuto gsa, BHoMP.SectionProperty secProp, out string id)
        {
            id = "";
            string index = secProp.CustomData["GSA_id"].ToString();
            if (index == null)
            {
                index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;
            }
        
            string num = index.ToString();
            string name = secProp.Name;
            string mat = "2";// material.Name;
            string desc = secProp.Description;

            string area = ""; // secProp.GrossArea.ToString();
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

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                id = num;
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            gsa.UpdateViews();
            return true;
        }

        /// <summary>
        /// Gets a section property in the format [SEC_PROP | num | prop | name | mat | desc | area | Iyy | Izz | J | Ky | Kz]
        /// </summary>
        /// <param name="gsa"></param>
        /// <param name="index">Section property index</param>
        /// <returns></returns>
        static public string GetSectionPropertyString(ComAuto gsa, int index)
        {
            string sResult = gsa.GwaCommand("GET, SEC_PROP," + index).ToString();
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


        static public string GetOrCreateGSA_id(ComAuto gsa, BHoME.Bar bar, BHoMB.ObjectManager<BHoMP.SectionProperty> sections)
        {
            BHoMP.SectionProperty section = sections[bar.SectionProperty.Name];
            string sectionPropertyIndex;

            if (section == null || section.CustomData["GSA_id"] == null)
            {
                int index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;
                section = bar.SectionProperty;
                section.CustomData.Add("GSA_id", index);
                sections.Add(section.Name, section);
                SetSectionProperty(gsa, section, bar.Material, out sectionPropertyIndex );
            }

            return section.CustomData["GSA_id"].ToString();
        }



        public static string GetOrCreateSectionPropertyIndex(ComAuto gsa, BHoME.Bar bar, List<string> secProps, List<string> materials)
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
                MaterialIO.SetMaterial(gsa, bar.Material, out materialIndex);
                materials.Add(MaterialIO.GetMaterialString(gsa, int.Parse(materialIndex)));

            }

            if (sectionPropertyIndex == "")
            {

                PropertyIO.SetSectionProperty(gsa, bar.SectionProperty, bar.Material, out sectionPropertyIndex);
                secProps.Add(PropertyIO.GetSectionPropertyString(gsa, int.Parse(sectionPropertyIndex)));
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

        public static string GetOrCreate2DPropertyIndex(ComAuto GSA, BHoME.Panel panel, List<string> props)
        {
            string propertyIndex = "";

            if (panel.PanelProperty == null)
                return "1";

            foreach (string propString in props)
                if (PropertyIO.GetDataStringFromSecPropStr(propString, 3) == panel.PanelProperty.Name)
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
