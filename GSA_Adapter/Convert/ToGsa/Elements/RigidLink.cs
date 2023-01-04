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

using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Elements;
using BH.Engine.Adapters.GSA;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this RigidLink link, string index, int secondaryIndex = 0)
        {
            string command = "EL.2";
            string name = link.TaggedName().ToGSACleanName();
            string type = "LINK";

            string constraintIndex = link.Constraint.GSAId().ToString();
            string group = "0";

            string startIndex = link.PrimaryNode.GSAId().ToString();

            string endIndex = link.SecondaryNodes[secondaryIndex].GSAId().ToString();

            string dummy = CheckDummy(link);


            //EL	1	gfdgfdg	NO_RGB	LINK	1	1	1	2	0	0	NO_RLS	NO_OFFSET	DUMMY
            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + constraintIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0" + ",0" + ", NO_RLS" + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************************/

    }
}




