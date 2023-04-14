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


using BH.oM.Structure.Loads;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Collections.Generic;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Base;
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PointLoad FromGsaNodeLoad(string gsaString, Dictionary<int, Loadcase> lCases, Dictionary<int, Node> nodes, double unitFactor)
        {
            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            //Example gsaString: "LOAD_NODE.2,,7,1,GLOBAL,Z,-24.016"

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 7)
                return null;

            int lCaseNo = int.Parse(gStr[3]);

            Loadcase loadCase;
            if (!lCases.TryGetValue(lCaseNo, out loadCase))
            {
                loadCase = new Loadcase { Number = lCaseNo, Nature = LoadNature.Other };
                loadCase.SetAdapterId(typeof(GSAId), lCaseNo);
            }

            string[] nodeNos = gStr[2].Split(' ');
            string nodeNosClean = nodeNos[0];
            for (int i = 1; i < nodeNos.Length; i++)
            {
                string addNo = nodeNos[i];
                if(nodeNos[i] == "to")
                {
                    addNo = "";
                    int range = int.Parse(nodeNos[i + 1]) - int.Parse(nodeNos[i - 1]);
                    for (int j = 1; j < range; j++)
                    {
                        int number = int.Parse(nodeNos[i - 1]) + j;
                        addNo = addNo + " " + number.ToString();
                    }
                }

                nodeNosClean = nodeNosClean + " " + addNo;
            }

            string[] nodeNosCleanArray = nodeNosClean.Split(' ');

            BHoMGroup<Node> nodeGroup = new BHoMGroup<Node>();

            for (int i = 0; i < nodeNosCleanArray.Length; i++)
            {
                string cleanStr = nodeNosCleanArray[i].Replace(" ", "");
                if (!string.IsNullOrEmpty(cleanStr))
                {
                    Node node;
                    if (!nodes.TryGetValue(int.Parse(cleanStr), out node))
                    {
                        node = new Node();
                        node.SetAdapterId(typeof(GSAId), int.Parse(cleanStr));
                    }
                    nodeGroup.Elements.Add(node);
                }
            }

            double fx = 0;
            double fy = 0;
            double fz = 0;
            double mx = 0;
            double my = 0;
            double mz = 0;
     
            double convertedFactor = double.Parse(gStr[6]) / unitFactor;
            
            if (gStr[5] == "X")
                fx = convertedFactor;
            else if (gStr[5] == "Y")
                fy = convertedFactor;
            else if (gStr[5] == "Z")
                fz = convertedFactor;
            else if (gStr[5] == "XX")
                mx = convertedFactor;
            else if (gStr[5] == "YY")
                my = convertedFactor;
            else if (gStr[5] == "ZZ")
                mz = convertedFactor;

            LoadAxis axis = LoadAxis.Global;
            if (gStr[4] == "LOCAL")
                axis = LoadAxis.Local;

            PointLoad pointLoad = new PointLoad
            {
                Loadcase = loadCase,
                Objects = nodeGroup,
                Force = new Vector { X = fx, Y = fy, Z = fz },
                Moment = new Vector { X = mx, Y = my, Z = mz },
                Axis = axis
            };

            return pointLoad;
        }
    }
}


