using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMP = BHoM.Structural.Properties;
using BHoMM = BHoM.Materials;
using BHoMD = BHoM.Structural.Databases;
using BHoME = BHoM.Structural.Elements;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Properties
{
    /// <summary>
    /// GSA material class, for all material objects and operations
    /// </summary>
    public class MaterialIO
    {

        //static public string GetOrCreateMasterialGSA_id(ComAuto gsa, BHoME.Bar bar, Dictionary<string, BHoMM.Material> materials)
        //{
            
        //    string sectionPropertyIndex;
        //    BHoMM.Material material;
        //    //object id;

        //    if (!materials.TryGetValue(bar.Material.Name, out material)/* || !bar.Material.CustomData.TryGetValue(Utils.ID, out id) || id == null*/)
        //    {
        //        material = bar.Material;
        //        int index = gsa.GwaCommand("HIGHEST, PROP_SEC") + 1;

        //        if (SetMaterial(gsa, material, out sectionPropertyIndex))
        //        {
        //            //material.CustomData.Add(Utils.ID, index);
        //            materials.Add(material.Name, material);
        //        }

        //    }

        //    return material.CustomData[Utils.ID].ToString();
        //}

        public static Dictionary<string, BHoMM.Material> GetMaterials(ComAuto gsa, bool nameAsKey = true, bool includeStandardMaterials = false)
        {
            Dictionary<string, BHoMM.Material> materials;
            if (includeStandardMaterials)
                materials = GetStandardGsaMaterials(nameAsKey);
            else
                materials = new Dictionary<string, BHoM.Materials.Material>();

            List<string> gsaMats = GetMaterialStringList(gsa);

            foreach (string gsaMat in gsaMats)
            {
                BHoMM.Material mat = GetMaterialFromGsaString(gsaMat);
                if (nameAsKey) materials.Add(mat.Name, mat);
                else materials.Add(mat.CustomData[Utils.ID].ToString(), mat);
            }

            return materials;
        }

        private static Dictionary<string, BHoMM.Material> GetStandardGsaMaterials(bool nameAsKey)
        {
            Dictionary<string, BHoMM.Material> materials = new Dictionary<string, BHoM.Materials.Material>();
            AddStandardGsaMaterial(ref materials, "STEEL", nameAsKey);
            AddStandardGsaMaterial(ref materials, "CONC_SHORT", nameAsKey);
            AddStandardGsaMaterial(ref materials, "CONC_LONG", nameAsKey);
            AddStandardGsaMaterial(ref materials, "ALUMINIUM", nameAsKey);
            AddStandardGsaMaterial(ref materials, "GLASS", nameAsKey);
            return materials;
        }

        private static void AddStandardGsaMaterial(ref Dictionary<string, BHoMM.Material> materials, string name, bool nameAsKey)
        {
            BHoMM.Material mat = new BHoMM.Material("GSA Standard "+name);
            mat.CustomData.Add(Utils.ID, name);

            if (nameAsKey)
                materials.Add(mat.Name, mat);
            else
                materials.Add(name, mat);

        }

        public static void CreateMaterials(ComAuto gsa, List<BHoMM.Material> materials)
        {
            Dictionary<string, BHoMM.Material> gsaMaterials = GetMaterials(gsa, true);

            int highestId = gsa.GwaCommand("HIGHEST, MAT") + 1;

            foreach (BHoMM.Material mat in materials)
            {
                object gsaIdobj;

                //Replace material at position "id"
                if (mat.CustomData.TryGetValue(Utils.ID, out gsaIdobj))
                {
                    string id = gsaIdobj.ToString();
                    SetMaterial(gsa, mat, id);
                    if (gsaMaterials.ContainsKey(mat.Name))
                        gsaMaterials[mat.Name] = mat;
                    else
                        gsaMaterials.Add(mat.Name, mat);

                }
                //Check if exists, otherwhise add
                else if (!gsaMaterials.ContainsKey(mat.Name))
                {
                    string id = highestId.ToString();
                    highestId++;
                    SetMaterial(gsa, mat, id);
                    mat.CustomData.Add(Utils.ID, id);
                    gsaMaterials.Add(mat.Name, mat);
                }
                else
                {
                    mat.CustomData.Add(Utils.ID, gsaMaterials[mat.Name].CustomData[Utils.ID]);
                }
            }

        }


        /// <summary>
        /// Create GSA Material
        /// </summary>
        /// <returns></returns>
        public static bool SetMaterial(ComAuto GSA, BHoMM.Material material, string id)
        {
            //id = "";

            string command = "MAT";
            string num = id;//(GSA.GwaCommand("HIGHEST, PROP_SEC") + 1).ToString();
            string mModel = "MAT_ELAS_ISO";
            string name = material.Name;
            string colour = "NO_RGB";
            string type = GetMaterialType(GSA, material).ToString();
            string E = material.YoungsModulus.ToString();
            string nu = material.PoissonsRatio.ToString();
            string rho = material.Density.ToString();
            string alpha = material.CoeffThermalExpansion.ToString();
            string G = material.ShearModulus.ToString();
            string damp = material.DampingRatio.ToString();

            string str = command + "," + num + "," + mModel + "," + name + "," + colour + "," + type + ",6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
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

        static public GsaEnums.MaterialType GetMaterialType(ComAuto GSA, BHoMM.Material material)
        {
            switch (material.Type)
            {
                case BHoMM.MaterialType.Aluminium:
                    return GsaEnums.MaterialType.MT_ALUMINIUM;

                case BHoMM.MaterialType.Concrete:
                    return GsaEnums.MaterialType.MT_CONCRETE;

                case BHoMM.MaterialType.Glass:
                    return GsaEnums.MaterialType.MT_GLASS;

                case BHoMM.MaterialType.Rebar:
                    return GsaEnums.MaterialType.MT_UNDEF;

                case BHoMM.MaterialType.Steel:
                    return GsaEnums.MaterialType.MT_STEEL;

                case BHoMM.MaterialType.Tendon:
                    return GsaEnums.MaterialType.MT_UNDEF;

                case BHoMM.MaterialType.Timber:
                    return GsaEnums.MaterialType.MT_TIMBER;

                default:
                    return GsaEnums.MaterialType.MT_UNDEF;
            }
        }

        /// <summary>
        /// Gets a material string
        /// </summary>
        /// <param name="gsa"></param>
        /// <param name="index">Section property index</param>
        /// <returns></returns>
        static public string GetMaterialString(ComAuto GSA, int index)
        {
            string sResult = GSA.GwaCommand("GET, MAT," + index).ToString();
            return sResult;
        }

        /// <summary>
        /// Gets all materials in a list
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all material properties</returns>
        static public List<string> GetMaterialStringList(ComAuto GSA)
        {
            List<string> sResult = new List<string>();
            bool b = true;
            int i = 1;
            int chkNum = 0;
            int abortNum = 1000; //Amount of "" rows after which to abort

            while (b)
            {
                string iResult = GSA.GwaCommand("GET, MAT," + i).ToString();

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
        /// Gets a specific material property from a string of material properties
        /// </summary>
        /// <param name="sectionProperties"></param>
        /// <param name="index">0 = MAT, 1 = Material index, 2 = Material Model, 3 = Name, 4 = Colour, 5 = Type, 6 = Undef, 7 = Young's modulus, 8 = Poisson's ratio, 9 = Density, 10 = Thermal expansion coefficient, 11 = Shear modulus, 12 = Damping</param>
        /// <returns></returns>
        static public string GetDataStringFromMatStr(string sectionProperties, int index)
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

        public static BHoMM.Material GetMaterialFromGsaString(string gsaString)
        {
            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 11)
                return null;

            BHoMM.MaterialType type = GetTypeFromString(gStr[2]);
            string name = gStr[3];
            double E, v, tC, G, rho;

            if (!double.TryParse(gStr[7], out E))
                return null;
            if (!double.TryParse(gStr[8], out v))
                return null;
            if (!double.TryParse(gStr[10], out tC))
                return null;
            if (!double.TryParse(gStr[11], out G))
                return null;
            if (!double.TryParse(gStr[9], out rho))
                return null;

            BHoMM.Material mat =new BHoM.Materials.Material(name, type, E, v, tC, G, rho);

            mat.CustomData.Add(Utils.ID, gStr[1].ToString());

            return mat;
        }

        private static BHoMM.MaterialType GetTypeFromString(string gsaString)
        {
            switch (gsaString)
            {
                case "MT_ALUMINIUM":
                    return BHoMM.MaterialType.Aluminium;

                case "MT_CONCRETE":
                    return BHoMM.MaterialType.Concrete;

                case "MT_GLASS":
                    return BHoMM.MaterialType.Glass;

                case "MT_STEEL":
                    return BHoMM.MaterialType.Steel;

                case "MT_TIMBER":
                    return BHoMM.MaterialType.Timber;
                    //Undef set to steel for now. Need to implement an undef material enum.
                case "MT_UNDEF":
                default:
                    return BHoMM.MaterialType.Steel;
            }
        }

    }
}
