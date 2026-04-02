/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
        /**** Private Methods                           ****/
        /***************************************************/

        private static Vector[] TranslationVector(this PointLoad load)
        {
            Vector[] loadVec = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return loadVec;
        }

        /***************************************************/

        private static Vector[] TranslationVector(this PointDisplacement load)
        {
            Vector[] transVecs = { load.Translation, BH.Engine.Geometry.Create.Vector() };
            return transVecs;
        }

        /***************************************************/

        private static Vector[] TranslationVector(this BarPointLoad load)
        {
            Vector[] forceVecs = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        /***************************************************/

        private static Vector[] TranslationVector(this BarUniformlyDistributedLoad load)
        {
            Vector[] forceVecs = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        /***************************************************/

        private static Vector[] TranslationVector(this BarVaryingDistributedLoad load)
        {
            Vector[] forceVecs = { load.ForceAtStart, load.ForceAtEnd };
            return forceVecs;
        }

        /***************************************************/

        private static Vector[] TranslationVector(this AreaUniformlyDistributedLoad load)
        {
            Vector[] forceVecs = { load.Pressure, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        /***************************************************/
        /**** private Methods - Interfaces               ****/
        /***************************************************/

        private static Vector[] ITranslationVector(this ILoad load)
        {
            return TranslationVector(load as dynamic);
        }

        /***************************************************/
    }
}







