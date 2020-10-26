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

using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using Interop.gsa_8_7;
using System;
using System.Collections.Generic;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Bar> FromGsaBars(IEnumerable<GsaElement> gsaElements, Dictionary<string, ISectionProperty> secProps, Dictionary<string, Node> nodes)
        {
            List<Bar> barList = new List<Bar>();

            foreach (GsaElement gsaBar in gsaElements)
            {

                BarFEAType feType;

                switch (gsaBar.eType)
                {
                    case 1:
                        feType = BarFEAType.Axial;
                        break;
                    case 2:
                        feType = BarFEAType.Flexural;
                        break;
                    case 20:
                        feType = BarFEAType.CompressionOnly;
                        break;
                    case 21:
                        feType = BarFEAType.TensionOnly;
                        break;
                    default:
                        continue;
                }

                Node n1, n2;
                nodes.TryGetValue(gsaBar.Topo[0].ToString(), out n1);
                nodes.TryGetValue(gsaBar.Topo[1].ToString(), out n2);

                Bar bar = new Bar { StartNode = n1, EndNode = n2 };
                bar.ApplyTaggedName(gsaBar.Name);


                bar.FEAType = feType;

                bar.OrientationAngle = gsaBar.Beta;

                ISectionProperty prop;
                secProps.TryGetValue(gsaBar.Property.ToString(), out prop);

                bar.SectionProperty = prop;

                int id = gsaBar.Ref;
                bar.SetAdapterId(typeof(GSAId), id);

                barList.Add(bar);

            }
            return barList;
        }

        /***************************************************/

        public static List<Bar> FromGsaBars(IEnumerable<string> gsaStrings, Dictionary<string, ISectionProperty> secProps, Dictionary<string, Node> nodes, List<string> ids)
        {
            List<Bar> barList = new List<Bar>();

            bool checkId = ids != null;

            foreach (string gsaBar in gsaStrings)
            {

                string[] arr = gsaBar.Split(',');

                string index = arr[1];

                if (checkId && !ids.Contains(index))
                    continue;

                BarFEAType feType;

                switch (arr[4])
                {
                    case "BEAM":
                        feType = BarFEAType.Flexural;
                        break;
                    case "BAR":
                        feType = BarFEAType.Axial;
                        break;
                    case "TIE":
                        feType = BarFEAType.TensionOnly;
                        break;
                    case "STRUT":
                        feType = BarFEAType.CompressionOnly;
                        break;
                    default:
                        continue;
                }


                Bar bar = new Bar()
                {
                    StartNode = nodes[arr[7]],
                    EndNode = nodes[arr[8]],
                    FEAType = feType,
                };

                bar.ApplyTaggedName(arr[2]);

                ISectionProperty prop;
                if (secProps.TryGetValue(arr[5], out prop))
                    bar.SectionProperty = prop;

                if (arr.Length > 10)
                    bar.OrientationAngle = double.Parse(arr[10]) / 180 * Math.PI; //From degrees to radians
                else
                    bar.OrientationAngle = 0;

                Constraint6DOF startConst, endConst;

                if (arr.Length > 13 && arr[11] == "RLS")
                {
                    List<bool> fixities = new List<bool>();
                    List<double> values = new List<double>();

                    int nbSprings = 0;

                    foreach (char c in arr[12])
                    {
                        if (c == 'F')
                        {
                            fixities.Add(true);
                            values.Add(0);
                        }
                        else if (c == 'R')
                        {
                            fixities.Add(false);
                            values.Add(0);
                        }
                        else if (c == 'K')
                        {
                            fixities.Add(false);
                            nbSprings++;
                            values.Add(double.Parse(arr[12 + nbSprings]));
                        }
                    }

                    startConst = Engine.Structure.Create.Constraint6DOF("", fixities, values);

                    fixities = new List<bool>();
                    values = new List<double>();

                    foreach (char c in arr[13 + nbSprings])
                    {
                        if (c == 'F')
                        {
                            fixities.Add(true);
                            values.Add(0);
                        }
                        else if (c == 'R')
                        {
                            fixities.Add(false);
                            values.Add(0);
                        }
                        else if (c == 'K')
                        {
                            fixities.Add(false);
                            nbSprings++;
                            values.Add(double.Parse(arr[13 + nbSprings]));
                        }
                    }

                    endConst = Engine.Structure.Create.Constraint6DOF("", fixities, values);
                }
                else
                {
                    startConst = Engine.Structure.Create.FixConstraint6DOF();
                    endConst = Engine.Structure.Create.FixConstraint6DOF();
                }

                bar.Release = new BarRelease() { StartRelease = startConst, EndRelease = endConst };

                int id = int.Parse(arr[1]);
                bar.SetAdapterId(typeof(GSAId), id);

                barList.Add(bar);
            }
            return barList;
        }

        /***************************************/
        
    }
}
