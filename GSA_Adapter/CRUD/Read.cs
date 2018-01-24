using BH.Engine.GSA;
using BH.oM.Base;
using BH.oM.Common.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Structural.Loads;
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

        protected override IEnumerable<IObject> Read(Type type, IList indices)
        {
            if (type == typeof(Node))
                return ReadNodes(indices as dynamic);
            else if (type == typeof(Bar))
                return ReadBars(indices as dynamic);
            else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                return ((List<ISectionProperty>)ReadSectionProperties(indices as dynamic)).Cast<BHoMObject>();
            else if (type == typeof(Material))
                return ReadMaterials(indices as dynamic);
            else if (type == typeof(LoadCombination))
                return new List<LoadCombination>(); //TODO: Implement loadcombination extraction
            else if (type == typeof(Loadcase))
                return new List<LoadCombination>(); //TODO: Implement loadcombination extraction
            else if (type == typeof(ILoad) || type.GetInterfaces().Contains(typeof(ILoad)))
                return new List<ILoad>();

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
                materials = matArr.Select(x => Engine.GSA.Convert.FromGsaMaterial(x)).ToList();
            else
                materials = matArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.FromGsaMaterial(x)).ToList();

            if (includeStandard)
                materials.AddRange(GetStandardGsaMaterials());

            return materials;
        }

        /***************************************/

        public List<Bar> ReadBars(List<string> ids = null)
        {

            int[] potentialBeamRefs = GenerateIndices(ids, typeof(Bar));

            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            m_gsaCom.Elements(potentialBeamRefs, out gsaElements);

            List<ISectionProperty> secPropList = ReadSectionProperties();
            List<Node> nodeList = ReadNodes();

            Dictionary<string, ISectionProperty> secProps = secPropList.ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            return Engine.GSA.Convert.FromGsaBars(gsaElements, secProps, nodes);
        }

        /***************************************/

        public List<Node> ReadNodes(List<string> ids = null)
        {

            GsaNode[] gsaNodes;
            m_gsaCom.Nodes(GenerateIndices(ids, typeof(Node)), out gsaNodes);

            return gsaNodes.Select(x => Engine.GSA.Convert.FromGsaNode(x)).ToList();
        }

        /***************************************/

        public List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<Material> matList = ReadMaterials(null, true);
            Dictionary<string, Material> materials = matList.ToDictionary(x => x.CustomData[AdapterId].ToString());

            string allProps = m_gsaCom.GwaCommand("GET_ALL, PROP_SEC").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                return proArr.Select(x => Engine.GSA.Convert.FromGsaSectionProperty(x, materials)).ToList();
            else
                return proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Engine.GSA.Convert.FromGsaSectionProperty(x, materials)).ToList();
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
