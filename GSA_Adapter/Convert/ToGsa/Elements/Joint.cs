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

using BH.Engine.Adapter;
using BH.Engine.Adapters.GSA;
using BH.Engine.Serialiser;
using BH.oM.Adapters.GSA;
using BH.oM.Adapters.GSA.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this Joint joint)
        {
            string command = "JOINT.2";
            string name = joint.TaggedName().ToGSACleanName();

            string primaryNode = joint.PrimaryNode.GSAId().ToString();
            string constrainedNode = joint.ConstrainedNode.GSAId().ToString();

            string restraint = GetRestraintString(joint);

            string stageNumbers = string.Join(" ", joint.StageList.ToArray());

            //JOINT.2 | name | constrained_node | X | Y | Z | XX | YY | ZZ | primary_node | stage
            string str = command + ", " + name + ", " + constrainedNode + " , " + restraint + " , " + primaryNode + ", " + stageNumbers;
            return str;
        }

        /***************************************************/

        private static string GetRestraintString(Joint joint)
        {
            string str = "";

            if (joint.X)
                str += "1 , ";
            else
                str += "0 , ";

            if (joint.Y)
                str += "1 , ";
            else
                str += "0 , ";

            if (joint.Z)
                str += "1 , ";
            else
                str += "0 , ";

            if (joint.XX)
                str += "1 , ";
            else
                str += "0 , ";

            if (joint.YY)
                str += "1 , ";
            else
                str += "0 , ";

            if (joint.ZZ)
                str += "1";
            else
                str += "0";

            return str;
        }
    }
}







