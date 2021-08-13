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


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<string> ToGsaString(this Node node, string index)
        {
            string name = node.TaggedName().ToGSACleanName();

            string restraint = GetRestraintString(node);
            Point position = node.Position;

            string axisString;
            string axisId = GetAndCreateAxis(node, out axisString);

            List<string> gsaStrings = new List<string>();

            if (!string.IsNullOrWhiteSpace(axisString))
                gsaStrings.Add(axisString);

#if GSA_10_1
            string dampPropString;
            string dampPropId = GetAndCreateDamperProperty(node, out dampPropString);

            if (!string.IsNullOrWhiteSpace(dampPropString))
                gsaStrings.Add(dampPropString);

            //To do: Spring Property, Mass Property

            string str = "NODE.3" + ", " + index + ", " + name + " , NO_RGB, " + position.X + " , " + position.Y + " , " + position.Z + "," + restraint + "," + axisId + ",0,0,0," + dampPropId;
#else
            string str = "NODE.2" + ", " + index + ", " + name + " , NO_RGB, " + position.X + " , " + position.Y + " , " + position.Z + ", NO_GRID, " + axisId + "," + restraint;
#endif
            gsaStrings.Add(str);
            return gsaStrings;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string GetAndCreateAxis(Node node, out string axisString)
        {
            Basis basis = node.Orientation;

            //Check if global orientation
            if (basis == null || (basis.X.Angle(Vector.XAxis) < Tolerance.Angle && (basis.Y.Angle(Vector.YAxis) < Tolerance.Angle)))
            {
                axisString = "";
                return "0";
            }

            //AXIS	1	Axis 1	CART	0.000000	0.000000	0.000000	0.500000	0.500000	0.000000	0.000000	1.00000	0.000000
            string command = "AXIS";
            string id = node.GSAId().ToString();
            string name = $"Node {id} local axis";
            string origin = "CART, 0, 0, 0";
            string x = $"{basis.X.X} , {basis.X.Y} , {basis.X.Z}";
            string y = $"{basis.Y.X} , {basis.Y.Y} , {basis.Y.Z}";

            axisString = $"{command}, {id}, {name}, {origin}, {x}, {y}";
            return id;
        }

            /***************************************************/

            private static string GetAndCreateDamperProperty(Node node, out string dampPropString)
        {
            if (node.Support != null)
            {
                //PROP_DAMP.2	1	Damper Property 1   NO_RGB	AXIAL	0.000000	0.000000	0.000000	0.500000	0.500000	0.000000
                string command = "PROP_DAMP.2";
                string id = node.GSAId().ToString();
                string name = $"Node {id} damper property";
                string stiff = "";
                foreach (double s in node.Support.ElasticValues())
                {
                    stiff += "," + s.ToString();
                }

                dampPropString = $"{command}, {id}, {name}, NO_RGB, GENERAL {stiff}";
                return id;
            }

                dampPropString = null;
                return "0";

        }

        /***************************************************/

        private static string GetRestraintString(Node node)
        {
            if (node.Support != null)
            {
                string rest = "REST";

                bool[] fixities = node.Support.Fixities();
                double[] stiffnesses = node.Support.ElasticValues();

#if GSA_10_1
                rest = (fixities[0] == true) ? "x" : "";
                rest += (fixities[1] == true) ? "y" : "";
                rest += (fixities[2] == true) ? "z" : "";
                rest += (fixities[3] == true) ? "xx" : "";
                rest += (fixities[4] == true) ? "yy" : "";
                rest += (fixities[5] == true) ? "zz" : "";

                if (rest == "xyz")
                    rest = "pin";
                else if (rest == "")
                    rest = "free";
                else if (rest == "xyzxxyyzz")
                    rest = "fix";

                return rest;
            }
            else
                return "free";
#else
                for (int i = 0; i < fixities.Length; i++)
                {
                    rest += "," + (fixities[i] ? 1 : 0);
                }

                rest += ",STIFF";

                for (int i = 0; i < stiffnesses.Length; i++)
                {
                    rest += "," + ((stiffnesses[i] > 0) ? stiffnesses[i] : 0);
                }

                return rest;
            }
            else
                return "NO_REST,NO_STIFF";
#endif


        }

        /***************************************************/

    }
}



