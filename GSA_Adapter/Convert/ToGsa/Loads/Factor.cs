/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static double Factor(this PointLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        private static double Factor(this PointDisplacement load, double[] unitType)
        {
            return unitType[(int)UnitType.LENGTH];
        }

        /***************************************************/

        private static double Factor(this BarPointLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        private static double Factor(this BarUniformlyDistributedLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        private static double Factor(this BarVaryingDistributedLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/

        private static double Factor(this AreaUniformlyDistributedLoad load, double[] unitType)
        {
            return unitType[(int)UnitType.FORCE];
        }

        /***************************************************/
        //private static double Factor(this AreaVaryingDistributedLoad load, double[] unitType)
        //{
        //    return unitType[(int)UnitType.FORCE];
        //}

        /***************************************************/
        /**** private Methods - Interfaces              ****/
        /***************************************************/

        private static double IFactor(this ILoad load, double[] unitType)
        {
            return Factor(load as dynamic, unitType);
        }

        /***************************************************/
    }
}


