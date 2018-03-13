using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structural.Loads;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ForceTypeString(this PointForce load)
        {
            return "LOAD_NODE";
        }

        /***************************************************/

        public static string ForceTypeString(this PointDisplacement load)
        {
            return "DISP_NODE";
        }

        /***************************************************/

        public static string ForceTypeString(this BarPointLoad load)
        {
            return "LOAD_BEAM_POINT";
        }

        /***************************************************/

        public static string ForceTypeString(this BarUniformlyDistributedLoad load)
        {
            return "LOAD_BEAM_UDL";
        }

        /***************************************************/

        public static string ForceTypeString(this BarVaryingDistributedLoad load)
        {
            return "LOAD_BEAM_TRILIN";
        }

        /***************************************************/

        public static string ForceTypeString(this GravityLoad load)
        {
            return "LOAD_GRAVITY.2";
        }

        /***************************************************/

        public static string ForceTypeString(this BarPrestressLoad  load)
        {
            return "LOAD_BEAM_PRE.2";
        }

        /***************************************************/

        public static string ForceTypeString(this BarTemperatureLoad load)
        {
            return "TEMP_BEAM";
        }

        /***************************************************/

        public static string ForceTypeString(this AreaUniformalyDistributedLoad load)
        {
            return "LOAD_2D_FACE";
        }

        /***************************************************/

        //public static string ForceTypeString(this AreaVaryingDistributedLoad load)
        //{
        //    return "LOAD_2D_FACE";
        //}

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static string IForceTypeString(this ILoad load)
        {
            return ForceTypeString(load as dynamic);
        }
    }
}
