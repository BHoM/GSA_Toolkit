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

        public static string[] LoadPosition(ILoad load)
        {
            string[] positions = { "", "" };
            return positions;
        }

        /***************************************************/
        public static string[] LoadPosition(BarPointLoad load)
        {
            string[] positions = { load.DistanceFromA.ToString() + ",", "" };
            return positions;
        }

        /***************************************************/

        public static string[] LoadPosition(BarVaryingDistributedLoad load)
        {
            string[] positions = { load.DistanceFromA.ToString() + ",", load.DistanceFromB.ToString() + "," };
            return positions;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static string[] ILoadPosition(this ILoad load)
        {
            return LoadPosition(load as dynamic);
        }

    }
}
