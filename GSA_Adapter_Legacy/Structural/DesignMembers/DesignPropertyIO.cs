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
using GSA_Adapter.Utility;
using BHE = BHoM.Structural.Elements;
using BHM = BHoM.Materials;

namespace GSA_Adapter.Structural.DesignMembers
{
    public static class DesignPropertyIO
    {

        public static Dictionary<string, string> CreateSteelGradeDesignProperties(IComAuto gsa, List<BHE.Bar> bars)
        {
            Dictionary<string, string> gradePropRefs = new Dictionary<string, string>();

            List<BHM.Material> materials = bars.Select(x => x.Material).Distinct().ToList();

            int counter = 1;

            foreach (BHM.Material mat in materials)
            {
                if (mat.Type != BHM.MaterialType.Steel)
                    continue;

                if (!gradePropRefs.ContainsKey(mat.Name))
                {
                    CreateDefaultSteelDesignProperty(gsa, mat.Name, counter.ToString());
                    gradePropRefs.Add(mat.Name, counter.ToString());
                    counter++;
                }
                
            }


            return gradePropRefs;
        }

        public static bool CreateDefaultSteelDesignProperty(IComAuto gsa, string steelGrade, string num)
        {
            return CreateSteelDesignProperty(gsa, "Default " + steelGrade + " property", num, steelGrade, "1", "1.2", "CALC", "100%", "100%", "100%");
        }

        public static bool CreateSteelDesignProperty(IComAuto gsa, string name, string num, string steelGrade, string areaRatio, string plastElastRatio, string effLengthSetting, string effLengthMajor, string effLegnthMinor, string effLengthLatTor)
        {
            string command = "STEEL_BEAM_DES";
            string temp = "20";
            string expoType = "EXPO_ALL";
            string expo = "";


            string str = command + "," + num + "," + name + "," + steelGrade + "," + areaRatio + "," + plastElastRatio + "," + effLengthSetting + "," + effLengthMajor + "," + effLegnthMinor + "," + effLengthLatTor + "," + temp + "," + expoType + "," + expo;

            dynamic commandResult = gsa.GwaCommand(str); 

            if (1 == (int)commandResult)
            {
                return true;
            }
            else
            {
                return Utils.CommandFailed(command);
            }
        }
    }
}
