/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHG = BHoM.Global;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;
using BHM = BHoM.Materials;
using GSA_Adapter.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    public static class MeshIO
    {

        /// <summary>
        /// Get bars method, gets bars from a GSA model and all associated data. 
        /// </summary>
        /// <returns></returns>
        public static bool GetMeshes(ComAuto gsa, out List<BHE.FEMesh> meshList, List<string> barNumbers = null)
        {
            meshList = new List<BHE.FEMesh>();


            int[] potentialMeshRefs = BarIO.GeneratePotentialElemRef(gsa, barNumbers);

            GsaElement[] gsaElements = new GsaElement[potentialMeshRefs.Length];
            gsa.Elements(potentialMeshRefs, out gsaElements);

            Dictionary<string, BHP.PanelProperty> secProps =  PropertyIO.GetPanelProperties(gsa, false);
            Dictionary<string, BHE.Node> nodes = NodeIO.GetNodes(gsa);


            for (int i = 0; i < gsaElements.Length; i++)
            {
                GsaElement gsaMesh = gsaElements[i]; //TODO: filter elements based on topology


                BHE.FEFace face;
                switch (gsaMesh.eType)
                {
                    case 5://Quad4
                        face = new BHE.FEFace();
                        face.NodeIndices = new List<int> { 0, 1, 2, 3 };
                        break;
                    case 7://Tri3
                        face = new BHE.FEFace();
                        face.NodeIndices = new List<int> { 0, 1, 2 };
                        break;
                    default:
                        continue;

                }

                BHE.FEMesh mesh = new BHE.FEMesh();
                mesh.Faces.Add(face);
                face.CustomData[Utils.ID] = gsaMesh.Ref;
                foreach (int ind in gsaMesh.Topo)
                {
                    mesh.Nodes.Add(nodes[ind.ToString()]);
                }

                BHP.PanelProperty prop;
                if (secProps.TryGetValue(gsaMesh.Property.ToString(), out prop))
                {
                    mesh.PanelProperty = prop;
                }
                mesh.CustomData[Utils.ID] = gsaMesh.Ref;

                meshList.Add(mesh);


            }

            //barList = bars.ToList();
            return true;
        }

        /// <summary>
        /// Create GSA meshes
        /// </summary>
        /// <returns></returns>
        public static bool CreateMeshes(ComAuto gsa, List<BHE.FEMesh> meshes, out List<string> ids)
        {
            //Shallowclone the bars and their custom data
            meshes.ForEach(x => x = (BHE.FEMesh)x.ShallowClone());
            meshes.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            ids = new List<string>();

            //Get unique section properties and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHP.PanelProperty> panelProperties = meshes.Select(x => x.PanelProperty).GetDistinctDictionary();
            Dictionary<Guid, BHP.PanelProperty> clonedPanProps = Utils.CloneSectionProperties(panelProperties);


            //Create the section properties
            PropertyIO.CreatePanelProperties(gsa, clonedPanProps.Values.ToList());

            //Get unique nodes and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHE.Node> nodes = meshes.SelectMany(x => x.Nodes).GetDistinctDictionary();
            //Dictionary<Guid, BHE.Node> clonedNodes = nodes.Select(x => x.Value.CustomData.ContainsKey(Utils.ID) ? x : new KeyValuePair<Guid, BHE.Node>(x.Key, (BHE.Node)x.Value.ShallowClone())).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<Guid, BHE.Node> clonedNodes = Utils.CloneObjects(nodes);


            //Assign the clones section properties to the abrs
            meshes.ForEach(x => x.PanelProperty = clonedPanProps[x.PanelProperty.BHoM_Guid]);

            foreach (BHE.FEMesh mesh in meshes)
            {
                List<BHE.Node> newNodes = new List<BHE.Node>();
                for (int i = 0; i < mesh.Nodes.Count; i++)
                {
                    newNodes.Add(clonedNodes[mesh.Nodes[i].BHoM_Guid]);
                }
                mesh.Nodes = newNodes;
            }


            foreach (BHE.FEMesh mesh in meshes)
            {
                List<BHE.FEFace> newFaces = new List<BHE.FEFace>();
                for (int i = 0; i < mesh.Faces.Count; i++)
                {
                    BHE.FEFace newFace = mesh.Faces[i].ShallowClone() as BHE.FEFace;
                    newFace.CustomData = new Dictionary<string, object>(newFace.CustomData);
                    newFaces.Add(newFace);
                }
                mesh.Faces = newFaces;
            }


            //Create nodes
            NodeIO.CreateNodes(gsa, clonedNodes.Values.ToList());

            int highestIndex = gsa.GwaCommand("HIGHEST, EL") + 1;

            foreach (BHE.FEMesh mesh in meshes)
            {
                if (!CreateMeshFace(gsa, mesh, ref highestIndex, ids))
                    return false;
            }

            gsa.UpdateViews();
            return true;
        }

        public static bool CreateMeshFace(ComAuto gsa, BHE.FEMesh mesh, ref int highestIndex, List<string> ids)
        {
            string command = "EL.2";
            string sectionPropertyIndex = mesh.PanelProperty.CustomData[Utils.ID].ToString();
            List<string> meshIds = new List<string>();

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                BHE.FEFace face = mesh.Faces[i];
            
                string index;

                if (!Utils.CheckAndGetGsaId(face, out index))
                {
                    index = highestIndex.ToString();
                    highestIndex++;
                }

                string name = mesh.Name;
                string type = GetElementTypeString(face);

                if (type == null)
                {
                    Utils.SendErrorMessage("Faces needs to be quads or triangles");
                    return false;
                }
                                       

                int group = 0;

                string nodes = "";
                face.NodeIndices.ForEach(x => nodes += mesh.Nodes[x].CustomData[Utils.ID].ToString() + ",");

                string orientationAngle = "0";
                string releases = CreateReleaseString(face);
                string dummy = Utils.CheckDummy(face);

                string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + nodes + "0 ," + orientationAngle + ", RLS, " + releases + "NO_OFFSET," + dummy;
                dynamic commandResult = gsa.GwaCommand(str); //"EL.2, 1,, NO_RGB , BEAM , 1, 1, 1, 2 , 0 ,0, RLS, FFFFFF , FFFFFF, NO_OFFSET, "

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    meshIds.Add(index);
                    mesh.Faces[i] = Utils.TagWithIdAndClone(face, index);
                    continue;
                }
                else
                {
                    return Utils.CommandFailed(command);
                }
            }

            mesh.CustomData[Utils.ID] = meshIds;
            return true;
        }

        private static string CreateReleaseString(BHE.FEFace face)
        {
            string rel = "";

            for (int i = 0; i < face.NodeIndices.Count; i++)
            {
                rel += "ffffff,";
            }

            return rel;
        }

        private static string GetElementTypeString(BHE.FEFace face)
        {
            if (face.IsQuad)
                return "QUAD4";

            if (face.IsTri)
                return "TRI3";

            return null;
        }
    }
}
