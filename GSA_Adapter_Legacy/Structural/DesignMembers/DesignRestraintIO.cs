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

namespace GSA_Adapter.Structural.DesignMembers
{
    public static class DesignRestraintIO
    {

        /// <summary>
        /// Creates default designmember restraints. numbered: 1 Fix-Fix, 2 Pin-Pin, 3 Cantileaver start, 4 Cantileaver end
        /// </summary>
        /// <param name="gsa"></param>
        public static void CreateDefaultDesignRestraintProperties(IComAuto gsa)
        {
            string startFix = "DES_NODE,1,0,enc";
            string endFix = "DES_NODE,0,0,enc";
            string startPin = "DES_NODE,1,0,F12L TR MAJV MINV";
            string endPin = "DES_NODE,0,0,F12L TR MAJV MINV";

            List<string> restraintDesc = new List<string>();

            //Create fix-fix
            restraintDesc.Add(startFix);
            restraintDesc.Add(endFix);
            CreateSteelDesignRestrainProperty(gsa, "1", "Fix-Fix", restraintDesc);

            restraintDesc.Clear();

            //Create Pin-Pin
            restraintDesc.Add(startPin);
            restraintDesc.Add(endPin);
            CreateSteelDesignRestrainProperty(gsa, "2", "Pin-Pin", restraintDesc);

            restraintDesc.Clear();

            //Create cantileaver start
            restraintDesc.Add(startFix);
            CreateSteelDesignRestrainProperty(gsa, "3", "Cantileaver start", restraintDesc);

            restraintDesc.Clear();

            //Create cantileaver end
            restraintDesc.Add(endFix);
            CreateSteelDesignRestrainProperty(gsa, "4", "Cantileaver end", restraintDesc);

            restraintDesc.Clear();

        }


        public static bool CreateSteelDesignRestrainProperty(IComAuto gsa, string num, string name, List<string> restraintDescriptions)
        {
            string command = "DESIGN_STEEL_REST_PROP";

            string str = command + "," + num + "," + name +  "," + restraintDescriptions.Count + ",";

            foreach (string s in restraintDescriptions)
            {
                str += s + ",";
            }

            str = str.Trim(',');

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
