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

using BH.Engine.GSA;
using BH.oM.Base;
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;
using Interop.gsa_8_7;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Methods                     ****/
        /***************************************************/

        protected override IEnumerable<IBHoMObject> Read(Type type, IList indices)
        {
            if (type == typeof(Node))
                return ReadNodes(indices as dynamic);
            else if (type == typeof(Bar))
                return ReadBars(indices as dynamic);
            else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                return ((List<ISectionProperty>)ReadSectionProperties(indices as dynamic)).Cast<BHoMObject>();
            else if (type == typeof(Material))
                return ReadMaterials(indices as dynamic);
            else if (type == typeof(Material))
                return ReadMaterials(indices as dynamic);
            else if (type == typeof(LoadCombination))
                return ReadLoadCombinations(indices as dynamic);
            else if (type == typeof(ILoad) || type.GetInterfaces().Contains(typeof(ILoad)))
                return new List<ILoad>(); //TODO: Implement load extraction
            if (type == typeof(RigidLink))
                return ReadRigidLink(indices as dynamic);
            if (type == typeof(LinkConstraint))
                return ReadLinkConstraint(indices as dynamic);
            //if (type == typeof(MeshFace))
            //    return ReadMeshFace(indices as dynamic);
            if (type == typeof(FEMesh))
                return ReadFEMesh(indices as dynamic);
            if (type == typeof(ISurfaceProperty))
                return ReadProperty2d(indices as dynamic);
            if (type == typeof(Loadcase))
                return ReadLoadCases(indices as dynamic);
            if (type.IsGenericType && type.Name == typeof(BHoMGroup<IBHoMObject>).Name)
                return new List<BHoMGroup<IBHoMObject>>();

            return null;
        }


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        public List<Material> ReadMaterials(List<string> ids = null)
        {
            return ReadMaterials(ids, false);
        }

        /***************************************************/

        public List<Material> ReadMaterials(List<string> ids = null, bool includeStandard = false)
        {
            List<Material> materials = new List<Material>();

            string allProps = m_gsaCom.GwaCommand("GET_ALL, MAT").ToString();
            string[] matArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');
            if (ids == null)
                materials = matArr.Select(x => Engine.GSA.Convert.ToBHoMMaterial(x)).ToList();
            else
                materials = matArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMMaterial(x)).ToList();

            if (includeStandard)
                materials.AddRange(GetStandardGsaMaterials());

            return materials;
        }

        /***************************************************/

        public List<Loadcase> ReadLoadCases(List<string> ids = null)
        {
            List<Loadcase> lCases = new List<Loadcase>();

            string allLoadCases = m_gsaCom.GwaCommand("GET_ALL, LOAD_TITLE.1").ToString();
            string[] lCaseArr = string.IsNullOrWhiteSpace(allLoadCases) ? new string[0] : allLoadCases.Split('\n');

            if (ids == null)
                lCases = lCaseArr.Select(x => Engine.GSA.Convert.ToBHoMLoadcase(x)).ToList();
            else
                lCases = lCaseArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMLoadcase(x)).ToList();

            return lCases;
        }

        /***************************************/

        public List<Bar> ReadBars(List<string> ids = null)
        {

            //int[] potentialBeamRefs = GenerateIndices(ids, typeof(Bar));

            //GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            //m_gsaCom.Elements(potentialBeamRefs, out gsaElements);


            string allNodes = m_gsaCom.GwaCommand("GET_ALL, EL.2").ToString();
            string[] barArr = string.IsNullOrWhiteSpace(allNodes) ? new string[0] : allNodes.Split('\n');

            List<ISectionProperty> secPropList = ReadSectionProperties();
            List<Node> nodeList = ReadNodes();

            Dictionary<string, ISectionProperty> secProps = secPropList.Where(x => x != null).ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            return Engine.GSA.Convert.ToBHoMBars(barArr, secProps, nodes, ids);
        }

        /***************************************/

        public List<LoadCombination> ReadLoadCombinations(List<string> ids = null)
        {

            List<LoadCombination> lComabinations = new List<LoadCombination>();
            int loadCasesCount = m_gsaCom.GwaCommand("HIGHEST, ANAL");
            List<string> analList = new List<string>();
            for (int i = 0; i < loadCasesCount; i++)
            {
                string anal = m_gsaCom.GwaCommand("GET, ANAL," + (i + 1)).ToString();
                if (!string.IsNullOrWhiteSpace(anal))
                    analList.Add(anal);
            }

            List<Loadcase> lCaseList = ReadLoadCases();
            Dictionary<string, Loadcase> lCases = lCaseList.ToDictionary(x => x.Number.ToString());

            if (ids == null)
                lComabinations = analList.Select(x => Engine.GSA.Convert.ToBHoMAnalTask(x, lCases)).ToList();
            else
                lComabinations = analList.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMAnalTask(x, lCases)).ToList();

            return lComabinations;
        }

        /***************************************/

        public List<Node> ReadNodes(List<string> ids = null)
        {

            //GsaNode[] gsaNodes;
            //m_gsaCom.Nodes(GenerateIndices(ids, typeof(Node)), out gsaNodes);

            //return gsaNodes.Select(x => Engine.GSA.Convert.ToBHoMNode(x)).ToList();
            string allNodes = m_gsaCom.GwaCommand("GET_ALL, NODE").ToString();
            string[] nodeArr = string.IsNullOrWhiteSpace(allNodes) ? new string[0] : allNodes.Split('\n');

            if (ids == null)
                return nodeArr.Select(x => Engine.GSA.Convert.ToBHoMNode(x)).ToList();
            else
                return nodeArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMNode(x)).ToList();
        }

        /***************************************/

        public List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<Material> matList = ReadMaterials(null, true);
            Dictionary<string, Material> materials = matList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_SEC").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Engine.GSA.Convert.ToBHoMSectionProperty(x, materials)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMSectionProperty(x, materials)).ToList();
        }

        /***************************************/

        public List<ISurfaceProperty> ReadProperty2d(List<string> ids = null)
        {
            List<Material> matList = ReadMaterials(null, true);
            Dictionary<string, Material> materials = matList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_2D").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Engine.GSA.Convert.ToBHoMProperty2d(x, materials)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMProperty2d(x, materials)).ToList();
        }

        /***************************************/

        //public List<MeshFace> ReadMeshFace(List<string> ids = null)
        //{
        //    int[] potentialMeshRefs = GenerateIndices(ids, typeof(MeshFace));

        //    GsaElement[] gsaElements = new GsaElement[potentialMeshRefs.Length];
        //    m_gsaCom.Elements(potentialMeshRefs, out gsaElements);

        //    List<ISurfaceProperty> secPropList = ReadProperty2d();
        //    List<Node> nodeList = ReadNodes();

        //    Dictionary<string, ISurfaceProperty> props = secPropList.ToDictionary(x => x.CustomData[AdapterId].ToString());
        //    Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[AdapterId].ToString());

        //    return Engine.GSA.Convert.ToBHoMMeshFace(gsaElements, props, nodes);
        //}

        /***************************************/

        public List<FEMesh> ReadFEMesh(List<string> ids = null)
        {
            int[] potentialMeshRefs = GenerateIndices(ids, typeof(FEMesh));

            GsaElement[] gsaElements = new GsaElement[potentialMeshRefs.Length];
            m_gsaCom.Elements(potentialMeshRefs, out gsaElements);

            List<ISurfaceProperty> secPropList = ReadProperty2d();
            List<Node> nodeList = ReadNodes();

            Dictionary<string, ISurfaceProperty> props = secPropList.ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            return Engine.GSA.Convert.ToBHoMFEMesh(gsaElements, props, nodes);
        }

        /***************************************/

        public List<LinkConstraint> ReadLinkConstraint(List<string> ids = null)
        {
            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_LINK").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Engine.GSA.Convert.ToBHoMLinkConstraint(x)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMLinkConstraint(x)).ToList();
        }

        /***************************************/

        public List<RigidLink> ReadRigidLink(List<string> ids = null)
        {
            List<LinkConstraint> constraintList = ReadLinkConstraint(null);
            List<Node> nodeList = ReadNodes();

            Dictionary<string, LinkConstraint> constraints = constraintList.ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            int[] potentialBeamRefs = GenerateIndices(ids, typeof(RigidLink));
            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            m_gsaCom.Elements(potentialBeamRefs, out gsaElements);

            return Engine.GSA.Convert.ToBHoMRigidLinks(gsaElements, constraints, nodes);

            //if (ids == null)
            //    return proArr.Select(x => Engine.GSA.Convert.ToBHoMSectionProperty(x, materials)).ToList();
            //else
            //    return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.ToBHoMSectionProperty(x, materials)).ToList();
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private int[] GenerateIndices(List<string> ids, Type elementType)
        {
            if (ids == null)
            {
                int maxIndex = m_gsaCom.GwaCommand("HIGHEST, " + elementType.ToGsaString());
                maxIndex = maxIndex > 0 ? maxIndex : 1;
                int[] potentialBeamRefs = new int[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                    potentialBeamRefs[i] = i + 1;

                return potentialBeamRefs;
            }

            return ids.Select(x => int.Parse(x)).ToArray();
        }

        /***************************************/

        private List<Material> GetStandardGsaMaterials()
        {
            // TODO: What about the other materials in MaterialType enum? Shouldn't they match?
            List<string> names = new List<string> { "STEEL", "CONC_SHORT", "CONC_LONG", "ALUMINIUM", "GLASS" };

            List<Material> materials = new List<Material>();
            foreach (string name in names)
            {
                Material mat = new Material { Name = "GSA Standard " + name };
                mat.CustomData.Add(AdapterId, name);
                materials.Add(mat);
            }

            return materials;
        }

        /***************************************************/
    }
}
