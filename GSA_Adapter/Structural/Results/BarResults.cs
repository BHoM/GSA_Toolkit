using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHG = BHoM.Global;
using BHE = BHoM.Structural.Elements;
using BHoMBR = BHoM.Base.Results;
using BHoMSR = BHoM.Structural.Results;
using GSAUtil = GSA_Adapter.Utility; // not sure if I should do this?

namespace GSA_Adapter.Structural.Results
{
    public static class BarResults
    {

        public static bool GetBarDisplacement(ComAuto gsa, BHoMBR.ResultServer<BHoMSR.BarForce<int, string, int>> resultServer, List<string> bars, List<string> cases, int divisions)
        {
            throw new NotImplementedException();
        }

        public static bool GetBarForces(ComAuto gsa, BHoMBR.ResultServer<BHoMSR.BarForce<int,string,int>> resultServer, List<string> bars, List<string> cases, int divisions)
        {
            string message = "";
            List<BHoMSR.BarForce<int, string, int>> barForces = new List<BHoMSR.BarForce<int, string, int>>();
            int counter = 0;

            bars = CheckAndGetBars(gsa, bars);

            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);

            double unitFactor = Utility.Utils.GetUnitFactor(gsa, GSAUtil.GsaEnums.UnitType.FORCE);

            foreach (string ac in cases)
            {

                foreach (string b in bars)
                {
                    int idBar = Int32.Parse(b);
                    List<double[]> beamResults;
                    int idPos = 0; //not sure how to set position ID?
                    if (GetBeamResults(gsa, idBar, ac, out beamResults, ResHeader.REF_FORCE_EL1D, unitFactor, out message))
                    {
                        divisions = beamResults.Count;
                        foreach (double[] br in beamResults)
                        {
                            barForces.Add(new BHoMSR.BarForce<int, string, int>(idBar, ac, idPos, divisions, 1, br[1], br[2], br[3], br[5], br[6], br[7]));
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

        public static bool GetBarStresses(ComAuto gsa, BHoMBR.ResultServer<BHoMSR.BarStress<int, string, int>> resultServer, List<string> bars, List<string> cases, int divisions)
        {
            string message = "";
            List<BHoMSR.BarStress<int, string, int>> barForces = new List<BHoMSR.BarStress<int, string, int>>();
            int counter = 0;



            bars = CheckAndGetBars(gsa, bars);

            double unitFactor = Utility.Utils.GetUnitFactor(gsa, GSAUtil.GsaEnums.UnitType.STRESS);

            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);

            foreach (string ac in cases)
            {

                foreach (string b in bars)
                {
                    int idBar = Int32.Parse(b);
                    List<double[]> beamResults;
                    int idPos = 0; //not sure how to set position ID?
                    if (GetBeamResults(gsa, idBar, ac, out beamResults, ResHeader.REF_STRESS_EL1D, unitFactor, out message))
                    {
                        divisions = beamResults.Count;
                        foreach (double[] br in beamResults)
                        {
                            barForces.Add(new BHoMSR.BarStress<int, string, int>(idBar, ac, idPos, divisions, 1, br[1], br[2],br[3],br[4],br[5],br[6],br[7],br[8],br[9]));
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

        static public bool GetBeamResults(ComAuto gsa, int bId, string caseDescription, out List<double[]> resultsPos, ResHeader header, double unitFactor, out string message)
        {
            GsaResults[] GSAresults;

            if (!ExtractBeamResults(gsa, bId, caseDescription, header, unitFactor, out GSAresults))
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
            List<int> resultIndecies = new List<int>();
            resultIndecies.Add(0);

            double[] resultsSrt = null;
            double[] resultsQtr = null;
            double[] resultsMid = null;
            double[] results3Qr = null;
            double[] resultsEnd = null;

            if (GSAresults.Length == 5)
            {
                resultIndecies.Add(1);
                resultIndecies.Add(2);
                resultIndecies.Add(3);
                message = "";
            }
            else if (GSAresults.Length < 5)
            {
                resultIndecies.Add(-1);
                int indexMid = (GSAresults.Length - 1) / 2;
                resultIndecies.Add(indexMid);
                resultIndecies.Add(-1);
                message = "";
            }
            else
            {
                int indexQrt = (GSAresults.Length - 1) / 4;
                int indexMid = (GSAresults.Length - 1) / 2;
                int index3Qr = indexQrt * 3;

                resultIndecies.Add(indexQrt);
                resultIndecies.Add(indexMid);
                resultIndecies.Add(index3Qr);
                message = "WARNING! A weird number of results was extracted from element. This may indicate a pointload, and Crocodile cannot guarantee that mid load is actually the mid load";
            }

            resultIndecies.Add(GSAresults.Length - 1);


            resultsPos = new List<double[]>();
            double pos = 0;
            for (int i = 0; i < resultIndecies.Count; i++)
            {
                if (resultIndecies[i] >= 0)
                {
                    double[] res = GSAresults[resultIndecies[i]].dynaResults;
                    double[] resPos = new double[res.Length + 1];
                    resPos[0] = pos;

                    for (int j = 0; j < res.Length; j++)
                    {
                        resPos[j + 1] = res[j];
                    }
                    resultsPos.Add(resPos);
                }

                pos += 0.25;
            }


            return true;
        }

        //static private bool SortBeamResultsIntoPositions(GsaResults[] GSAresults, out List<double[]> resultsPos, out string message)
        //{
        //    double indexMid;
        //    double indexQrt;
        //    double index3Qr;
        //    double[] resultsSrt = null;
        //    double[] resultsQtr = null;
        //    double[] resultsMid = null;
        //    double[] results3Qr = null;
        //    double[] resultsEnd = null;

        //    if (GSAresults.Length == 5)
        //    {
        //        resultsSrt = GSAresults[0].dynaResults;
        //        resultsQtr = GSAresults[1].dynaResults;
        //        resultsMid = GSAresults[2].dynaResults;
        //        results3Qr = GSAresults[3].dynaResults;
        //        resultsEnd = GSAresults[4].dynaResults;
        //        message = "";
        //    }
        //    else if (GSAresults.Length < 5)
        //    {
        //        indexMid = (GSAresults.Length - 1) / 2;

        //        resultsSrt = GSAresults[0].dynaResults;
        //        resultsMid = GSAresults[Convert.ToInt32(indexMid)].dynaResults;
        //        resultsEnd = GSAresults[GSAresults.Length - 1].dynaResults;
        //        message = "";
        //    }
        //    else
        //    {
        //        resultsSrt = GSAresults[0].dynaResults;

        //        indexQrt = (GSAresults.Length - 1) / 4;
        //        resultsQtr = GSAresults[Convert.ToInt32(indexQrt)].dynaResults;

        //        indexMid = (GSAresults.Length - 1) / 2;
        //        resultsMid = GSAresults[Convert.ToInt32(indexMid)].dynaResults;

        //        index3Qr = indexQrt * 3;
        //        results3Qr = GSAresults[Convert.ToInt32(indexQrt)].dynaResults;

        //        resultsEnd = GSAresults[GSAresults.Length - 1].dynaResults;

        //        message = "WARNING! A weird number of results was extracted from element. This may indicate a pointload, and Crocodile cannot guarantee that mid load is actually the mid load";
        //    }

        //    resultsPos = new List<double[]>();
        //    resultsPos.Add(new double[] { 0.00, resultsSrt[0], resultsSrt[1], resultsSrt[2], resultsSrt[4], resultsSrt[5], resultsSrt[6] });
        //    if (resultsQtr != null) resultsPos.Add(new double[] { 0.25, resultsQtr[0], resultsQtr[1], resultsQtr[2], resultsQtr[4], resultsQtr[5], resultsQtr[6] });
        //    resultsPos.Add(new double[] { 0.50, resultsMid[0], resultsMid[1], resultsMid[2], resultsMid[4], resultsMid[5], resultsMid[6] });
        //    if (results3Qr != null) resultsPos.Add(new double[] { 0.75, results3Qr[0], results3Qr[1], results3Qr[2], results3Qr[4], results3Qr[5], results3Qr[6] });
        //    resultsPos.Add(new double[] { 1.00, resultsEnd[0], resultsEnd[1], resultsEnd[2], resultsEnd[4], resultsEnd[5], resultsEnd[6] });

        //    return true;
        //}


        static private bool ExtractBeamResults(ComAuto GSA, int bId, string caseDescription, ResHeader header, double unitFactor, out GsaResults[] GSAresults)
        {
            int inputFlags = (int)GSAUtil.GsaEnums.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            string sAxis = GSAUtil.GsaEnums.Output_Axis.Local();
            int nComp = 0;

            // Get unit factor for extracted results.


            if (GSA.Output_Init_Arr(inputFlags, sAxis, caseDescription, header, 0) != 0)
            {
                GSAUtil.Utils.SendErrorMessage("Initialisation failed");
                GSAresults = null;
                return false;
            }

            try
            {
                if (GSA.Output_Extract_Arr(bId, out GSAresults, out nComp) != 0)
                {
                    GSAUtil.Utils.SendErrorMessage("Extraction failed");
                    return false;
                }
            }

            catch (Exception e)
            {
                GSAUtil.Utils.SendErrorMessage(e.Message);
                GSAUtil.Utils.SendErrorMessage("Extraction failed on element " + bId);

                GSAresults = new GsaResults[0];

                return false;
            }

            // Convert to SI
            foreach (GsaResults r in GSAresults)
                for (int i = 0; i < r.dynaResults.Length; i++)
                    r.dynaResults[i] /= unitFactor;

            return true;
        }

        static public bool GetBarCoordinates(ComAuto GSA, BHoMBR.ResultServer<BHoMSR.BarCoordinates> resultServer, List<string> bars)
        {
            List<BHE.Bar> barList = new List<BHE.Bar>();
            if (!GSA_Adapter.Structural.Elements.BarIO.GetBars(GSA, out barList))
                return false;

            List<BHoMSR.BarCoordinates> barCoords = new List<BHoMSR.BarCoordinates>();
            foreach (BHE.Bar bar in barList)
            {
                barCoords.Add(new BHoMSR.BarCoordinates(bar.CustomData[GSAUtil.Utils.ID].ToString(), bar.StartPoint.X, bar.StartPoint.Y, bar.StartPoint.Z, bar.EndPoint.X, bar.EndPoint.Y, bar.EndPoint.Z, bar.SectionProperty.Name, bar.OrientationAngle));
            }

            resultServer.StoreData(barCoords);

            return true;
        }

        static public List<string> CheckAndGetBars(IComAuto gsa, List<string> bars)
        {
            if (bars == null || bars.Count == 0)
            {
                bars = new List<string>();
                int maxIndex = gsa.GwaCommand("HIGHEST, EL");
                int[] potentialBeamRefs = new int[maxIndex];
                //for (int i = 0; i < maxIndex; i++)
                //    potentialBeamRefs[i] = i + 1;



                string barStr = gsa.GwaCommand("GET_ALL, EL");

                foreach (string str in barStr.Split('\n'))
                {
                    string[] barProps = str.Split(',');
                    if (barProps.Length < 1)
                        continue;

                    string type = barProps[4];

                    //Check type
                    if (!(type == "BEAM" || type == "BAR" || type == "TIE" || type == "STRUT"))
                        continue;

                    //Check if dummy
                    if (barProps[barProps.Length - 1] == "DUMMY")
                        continue;

                    bars.Add(barProps[1]);

                }

            }

            return bars;
        }


    }
}
