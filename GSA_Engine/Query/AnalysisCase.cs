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

        public static string AnalysisCase(string anal_caseNo, string name, string taskNo, string desc)
        {
            string addCase;
            string command = "ANAL";

            addCase = command
                + "," + anal_caseNo
                + "," + name
                + "," + taskNo
                + "," + desc;

            return addCase;
        }
    }
}