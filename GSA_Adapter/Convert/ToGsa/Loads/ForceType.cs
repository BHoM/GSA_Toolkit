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


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ForceTypeString(this PointLoad load)
        {
            return "LOAD_NODE";
        }

        /***************************************************/

        private static string ForceTypeString(this PointDisplacement load)
        {
            return "DISP_NODE";
        }

        /***************************************************/

        private static string ForceTypeString(this BarPointLoad load)
        {
            return "LOAD_BEAM_POINT";
        }

        /***************************************************/

        private static string ForceTypeString(this BarUniformlyDistributedLoad load)
        {
            return "LOAD_BEAM_UDL";
        }

        /***************************************************/

        private static string ForceTypeString(this BarVaryingDistributedLoad load)
        {
            return "LOAD_BEAM_PATCH";
        }

        /***************************************************/

        private static string ForceTypeString(this GravityLoad load)
        {
            return "LOAD_GRAVITY.2";
        }

        /***************************************************/

        private static string ForceTypeString(this BarPrestressLoad load)
        {
            return "LOAD_BEAM_PRE.2";
        }

        /***************************************************/

        private static string ForceTypeString(this BarUniformTemperatureLoad load)
        {
#if GSA_10_1
            return "LOAD_1D_THERMAL.2";
#else
            return "TEMP_BEAM";
#endif
        }

        /***************************************************/

        private static string ForceTypeString(this AreaUniformlyDistributedLoad load)
        {
            return "LOAD_2D_FACE";
        }

        /***************************************************/

        //private static string ForceTypeString(this AreaVaryingDistributedLoad load)
        //{
        //    return "LOAD_2D_FACE";
        //}

        /***************************************************/
        /**** private Methods - Interfaces               ****/
        /***************************************************/

        private static string IForceTypeString(this ILoad load)
        {
            return ForceTypeString(load as dynamic);
        }

        /***************************************************/

    }
}






