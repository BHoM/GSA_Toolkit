/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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


using BH.oM.Common;
using BH.oM.Structure.Requests;
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapter;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {

        /***************************************************/
        /**** Adaptor  Methods                          ****/
        /***************************************************/

        protected override IEnumerable<IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5, ActionConfig actionConfig = null)
        {
            IResultRequest request = Engine.Structure.Create.IResultRequest(type, ids?.Cast<object>(), cases?.Cast<object>(), divisions);

            if (request != null)
                return this.ReadResults(request as dynamic, actionConfig);
            else
                return new List<IResult>();
        }



        /***************************************************/
        /**** Private  Methods - Index checking         ****/
        /***************************************************/

        private void CheckModes(IStructuralResultRequest request)
        {
            //TODO: Handle mode selection

            if (request.Modes != null && request.Modes.Count > 0)
                Engine.Reflection.Compute.RecordWarning("Mode selection is not yet implemented in the GSA_Adapter.");
        }


        private List<int> CheckAndGetAnalysisCaseNumbers(IList cases)
        {
            List<int> loadCases;
            bool raiseMessages = true;
            if (cases == null || cases.Count == 0)
            {
                raiseMessages = false;
                loadCases = new List<int>();
                string sResult;
                int maxIndex = m_gsaCom.GwaCommand("HIGHEST, ANAL");

                for (int i = 1; i <= maxIndex; i++)
                {
                    try
                    {
                        sResult = m_gsaCom.GwaCommand("GET, ANAL," + i).ToString();
                        int number;
                        if (!string.IsNullOrWhiteSpace(sResult) && int.TryParse(m_gsaCom.Arg(1, sResult), out number))
                            loadCases.Add(number);
                    }
                    catch
                    {
                        //Engine.Reflection.Compute.RecordError("Analysis task " + i + "could not be found in the model.");
                    }
                }

            }
            else if (cases is List<int>)
                loadCases = cases as List<int>;
            else if (cases is List<string>)
            {
                loadCases = new List<int>();
                foreach (string ac in cases)
                {
                    string descriptionCase = ac;
                    int idCase = System.Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                    loadCases.Add(idCase);
                }
            }
            else
            {
                loadCases = new List<int>();

                foreach (object o in cases)
                {
                    int id;
                    if (int.TryParse(o.ToString(), out id))
                    {
                        loadCases.Add(id);
                    }
                    else if (o is string)
                    {
                        string s = o as string;
                        if (s.StartsWith("A") && int.TryParse(s.TrimStart('A'), out id))
                            loadCases.Add(id);
                    }
                    else if (o is BH.oM.Structure.Loads.LoadCombination)
                    {
                        loadCases.Add((o as oM.Structure.Loads.LoadCombination).Number);
                    }
                }
            }

            return loadCases.Where(x => CheckAnalysisCaseExists(x, "A" + x.ToString(), raiseMessages) && CheckAnalysisCaseResultsExists(x, "A" + x)).ToList();
        }

        /***************************************************/

        private List<string> CheckAndGetAnalysisCases(IResultRequest request)
        {
            return CheckAndGetAnalysisCases(request.Cases);
        }

        /***************************************************/

        private List<string> CheckAndGetAnalysisCases(IList cases)
        {
            List<string> loadCases;
            bool raiseMessages = true;

            if (cases == null || cases.Count == 0)
            {
                raiseMessages = false;  //Do not rasie messages for cases if all are being pulled.
                loadCases = new List<string>();
                string sResult;
                int maxIndex = m_gsaCom.GwaCommand("HIGHEST, ANAL");
                int maxCombIndex = m_gsaCom.GwaCommand("HIGHEST, COMBINATION");

                for (int i = 1; i <= maxIndex; i++)
                {
                    try
                    {
                        sResult = m_gsaCom.GwaCommand("GET, ANAL," + i).ToString();
                        if(!string.IsNullOrWhiteSpace(sResult))
                            loadCases.Add("A" + m_gsaCom.Arg(1, sResult));
                    }
                    catch
                    {
                        Engine.Reflection.Compute.RecordError("Analysis task " + i + "could not be found in the model.");
                    }
                }

                for (int i = 1; i <= maxCombIndex; i++)
                {
                    try
                    {
                        sResult = m_gsaCom.GwaCommand("GET, COMBINATION," + i).ToString();
                        if (!string.IsNullOrWhiteSpace(sResult))
                            loadCases.Add("C" + m_gsaCom.Arg(1, sResult));
                    }
                    catch
                    {
                        Engine.Reflection.Compute.RecordError("Analysis task " + i + "could not be found in the model.");
                    }
                }
            }
            else if (cases is List<string>)
                loadCases = cases as List<string>;
            else
            {
                loadCases = new List<string>();
                foreach (object o in cases)
                {
                    int idInt;
                    if (int.TryParse(o.ToString(), out idInt))
                    {
                        loadCases.Add("A" + idInt);
                    }
                    else if (o is string)
                    {
                        string s = o as string;
                        if (s.StartsWith("A") || s.StartsWith("C"))
                            loadCases.Add(s);
                    }
                    else if (o is BH.oM.Structure.Loads.LoadCombination)
                    {
                        loadCases.Add("A" + (o as oM.Structure.Loads.LoadCombination).Number);
                    }
                }
            }

            return CheckAnalysisCasesExist(loadCases, true, raiseMessages);
        }

        /***************************************************/
        private List<string> CheckAnalysisCasesExist(List<string> cases, bool checkResults = true, bool raiseMessages = true)
        {
            List<string> checkedCases = new List<string>();
            foreach (string ac in cases)
            {
                string descriptionCase = ac;
                int idCase = System.Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                if (CheckAnalysisCaseExists(idCase, ac, raiseMessages) && (!checkResults || CheckAnalysisCaseResultsExists(idCase, ac)))
                {
                    checkedCases.Add(ac);
                }
            }
            return checkedCases;
        }

        /***************************************************/
        private bool CheckAnalysisCaseExists(int caseId, string caseDescription, bool raiseError = true)
        {

            if (m_gsaCom.CaseExist(caseDescription[0].ToString(), caseId) != 1)
            {
                if(raiseError)
                    Engine.Reflection.Compute.RecordError("Analysis case " + caseDescription + " does not exist.");
                return false;
            }

            return true;

        }

        /***************************************************/

        private bool CheckAnalysisCaseResultsExists(int caseId, string caseDescription)
        {

            if (m_gsaCom.CaseResultsExist(caseDescription[0].ToString(), caseId, 0) != 1)
            {
                Engine.Reflection.Compute.RecordWarning("Analysis case " + caseDescription + " has no results.");
                return false;
            }

            return true;

        }

        /***************************************************/
    }
}

