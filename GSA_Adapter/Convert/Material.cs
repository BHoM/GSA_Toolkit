using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Materials;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        
        /***************************************/
        private static GsaEnums.MaterialType GetMaterialType(Material material)
        {
            switch (material.Type)
            {
                case MaterialType.Aluminium:
                    return GsaEnums.MaterialType.MT_ALUMINIUM;

                case MaterialType.Concrete:
                    return GsaEnums.MaterialType.MT_CONCRETE;

                case MaterialType.Glass:
                    return GsaEnums.MaterialType.MT_GLASS;

                case MaterialType.Rebar:
                    return GsaEnums.MaterialType.MT_UNDEF;

                case MaterialType.Steel:
                    return GsaEnums.MaterialType.MT_STEEL;

                case MaterialType.Tendon:
                    return GsaEnums.MaterialType.MT_UNDEF;

                case MaterialType.Timber:
                    return GsaEnums.MaterialType.MT_TIMBER;

                default:
                    return GsaEnums.MaterialType.MT_UNDEF;
            }
        }
        
        /***************************************/
        private static MaterialType GetTypeFromString(string gsaString)
        {
            switch (gsaString)
            {
                case "MT_ALUMINIUM":
                    return MaterialType.Aluminium;

                case "MT_CONCRETE":
                    return MaterialType.Concrete;

                case "MT_GLASS":
                    return MaterialType.Glass;

                case "MT_STEEL":
                    return MaterialType.Steel;

                case "MT_TIMBER":
                    return MaterialType.Timber;
                //Undef set to steel for now. Need to implement an undef material enum.
                case "MT_UNDEF":
                default:
                    return MaterialType.Steel;
            }
        }

        /***************************************/
        public static List<Material> GetStandardGsaMaterials()
        {
            List<Material> materials = new List<Material>();
            AddStandardGsaMaterial(ref materials, "STEEL");
            AddStandardGsaMaterial(ref materials, "CONC_SHORT");
            AddStandardGsaMaterial(ref materials, "CONC_LONG");
            AddStandardGsaMaterial(ref materials, "ALUMINIUM");
            AddStandardGsaMaterial(ref materials, "GLASS");
            return materials;
        }

        /***************************************/
        public static void AddStandardGsaMaterial(ref List<Material> materials, string name)
        {
            Material mat = new Material("GSA Standard " + name);
            mat.CustomData.Add(GSAAdapter.ID, name);
            materials.Add(mat);


        }
    }
}
