/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.Engine.Adapters.GSA;
using BH.oM.Adapters.GSA.Elements;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this Spacer spacer, string index)
        {
            string command = "EL.2";
            string name = spacer.TaggedName().ToGSACleanName();
            string type = "SPACER";

            string spacerPropertyIndex = spacer.SpacerProperty != null ? spacer.SpacerProperty.GSAId().ToString() : "1";
            int group = 0;

            string startIndex = spacer.Start.GSAId().ToString();
            string endIndex = spacer.End.GSAId().ToString();


  
            string dummy = CheckDummy(spacer);

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + spacerPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 , 0 , NO_RLS , NO_OFFSET , " + dummy;
            return str;
        }

        /***************************************************/

    }
}






