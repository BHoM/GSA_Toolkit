using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMP = BHoM.Structural.Properties;
using BHoMM = BHoM.Materials;
using BHoMD = BHoM.Structural.Databases;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Properties
{
    /// <summary>
    /// GSA material class, for all material objects and operations
    /// </summary>
    public class MaterialIO
    {
        /// <summary>
        /// Create GSA Material
        /// </summary>
        /// <returns></returns>
        public static bool SetMaterial(ComAuto GSA, BHoMM.Material material, out string id)
        {
            id = "";

            string command = "MAT";
            string num = (GSA.GwaCommand("HIGHEST, PROP_SEC") + 1).ToString();
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

        static public Utils.GsaEnums.MaterialType GetMaterialType(ComAuto GSA, BHoMM.Material material)
        {
            switch (material.Type)
            {
                case BHoMM.MaterialType.Aluminium:
                    return Utils.GsaEnums.MaterialType.MT_ALUMINIUM;

                case BHoMM.MaterialType.Concrete:
                    return Utils.GsaEnums.MaterialType.MT_CONCRETE;

                case BHoMM.MaterialType.Glass:
                    return Utils.GsaEnums.MaterialType.MT_GLASS;

                case BHoMM.MaterialType.Rebar:
                    return Utils.GsaEnums.MaterialType.MT_UNDEF;

                case BHoMM.MaterialType.Steel:
                    return Utils.GsaEnums.MaterialType.MT_STEEL;

                case BHoMM.MaterialType.Tendon:
                    return Utils.GsaEnums.MaterialType.MT_UNDEF;

                case BHoMM.MaterialType.Timber:
                    return Utils.GsaEnums.MaterialType.MT_TIMBER;

                default:
                    return Utils.GsaEnums.MaterialType.MT_UNDEF;
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
    }
}
