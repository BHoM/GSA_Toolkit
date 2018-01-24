using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structural.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        public static string IsProjectedString(this ILoad load)
        {
            if (load.Projected)
                return "YES";
            else
                return "NO";
        }
    }
}
