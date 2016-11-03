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
using GSA_Adapter.Utility;


namespace GSA_Adapter.Structural.Properties
{
    public class PropertyIO
    {
        /*******************************************************/
        /*********** Section Property Get Methods **************/
        /*******************************************************/

        public static Dictionary<string,BHP.SectionProperty> GetSections(IComAuto gsa, bool nameAsKey = true)
        {
            //BHoMB.ObjectManager<BHoMP.SectionProperty> secProps = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);

            Dictionary<string, BHP.SectionProperty> secProps = new Dictionary<string, BHP.SectionProperty>();
            List<string> gsaProps = GetGsaSectionPropertyStrings(gsa);
            Dictionary<string, BHM.Material> materials = MaterialIO.GetMaterials(gsa, false, true);

            foreach (string gsaProp in gsaProps)
            {
                BHP.SectionProperty secProp = PropertyStringGenerators.GetSectionFromGsaString(gsaProp, materials);
                if (nameAsKey)
                {
                    if(!secProps.ContainsKey(secProp.Name))
                        secProps.Add(secProp.Name, secProp);
                }
                else secProps.Add(secProp.CustomData[Utils.ID].ToString(), secProp);
            }

            return secProps;
        }

        /*******************************************************/

        /// <summary>
        /// Gets all section properties in a list of the format [SEC_PROP | num | prop | name | mat | desc | area | Iyy | Izz | J | Ky | Kz]
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all section properties</returns>
        static public List<string> GetGsaSectionPropertyStrings(IComAuto gsa)
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

        /*******************************************************/

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

        /*******************************************************/

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

        /*******************************************************/


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


        /*******************************************************/
        /*********** Section Property Set Methods **************/
        /*******************************************************/

        static public void CreateSectionProperties(IComAuto gsa, List<BHP.SectionProperty> sectionProperties)
        {
            //Get existing GSA section properties sorted by name as key
            Dictionary<string, BHP.SectionProperty> gsaSections = GetSections(gsa, true);

            //Get all unique materials from imported sectionproperties
            Dictionary<Guid, BHM.Material> materials = sectionProperties.Select(x => x.Material).Distinct().ToDictionary(x => x.BHoM_Guid);
            //Dictionary<Guid, BHM.Material> clonedMaterials = materials.Select(x => x.Value.CustomData.ContainsKey(Utils.ID) ? x : new KeyValuePair<Guid, BHM.Material>(x.Key, (BHM.Material)x.Value.ShallowClone())).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<Guid, BHM.Material> clonedMaterials = Utils.CloneObjects(materials);
            //Assign cloned materials to section properties
            sectionProperties.ForEach(x => x.Material = clonedMaterials[x.Material.BHoM_Guid]);

            //Create all new materials
            MaterialIO.CreateMaterials(gsa, clonedMaterials.Values.ToList());

            string sectionPropertyIndex;
            
            //Get the highest section property index
            int index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;

            //Loop trough and add all nonexisting section properties to gsa
            foreach (BHP.SectionProperty sectionProperty in sectionProperties)
            {
                object gsaIdobj;

                //Replace section property at position "id"
                if (sectionProperty.CustomData.TryGetValue(Utils.ID, out gsaIdobj))
                {
                    string id = gsaIdobj.ToString();
                    SetSectionProperty(gsa, sectionProperty, id);
                    if (gsaSections.ContainsKey(Utils.GetName(sectionProperty)))
                        gsaSections[Utils.GetName(sectionProperty)] = sectionProperty;
                    else
                        gsaSections.Add(Utils.GetName(sectionProperty), sectionProperty);

                }
                //Add section property
                else if (!gsaSections.ContainsKey(Utils.GetName(sectionProperty)))
                {
                    sectionPropertyIndex = index.ToString();
                    index++;

                    if (SetSectionProperty(gsa, sectionProperty, sectionPropertyIndex))
                    {
                        sectionProperty.CustomData.Add(Utils.ID, sectionPropertyIndex);
                        gsaSections.Add(Utils.GetName(sectionProperty), sectionProperty);
                    }

                }
                else
                {
                    sectionProperty.CustomData.Add(Utils.ID, gsaSections[Utils.GetName(sectionProperty)].CustomData[Utils.ID]);
                }
            }
        }

        /*******************************************************/

        static public bool SetSectionProperty(IComAuto gsa, BHP.SectionProperty secProp, string index)
        {
            string name = Utils.GetName(secProp);

            string mat = secProp.Material.CustomData[Utils.ID].ToString();// materialId;  //"STEEL";// material.Name;

            string desc;
            string props;

            PropertyStringGenerators.CreateDescAndPropString(secProp, out desc, out props);

            string command = "PROP_SEC";
            string colour = "NO_RGB";
            string principle = "NO";
            string type = "NA";
            string cost = "0";
            string plate_type = "FLAME_CUT";
            string calc_J = "NO_J";
            string mods = "NO_MOD_PROP";


            //PROP_SEC    2   Section 2   NO_RGB  1   CAT % UB % UB914x419x388 % 19990407   NO NA  0   NO_PROP NO_MOD_PROP FLAME_CUT NO_J

            string str = command + "," + index + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                //id = num;
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            gsa.UpdateViews();
            return true;
        }

        /*******************************************************/
        /************* Panel Property Get Methods **************/
        /*******************************************************/

        public static Dictionary<string, BHP.PanelProperty> GetPanelProperties(ComAuto gsa, bool nameAsKey = true)
        {
            //BHoMB.ObjectManager<BHoMP.SectionProperty> secProps = new BHoMB.ObjectManager<BHoMP.SectionProperty>(BHoMG.Project.ActiveProject);

            Dictionary<string, BHP.PanelProperty> panProps = new Dictionary<string, BHP.PanelProperty>();
            List<string> gsaProps = Get2DPropertyStringList(gsa);
            Dictionary<string, BHM.Material> materials = MaterialIO.GetMaterials(gsa, false, true);

            foreach (string gsaProp in gsaProps)
            {
                BHP.PanelProperty panProp = PropertyStringGenerators.GetPanelPropertyFromGsaString(gsaProp, materials);
                if (nameAsKey)
                {
                    if (!panProps.ContainsKey(panProp.Name))
                        panProps.Add(panProp.Name, panProp);
                }
                else panProps.Add(panProp.CustomData[Utils.ID].ToString(), panProp);
            }

            return panProps;
        }

        /*******************************************************/

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

        /*******************************************************/

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

        /*******************************************************/

        public static string GetOrCreate2DPropertyIndex(ComAuto GSA, BHE.Panel panel, List<string> props)
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


        /*******************************************************/
        /************* Panel Property Set Methods **************/
        /*******************************************************/


        static public void CreatePanelProperties(ComAuto gsa, List<BHP.PanelProperty> panel)
        {
            //Get existing GSA section properties sorted by name as key
            //Getting panel properties not implemented yet. Uncomment as soon as this function works
            //Dictionary<string, BHP.PanelProperty> gsaSections = PropertyIO.GetPanelProperties(gsa, true);

            //Get all unique materials from imported sectionproperties
            Dictionary<Guid, BHM.Material> materials = panel.Select(x => x.Material).Where(x => x != null).Distinct().ToDictionary(x => x.BHoM_Guid);
            //Dictionary<Guid, BHM.Material> clonedMaterials = materials.Select(x => x.Value.CustomData.ContainsKey(Utils.ID) ? x : new KeyValuePair<Guid, BHM.Material>(x.Key, (BHM.Material)x.Value.ShallowClone())).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<Guid, BHM.Material> clonedMaterials = Utils.CloneObjects(materials);
            //Assign cloned materials to section properties
            panel.ForEach(x => x.Material = x.Material != null? clonedMaterials[x.Material.BHoM_Guid] : null);

            //Create all new materials
            MaterialIO.CreateMaterials(gsa, clonedMaterials.Values.ToList());

            string sectionPropertyIndex;

            //Get the highest section property index
            int index = gsa.GwaCommand("HIGHEST, PROP_2D") + 1;

            //Loop trough and add all nonexisting section properties to gsa
            foreach (BHP.PanelProperty panProp in panel)
            {

                if (!(panProp is BHP.ConstantThickness || panProp is BHP.LoadingPanelProperty))
                {
                    Utils.SendErrorMessage("Only constant thickness panel property and loading panel property is implemented in GSA");
                    return;
                }


                object gsaIdobj;

                if (panProp.CustomData.TryGetValue(Utils.ID, out gsaIdobj))
                {
                    string id = gsaIdobj.ToString();
                    Set2DProperty(gsa, panProp, id);

                    //if (gsaSections.ContainsKey(Utils.GetName(sectionProperty)))
                    //    gsaSections[Utils.GetName(sectionProperty)] = sectionProperty;
                    //else
                    //    gsaSections.Add(Utils.GetName(sectionProperty), sectionProperty);

                }
                else
                {
                    string num = index.ToString();
                    index++;

                    if (Set2DProperty(gsa, panProp, num))
                    {
                        panProp.CustomData.Add(Utils.ID, num);
                    }
                }
                //Uncomment bit bellow as soon as get panel properties has been implemented




                ////Add section property
                //else if (!gsaSections.ContainsKey(Utils.GetName(sectionProperty)))
                //{
                //    sectionPropertyIndex = index.ToString();
                //    index++;

                //    if (SetSectionProperty(gsa, sectionProperty, sectionPropertyIndex))
                //    {
                //        sectionProperty.CustomData.Add(Utils.ID, sectionPropertyIndex);
                //        gsaSections.Add(Utils.GetName(sectionProperty), sectionProperty);
                //    }

                //}
                //else
                //{
                //    sectionProperty.CustomData.Add(Utils.ID, gsaSections[Utils.GetName(sectionProperty)].CustomData[Utils.ID]);
                //}
            }
        }

        /*******************************************************/

        public static bool Set2DProperty(ComAuto gsa, BHP.PanelProperty panProp, string num)
        {

            if (panProp is BHP.ConstantThickness)
                return SetConstantThicknessProperty(gsa, (BHP.ConstantThickness)panProp, num);

            if (panProp is BHP.LoadingPanelProperty)
                return SetLoadingPanelProperty(gsa, (BHP.LoadingPanelProperty)panProp, num);

            return false;
        }


        static public bool SetConstantThicknessProperty(ComAuto gsa, BHP.ConstantThickness panProp, string num)
        {

            string name = panProp.Name;
            string mat = panProp.Material.CustomData[Utils.ID].ToString();


            string command = "PROP_2D";
            string colour = "NO_RGB";
            string type = "SHELL";
            string axis = "GLOBAL";
            string thick = panProp.Thickness.ToString();
            string mass = "100%";
            string bending = "100%";
            string inplane = "100%";
            string weight = "0";

            string str = command + "," + num + "," + name + "," + colour + "," + axis + "," + mat + "," + type + "," + thick +"," + weight + "," + mass + "," + bending + "," + inplane;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                return true;
            }

            return Utils.CommandFailed(command);

        }

        private static bool SetLoadingPanelProperty(ComAuto gsa, BHP.LoadingPanelProperty panProp, string num)
        {
            string command = "PROP_2D";
            string name = panProp.Name;
            string colour = "NO_RGB";
            string axis = "0";
            string mat = "0";
            string type = "LOAD";
            string support = GetLoadPanelSupportConditions(panProp.LoadApplication);
            string edge = panProp.ReferenceEdge.ToString();

            string str = command + "," + num + "," + name + "," + colour + "," + axis + "," + mat + "," + type + "," + support + "," + edge;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                return true;
            }

            return Utils.CommandFailed(command);

        }

        private static string GetLoadPanelSupportConditions(BHP.LoadPanelSupportConditions cond)
        {
            switch (cond)
            {
                case BHP.LoadPanelSupportConditions.AllSides:
                    return "SUP_ALL";
                case BHP.LoadPanelSupportConditions.ThreeSides:
                    return "SUP_THREE";
                case BHP.LoadPanelSupportConditions.TwoSides:
                    return "SUP_TWO";
                case BHP.LoadPanelSupportConditions.TwoAdjacentSides:
                    return "SUP_TWO_ADJ";
                case BHP.LoadPanelSupportConditions.OneSide:
                    return "SUP_ONE";
                case BHP.LoadPanelSupportConditions.Cantilever:
                    return "SUP_ONE_MOM";
                default:
                    return "SUP_AUTO";
            }
        }
    }
}
