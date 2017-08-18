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
using BH.oM.Base;
using System.Reflection;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public IEnumerable<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            // Make sure there is at least one query
            if (query.Count() == 0)
                return new List<object>();

            // Make sure this is a FilterQuery
            FilterQuery filter = query.First() as FilterQuery;
            if (filter == null)
                return new List<object>();


            MethodInfo method = typeof(Merge).GetMethod("Pull");
            MethodInfo generic = method.MakeGenericMethod(new Type[] { filter.Type });
            return (IEnumerable<object>) generic.Invoke(null, new object[] { query, config });

        }

        /***************************************************/

        public IEnumerable<T> Pull<T>(IEnumerable<IQuery> query, Dictionary<string, string> config = null) where T: BHoMObject //TODO: Need to handle tags as well
        {
            // Make sure there is at least one query
            if (query.Count() == 0)
                return new List<T>();

            // Make sure this is a FilterQuery
            FilterQuery filter = query.First() as FilterQuery;
            if (filter == null)
                return new List<T>();

            // Define the dictionary of Pull methods
            Dictionary<Type, Func<List<string>, IList>> m_PullMethods = new Dictionary<Type, Func<List<string>, IList>>()
            {
                {typeof(Material), PullMaterials },
                {typeof(SectionProperty), PullSectionProperties },
                {typeof(Node), PullNodes },
                {typeof(Bar), PullBars }
            };

            // Get the indices if any
            List<string> indices = null;
            if (filter.Equalities.ContainsKey("Indices"))
                indices = (List<string>)filter.Equalities["Indices"];

            // Get the objects based on the indices
            IEnumerable<T> fromIndices; 
            if (m_PullMethods.ContainsKey(filter.Type))
                fromIndices = m_PullMethods[filter.Type](indices).Cast<T>();
            else
                fromIndices = new List<T>();

            // Filter by tag if any 
            IEnumerable<T> fromTag;
            if (filter.Tag == "")
                fromTag = fromIndices;
            else
                fromTag = fromIndices.Where(x => x.Tags.Contains(filter.Tag));

            return fromTag;
        }

        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public List<T> Pull<T>(string tag = "", List<string> indices = null) where T : BHoMObject
        {
            Dictionary<Type, Func<List<string>, IList>> m_PullMethods = new Dictionary<Type, Func<List<string>, IList>>()
            {
                {typeof(Material), PullMaterials },
                {typeof(SectionProperty), PullSectionProperties },
                {typeof(Node), PullNodes },
                {typeof(Bar), PullBars }
            };

            Type type = typeof(T);

            if (m_PullMethods.ContainsKey(type))
            {
                if (tag == "")
                    return m_PullMethods[type](indices).Cast<T>().ToList();
                else
                    return m_PullMethods[type](indices).Cast<T>().Where(x => x.Tags.Contains(tag)).ToList();
            }  
            else
                return new List<T>();
        }

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

            int[] potentialBeamRefs = GenerateIndices(ids, typeof(Bar));

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
            m_gsa.Nodes(GenerateIndices(ids, typeof(Node)), out gsaNodes);

            return gsaNodes.Select(x => Convert.FromGsaNode(x)).ToList();
        }

        /***************************************/

        public List<SectionProperty> PullSectionProperties(List<string> ids = null)
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

        private static List<Material> GetStandardGsaMaterials()
        {
            // TODO: What about the other materials in MaterialType enum? Shouldn't they match?
            List<string> names = new List<string> { "STEEL", "CONC_SHORT", "CONC_LONG", "ALUMINIUM", "GLASS" };

            List<Material> materials = new List<Material>();
            foreach (string name in names)
            {
                Material mat = new Material("GSA Standard " + name);
                mat.CustomData.Add(GSAAdapter.ID, name);
                materials.Add(mat);
            }

            return materials;
        }

    }
}
