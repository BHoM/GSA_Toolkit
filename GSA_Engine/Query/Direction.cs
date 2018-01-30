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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string[] Directions(bool translations)
        {
            if (translations)
                return new string[] { "X", "Y", "Z" };
            else
                return new string[] { "XX", "YY", "ZZ" };
        }
    }
}
