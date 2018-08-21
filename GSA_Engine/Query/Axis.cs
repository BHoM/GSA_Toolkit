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

        public static string IsGlobal(this ILoad load)
        {
            if (load.Axis == LoadAxis.Global)
                return "GLOBAL";
            else
                return "LOCAL";
        }
    }
}
