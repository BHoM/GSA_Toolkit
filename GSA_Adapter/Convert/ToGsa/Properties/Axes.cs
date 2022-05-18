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

using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.Collections.Generic;
using BH.Engine.Adapters.GSA;
using BH.Engine.Base;
using BH.oM.Adapters.GSA.Fragments;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(Axes axes, string index)
        {
            if (axes == null)
                return "";

            Basis basis = axes.Orientation;

            //Check if global orientation
            if (basis == null || (basis.X.Angle(Vector.XAxis) < Tolerance.Angle && (basis.Y.Angle(Vector.YAxis) < Tolerance.Angle)))
            {
                return "";
            }

            //AXIS	1	Axis 1	CART	0.000000	0.000000	0.000000	0.500000	0.500000	0.000000	0.000000	1.00000	0.000000
            string command = "AXIS";
            string id = index;
            string name = axes.Name ?? "";
            string origin = "CART, 0, 0, 0";
            string x = $"{basis.X.X} , {basis.X.Y} , {basis.X.Z}";
            string y = $"{basis.Y.X} , {basis.Y.Y} , {basis.Y.Z}";

            return $"{command}, {id}, {name}, {origin}, {x}, {y}";
        }

            /***************************************************/


    }
}



