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

            string X = Fixity(joint.X);
            string Y = Fixity(joint.Y);
            string Z = Fixity(joint.Z);
            string XX = Fixity(joint.XX);
            string YY = Fixity(joint.YY);
            string ZZ = Fixity(joint.ZZ);

            string stage = joint.Stage;

            //JOINT.2 | name | constrained_node | X | Y | Z | XX | YY | ZZ | primary_node | stage
            string str = command + ", " + name + ", " + constrainedNode + " , " + X + " , " + Y + " , " + Z + " , " + XX + " , " + YY + " , " + ZZ + ", " + primaryNode + ", " + stage;
            return str;
        }

        /***************************************************/

        private static string Fixity(bool fixity)
        {
            string fixString = "0";

            if (fixity)
            {
                fixString = "1";
            }

            return fixString;
        }
    }
}






