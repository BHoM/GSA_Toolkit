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
        public static Dictionary<string,BHoMP.SectionProperty> GetSections(ComAuto gsa, bool nameAsKey = true)
        {
            //BHoMB.ObjectManager<BHoMP.SectionProperty> secProps = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);

            Dictionary<string, BHoMP.SectionProperty> secProps = new Dictionary<string, BHoMP.SectionProperty>();
            List<string> gsaProps = GetGsaSectionPropertyStrings(gsa);
            Dictionary<string, BHoMM.Material> materials = MaterialIO.GetMaterials(gsa, false, true);

            foreach (string gsaProp in gsaProps)
            {
                BHoMP.SectionProperty secProp = PropertyStringGenerators.GetSectionFromGsaString(gsaProp, materials);
                if (nameAsKey)
                {
                    if(!secProps.ContainsKey(secProp.Name))
                        secProps.Add(secProp.Name, secProp);
                }
                else secProps.Add(secProp.CustomData["GSA_id"].ToString(), secProp);
            }

            return secProps;
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

        static public bool SetSectionProperty(ComAuto GSA, BHoMP.SectionProperty secProp, string index)
        {
            //id = "";
            //Check to see if "GSA_id" Custom data
            //int index = GSA.GwaCommand("HIGHEST, PROP_SEC") + 1;
            //string num = index.ToString();
            string name = secProp.Name;

            //Test to check if name is set, if no name found it is set to the property number. 
            //Might be a better way to do this

            //if (string.IsNullOrWhiteSpace(secProp.Name))
            //{
            //    name = num;
            //}
            //else
            //{
            //    name = secProp.Name;
            //}

            string mat = secProp.Material.CustomData["GSA_id"].ToString();// materialId;  //"STEEL";// material.Name;

            string desc;// = "EXP";// secProp.Description;
            string props;

            PropertyStringGenerators.CreateDescAndPropString(secProp, out desc, out props);

            //double ModA; double ModIyy; double ModIzz; double ModJ;

            string command = "PROP_SEC";
            string colour = "NO_RGB";
            string principle = "NO";
            string type = "NA";
            string cost = "0";
            string plate_type = "FLAME_CUT";
            string calc_J = "NO_J";

            //string props = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;
            //string props = "NO_PROP";
            //string mods = "MOD_PROP, BY," + ModA + ",BY," + ModIyy + ",BY," + ModIzz + ",BY," + ModJ + ",BY, 1, BY, 1, NO, NO_MOD";
            string mods = "NO_MOD_PROP";
            //PROP_SEC    2   Section 2   NO_RGB  1   CAT % UB % UB914x419x388 % 19990407   NO NA  0   NO_PROP NO_MOD_PROP FLAME_CUT NO_J

            string str = command + "," + index + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;

            dynamic commandResult = GSA.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                //id = num;
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            GSA.UpdateViews();
            return true;
        }

       

        //static public bool SetSectionProperty(ComAuto gsa, BHoMP.SectionProperty secProp, out string id)
        //{
        //    id = "";
        //    string index = secProp.CustomData["GSA_id"].ToString();
        //    if (index == null)
        //    {
        //        index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;
        //    }
        
        //    string num = index.ToString();
        //    string name = secProp.Name;
        //    string mat = secProp.Material.CustomData["GSA_id"].ToString();// material.Name;
        //    string desc = secProp.Description;

        //    string area = ""; // secProp.GrossArea.ToString();
        //    string Iyy = "";
        //    string Izz = "";
        //    string J = "";
        //    string Avy = "";
        //    string Avz = "";
        //    double ModA; double ModIyy; double ModIzz; double ModJ;

        //    string command = "PROP_SEC";
        //    string colour = "NO_RGB";
        //    string principle = "NO";
        //    string type = "NA";
        //    string cost = "0";
        //    string plate_type = "FLAME_CUT";
        //    string calc_J = "NO_J";

        //    string props = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;
        //    //string props = "NO_PROP";
        //    //string mods = "MOD_PROP, BY," + ModA + ",BY," + ModIyy + ",BY," + ModIzz + ",BY," + ModJ + ",BY, 1, BY, 1, NO, NO_MOD";
        //    string mods = "NO_MOD_PROP";

        //    string str = command + "," + num + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;

        //    dynamic commandResult = gsa.GwaCommand(str);

        //    if (1 == (int)commandResult)
        //    {
        //        id = num;
        //    }
        //    else
        //    {
        //        Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
        //        return false;
        //    }

        //    gsa.UpdateViews();
        //    return true;
        //}

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


        //static public string GetOrCreateGSA_id(ComAuto gsa, BHoME.Bar bar, Dictionary<string,BHoMP.SectionProperty> sections, string materialId)
        //{
        //    //BHoMP.SectionProperty section = sections[bar.SectionProperty.Name];

        //    BHoMP.SectionProperty section;


        //    string sectionPropertyIndex;

        //    //Saving the sectionnames with name+material id to make sure that identical section properties with different names gets added

        //    if (!sections.TryGetValue(bar.SectionProperty.Name+materialId, out section)/* || section.CustomData["GSA_id"] == null*/)
        //    {
        //        int index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;
        //        section = bar.SectionProperty;

        //        if (SetSectionProperty(gsa, section, materialId, out sectionPropertyIndex))
        //        {
        //            //section.CustomData.Add("GSA_id", index);
        //            sections.Add(section.Name+materialId, section);
        //        }
                
        //    }

        //    return section.CustomData["GSA_id"].ToString();
        //}

        static public void CreateSectionProperties(ComAuto gsa, List<BHoMP.SectionProperty> sectionProperties)
        {
            //Get existing GSA section properties sorted by name as key
            Dictionary<string, BHoMP.SectionProperty> gsaSections = PropertyIO.GetSections(gsa, true);

            //Get all unique materials from imported secttionproperties
            List<BHoMM.Material> materials = sectionProperties.Select(x => x.Material).Distinct().ToList();

            //Create all new materials
            MaterialIO.CreateMaterials(gsa, materials);

            string sectionPropertyIndex;
            
            //Get the highest section property index
            int index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;

            //Loop trough and add all nonexisting section properties to gsa
            foreach (BHoMP.SectionProperty sectionProperty in sectionProperties)
            {
                object gsaIdobj;

                //Replace section property at position "id"
                if (sectionProperty.CustomData.TryGetValue("GSA_id", out gsaIdobj))
                {
                    string id = gsaIdobj.ToString();
                    SetSectionProperty(gsa, sectionProperty, id);
                    if (gsaSections.ContainsKey(sectionProperty.Name))
                        gsaSections[sectionProperty.Name] = sectionProperty;
                    else
                        gsaSections.Add(sectionProperty.Name, sectionProperty);

                }
                //Add section property
                else if (!gsaSections.ContainsKey(sectionProperty.Name))
                {
                    sectionPropertyIndex = index.ToString();
                    index++;

                    if (SetSectionProperty(gsa, sectionProperty, sectionPropertyIndex))
                    {
                        sectionProperty.CustomData.Add("GSA_id", sectionPropertyIndex);
                        gsaSections.Add(sectionProperty.Name, sectionProperty);
                    }

                }
                else
                {
                    sectionProperty.CustomData.Add("GSA_id", gsaSections[sectionProperty.Name].CustomData["GSA_id"]);
                }
            }
        }


        //public static string GetOrCreateSectionPropertyIndex(ComAuto gsa, BHoME.Bar bar, List<string> secProps, List<string> materials)
        //{
        //    string sectionPropertyIndex = "";
        //    string materialIndex = "";
        //    foreach (string secPropString in secProps)
        //        if (PropertyIO.GetDataStringFromSecPropStr(secPropString, 3) == bar.SectionProperty.Name)
        //            sectionPropertyIndex = PropertyIO.GetDataStringFromSecPropStr(secPropString, 1);

        //    foreach (string matString in materials)
        //        if (MaterialIO.GetDataStringFromMatStr(matString, 3) == bar.Material.Name)
        //            materialIndex = MaterialIO.GetDataStringFromMatStr(matString, 1);

        //    if(materialIndex =="")
        //    {
        //        MaterialIO.SetMaterial(gsa, bar.Material, out materialIndex);
        //        materials.Add(MaterialIO.GetMaterialString(gsa, int.Parse(materialIndex)));

        //    }

        //    if (sectionPropertyIndex == "")
        //    {

        //        PropertyIO.SetSectionProperty(gsa, bar.SectionProperty, bar.Material, out sectionPropertyIndex);
        //        secProps.Add(PropertyIO.GetSectionPropertyString(gsa, int.Parse(sectionPropertyIndex)));
        //    }
        //    return sectionPropertyIndex;
        //}

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
