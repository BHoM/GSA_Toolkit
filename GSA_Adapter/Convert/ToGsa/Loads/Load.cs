/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Collections.Generic;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<string> ToGsaString(this GravityLoad load, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();
            string command = load.IForceTypeString();
            string name = load.Name;
            string list = load.CreateIdListOrGroupName();

            string caseNo = load.Loadcase.Number.ToString();

            string x = load.GravityDirection.X.ToString();
            string y = load.GravityDirection.Y.ToString();
            string z = load.GravityDirection.Z.ToString();

#if GSA_10_1
            string str = command + ", " + name + ", " + list + ", , " + caseNo + "," + x + "," + y + "," + z;
#else
            string str = command + ",," + list + "," + caseNo + "," + x + "," + y + "," + z;
#endif
            forceStrings.Add(str);
            return forceStrings;


        }

        /***************************************************/

        public static List<string> IToGsaString(this ILoad load, double[] unitFactors)
        {
            return ToGsaString(load as dynamic, unitFactors);
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string IsProjectedString(this ILoad load)
        {
            if (load.Projected)
                return "YES";
            else
                return "NO";
        }

        /***************************************************/

        private static string IsGlobal(this ILoad load)
        {
            if (load.Axis == LoadAxis.Global)
                return "GLOBAL";
            else
                return "LOCAL";
        }

        /***************************************************/

        private static List<string> ForceVectorsStrings(BH.oM.Geometry.Vector[] vec, double factor, bool translational, string[] pos)
        {
            List<string> strings = new List<string>();

            if (vec != null)
            {
                string[] dir = Directions(translational);

                if (vec[0].X != 0 || vec[1].X != 0)
                    strings.Add(dir[0] + pos[0] + (factor * vec[0].X).ToString() + pos[1] + (factor * vec[1].X).ToString());
                if (vec[0].Y != 0 || vec[1].Y != 0)
                    strings.Add(dir[1] + pos[0] + (factor * vec[0].Y).ToString() + pos[1] + (factor * vec[1].Y).ToString());
                if (vec[0].Z != 0 || vec[1].Z != 0)
                    strings.Add(dir[2] + pos[0] + (factor * vec[0].Z).ToString() + pos[1] + (factor * vec[1].Z).ToString());
            }
            return strings;
        }

        /***************************************************/

        private static string[] Directions(bool translations)
        {
            if (translations)
                return new string[] { "X", "Y", "Z" };
            else
                return new string[] { "XX", "YY", "ZZ" };
        }

        /***************************************************/

        private static void VectorDataToString(string startStr, BH.oM.Geometry.Vector[] vec, ref List<string> strings, double factor, bool translational, string[] pos)
        {
            foreach (string str in ForceVectorsStrings(vec, factor, translational, pos))
            {
                strings.Add(startStr + "," + str);
            }
        }

        /***************************************************/
    }
}



