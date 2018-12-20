/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHBR = BHoM.Base.Results;
using BHSR = BHoM.Structural.Results;
using GSAUtil = GSA_Adapter.Utility; // not sure if I should do this?

namespace GSA_Adapter.Structural.Results
{
    public static class Utilisation
    {


        public static bool GetSteelBarUtilisation(ComAuto gsa, BHBR.ResultServer<BHSR.SteelUtilisation<int, string, int>> resultServer, List<string> bars, List<string> cases)
        {

            List<BHSR.SteelUtilisation<int, string, int>> steelUtils = new List<BHSR.SteelUtilisation<int, string, int>>();
            int counter = 0;

            bars = CheckAndGetMembers(gsa, bars);
            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);


            foreach (string ac in cases)
            {
                foreach (string bId in bars)
                {
                    GsaResults[] results;
                    if (ExtractSteelBeamUtils(gsa, int.Parse(bId), ac, out results))
                    {
                        BHSR.SteelUtilisation<int, string, int> util = new BHSR.SteelUtilisation<int, string, int>(int.Parse(bId), ac, 0);
                        double[] res = results[0].dynaResults;

                        util.TensionCompressionRatio = res[3];
                        util.MajorShearRatio = res[4];
                        util.MinorShearRatio = res[5];
                        util.TorsionRatio = res[6];
                        util.MajorBendingRatio = res[7];
                        util.MinorBendingRatio = res[8];
                        util.MajorUniformCompressionRatio = res[9];
                        util.MinorUniformCompressionRatio = res[10];
                        util.UniformBendingRatio = res[11];
                        util.BiaxialBendingAxialRatio = res[1];
                        util.MajorUniformBendingCompressionRatio = res[2];
                        util.MinorUniformBendingCompressionRatio = res[2];

                        steelUtils.Add(util);

                        counter++;
                        if (counter % 1000000 == 0 && resultServer.CanStore)
                        {
                            resultServer.StoreData(steelUtils);
                            steelUtils.Clear();
                        }

                    }

                }
            }

            resultServer.StoreData(steelUtils);
            return false;
        }


        static private bool ExtractSteelBeamUtils(ComAuto gsa, int bId, string caseDescription, out GsaResults[] GSAresults)
        {
            int inputFlags = (int)GSAUtil.GsaEnums.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            string sAxis = GSAUtil.GsaEnums.Output_Axis.Local();
            ResHeader header = ResHeader.REF_STL_UTIL;

            int nComp = 0;

            // Get unit factor for extracted results.
            string unitString = gsa.GwaCommand("GET, UNIT_DATA, FORCE");
            string[] unitStrings = unitString.Split(',');

            double unitFactor = Convert.ToDouble(unitStrings[unitStrings.Length - 1].Trim());

            if (gsa.Output_Init_Arr(inputFlags, sAxis, caseDescription, header, 0) != 0)
            {
                GSAUtil.Utils.SendErrorMessage("Initialisation failed");
                GSAresults = null;
                return false;
            }

            try
            {
                if (gsa.Output_Extract_Arr(bId, out GSAresults, out nComp) != 0)
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

        
            return true;
        }

        static public List<string> CheckAndGetMembers(IComAuto gsa, List<string> bars)
        {
            if (bars == null || bars.Count == 0)
            {
                bars = new List<string>();



                int maxIndex = gsa.GwaCommand("HIGHEST, MEMB");

                for (int i = 1; i <= maxIndex; i++)
                {
                    string gsaMemb = gsa.GwaCommand("GET, MEMB," + i).ToString();

                    //TODO: Add check to check for material type for member

                    if (gsaMemb != "")
                        bars.Add(i.ToString());
                }

            }

            return bars;
        }
        
    }
}
