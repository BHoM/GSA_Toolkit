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
using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector[] TranslationVector(this PointForce load)
        {
            Vector[] loadVec = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return loadVec;
        }

        /***************************************************/

        public static Vector[] TranslationVector(this PointDisplacement load)
        {
            Vector[] transVecs = { load.Translation, BH.Engine.Geometry.Create.Vector() };
            return transVecs;
        }

        /***************************************************/

        public static Vector[] TranslationVector(this BarPointLoad load)
        {
            Vector[] forceVecs = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        /***************************************************/

        public static Vector[] TranslationVector(this BarUniformlyDistributedLoad load)
        {
            Vector[] forceVecs = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        /***************************************************/

        public static Vector[] TranslationVector(this BarVaryingDistributedLoad load)
        {
            Vector[] forceVecs = { load.ForceA, load.ForceB };
            return forceVecs;
        }

        /***************************************************/

        public static Vector[] TranslationVector(this AreaUniformalyDistributedLoad load)
        {
            Vector[] forceVecs = { load.Pressure, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector[] ITranslationVector(this ILoad load)
        {
            return TranslationVector(load as dynamic);
        }
    }
}
