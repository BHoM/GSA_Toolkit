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

namespace BH.oM.Adapters.GSA.Modules
{
    [Description("Dependency module for fetching all Loadcase stored in a list of Loadcombinations.")]
    public class GetPanelNodes : IGetDependencyModule<Panel, Node>
    {
        public IEnumerable<Node> GetDependencies(IEnumerable<Panel> objects)
        {
            List<Node> nodes = new List<Node>();
            foreach (Panel panel in objects)
            {
                nodes.AddRange(SetNodeFragmentToPanel(panel));
            }
            return nodes;
        }

        private List<Node> SetNodeFragmentToPanel(Panel panel)
        {
            PanelBoundaryNodeFragment fragment = new PanelBoundaryNodeFragment();
            fragment.ExternalNodes = EdgeNodes(panel.ExternalEdges);
            fragment.OpeningNodes = panel.Openings.Select(o => EdgeNodes(o.Edges)).ToList();
            panel.Fragments.AddOrReplace(fragment);
            return fragment.ExternalNodes.Concat(fragment.OpeningNodes.SelectMany(x => x)).ToList();
        }

        private List<Node> EdgeNodes(List<Edge> edges)
        {
            List<Node> nodes = new List<Node>();
            foreach (Edge edge in edges)
            {
                if (edge.Curve is Line)
                    nodes.Add(new Node { Position = (edge.Curve as Line).Start });
                else if (edge is Polyline)
                {
                    foreach (Point point in (edge.Curve as Polyline).ControlPoints.Take((edge.Curve as Polyline).ControlPoints.Count -1))
                    {
                        nodes.Add(new Node { Position = point });
                    }
                }
            }

            return nodes;
        }
    }
}


