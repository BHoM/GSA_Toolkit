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
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidLink> FromGsaRigidLinks(IEnumerable<GsaElement> gsaElements, IEnumerable<string> gsaStrings, Dictionary<int, LinkConstraint> constraints, Dictionary<int, Node> nodes)
        {
            
            List<RigidLink> linkList = new List<RigidLink>();

            //Rigid Link
            foreach (GsaElement gsaLink in gsaElements)
            {
                if (gsaLink.eType != 9)
                    continue;

                RigidLink link = new RigidLink()
                {
                    PrimaryNode = nodes[gsaLink.Topo[0]],
                    SecondaryNodes = new List<Node> { nodes[gsaLink.Topo[1]] },
                    Constraint = constraints[gsaLink.Property]
                };

                link.ApplyTaggedName(gsaLink.Name);
                int id = gsaLink.Ref;
                link.SetAdapterId(typeof(GSAId), id);
                linkList.Add(link);
            }

            //Rigid Constraint
            foreach (string gsaString in gsaStrings)
            {
                string[] tokens = gsaString.Split(',');

                string linkName = tokens[1];
                int primaryNodeName = int.Parse(tokens[2]);
                string linkTypeString = tokens[3];
                string[] constrainedNodeNames = tokens[4].Split(' ');

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

                LinkConstraint linkType;

                switch (linkTypeString)
                {
                    case "ALL":
                        linkType = Engine.Structure.Create.LinkConstraintFixed();
                        break;
                    case "PIN":
                        linkType = Engine.Structure.Create.LinkConstraintPinned();
                        break;
                    case "XY_PLANE":
                        linkType = Engine.Structure.Create.LinkConstraintXYPlane();
                        break;
                    case "ZX_PLANE":
                        linkType = Engine.Structure.Create.LinkConstraintZXPlane();
                        break;
                    case "YZ_PLANE":
                        linkType = Engine.Structure.Create.LinkConstraintYZPlane();
                        break;
                    case "XY_PLANE_PIN":
                        linkType = Engine.Structure.Create.LinkConstraintXYPlanePin();
                        break;
                    case "ZX_PLANE_PIN":
                        linkType = Engine.Structure.Create.LinkConstraintZXPlanePin();
                        break;
                    case "YZ_PLANE_PIN":
                        linkType = Engine.Structure.Create.LinkConstraintYZPlanePin();
                        break;
                    case "XY_PLATE":
                        linkType = Engine.Structure.Create.LinkConstraintYPlateZPlate();     //Wrong name in engine. Should be just ZPlate.
                        break;
                    case "ZX_PLATE":
                        linkType = Engine.Structure.Create.LinkConstraintYPlate();
                        break;
                    case "YZ_PLATE":
                        linkType = Engine.Structure.Create.LinkConstraintXPlate();
                        break;
                    case "XY_PLATE_PIN":
                        linkType = Engine.Structure.Create.LinkConstraintZPlatePin();
                        break;
                    case "ZX_PLATE_PIN":
                        linkType = Engine.Structure.Create.LinkConstraintYPlatePin();
                        break;
                    case "YZ_PLATE_PIN":
                        linkType = Engine.Structure.Create.LinkConstraintXPlatePin();
                        break;
                    default:
                        //String in format example: X:XYY-Y:YZZXX-Z:YY-XX:XX-YY:YY-ZZ:ZZ
                        linkType = new LinkConstraint();
                        string[] constraintProps = linkTypeString.Split('-');

                        foreach (string c in constraintProps)
                        {
                            string[] fromTo = c.Split(':');
                            string from = fromTo[0];
                            string to = fromTo[1];
                            switch (from)
                            {
                                case "X":
                                    if (to.Contains('X'))
                                        linkType.XtoX = true;
                                    if (to.Contains('Y'))
                                        linkType.XtoYY = true;
                                    if (to.Contains('Z'))
                                        linkType.XtoZZ = true;
                                    break;
                                case "Y":
                                    if (to.Contains('X'))
                                        linkType.YtoXX = true;
                                    if (to.Contains('Y'))
                                        linkType.YtoY = true;
                                    if (to.Contains('Z'))
                                        linkType.YtoZZ = true;
                                    break;
                                case "Z":
                                    if (to.Contains('X'))
                                        linkType.ZtoXX = true;
                                    if (to.Contains('Y'))
                                        linkType.ZtoYY = true;
                                    if (to.Contains('Z'))
                                        linkType.ZtoZ = true;
                                    break;
                                case "XX":
                                    if (to.Contains("XX"))
                                        linkType.XXtoXX = true;
                                    break;
                                case "YY":
                                    if (to.Contains("YY"))
                                        linkType.YYtoYY = true;
                                    break;
                                case "ZZ":
                                    if (to.Contains("ZZ"))
                                        linkType.ZZtoZZ = true;
                                    break;
                            }
                        }
                        break;
                }

                RigidLink link = new RigidLink()
                {
                    PrimaryNode = primaryNode,
                    SecondaryNodes = constrainedNodes,
                    Constraint = linkType
                };

                link.ApplyTaggedName(linkName);

                IsRigidConstraint RCtag = new IsRigidConstraint
                {
                    RigidConstraint = true
                };

                link.Fragments.Add(RCtag);

                linkList.Add(link);
            }

            return linkList;
        }

        /***************************************************/

    }
}







