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
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(this Node node, string index)
        {
            string command = "NODE.2";
            string name = node.TaggedName().ToGSACleanName();

            string restraint = GetRestraintString(node);
            Point position = node.Position();
            string str = command + ", " + index + ", " + name + " , NO_RGB, " + position.X + " , " + position.Y + " , " + position.Z + ", NO_GRID, " + 0 + "," + restraint;
            return str;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string GetRestraintString(Node node)
        {
            if (node.Support != null)
            {
                string rest = "REST";


                bool[] fixities = node.Support.Fixities();
                for (int i = 0; i < fixities.Length; i++)
                {
                    rest += "," + (fixities[i] ? 1 : 0);
                }

                rest += ",STIFF";

                double[] stiffnesses = node.Support.ElasticValues();
                for (int i = 0; i < stiffnesses.Length; i++)
                {
                    rest += "," + ((stiffnesses[i] > 0) ? stiffnesses[i] : 0);
                }


                return rest;
            }
            else
                return "NO_REST,NO_STIFF";


        }

        /***************************************************/

    }
}

