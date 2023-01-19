/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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


using BH.oM.Structure.Loads;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Collections.Generic;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadCombination FromGsaAnalTask(string gsaString, Dictionary<int, Loadcase> lCases)
        {

            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            List<Tuple<double, ICase>> lCasesForTask = new List<Tuple<double, ICase>>();
            string[] gStr = gsaString.Split(',');
            string[] lCaseArr = gStr[4].Split('+');

            if (gStr.Length < 5)
                return null;

            foreach (string str in lCaseArr)
            {
                string cleanStr = str.Replace(" ", "");
                cleanStr = cleanStr.Replace("L", ",");
                string[] lCaseParam = cleanStr.Split(',');

                if (lCaseParam.Length == 2)
                {
                    if (string.IsNullOrEmpty(lCaseParam[0]))
                        lCaseParam[0] = "1.0";

                    Loadcase templCase;
                    if (!lCases.TryGetValue(int.Parse(lCaseParam[1]), out templCase))
                    {
                        templCase = new Loadcase { Number = int.Parse(lCaseParam[1]), Nature = LoadNature.Other };
                        templCase.SetAdapterId(typeof(GSAId), templCase.Number);
                    }
                    Tuple<double, ICase> loadCase = new Tuple<double, ICase>(double.Parse(lCaseParam[0]), templCase);
                    lCasesForTask.Add(loadCase);
                }
            }

            LoadCombination combo = new LoadCombination { Name = gStr[2], Number = int.Parse(gStr[1]), LoadCases = lCasesForTask };
            combo.SetAdapterId(typeof(GSAId), "A" +combo.Number);
            return combo;
        }
        
        /***************************************************/

    }
}



