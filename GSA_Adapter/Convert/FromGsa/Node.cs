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

using BH.Engine.Serialiser;
using BH.Engine.Structure;
using BHM = BH.oM.Structure.MaterialFragments;
using BHL = BH.oM.Structure.Loads;
using BHMF = BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using Interop.gsa_8_7;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.Engine.Adapter;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node FromGsaNode(string gsaString)
        {
            if (gsaString == "")
            {
                return null;
            }

            string[] arr = gsaString.Split(',');

            int id = Int32.Parse(arr[1]);
            string name = arr[2];

            Point pos = new Point() { X = double.Parse(arr[4]), Y = double.Parse(arr[5]), Z = double.Parse(arr[6]) };

            Constraint6DOF con;
            if (arr.Length > 7)
            {
                List<bool> fixities;
                List<double> stiff;
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

            Node node = Engine.Structure.Create.Node(pos, "", con);
            node.ApplyTaggedName(name);
            node.CustomData.Add(AdapterIdName, id);
            return node;
            //Node node = new Node { Position = new Point { X = gn.Coor[0], Y = gn.Coor[1], Z = gn.Coor[2] } };
            //node.ApplyTaggedName(gn.Name);
            //node.CustomData.Add(AdapterID, gn.Ref);

            ////Check if the node is restrained in some way
            //if (gn.Restraint != 0 || gn.Stiffness.Sum() != 0)
            //    node.Constraint = GetConstraint(gn.Restraint, gn.Stiffness);

            //return node;
        }

        /***************************************************/

    }
}
