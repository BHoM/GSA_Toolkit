using BH.Engine.GSA;
using System;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override object NextId(Type type, bool refresh)
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


        /***************************************************/
    }
}
