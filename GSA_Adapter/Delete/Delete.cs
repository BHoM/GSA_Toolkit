using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;
using BH.Adapter.Queries;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {

        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            Type type = (Type)filter.Equalities["Type"];
            List<string> indicies = (List<string>)filter.Equalities["Indices"];

            return DeleteObjects(type, indicies.Select(x => int.Parse(x)).ToList())? 1:0;

        }

        /***************************************************/

        public bool DeleteObjects(Type type, List<int> indecies = null)
        {
            string typeString = type.GetGsaTypeString();

            if (typeString == null)
            {
                ErrorLog.Add("Delete failed due to wrong type");
                return false;
            }
            else
            {
                bool success = true;
                foreach (string range in GetRanges(type,indecies))
                {
                    success &= ComCall("BLANK," + typeString + "," + range);
                }

                return success;
            }
            
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private List<string> GetRanges(Type type, List<int> indecies)
        {
            List<string> ranges = new List<string>();
            if (indecies == null || indecies.Count < 1)
            {
                return new List<string>();
                //string str = "1," + Indexing.GetHighIndex(type, adapter);
                //ranges.Add(str);
            }
            else
            {
                indecies.Sort();
                int first = indecies[0];

                for (int i = 0; i < indecies.Count-1; i++)
                {
                    if (indecies[i] == indecies[i + 1] - 1)
                        continue;
                    else
                    {
                        ranges.Add(first + "," + indecies[i]);
                        first = indecies[i + 1];
                    }

                }

                ranges.Add(first + "," + indecies.Last());

            }

            return ranges;
        }
    }
}
