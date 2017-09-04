using BH.oM.Base;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
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

        protected override IEnumerable<BHoMObject> Read(Type type, List<string> indices)
        {
            // Define the dictionary of Pull methods
            Dictionary<Type, Func<List<string>, IList>> m_PullMethods = new Dictionary<Type, Func<List<string>, IList>>()
            {
                {typeof(Material), PullMaterials },
                {typeof(SectionProperty), PullSectionProperties },
                {typeof(Node), PullNodes },
                {typeof(Bar), PullBars }
            };

            // Get the objects based on the indices
            if (m_PullMethods.ContainsKey(type))
                return m_PullMethods[type](indices).Cast<BHoMObject>();
            else
                return new List<BHoMObject>();
        }
    }
}
