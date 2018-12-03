using BHM = BH.oM.Common.Materials;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************/
        private static MaterialType GetMaterialType(BHM.Material material)
        {

            switch (material.Type)
            {
                case BHM.MaterialType.Aluminium:
                    return MaterialType.MT_ALUMINIUM;

                case BHM.MaterialType.Concrete:
                    return MaterialType.MT_CONCRETE;

                case BHM.MaterialType.Glass:
                    return MaterialType.MT_GLASS;

                case BHM.MaterialType.Rebar:
                    return MaterialType.MT_REBAR;

                case BHM.MaterialType.Steel:
                    return MaterialType.MT_STEEL;

                case BHM.MaterialType.Tendon:
                    return MaterialType.MT_UNDEF;

                case BHM.MaterialType.Timber:
                    return MaterialType.MT_TIMBER;

                default:
                    return MaterialType.MT_UNDEF;
            }
        }
        
        /***************************************/

        private static BHM.MaterialType GetTypeFromString(string gsaString)
        {
            switch (gsaString)
            {
                case "MT_ALUMINIUM":
                    return BHM.MaterialType.Aluminium;

                case "MT_CONCRETE":
                    return BHM.MaterialType.Concrete;

                case "MT_GLASS":
                    return BHM.MaterialType.Glass;

                case "MT_STEEL":
                    return BHM.MaterialType.Steel;

                case "MT_TIMBER":
                    return BHM.MaterialType.Timber;

                case "MT_REBAR":
                    return BHM.MaterialType.Rebar;
                //Undef set to steel for now. Need to implement an undef material enum.
                case "MT_UNDEF":
                default:
                    return BHM.MaterialType.Steel;
            }
        }

        /***************************************/
    }
}
