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
using BH.oM.Structure.SurfaceProperties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<FEMesh> FromGsaFEMesh(IEnumerable<GsaElement> gsaElements, Dictionary<int, ISurfaceProperty> props, Dictionary<int, Node> nodes)
        {
            List<FEMesh> meshList = new List<FEMesh>();

            foreach (GsaElement gsaMesh in gsaElements)
            {
                switch (gsaMesh.eType)
                {
                    case 5://Quad4      //TODO: Quad8 and Tri6
                    case 7://Tri3
                        break;
                    default:
                        continue;
                }

                int id = gsaMesh.Ref;

                FEMeshFace face = new FEMeshFace() { NodeListIndices = Enumerable.Range(0, gsaMesh.NumTopo).ToList(), OrientationAngle = gsaMesh.Beta * System.Math.PI / 180  };
                face.SetAdapterId(typeof(GSAId), id);
                ISurfaceProperty property;
                props.TryGetValue(gsaMesh.Property, out property);

                FEMesh mesh = new FEMesh()
                {
                    Faces = new List<FEMeshFace>() { face },
                    Nodes = gsaMesh.Topo.Select(x => nodes[x]).ToList(),
                    Property = property
                };

                mesh.ApplyTaggedName(gsaMesh.Name);

                mesh.SetAdapterId(typeof(GSAId), id);

                meshList.Add(mesh);
            }
            return meshList;
        }

        /***************************************/

    }
}







