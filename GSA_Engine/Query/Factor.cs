using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structure.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Factor(this PointForce load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        public static double Factor(this PointDisplacement load, double[] unitType)
        {
            return unitType[(int)UnitType.LENGTH];
        }

        /***************************************************/

        public static double Factor(this BarPointLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        public static double Factor(this BarUniformlyDistributedLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        public static double Factor(this BarVaryingDistributedLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/
        public static double Factor(this AreaUniformalyDistributedLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/
        //public static double Factor(this AreaVaryingDistributedLoad load, double[] unitType)
        //{
        //    return unitType[(int)UnitType.FORCE];
        //}

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IFactor(this ILoad load, double[] unitType)
        {
            return Factor(load as dynamic, unitType);
        }
    }
}
