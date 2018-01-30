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

        public static void VectorDataToString(string startStr, BH.oM.Geometry.Vector[] vec, ref List<string> strings, double factor, bool translational, string[] pos)
        {
            foreach (string str in ForceVectorsStrings(vec, factor, translational, pos))
            {
                strings.Add(startStr + "," + str);
            }
        }
    }
}
