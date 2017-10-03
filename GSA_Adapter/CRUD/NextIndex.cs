using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override object GetNextId(Type type, bool refresh)
        {
            string typeString = type.ToGsaString();

            int index;
            if (!refresh && m_indexDict.TryGetValue(type, out index))
            {
                index++;
                m_indexDict[type] = index;
            }
            else
            {
                index =  gsaCom.GwaCommand("HIGHEST, " + typeString) + 1;
                m_indexDict[type] = index;
            }

            return index;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private Dictionary<Type, int> m_indexDict = new Dictionary<Type, int>();
    }
}
