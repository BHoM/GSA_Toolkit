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


using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Constraints;
using System.Linq;
using BH.oM.Adapters.GSA.SpacerProperties;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SpacerProperty FromGsaSpacerProperty(string gsaProp)
        {
            
            string[] props = gsaProp.Split(',');
            string name = props[2];
            string type = props[5];
            string lengthType = props[6];
            string stiffness = props[7];
            string ratio = props[8];
            string id = props[1];

            SpacerProperty spacerProp = new SpacerProperty();
            spacerProp.Name = name;
            spacerProp.SetAdapterId(typeof(GSAId), int.Parse(id));
            spacerProp.Stiffness = double.Parse(stiffness);
            spacerProp.Ratio = double.Parse(ratio);

            switch (lengthType)
            {
                case "PROPORTIONAL":
                    spacerProp.LengthType = SpacerLengthType.Proportional;
                    break;
                case "RATIO":
                    spacerProp.LengthType = SpacerLengthType.Ratio;
                    break;
                case "PROJECTED_RATIO_XY":
                    spacerProp.LengthType = SpacerLengthType.Projected_ratio_xy;
                    break;
                case "PROJECTED_RATIO_XX":
                    spacerProp.LengthType = SpacerLengthType.Projected_ratio_xx;
                    break;
                default:
                    spacerProp.LengthType = SpacerLengthType.Proportional;
                    break;
            }

            switch (type)
            {
                case "GEODESIC":
                    spacerProp.Type = SpacerType.Geodesic;
                    break;
                case "FREE":
                    spacerProp.Type = SpacerType.Free;
                    break;
                case "BAR":
                    spacerProp.Type = SpacerType.Bar;
                    break;
                default:
                    spacerProp.Type = SpacerType.Geodesic;
                    break;
            }

            return spacerProp;
        }

        /***************************************************/
    }
}

