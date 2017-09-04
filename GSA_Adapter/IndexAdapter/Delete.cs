using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;
using BH.Adapter.Queries;
using BH.oM.Base;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Methods                     ****/
        /***************************************************/

        protected override int Delete(Type type, List<int> indices = null)
        {
            string typeString = type.ToGsaString();

            if (typeString == null)
            {
                ErrorLog.Add("Delete failed due to wrong type");
                return 0;
            }
            else if (indices == null)
            {
                // Delete them all
                int maxIndex = m_gsa.GwaCommand("HIGHEST, " + typeString);
                if (ComCall("BLANK," + typeString + "," + 0 + "," + maxIndex))
                    return (maxIndex + 1); // TODO: Check that this is correct for Gsa
                else
                    return 0;
            }
            else
            {
                int successful = 0;
                foreach (Tuple<int, int> range in GetRanges(type, indices))
                {
                    if (ComCall("BLANK," + typeString + "," + range.Item1 + "," + range.Item2))
                        successful += (range.Item2 - range.Item1 + 1); // TODO: Check that this is correct for Gsa
                }

                return successful;
            }

        }


        /***************************************************/
        /**** Private  Helpers                          ****/
        /***************************************************/

        private List<Tuple<int, int>> GetRanges(Type type, List<int> indices)
        {
            List<Tuple<int, int>> ranges = new List<Tuple<int, int>>();
            if (indices == null || indices.Count < 1)
            {
                return new List<Tuple<int, int>>();
            }
            else
            {
                indices.Sort();
                int first = indices[0];

                for (int i = 0; i < indices.Count-1; i++)
                {
                    if (indices[i] == indices[i + 1] - 1)
                        continue;
                    else
                    {
                        ranges.Add(new Tuple<int, int>(first, indices[i]));
                        first = indices[i + 1];
                    }
                }

                ranges.Add(new Tuple<int, int>(first, indices.Last()));
            }

            return ranges;
        }
    }
}
