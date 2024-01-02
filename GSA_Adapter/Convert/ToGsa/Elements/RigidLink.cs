/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base;
using BH.Engine.Base;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this RigidLink link, string index, int secondaryIndex = 0)
        {
            if (CheckRigCon(link) == false)
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
            else {
                string command = "RIGID.2";
                string name = link.TaggedName().ToGSACleanName();

                string primaryNode = link.PrimaryNode.GSAId().ToString();

                List<Node> constrainedNodes = link.SecondaryNodes;
                string constrainedNodesIds = "";

                foreach (Node constrainedNode in constrainedNodes)
                {
                    string id = constrainedNode.GSAId().ToString();
                    constrainedNodesIds = constrainedNodesIds + " " + id;
                }

                string typename = link.Name.ToString();
                string type;

                switch (typename)
                {
                    case "Fixed":
                        type = "ALL";
                        break;
                    case "xy-Plane":
                        type = "XY_PLANE";
                        break;
                    case "yz-Plane":
                        type = "YZ_PLANE";
                        break;
                    case "zx-Plane":
                        type = "ZX_PLANE";
                        break;
                    case "z-Plate":
                        type = "XY_PLATE";
                        break;
                    case "x-Plate":
                        type = "YZ_PLATE";
                        break;
                    case "y-Plate":
                        type = "ZX_PLATE";
                        break;
                    case "Pinned":
                        type = "PIN";
                        break;
                    case "xy-Plane Pin":
                        type = "XY_PLANE_PIN";
                        break;
                    case "yz-Plane Pin":
                        type = "YZ_PLANE_PIN";
                        break;
                    case "zx-Plane Pin":
                        type = "ZX_PLANE_PIN";
                        break;
                    case "z-Plate Pin":
                        type = "XY_PLATE_PIN";
                        break;
                    case "x-Plate Pin":
                        type = "YZ_PLATE_PIN";
                        break;
                    case "y-Plate Pin":
                        type = "ZX_PLATE_PIN";
                        break;
                    default:
                        type = "All";
                        break;
                }

                //RIGID.2 | name | primary_node | type | constrained_nodes | stage
                string str = command + ", " + name + ", " + primaryNode + " , " + type + ", " + constrainedNodesIds;
                return str;
            }
        }

        /***************************************************/

        public static bool CheckRigCon(BHoMObject obj)
        {
            IsRigidConstraint tag = obj.FindFragment<IsRigidConstraint>();

            if (tag != null && tag.RigidConstraint)
                return true;

            return false;
        }
    }
}





