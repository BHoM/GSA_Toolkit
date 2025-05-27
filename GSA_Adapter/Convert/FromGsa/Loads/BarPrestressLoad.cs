/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Base;
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BarPrestressLoad FromGsaBarPrestressLoad(string gsaString, Dictionary<int, Loadcase> lCases, Dictionary<int, Bar> bars, double unitFactor)
        {
            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            //Example gsaString: "LOAD_BEAM_PRE,,beam list,load case no,force"

            string[] gStr = gsaString.Split(',');

            if (gStr[0].Trim() == "LOAD_BEAM_PRE.3")
                gStr = gStr.Where((source, index) => index != 2).ToArray();

            if (gStr.Length < 4)
                return null;

            int lCaseNo = int.Parse(gStr[3]);

            Loadcase loadCase;
            if (!lCases.TryGetValue(lCaseNo, out loadCase))
            {
                loadCase = new Loadcase { Number = lCaseNo, Nature = LoadNature.Other };
                loadCase.SetAdapterId(typeof(GSAId), lCaseNo);
            }

            string[] barNos = gStr[2].Split(' ');
            string barNosClean = barNos[0];
            for (int i = 1; i < barNos.Length; i++)
            {
                string addNo = barNos[i];
                if(barNos[i] == "to")
                {
                    addNo = "";
                    int range = int.Parse(barNos[i + 1]) - int.Parse(barNos[i - 1]);
                    for (int j = 1; j < range; j++)
                    {
                        int number = int.Parse(barNos[i - 1]) + j;
                        addNo = addNo + " " + number.ToString();
                    }
                }

                barNosClean = barNosClean + " " + addNo;
            }

            string[] barNosCleanArray = barNosClean.Split(' ');

            BHoMGroup<Bar> barGroup = new BHoMGroup<Bar>();

            for (int i = 0; i < barNosCleanArray.Length; i++)
            {
                string cleanStr = barNosCleanArray[i].Replace(" ", "");
                if (!string.IsNullOrEmpty(cleanStr))
                {
                    Bar bar;
                    if (!bars.TryGetValue(int.Parse(cleanStr), out bar))
                    {
                        bar = new Bar();
                        bar.SetAdapterId(typeof(GSAId), int.Parse(cleanStr));
                    }
                    barGroup.Elements.Add(bar);
                }
            }
     
            double prestressForce = double.Parse(gStr[4]) / unitFactor;

            LoadAxis axis = LoadAxis.Global;
            if (gStr[4] == "LOCAL")
                axis = LoadAxis.Local;

            BarPrestressLoad barPrestressLoad = new BarPrestressLoad
            {
                Prestress = prestressForce,
                Loadcase = loadCase,
                Objects = barGroup,
                Axis = axis,
                Projected = false
            };

            return barPrestressLoad;
        }
    }
}




