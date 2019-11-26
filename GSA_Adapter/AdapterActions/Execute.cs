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

using System.Collections.Generic;
using System.Collections;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public override bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            string commandUpper = command.ToUpper();

            if (commandUpper == "CLOSE")
                return Close();

            else if (commandUpper == "SAVE")
            {
                string fileName = default(string);
                string[] fileNameStringAlt = {
                    "Filename",
                    "File name",
                    "File_name",
                    "filename",
                    "file name",
                    "file_name",
                    "FileName",
                    "File Name",
                    "File_Name",
                    "FILENAME",
                    "FILE NAME",
                    "FILE_NAME"
                };
                foreach (string str in fileNameStringAlt)
                {
                    if (parameters.ContainsKey(str))
                    {
                        fileName = (string)parameters[str];
                        break;
                    }
                }
                return Save(fileName);
            }

            else if (commandUpper == "CLEARRESULTS" || commandUpper == "DELETERESULTS")
            {
                return ClearResults();
            }

            else if (commandUpper == "ANALYSE" || commandUpper == "RUN")
            {
                IList cases = null;
                string[] caseStringAlt = 
                {
                    "Cases",
                    "CASES",
                    "cases",
                    "LoadCases",
                    "LOADCASES",
                    "loadcases",
                    "Loadcases",
                    "Load Cases",
                    "LOAD CASES",
                    "load cases",
                    "Load cases",
                    "Load_Cases",
                    "LOAD_CASES",
                    "load_cases",
                    "Load_cases"
                };
                foreach (string str in caseStringAlt)
                {
                    object obj;
                    if (parameters.TryGetValue(str, out obj))
                    {
                        cases = obj as IList;
                        break;
                    }
                }
                return Analyse(cases);
            }

            else
                return ComCall(command);
        }


        /***************************************************/

        public bool ClearResults()
        {
            return m_gsaCom.Delete("RESULTS") == 0;
        }

        /***************************************************/

        public bool Close()
        {
            return m_gsaCom.Close() == 0;
        }

        /***************************************************/

        public bool Save(string fileName = null)
        {
            if (fileName == null)
                return m_gsaCom.Save() == 0;
            else
                return m_gsaCom.SaveAs("@"+fileName) == 0;
        }

        /***************************************************/

        public bool Analyse(IList cases = null)
        {
            short res;

            if (cases == null)
            {
                res = m_gsaCom.Analyse();
            }
            else
            {
                res = 0;

                foreach (string c in cases)
                {
                    int num;

                    if (int.TryParse(c, out num))
                        res += m_gsaCom.Analyse(num);
                }
            }
            return res == 0;
        }

        /***************************************************/
    }
}