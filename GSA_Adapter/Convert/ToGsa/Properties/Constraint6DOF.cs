/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Constraints;
using BH.Engine.Structure;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(Constraint6DOF constraint, string index)
        {
            double[] values = constraint.ElasticValues();

            if (values == null || values.All(x => x == 0))
            {
                constraint.Fragments.Remove(typeof(GSAId));
                return "";
            }

            //PROP_SPR.4	1	Spring prop. 1	NO_RGB	GENERAL	0	123	0	234	0	5234	0	1234	0	5464	0	564	0
            return $"PROP_SPR.4, {index}, {constraint.DescriptionOrName()}, NO_RGB, GENERAL," + values.Select(x => "0, " + x.ToString()).Aggregate((a, b) => a + "," + b);
        }

        /***************************************************/

    }
}







