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
using GSAE = GSA_Adapter.Structural.Elements;

namespace GSA_Adapter.Structural.Loads
{
    public static class BarLoadIO
    {
        public static bool AddBarLoad(ComAuto gsa, BHL.Load<BHE.Bar> load, double loadFactor, double lengthFactor)
        {
            List<string> forceStrings;// = new List<string>();


            string appliedTo = LoadIO.CreateIdListOrGroupName(gsa, load.Objects);

            if (appliedTo == null)
                return false;

            string caseNo = load.Loadcase.Number.ToString();

            //if (!LoadcaseIO.GetOrCreateLoadCaseId(gsa, load.Loadcase, out caseNo)) { return false; }

            switch (load.LoadType)
            {
                case BHL.LoadType.BarPointLoad:
                    CreateBarPointLoadStrings((BHL.BarPointLoad)load, loadFactor, lengthFactor, appliedTo, caseNo, out forceStrings);
                    break;
                case BHL.LoadType.BarUniformLoad:
                    CreateBarUniformLoadStrings((BHL.BarUniformlyDistributedLoad)load, loadFactor, appliedTo, caseNo, out forceStrings);
                    break;
                case BHL.LoadType.BarVaryingLoad:
                    BHL.BarVaryingDistributedLoad barLoad = load as BHL.BarVaryingDistributedLoad;
                    if (barLoad.DistanceFromA == 0 && barLoad.DistanceFromB == 0)
                        CreateBarLineLoadStrings(barLoad, loadFactor, appliedTo, caseNo, out forceStrings);
                    else
                        CreateBarTriLineLoadStrings(barLoad, loadFactor, lengthFactor, appliedTo, caseNo, out forceStrings);
                    break;
                default:
                    LoadIO.LoadNotImplementedWarning(load.LoadType.ToString());
                    return false;
            }

            foreach (string s in forceStrings)
            {
                dynamic commandResult = gsa.GwaCommand(s);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.CommandFailed(load.LoadType.ToString());
                    return false;
                }
            }

            return true;
        }

        private static bool CreateBarLineLoadStrings(BHL.BarVaryingDistributedLoad load, double loadFactor, string list, string caseNo, out List<string> forceStrings)
        {
            string command = "LOAD_BEAM_LINE";
            //LOAD_BEAM_LINE	name	none 	1	GLOBAL	NO	Z	30	40		

            string name = load.Name;
            string axis = LoadIO.GetAxis(load);
            string proj = LoadIO.CheckProjected(load);

            string str = command + "," + name + "," + list + "," + caseNo + "," + axis + "," + proj;

            forceStrings = new List<string>();

            AddDualForceVectorsStrings(ref forceStrings, str, load.ForceVectorA, load.ForceVectorB, loadFactor, true);
            AddDualForceVectorsStrings(ref forceStrings, str, load.MomentVectorA, load.MomentVectorB, loadFactor, false);


            return true;
        }

        private static void AddDualForceVectorsStrings(ref List<string> strings, string startString, BHG.Vector vec1, BHG.Vector vec2, double factor, bool translational)
        {
            List<string> loadStrings = GetDualForceVectorsStrings(vec1, vec2, factor, translational);

            foreach (string str in loadStrings)
            {
                strings.Add(startString + str);
            }

        }

        public static List<string> GetDualForceVectorsStrings(BHG.Vector vec1, BHG.Vector vec2, double factor, bool translational)
        {
            List<string> strings = new List<string>();

            if (vec1 != null && vec2 != null)
            {
                string[] dir = LoadIO.Directions(translational);

                if (vec1.X != 0 || vec2.X != 0)
                    strings.Add(dir[0] + "," + (factor * vec1.X).ToString() + "," + (factor * vec2.X).ToString());
                if (vec1.Y != 0 || vec2.Y != 0)
                    strings.Add(dir[1] + "," + (factor * vec1.Y).ToString() + "," + (factor * vec2.Y).ToString());
                if (vec1.Z != 0 || vec2.Z != 0)
                    strings.Add(dir[2] + "," + (factor * vec1.Z).ToString() + "," + (factor * vec2.Z).ToString());
            }

            return strings;
        }

        private static void CreateBarTriLineLoadStrings(BHL.BarVaryingDistributedLoad load, double loadFactor, double lengthFactor, string list, string caseNo, out List<string> forceStrings)
        {
            string command = "LOAD_BEAM_TRILIN";
            //LOAD_BEAM_TRILIN	name	none 	1	GLOBAL	NO	Z	1	30	100.00%	70

            throw new NotImplementedException();
        }

        private static bool  CreateBarUniformLoadStrings(BHL.BarUniformlyDistributedLoad load, double loadFactor, string list, string caseNo, out List<string> forceStrings)
        {
            forceStrings = new List<string>();

            string command = "LOAD_BEAM_UDL";
            //LOAD_BEAM_UDL	name	none 	1	GLOBAL	NO	Z	50			
            string name = load.Name;

            string str = command + "," + name + "," + list + "," + caseNo + "," + "GLOBAL" + "," + "NO";

            LoadIO.AddVectorDataToStringSingle(str, load.ForceVector, ref forceStrings, loadFactor, true);
            LoadIO.AddVectorDataToStringSingle(str, load.MomentVector, ref forceStrings, loadFactor, false);

            return true;
        }

        private static void CreateBarPointLoadStrings(BHL.BarPointLoad load, double loadFactor, double lengthFactor, string list, string caseNo, out List<string> forceStrings)
        {
            string command = "LOAD_BEAM_POINT";

            //LOAD_BEAM_POINT	name	2	1	GLOBAL	NO	Z	3	60		
            //LOAD_BEAM_POINT	    	1	1	GLOBAL	NO	Z	1	-50000


            forceStrings = new List<string>();
            string name = load.Name;

            string pos = "," + load.DistanceFromA.ToString();

            string str = command + "," + name + "," + list + "," + caseNo + "," + "GLOBAL" + "," + "NO";

            LoadIO.AddVectorDataToStringSingle(str, load.ForceVector, ref forceStrings, loadFactor, true, pos);
            LoadIO.AddVectorDataToStringSingle(str, load.MomentVector, ref forceStrings, loadFactor, false, pos);

        }

        static public bool AddPreStressLoad(ComAuto gsa, BHL.BarPrestressLoad psLoad)
        {
            string name = psLoad.Name;
            string list = LoadIO.CreateIdListOrGroupName(gsa, psLoad.Objects);
            string caseNo = psLoad.Loadcase.Number.ToString(); ;
            double value = psLoad.PrestressValue;

            string command = "LOAD_BEAM_PRE.2";
            string str;

            str = command + ",," + list + "," + caseNo + "," + value * LoadIO.GetUnitFactor(gsa, GsaEnums.UnitType.FORCE);

            dynamic commandResult = gsa.GwaCommand(str);
            gsa.UpdateViews();

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }

        static public bool AddThermalLoad(ComAuto gsa, BHL.BarTemperatureLoad load)
        {
            string command = "TEMP_BEAM";
            string str;
            string list = LoadIO.CreateIdListOrGroupName(gsa, load.Objects);
            string caseNo = load.Loadcase.Number.ToString();
            string type = "CONS";
            string value = load.TemperatureChange.X.ToString();

            str = command + ",," + list + "," + caseNo + "," + type + "," + value;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.CommandFailed(command);
                return false;
            }
        }
    }
}
