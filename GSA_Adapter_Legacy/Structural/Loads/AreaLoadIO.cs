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
using BHG = BHoM.Geometry;
using BHE = BHoM.Structural.Elements;
using BHL = BHoM.Structural.Loads;
using BHB = BHoM.Base;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Loads
{
    public static class AreaLoadIO
    {
        /// <summary>
        /// ASSUMES Z DIRECTION CURRENTLY
        /// </summary>
        /// <param name="gsa"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        static public bool AddFaceLoad(ComAuto gsa, BHL.AreaUniformalyDistributedLoad load, double factor)
        {

            string command = "LOAD_2D_FACE";
            string name = load.Name;
            string list = LoadIO.CreateIdListOrGroupName(gsa, load.Objects);
            string caseNo = load.Loadcase.Number.ToString();
            string axis = LoadIO.GetAxis(load);
            string type = "CONS";
            string proj = LoadIO.CheckProjected(load);
            //string dir = "Z";
            //string value = load.Pressure.U.ToString();
            string str;

            //str = command + ",," + list + "," + caseNo + "," + axis + "," + type + "," + proj + "," + dir + "," + value;

            List<string> forceStrings = new List<string>();

            str = command + "," + name + "," + list + "," + caseNo + "," + axis + "," + type + "," + proj;

            LoadIO.AddVectorDataToStringSingle(str, load.Pressure, ref forceStrings, factor, true);

            foreach (string s in forceStrings)
            {
                dynamic commandResult = gsa.GwaCommand(s);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.CommandFailed(command);
                    return false;
                }
            }

            return true;

        }

    }
}
