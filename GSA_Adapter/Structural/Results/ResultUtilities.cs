using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;

namespace GSA_Adapter.Structural.Results
{
    public static class ResultUtilities
    {
        public static List<string> CheckAnalysisCasesExist(IComAuto gsa, List<string> cases)
        {
            string message;
            List<string> checkedCases = new List<string>();
            foreach (string ac in cases)
            {
                string descriptionCase = ac;
                int idCase = Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                if (!ResultUtilities.CheckAnalysisCaseExists(gsa, idCase, ac, out message))
                {
                    Utility.Utils.SendErrorMessage(message);
                }
                else
                {
                    checkedCases.Add(ac);
                }
            }
            return checkedCases;
        }

        static public bool CheckAnalysisCaseExists(IComAuto GSA, int caseId, string caseDescription, out string message) //to be mover to AnalysisIO or similar
        {

            if (GSA.CaseExist(caseDescription[0].ToString(), caseId) != 1)
            {
                message = "Error, analysis case " + caseDescription + " does not exist.";
                return false;
            }

            if (GSA.CaseResultsExist(caseDescription[0].ToString(), caseId, 0) != 1)
            {
                message = "Error, analysis case " + caseDescription + " has no results.";
                return false;
            }

            message = "Success";
            return true;

        }

        public static List<string> CheckAndGetAnalysisCases(IComAuto gsa, List<string> cases)
        {
            if (cases == null || cases.Count == 0)
            {
                cases = new List<string>();
                string sResult;
                int maxIndex = gsa.GwaCommand("HIGHEST, ANAL");
                int maxCombIndex = gsa.GwaCommand("HIGHEST, COMBINATION");

                for (int i = 1; i <= maxIndex; i++)
                {
                    try
                    {
                        sResult = gsa.GwaCommand("GET, ANAL," + i).ToString();
                        cases.Add("A" + gsa.Arg(1, sResult));
                    }
                    catch
                    {
                        //Utilities.SendErrorMessage("Analysis task " + i + "could not be found in the model.");
                        //return false;
                    }
                }

                for (int i = 1; i <= maxCombIndex; i++)
                {
                    try
                    {
                        sResult = gsa.GwaCommand("GET, COMBINATION," + i).ToString();
                        cases.Add("C" + gsa.Arg(1, sResult));
                    }
                    catch
                    {
                        //Utilities.SendErrorMessage("Analysis task " + i + "could not be found in the model.");
                        //return false;
                    }
                }
            }

            return CheckAnalysisCasesExist(gsa, cases);
        }

    }
}
