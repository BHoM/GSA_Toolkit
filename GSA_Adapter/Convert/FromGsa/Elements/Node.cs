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

using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static Node FromGsaNode(string gsaString, Dictionary<int, double[]> dampProp, Dictionary<int, double[]> springValues, Dictionary<int, Basis> axes)
        {
            if (gsaString == "")
            {
                return null;
            }

            string[] arr = gsaString.Split(',');

            int id = Int32.Parse(arr[1]);
            string name = arr[2];

            Point pos = new Point() { X = double.Parse(arr[4]), Y = double.Parse(arr[5]), Z = double.Parse(arr[6]) };

            Basis basis = null;
            if (arr.Length > 8)
            {
                int axesId;

                if (int.TryParse(arr[8], out axesId))
                {
                    axes.TryGetValue(axesId, out basis);
                }
            }

            Constraint6DOF con;
            FromGsaConstraint(arr, springValues, out con);

            Node node = new Node { Position = pos, Support = con, Orientation = basis };
            node.ApplyTaggedName(name);
            node.SetAdapterId(typeof(GSAId), id);
            return node;
        }

        /***************************************************/

        public static Basis FromGsaAxis(string gsaString, out int id)
        {
            if (gsaString == "")
            {
                id = -1;
                return null;
            }

            string[] arr = gsaString.Split(',');

            id = Int32.Parse(arr[1]);

            Vector x = new Vector() { X = double.Parse(arr[7]), Y = double.Parse(arr[8]), Z = double.Parse(arr[9]) };
            Vector y = new Vector() { X = double.Parse(arr[10]), Y = double.Parse(arr[11]), Z = double.Parse(arr[12]) };

            return Engine.Geometry.Create.Basis(x, y);
        }

        public static Constraint6DOF FromGsaConstraint(string[] arr, Dictionary<int, double[]> spingValues, out Constraint6DOF con)
        {
            List<bool> fixities;
            List<double> stiff;

#if GSA_10
            if (arr.Length > 7)
            {
                if (arr[7] == "free")
                    fixities = new List<bool>() { false, false, false, false, false, false };
                else if (arr[7] == "fix")
                    fixities = new List<bool>() { true, true, true, true, true, true };
                else if (arr[7] == "pin")
                    fixities = new List<bool>() { true, true, true, false, false, false };
                else
                {
                    int noX = arr[7].ToCharArray().Where(x => x == 'x').Count();
                    int noY = arr[7].ToCharArray().Where(x => x == 'y').Count();
                    int noZ = arr[7].ToCharArray().Where(x => x == 'z').Count();

                    fixities = new List<bool>()
                    {
                        noX%2 != 0,
                        noY%2 != 0,
                        noZ%2 != 0,
                        noX > 1,
                        noY > 1,
                        noZ > 1,
                    };
                }

                int springIndex = arr.Length > 10 ? int.Parse(arr[10]) : 0;

                if(springIndex != 0)
                    stiff = spingValues[springIndex].ToList();
                else
                    stiff = new List<double>() { 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                 con = null;
                 return con;
            }

            con = Engine.Structure.Create.Constraint6DOF("", fixities, stiff);
            return con;
#else
            if (arr.Length > 9)
            {
                if (arr[9] == "REST")
                {
                    fixities = new List<bool>()
                    {
                        arr[10] == "1",
                        arr[11] == "1",
                        arr[12] == "1",
                        arr[13] == "1",
                        arr[14] == "1",
                        arr[15] == "1",
                    };
                    if (arr.Length > 16 && arr[16] == "STIFF")
                    {
                        stiff = new List<double>()
                            {
                                double.Parse(arr[17]),
                                double.Parse(arr[18]),
                                double.Parse(arr[19]),
                                double.Parse(arr[20]),
                                double.Parse(arr[21]),
                                double.Parse(arr[22])
                            };
                    }
                    else
                        stiff = new List<double>() { 0, 0, 0, 0, 0, 0 };
                }
                else
                {
                    fixities = new List<bool>() { false, false, false, false, false, false };
                    if (arr[10] == "STIFF")
                    {
                        stiff = new List<double>()
                            {
                                double.Parse(arr[11]),
                                double.Parse(arr[12]),
                                double.Parse(arr[13]),
                                double.Parse(arr[14]),
                                double.Parse(arr[15]),
                                double.Parse(arr[16])
                            };
                    }
                    else
                        stiff = new List<double>() { 0, 0, 0, 0, 0, 0 };
                }

                con = Engine.Structure.Create.Constraint6DOF("", fixities, stiff);
            }
            else
                con = null;

            return con;
#endif
        }

        /***************************************************/

    }
}





