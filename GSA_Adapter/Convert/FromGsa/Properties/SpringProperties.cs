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


using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Adapters.GSA.SpringProperties;
using BH.oM.Structure.Constraints;
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LinearSpringProperty FromGsaSpringProperty(string gsaProp)
        {

            if (gsaProp == null)
                return new LinearSpringProperty();

            //PROP_SPR.4	1	name	NO_RGB	GENERAL	0	534	0	0	0	6587	0	4325	0	124	0	0	0.18
            //PROP_SPR.4	2	name	NO_RGB	AXIAL	12421	0.07

            string[] arr = gsaProp.Split(',');

            LinearSpringProperty springProperty = new LinearSpringProperty { Name = arr[2] };
            springProperty.SetAdapterId(typeof(GSAId), int.Parse(arr[1]));

            string springType = arr[4];

            if (springType == "AXIAL")
            {
                springProperty.UX = double.Parse(arr[5]);
            }
            else if (springType == "GENERAL")
            {
                springProperty.UX = double.Parse(arr[6]);
                springProperty.UY = double.Parse(arr[8]);
                springProperty.UZ = double.Parse(arr[10]);

                springProperty.RX = double.Parse(arr[12]);
                springProperty.RY = double.Parse(arr[14]);
                springProperty.RZ = double.Parse(arr[16]);
            }
            else
            {
                Engine.Base.Compute.RecordWarning($"Spring property of type {springType} not supported to be read in the GSAAdapter. A {nameof(LinearSpringProperty)} with 0 proeprties has been returned in its place.");
            }

            return springProperty;
        }

        /***************************************************/
    }
}


