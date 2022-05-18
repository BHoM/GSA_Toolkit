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

using System;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Elements;
using BH.Engine.Adapters.GSA;
using BH.oM.Adapters.GSA.Elements;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this Spring spring, string index)
        {
            string name = spring.TaggedName().ToGSACleanName();

            string propertyIndex = spring.SpringProperty != null ? spring.SpringProperty.GSAId().ToString() : "1";
            int group = 0;

            string startIndex = spring.StartNode.GSAId().ToString();
            string endIndex = spring.EndNode.GSAId().ToString();
            string dummy = CheckDummy(spring);

            string str = $"EL.4,{index},{name},NO_RGB,SPRING,{propertyIndex},{group},{startIndex},{endIndex},0,{spring.OrientationAngle * 180 / Math.PI}, NO_RLS,0,0,0,0";
            return str;
        }

        /***************************************************/

    }
}



