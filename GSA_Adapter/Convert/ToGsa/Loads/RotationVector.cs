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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        private static Vector[] RotationVector(this PointLoad load)
        {
            Vector[] momentVecs = { load.Moment, BH.Engine.Geometry.Create.Vector() };
            return momentVecs;
        }

        /***************************************************/

        private static Vector[] RotationVector(this PointDisplacement load)
        {
            Vector[] rotVecs = { load.Rotation, BH.Engine.Geometry.Create.Vector() };
            return rotVecs;
        }

        /***************************************************/

        private static Vector[] RotationVector(this BarPointLoad load)
        {
            Vector[] momentVecs = { load.Moment, BH.Engine.Geometry.Create.Vector() };
            return momentVecs;
        }

        /***************************************************/

        private static Vector[] RotationVector(this BarUniformlyDistributedLoad load)
        {
            Vector[] momentVecs = { load.Moment, BH.Engine.Geometry.Create.Vector() };
            return momentVecs;
        }

        /***************************************************/

        private static Vector[] RotationVector(this BarVaryingDistributedLoad load)
        {
            Vector[] momentVecs = { load.MomentAtStart, load.MomentAtEnd};
            return momentVecs;
        }

        /***************************************************/
        /**** private Methods - Interfaces              ****/
        /***************************************************/

        private static Vector[] IRotationVector(this ILoad load)
        {
            return RotationVector(load as dynamic);
        }

        /***************************************************/
    }
}

