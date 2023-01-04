/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

namespace BH.Engine.Adapters.GSA.Modules
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
                nodes.AddRange(IEdgeNodes(edge.Curve));
            }

            return nodes;
        }

        private List<Node> EdgeNodes(Line line)
        {
            return new List<Node> { new Node { Position = line.Start } };
        }

        private List<Node> EdgeNodes(Polyline polyLine)
        {
            return polyLine.ControlPoints.Take(polyLine.ControlPoints.Count - 1).Select(p => new Node() { Position = p }).ToList();
        }

        private List<Node> EdgeNodes(PolyCurve curve)
        {
            return curve.Curves.SelectMany(c => IEdgeNodes(c)).ToList();
        }

        private List<Node> IEdgeNodes(ICurve curve)
        {
            return EdgeNodes(curve as dynamic);
        }

        private List<Node> EdgeNodes(ICurve curve)
        {
            return new List<Node>();
        }
    }
}



