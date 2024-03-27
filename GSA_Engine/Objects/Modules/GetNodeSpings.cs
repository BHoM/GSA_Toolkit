/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

namespace BH.Engine.Adapters.GSA.Modules
{
    [Description("Fetch all constraints that contain elastic values from Nodes.")]
    public class GetNodeSpings : IGetDependencyModule<Node, Constraint6DOF>
    {
        public IEnumerable<Constraint6DOF> GetDependencies(IEnumerable<Node> objects)
        {
            List<Constraint6DOF> springConstraints = new List<Constraint6DOF>();
            foreach (Node node in objects)
            {
                double[] values = node?.Support?.ElasticValues();

                if (values != null && values.Any(x => x != 0))
                {
                    springConstraints.Add(node.Support);
                }
            }
            return springConstraints;
        }
    }
}




