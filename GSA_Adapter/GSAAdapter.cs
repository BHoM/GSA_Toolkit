using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Interfaces;
using BH.Adapter.Queries;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;



namespace BH.Adapter.GSA
{
    public partial class GSAAdapter : IAdapter, IStructuralAdapter, INodeAdapter, IBarAdapter
    {

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private ComAuto m_gsa;

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSAAdapter()
        {
            m_gsa = new ComAuto();
            ErrorLog = new List<string>();
        }

        /***************************************************/

        public GSAAdapter(string filePath) : this()
        {
            short result;
            if (!string.IsNullOrWhiteSpace(filePath))
                result = m_gsa.Open(filePath);
            else
                result = m_gsa.NewFile();
        }

        /***************************************************/
        /**** Public  properties                        ****/
        /***************************************************/

        public const string ID = "GSA_id";

        public string AdapterId
        {
            get { return ID; }
        }

        /***************************************************/

        public List<string> ErrorLog
        {
            get;
            set;
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null)
        {
            return ComCall(command);
        }

        /***************************************************/

        public List<string> GetBars(out List<Bar> bars, List<string> ids = null)
        {
            return FromGsa(out bars, ids);
        }

        /***************************************************/

        public List<string> GetMaterials(out List<Material> materials, List<string> ids = null)
        {
            return FromGsa(out materials, ids);
        }

        /***************************************************/

        public List<string> GetNodes(out List<Node> nodes, List<string> ids = null)
        {
            return FromGsa(out nodes, ids);
        }

        /***************************************************/

        public List<string> GetSectionProperties(out List<SectionProperty> sectionProperties, List<string> ids = null)
        {
            return FromGsa(out sectionProperties, ids);
        }

        /***************************************************/

        public IList Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public bool Push(IEnumerable<object> objects, string key = "", Dictionary<string, string> config = null)
        {
            return StructuralPush.PushByType(this, objects, key, config);
        }

        /***************************************************/

        public int Update(FilterQuery filter, Dictionary<string, object> changes, Dictionary<string, string> config = null)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public bool UpdateTags(IEnumerable<object> objects)
        {
            return CreateObjects(objects);
        }

        /***************************************************/
    }
}
