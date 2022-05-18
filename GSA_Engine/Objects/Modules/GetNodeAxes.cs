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

using BH.oM.Base;
using BH.oM.Adapter;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using System.Collections;
using BH.oM.Adapters.GSA.Fragments;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.Engine.Structure;
using BH.Engine.Geometry;

namespace BH.Engine.Adapters.GSA.Modules
{
    [Description("Fetch all constraints that contain elastic values from Nodes.")]
    public class GetNodeAxes : IGetDependencyModule<Node, Axes>
    {
        public IEnumerable<Axes> GetDependencies(IEnumerable<Node> objects)
        {
            List<Axes> nodeAxes = new List<Axes>();
            foreach (Node node in objects)
            {
                Basis orientation = node?.Orientation;
                if (orientation == null || (orientation.X.Angle(Vector.XAxis) < Tolerance.Angle && (orientation.Y.Angle(Vector.YAxis) < Tolerance.Angle)))
                    continue;

                Axes fragment = new Axes { Orientation = orientation };
                node.Fragments.AddOrReplace(fragment);
                nodeAxes.Add(fragment);
            }
            return nodeAxes;
        }
    }
}


