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
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Structure;
using BH.Engine.Adapters.GSA;
using BH.oM.Adapters.GSA.SpringProperties;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(LinearSpringProperty springProp, string index)
        {
            springProp.Name = springProp.DescriptionOrName().ToGSACleanName();
            string name = springProp.TaggedName();

            //PROP_SPR.4	1	name	NO_RGB	GENERAL	0	534	0	0	0	6587	0	4325	0	124	0	0	0.18

            return $"PROP_SPR.4,{index},{name},NO_RGB,GENERAL,0,{springProp.UX},0,{springProp.UY},0,{springProp.UZ},0,{springProp.RX},0,{springProp.RY},0,{springProp.RZ},0";

        }

    }
}



