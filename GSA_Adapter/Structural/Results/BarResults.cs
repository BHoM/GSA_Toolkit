using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMBR = BHoM.Base.Results;
using BHoMSR = BHoM.Structural.Results;
using GSAUtil = GSA_Adapter.Utility.Utils; // not sure if I should do this?

namespace GSA_Adapter.Structural.Results
{
    public static class BarResults
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GSAApp"></param>
        /// <param name="resultServer"></param>
        /// <param name="bars"></param>
        /// <param name="cases"></param>
        /// <param name="divisions"></param>
        /// <returns></returns>
        public static bool GetBarForces(ComAuto gsa, BHoMBR.ResultServer<BHoMSR.BarForce<int,string,int>> resultServer, List<string> bars, List<string> cases, int divisions)
        {
            string message = "";
            List<BHoMSR.BarForce<int, string, int>> barForces = new List<BHoMSR.BarForce<int, string, int>>();
            int counter = 0;

            if (bars == null || bars.Count == 0)
            {
                bars = new List<string>();
                int maxIndex = gsa.GwaCommand("HIGHEST, EL");
                int[] potentialBeamRefs = new int[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                    potentialBeamRefs[i] = i + 1;

                GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
                gsa.Elements(potentialBeamRefs, out gsaElements);

                int nobeams = gsaElements.Length;
                foreach (GsaElement e in gsaElements)
                {
                    bars.Add(e.Ref.ToString());
                }
            }

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


            foreach (string ac in cases)
            {
                string descriptionCase = ac;
                int idCase = Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                if (!CheckAnalysisCaseExists(gsa, idCase, ac, out message))
                {
                    GSAUtil.SendErrorMessage(message);
                    break;
                } 


                foreach (string b in bars)
                {
                    int idBar = Int32.Parse(b);
                    List<double[]> beamResults;
                    int idPos = 0; //not sure how to set position ID?
                    if (GetBeamResults(gsa, idBar, ac, out beamResults, out message))
                    {
                        divisions = beamResults.Count;
                        foreach (double[] br in beamResults)
                        {
                            barForces.Add(new BHoMSR.BarForce<int, string, int>(idBar, descriptionCase, idPos, divisions, 1, br[1], br[2], br[3], br[4], br[5], br[6]));
                            idPos++;
                            counter++;
                            if (counter % 1000000 == 0 && resultServer.CanStore)
                            {
                                resultServer.StoreData(barForces);
                                barForces.Clear();
                            }
                        }
                    }
                        
                }

            }
            resultServer.StoreData(barForces);
            return true;
        }

        static public bool GetBeamResults(ComAuto gsa, int bId, string caseDescription, out List<double[]> resultsPos, out string message)
        {
            GsaResults[] GSAresults;

            if (!ExtractBeamResults(gsa, bId, caseDescription, out GSAresults))
            {
                resultsPos = new List<double[]>();
                resultsPos.Add(new double[] { -1 });
                message = "Beam result extraction failed";
                return false;
            }

            SortBeamResultsIntoPositions(GSAresults, out resultsPos, out message);
            return true;
        }

        static private bool SortBeamResultsIntoPositions(GsaResults[] GSAresults, out List<double[]> resultsPos, out string message)
        {
            double indexMid;
            double indexQrt;
            double index3Qr;
            double[] resultsSrt = null;
            double[] resultsQtr = null;
            double[] resultsMid = null;
            double[] results3Qr = null;
            double[] resultsEnd = null;

            if (GSAresults.Length == 5)
            {
                resultsSrt = GSAresults[0].dynaResults;
                resultsQtr = GSAresults[1].dynaResults;
                resultsMid = GSAresults[2].dynaResults;
                results3Qr = GSAresults[3].dynaResults;
                resultsEnd = GSAresults[4].dynaResults;
                message = "";
            }
            else if (GSAresults.Length < 5)
            {
                indexMid = (GSAresults.Length - 1) / 2;

                resultsSrt = GSAresults[0].dynaResults;
                resultsMid = GSAresults[Convert.ToInt32(indexMid)].dynaResults;
                resultsEnd = GSAresults[GSAresults.Length - 1].dynaResults;
                message = "";
            }
            else
            {
                resultsSrt = GSAresults[0].dynaResults;

                indexQrt = (GSAresults.Length - 1) / 4;
                resultsQtr = GSAresults[Convert.ToInt32(indexQrt)].dynaResults;

                indexMid = (GSAresults.Length - 1) / 2;
                resultsMid = GSAresults[Convert.ToInt32(indexMid)].dynaResults;

                index3Qr = indexQrt * 3;
                results3Qr = GSAresults[Convert.ToInt32(indexQrt)].dynaResults;

                resultsEnd = GSAresults[GSAresults.Length - 1].dynaResults;

                message = "WARNING! A weird number of results was extracted from element. This may indicate a pointload, and Crocodile cannot guarantee that mid load is actually the mid load";
            }

            resultsPos = new List<double[]>();
            resultsPos.Add(new double[] { 0.00, resultsSrt[0], resultsSrt[1], resultsSrt[2], resultsSrt[4], resultsSrt[5], resultsSrt[6] });
            if (resultsQtr != null) resultsPos.Add(new double[] { 0.25, resultsQtr[0], resultsQtr[1], resultsQtr[2], resultsQtr[4], resultsQtr[5], resultsQtr[6] });
            resultsPos.Add(new double[] { 0.50, resultsMid[0], resultsMid[1], resultsMid[2], resultsMid[4], resultsMid[5], resultsMid[6] });
            if (results3Qr != null) resultsPos.Add(new double[] { 0.75, results3Qr[0], results3Qr[1], results3Qr[2], results3Qr[4], results3Qr[5], results3Qr[6] });
            resultsPos.Add(new double[] { 1.00, resultsEnd[0], resultsEnd[1], resultsEnd[2], resultsEnd[4], resultsEnd[5], resultsEnd[6] });

            return true;
        }
        static private bool ExtractBeamResults(ComAuto GSA, int bId, string caseDescription, out GsaResults[] GSAresults)
        {
            int inputFlags = (int)GSAUtil.GsaEnums.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            string sAxis = GSAUtil.GsaEnums.Output_Axis.Local();
            ResHeader header = ResHeader.REF_FORCE_EL1D;

            int nComp = 0;

            // Get unit factor for extracted results.
            string unitString = GSA.GwaCommand("GET, UNIT_DATA, FORCE");
            string[] unitStrings = unitString.Split(',');

            double unitFactor = Convert.ToDouble(unitStrings[unitStrings.Length - 1].Trim());

            if (GSA.Output_Init_Arr(inputFlags, sAxis, caseDescription, header, 0) != 0)
            {
                GSAUtil.SendErrorMessage("Initialisation failed");
                GSAresults = null;
                return false;
            }

            try
            {
                if (GSA.Output_Extract_Arr(bId, out GSAresults, out nComp) != 0)
                {
                    GSAUtil.SendErrorMessage("Extraction failed");
                    return false;
                }
            }

            catch (Exception e)
            {
                GSAUtil.SendErrorMessage(e.Message);
                GSAUtil.SendErrorMessage("Extraction failed on element " + bId);

                GSAresults = new GsaResults[0];

                return false;
            }

            // Convert to SI
            foreach (GsaResults r in GSAresults)
                for (int i = 0; i < r.dynaResults.Length; i++)
                    r.dynaResults[i] /= unitFactor;

            return true;
        }

        static public bool CheckAnalysisCaseExists(ComAuto GSA, int caseId, string caseDescription, out string message) //to be mover to AnalysisIO or similar
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

    }
}
