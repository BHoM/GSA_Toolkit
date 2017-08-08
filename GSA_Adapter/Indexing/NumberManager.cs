using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {

        private Dictionary<Type, int> m_indexDict = new Dictionary<Type, int>();

        /********************************************/

        public object GetNextIndex(Type type, bool refresh)
        {
            string typeString = type.GetGsaTypeString();

            int index;
            if (!refresh && m_indexDict.TryGetValue(type, out index))
            {
                index++;
                m_indexDict[type] = index;
            }
            else
            {
                index =  m_gsa.GwaCommand("HIGHEST, " + typeString) + 1;
                m_indexDict[type] = index;
            }

            return index;
        }

        /********************************************/

        private int[] GeneratePotentialElementIndecies(List<string> barNumbers)
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
        private int[] GeneratePotentialNodeIdIndecies(List<string> ids)
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

        /********************************************/

    }
}
