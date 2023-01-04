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

#if GSA_10_1
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


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidLink> FromGsaRigidLinks(IEnumerable<GsaElement> gsaElements, Dictionary<string, LinkConstraint> constraints, Dictionary<string, Node> nodes)
        {
            List<RigidLink> linkList = new List<RigidLink>();

            foreach (GsaElement gsaLink in gsaElements)
            {
                if (gsaLink.eType != 9)
                    continue;

                RigidLink face = new RigidLink()
                {
                    PrimaryNode = nodes[gsaLink.Topo[0].ToString()],
                    SecondaryNodes = new List<Node> { nodes[gsaLink.Topo[1].ToString()] },
                    Constraint = constraints[gsaLink.Property.ToString()]
                };

                face.ApplyTaggedName(gsaLink.Name);
                int id = gsaLink.Ref;
                face.SetAdapterId(typeof(GSAId), id);
                linkList.Add(face);

            }
            return linkList;
        }

        /***************************************************/

    }
}




