using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string AreaLoadTypeString(this AreaUniformalyDistributedLoad load)
        {
            return "CONS";
        }

        /***************************************************/

        //public static string AreaLoadTypeString(this AreaVaryingDistributedLoad load)
        //{
        //    return "GEN";
        //}

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static string IAreaLoadTypeString(this Load<IAreaElement> load)
        {
            return AreaLoadTypeString(load as dynamic);
        }
    }
}
