/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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


using BH.oM.Base;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Results;
using BH.oM.Structure.Requests;
using BH.oM.Analytical.Results;
using Interop.gsa_8_7;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapter;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Methods                     ****/
        /***************************************************/

        protected override IEnumerable<IBHoMObject> IRead(Type type, IList indices, ActionConfig actionConfig = null)
        {
            if (type == typeof(Node))
                return ReadNodes(indices as dynamic);
            else if (type == typeof(Bar))
                return ReadBars(indices as dynamic);
            else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                return ((List<ISectionProperty>)ReadSectionProperties(indices as dynamic)).Cast<BHoMObject>();
            else if (type == typeof(IMaterialFragment))
                return ReadMaterials(indices as dynamic);
            else if (type == typeof(LoadCombination))
                return ReadLoadCombinations(indices as dynamic);
            else if (type == typeof(ILoad) || type.GetInterfaces().Contains(typeof(ILoad)))
                return new List<ILoad>(); //TODO: Implement load extraction
            if (type == typeof(RigidLink))
                return ReadRigidLink(indices as dynamic);
            if (type == typeof(LinkConstraint))
                return ReadLinkConstraint(indices as dynamic);
            if (type == typeof(FEMesh))
                return ReadFEMesh(indices as dynamic);
            if (type == typeof(ISurfaceProperty))
                return ReadProperty2d(indices as dynamic);
            if (type == typeof(Loadcase))
                return ReadLoadCases(indices as dynamic);
            if (type.IsGenericType && type.Name == typeof(BHoMGroup<IBHoMObject>).Name)
                return new List<BHoMGroup<IBHoMObject>>();
            if (typeof(IResult).IsAssignableFrom(type))
            {
                Modules.Structure.ErrorMessages.ReadResultsError(type);
                return null;
            }


            return null;
        }


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        public List<IMaterialFragment> ReadMaterials(List<string> ids = null)
        {
            return ReadMaterials(ids, false);
        }

        /***************************************************/

        public List<IMaterialFragment> ReadMaterials(List<string> ids = null, bool includeStandard = false)
        {
            List<IMaterialFragment> materials = new List<IMaterialFragment>();

            string allProps = m_gsaCom.GwaCommand("GET_ALL, MAT").ToString();
            string[] matArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');
            if (ids == null)
                materials = matArr.Select(x => Convert.FromGsaMaterial(x)).Where(x => x != null).ToList();
            else
                materials = matArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaMaterial(x)).Where(x => x != null).ToList();

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
                lCases = lCaseArr.Select(x => Convert.FromGsaLoadcase(x)).ToList();
            else
                lCases = lCaseArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaLoadcase(x)).ToList();

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

            Dictionary<string, ISectionProperty> secProps = secPropList.Where(x => x != null).ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());

            return Convert.FromGsaBars(barArr, secProps, nodes, ids);
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
                lComabinations = analList.Select(x => Convert.FromGsaAnalTask(x, lCases)).ToList();
            else
                lComabinations = analList.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaAnalTask(x, lCases)).ToList();

            return lComabinations;
        }

        /***************************************/

        public List<Node> ReadNodes(List<string> ids = null)
        {

            //GsaNode[] gsaNodes;
            //m_gsaCom.Nodes(GenerateIndices(ids, typeof(Node)), out gsaNodes);

            //return gsaNodes.Select(x => Convert.FromGsaNode(x)).ToList();
            string allNodes = m_gsaCom.GwaCommand("GET_ALL, NODE").ToString();
            string[] nodeArr = string.IsNullOrWhiteSpace(allNodes) ? new string[0] : allNodes.Split('\n');

            if (ids == null)
                return nodeArr.Select(x => Convert.FromGsaNode(x)).ToList();
            else
                return nodeArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaNode(x)).ToList();
        }

        /***************************************/

        public List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<IMaterialFragment> matList = ReadMaterials(null, true);
            Dictionary<string, IMaterialFragment> materials = matList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());

            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_SEC").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Convert.FromGsaSectionProperty(x, materials)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaSectionProperty(x, materials)).ToList();
        }

        /***************************************/

        public List<ISurfaceProperty> ReadProperty2d(List<string> ids = null)
        {
            List<IMaterialFragment> matList = ReadMaterials(null, true);
            Dictionary<string, IMaterialFragment> materials = matList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());

            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_2D").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Convert.FromGsaSurfaceProperty(x, materials)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaSurfaceProperty(x, materials)).ToList();
        }

        /***************************************/


        public List<FEMesh> ReadFEMesh(List<string> ids = null)
        {
            int[] potentialMeshRefs = GenerateIndices(ids, typeof(FEMesh));

            GsaElement[] gsaElements = new GsaElement[potentialMeshRefs.Length];
            m_gsaCom.Elements(potentialMeshRefs, out gsaElements);

            List<ISurfaceProperty> secPropList = ReadProperty2d();
            List<Node> nodeList = ReadNodes();

            Dictionary<string, ISurfaceProperty> props = secPropList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());

            return Convert.FromGsaFEMesh(gsaElements, props, nodes);
        }

        /***************************************/

        public List<LinkConstraint> ReadLinkConstraint(List<string> ids = null)
        {
            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_LINK").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Convert.FromGsaLinkConstraint(x)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaLinkConstraint(x)).ToList();
        }

        /***************************************/

        public List<RigidLink> ReadRigidLink(List<string> ids = null)
        {
            List<LinkConstraint> constraintList = ReadLinkConstraint(null);
            List<Node> nodeList = ReadNodes();

            Dictionary<string, LinkConstraint> constraints = constraintList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.AdapterId(typeof(GSAId)).ToString());

            int[] potentialBeamRefs = GenerateIndices(ids, typeof(RigidLink));
            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            m_gsaCom.Elements(potentialBeamRefs, out gsaElements);

            return Convert.FromGsaRigidLinks(gsaElements, constraints, nodes);

            //if (ids == null)
            //    return proArr.Select(x => Convert.FromGsaSectionProperty(x, materials)).ToList();
            //else
            //    return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaSectionProperty(x, materials)).ToList();
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

        private List<IMaterialFragment> GetStandardGsaMaterials()
        {
            // TODO: What about the other materials in MaterialType enum? Shouldn't they match?
            List<string> names = new List<string> { "STEEL", "CONC_SHORT", "CONC_LONG", "ALUMINIUM", "GLASS" };

            List<IMaterialFragment> materials = new List<IMaterialFragment>();
            materials.Add(new Steel() { Name = "GSA Standard STEEL", CustomData = new Dictionary<string, object> { { AdapterIdName, "STEEL" } } });
            materials.Add(new Concrete() { Name = "GSA Standard CONC_SHORT", CustomData = new Dictionary<string, object> { { AdapterIdName, "CONC_SHORT" } } });
            materials.Add(new Concrete() { Name = "GSA Standard CONC_LONG", CustomData = new Dictionary<string, object> { { AdapterIdName, "CONC_LONG" } } });
            materials.Add(new Aluminium() { Name = "GSA Standard ALUMINIUM", CustomData = new Dictionary<string, object> { { AdapterIdName, "ALUMINIUM" } } });


            //foreach (string name in names)
            //{
            //    //IMaterialFragment mat = new IMaterialFragment { Name = "GSA Standard " + name };
            //    mat.CustomData.Add(AdapterIdName, name);
            //    materials.Add(mat);
            //}

            return materials;
        }

        /***************************************************/
    }
}

