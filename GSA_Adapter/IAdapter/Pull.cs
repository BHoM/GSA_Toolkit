using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;
using BH.Adapter.Queries;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public IList Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null) //TODO: Need to handle tags as well
        {
            if (query.Count() == 0)
                return new List<object>();

            FilterQuery filter = query.First() as FilterQuery;
            if (filter == null)
                return new List<object>();

            List<string> indices = null;
            if (filter.Equalities.ContainsKey("Indices"))
                indices = (List<string>)filter.Equalities["Indices"];
            else
            {
                //indices = getFromTag();
            }

            //TODO: Need to find a better place for the pullMethodss dictionary
            Dictionary<Type, Func<List<string>, IList>> m_PullMethods = new Dictionary<Type, Func<List<string>, IList>>()
            {
                {typeof(Material), PullMaterials },
                {typeof(SectionProperty), PullSectionProperties },
                {typeof(Node), PullNodes },
                {typeof(Bar), PullBars }
            };

            if (m_PullMethods.ContainsKey(filter.Type))
                return m_PullMethods[filter.Type](indices);
            else
                return new List<object>();
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public List<Material> PullMaterials(List<string> ids = null)
        {
            return PullMaterials(ids, false);
        }

        /***************************************************/

        public List<Material> PullMaterials(List<string> ids = null, bool includeStandard = false) 
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

        public List<Bar> PullBars(List<string> ids = null) 
        {

            int[] potentialBeamRefs = GeneratePotentialElementIndices(ids);

            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            m_gsa.Elements(potentialBeamRefs, out gsaElements);

            List<SectionProperty> secPropList = PullSectionProperties();
            List<Node> nodeList = PullNodes();

            Dictionary<string, SectionProperty> secProps = secPropList.ToDictionary(x => x.CustomData[GSAAdapter.ID].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[GSAAdapter.ID].ToString());

            return Convert.FromGsaBars(gsaElements, secProps, nodes);
        }

        /***************************************/

        public List<Node> PullNodes(List<string> ids = null)  
        {

            GsaNode[] gsaNodes;
            m_gsa.Nodes(GeneratePotentialNodeIdIndices(ids), out gsaNodes);

            return gsaNodes.Select(x => Convert.FromGsaNode(x)).ToList();
        }

        /***************************************/

        public List<SectionProperty> PullSectionProperties( List<string> ids = null)
        {
            List<Material> matList = PullMaterials(null, true);
            Dictionary<string, Material> materials = matList.ToDictionary(x => x.CustomData[GSAAdapter.ID].ToString());

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

        private int[] GeneratePotentialElementIndices(List<string> barNumbers)
        {
            if (barNumbers == null)
            {
                int maxIndex = m_gsa.GwaCommand("HIGHEST, EL");
                maxIndex = maxIndex > 0 ? maxIndex : 1;
                int[] potentialBeamRefs = new int[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                    potentialBeamRefs[i] = i + 1;

                return potentialBeamRefs;
            }

            return barNumbers.Select(x => int.Parse(x)).ToArray();
        }

        /********************************************/

        private int[] GeneratePotentialNodeIdIndices(List<string> ids)
        {
            if (ids == null)
            {
                int maxIndex = m_gsa.GwaCommand("HIGHEST, NODE");
                maxIndex = maxIndex > 0 ? maxIndex : 1;
                int[] potentialBeamRefs = new int[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                    potentialBeamRefs[i] = i + 1;

                return potentialBeamRefs;
            }

            return ids.Select(x => int.Parse(x)).ToArray();
        }

        /***************************************/

        private static List<Material> GetStandardGsaMaterials()
        {
            List<Material> materials = new List<Material>();
            AddStandardGsaMaterial(ref materials, "STEEL");
            AddStandardGsaMaterial(ref materials, "CONC_SHORT");
            AddStandardGsaMaterial(ref materials, "CONC_LONG");
            AddStandardGsaMaterial(ref materials, "ALUMINIUM");
            AddStandardGsaMaterial(ref materials, "GLASS");
            return materials;
        }

        /***************************************/

        private static void AddStandardGsaMaterial(ref List<Material> materials, string name)
        {
            Material mat = new Material("GSA Standard " + name);
            mat.CustomData.Add(GSAAdapter.ID, name);
            materials.Add(mat);
        }
    }
}
