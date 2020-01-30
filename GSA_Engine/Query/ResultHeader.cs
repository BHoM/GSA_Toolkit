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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structure.Results;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ResHeader ResultHeader(this Type type)
        {
            if (typeof(NodeReaction).IsAssignableFrom(type))
                return ResHeader.REF_REAC;
            else if (typeof(NodeDisplacement).IsAssignableFrom(type))
                return ResHeader.REF_DISP;
            else if (typeof(NodeAcceleration).IsAssignableFrom(type))
                return ResHeader.REF_ACC;
            else if (typeof(NodeVelocity).IsAssignableFrom(type))
                return ResHeader.REF_VEL;
            else if (typeof(BarForce).IsAssignableFrom(type))
                return ResHeader.REF_FORCE_EL1D;
            else if (typeof(BarDeformation).IsAssignableFrom(type))
                return ResHeader.REF_DISP_EL1D;
            else if (typeof(BarStress).IsAssignableFrom(type))
                return ResHeader.REF_STRESS_EL1D;
            else if (typeof(BarStress).IsAssignableFrom(type))
                return ResHeader.REF_STRAIN_EL1D;

            return ResHeader.REF_DISP;
        }

        /***************************************************/

        public static ResHeader ResultHeader(this NodeReaction result)
        {
            return ResHeader.REF_REAC;
        }

        /***************************************************/

        public static ResHeader ResultHeader(this NodeDisplacement result)
        {
            return ResHeader.REF_DISP;
        }

        /***************************************************/

        public static ResHeader ResultHeader(this NodeAcceleration result)
        {
            return ResHeader.REF_ACC;
        }

        /***************************************************/
        public static ResHeader ResultHeader(this NodeVelocity result)
        {
            return ResHeader.REF_VEL;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/
        public static ResHeader IResultHeader(this IResult result)
        {
            return ResultHeader(result as dynamic);
        }
    }
}

