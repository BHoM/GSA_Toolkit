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

        public static string TaskType(LoadCombination comb)
        {
            if (comb.CustomData.ContainsKey("Task Type"))
                return comb.CustomData["Task Type"].ToString();

            return "STATIC";
        }
    }
}
