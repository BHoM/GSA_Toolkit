using BH.Engine.GSA;
using BH.oM.Structural.Loads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override object NextId(Type type, bool refresh)
        {

            if (type == typeof(LoadCombination))
                return 0; //TODO: Needed?
            else if (type == typeof(Loadcase))
                return 0; //TODO: Needed?
            else if (type == typeof(ILoad) || type.GetInterfaces().Contains(typeof(ILoad)))
                return 0;

            string typeString = type.ToGsaString();

            int index;
            if (!refresh && m_indexDict.TryGetValue(type, out index))
            {
                index++;
                m_indexDict[type] = index;
            }
            else
            {
                index =  m_gsaCom.GwaCommand("HIGHEST, " + typeString) + 1;
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
