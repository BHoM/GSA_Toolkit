﻿using BH.oM.Base;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Methods                     ****/
        /***************************************************/

        protected override IEnumerable<BHoMObject> Read(Type type, List<object> indices)
        {
            // Define the dictionary of Read methods
            if (m_ReadMethods == null)
            {
                m_ReadMethods = new Dictionary<Type, Func<List<string>, IList>>()
                {
                    {typeof(Material), ReadMaterials },
                    {typeof(SectionProperty), ReadSectionProperties },
                    {typeof(Node), ReadNodes },
                    {typeof(Bar), ReadBars }
                };
            }
            
            // Get the objects based on the indices
            if (m_ReadMethods.ContainsKey(type))
                return m_ReadMethods[type](indices as dynamic).Cast<BHoMObject>();
            else
                return new List<BHoMObject>();
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

            string allProps = m_gsa.GwaCommand("GET_ALL, MAT").ToString();
            string[] matArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');
            if (ids == null)
                materials = matArr.Select(x => Convert.FromGsaMaterial(x)).ToList();
            else
                materials = matArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaMaterial(x)).ToList();

            if (includeStandard)
                materials.AddRange(GetStandardGsaMaterials());

            return materials;
        }

        /***************************************/

        public List<Bar> ReadBars(List<string> ids = null)
        {

            int[] potentialBeamRefs = GenerateIndices(ids, typeof(Bar));

            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            m_gsa.Elements(potentialBeamRefs, out gsaElements);

            List<SectionProperty> secPropList = ReadSectionProperties();
            List<Node> nodeList = ReadNodes();

            Dictionary<string, SectionProperty> secProps = secPropList.ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            return Convert.FromGsaBars(gsaElements, secProps, nodes);
        }

        /***************************************/

        public List<Node> ReadNodes(List<string> ids = null)
        {

            GsaNode[] gsaNodes;
            m_gsa.Nodes(GenerateIndices(ids, typeof(Node)), out gsaNodes);

            return gsaNodes.Select(x => Convert.FromGsaNode(x)).ToList();
        }

        /***************************************/

        public List<SectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<Material> matList = ReadMaterials(null, true);
            Dictionary<string, Material> materials = matList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            string allProps = m_gsa.GwaCommand("GET_ALL, PROP_SEC").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Convert.FromGsaSectionProperty(x, materials)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.FromGsaSectionProperty(x, materials)).ToList();
        }


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private int[] GenerateIndices(List<string> ids, Type elementType)
        {
            if (ids == null)
            {
                int maxIndex = m_gsa.GwaCommand("HIGHEST, " + elementType.ToGsaString());
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
                Material mat = new Material("GSA Standard " + name);
                mat.CustomData.Add(AdapterId, name);
                materials.Add(mat);
            }

            return materials;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private Dictionary<Type, Func<List<string>, IList>> m_ReadMethods = null;
    }
}