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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using System.Collections.Generic;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<string> ToGsaString(this Load<Bar> barLoad, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();

            string name = barLoad.Name;
            string projection = barLoad.IsProjectedString();
            string axis = barLoad.IsGlobal();
            double factor = barLoad.IFactor(unitFactors);
            string appliedTo = barLoad.CreateIdListOrGroupName();
            Vector[] force = { barLoad.ITranslationVector()[0], barLoad.ITranslationVector()[1] };
            Vector[] moment = { barLoad.IRotationVector()[0], barLoad.IRotationVector()[1] };
            string caseNo = barLoad.Loadcase.Number.ToString();
            string command = barLoad.IForceTypeString();
            string[] pos = { ("," + barLoad.ILoadPosition()[0]), ("," + barLoad.ILoadPosition()[1]) };

            if (appliedTo == null)
                return null;

            string str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis + "," + projection;

            VectorDataToString(str, force, ref forceStrings, factor, true, pos);
            VectorDataToString(str, moment, ref forceStrings, factor, false, pos);

            return forceStrings;
        }


        /***************************************************/

        public static List<string> ToGsaString(this BarPrestressLoad load, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();
            string command = load.IForceTypeString();
            string name = load.Name;
            string list = load.CreateIdListOrGroupName();
            string caseNo = load.Loadcase.Number.ToString();
            double value = load.Prestress;

            string str = command + ",," + list + "," + caseNo + "," + value * unitFactors[(int)UnitType.FORCE];
            forceStrings.Add(str);
            return forceStrings;
        }

        /***************************************************/
        public static List<string> ToGsaString(this BarTemperatureLoad load, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();
            string command = load.IForceTypeString();
            string name = load.Name;
            string list = load.CreateIdListOrGroupName();
            string caseNo = load.Loadcase.Number.ToString();
            string type = "CONS";
            string value = load.TemperatureChange.ToString();

            string str = command + ",," + list + "," + caseNo + "," + type + "," + value;
            forceStrings.Add(str);
            return forceStrings;
        }

        /***************************************************/
        /**** Public Methods - Load positions           ****/
        /***************************************************/

        public static string[] LoadPosition(ILoad load)
        {
            string[] positions = { "", "" };
            return positions;
        }

        /***************************************************/
        public static string[] LoadPosition(BarPointLoad load)
        {
            string[] positions = { load.DistanceFromA.ToString() + ",", "" };
            return positions;
        }

        /***************************************************/

        public static string[] LoadPosition(BarVaryingDistributedLoad load)
        {
            string[] positions = { load.DistanceFromA.ToString() + ",", load.DistanceFromB.ToString() + "," };
            return positions;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static string[] ILoadPosition(this ILoad load)
        {
            return LoadPosition(load as dynamic);
        }

        /***************************************************/
    }
}
