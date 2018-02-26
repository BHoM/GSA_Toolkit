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


        public static string CombinationString(this LoadCombination comb)
        {
            string str = "";
            for (int i = 0; i < comb.LoadCases.Count; i++)
            {
                str += comb.LoadCases[i].Item1.ToString() + "L" + ((Loadcase)comb.LoadCases[i].Item2).Number;

                if (i != comb.LoadCases.Count - 1)
                    str += " + ";
            }

            return str;
        }
    }
}