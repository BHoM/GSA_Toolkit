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
using BH.oM.Structure.Loads;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ForceTypeString(this PointLoad load)
        {
            return "LOAD_NODE";
        }

        /***************************************************/

        public static string ForceTypeString(this PointDisplacement load)
        {
            return "DISP_NODE";
        }

        /***************************************************/

        public static string ForceTypeString(this BarPointLoad load)
        {
            return "LOAD_BEAM_POINT";
        }

        /***************************************************/

        public static string ForceTypeString(this BarUniformlyDistributedLoad load)
        {
            return "LOAD_BEAM_UDL";
        }

        /***************************************************/

        public static string ForceTypeString(this BarVaryingDistributedLoad load)
        {
            return "LOAD_BEAM_TRILIN";
        }

        /***************************************************/

        public static string ForceTypeString(this GravityLoad load)
        {
            return "LOAD_GRAVITY.2";
        }

        /***************************************************/

        public static string ForceTypeString(this BarPrestressLoad  load)
        {
            return "LOAD_BEAM_PRE.2";
        }

        /***************************************************/

        public static string ForceTypeString(this BarTemperatureLoad load)
        {
            return "TEMP_BEAM";
        }

        /***************************************************/

        public static string ForceTypeString(this AreaUniformlyDistributedLoad load)
        {
            return "LOAD_2D_FACE";
        }

        /***************************************************/

        //public static string ForceTypeString(this AreaVaryingDistributedLoad load)
        //{
        //    return "LOAD_2D_FACE";
        //}

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static string IForceTypeString(this ILoad load)
        {
            return ForceTypeString(load as dynamic);
        }
    }
}
