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

using BH.oM.Base;
using BH.Engine.Serialiser;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Structure.Constraints;
using System;
using System.Collections.Generic;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CustomObject FromGsaAxis(string gsaString)
        {
            //AXIS	1	Axis 1	CART	0	0	0	1	0	0	0	1	0

            if (gsaString == "")
            {
                return null;
            }

            string[] arr = gsaString.Split(',');

            if (arr.Length < 14)
                return null;

            if (arr[3] != "CART")
            {
                Engine.Reflection.Compute.RecordWarning("The GSA Adapter currently only supports axis systems in cartesian coordinates.");
                return null;
            }

            int id = Int32.Parse(arr[1]);
            string name = arr[2];

            Point pos = new Point() { X = double.Parse(arr[5]), Y = double.Parse(arr[6]), Z = double.Parse(arr[7]) };
            Vector x = new Vector { X = double.Parse(arr[8]), Y = double.Parse(arr[9]), Z = double.Parse(arr[10]) };
            Vector xy = new Vector { X = double.Parse(arr[11]), Y = double.Parse(arr[12]), Z = double.Parse(arr[13]) };

            CustomObject obj = new CustomObject();
            obj.Name = name;
            obj.CustomData[AdapterIdName] = id;
            obj.CustomData["Axis"] = Engine.Geometry.Create.CartesianCoordinateSystem(pos, x, xy);
            return obj;
        }

        /***************************************************/

    }
}
