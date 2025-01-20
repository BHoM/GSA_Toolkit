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

#if GSA_10_2
using Interop.Gsa_10_2;
#elif  GSA_10_1
using Interop.Gsa_10_1;
#else
using Interop.gsa_8_7;
#endif
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using BH.oM.Adapters.GSA.Elements;
using System;
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidConstraint> FromGsaRigidConstraint(IEnumerable<string> gsaStrings, Dictionary<int, Node> nodes)
        {
            List<RigidConstraint> constraintList = new List<RigidConstraint>();

            foreach (string gsaString in gsaStrings)
            {
                string[] tokens = gsaString.Split(',');

                int primaryNodeName = int.Parse(tokens[2]);
                string linkTypeString = tokens[3];
                string[] constrainedNodeNames = tokens[4].Split(',');

                Node primaryNode;
                nodes.TryGetValue(primaryNodeName, out primaryNode);

                List<Node> constrainedNodes = new List<Node>();

                foreach (string constrainedNodeName in constrainedNodeNames)
                {
                    if (int.TryParse(constrainedNodeName, out int constrainedNodeId))
                    {
                        if (nodes.TryGetValue(constrainedNodeId, out Node constrainedNode))
                            constrainedNodes.Add(constrainedNode);
                    }
                }

                RigidConstraintLinkType linkType = RigidConstraintLinkType.ALL;

                if (Enum.TryParse(linkTypeString, out RigidConstraintLinkType parsedLinkType))
                    linkType = parsedLinkType;

                RigidConstraint constraint = new RigidConstraint()
                {
                    PrimaryNode = primaryNode,
                    ConstrainedNodes = constrainedNodes,
                    Type = linkType
                };

                constraintList.Add(constraint);
            }

            return constraintList;
        }





        /***************************************************/

    }
}






