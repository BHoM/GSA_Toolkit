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
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            List<string> indices = (List<string>)filter.Equalities["Indices"];

            if (indices != null && indices.Count > 0)
                return Delete(filter.Type, indices.Select(x => int.Parse(x)).ToList());
            else
                return Delete(filter.Type, filter.Tag);
        }

        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public int Delete(Type type, List<int> indices = null)
        {
            string typeString = type.ToGsaString();

            if (typeString == null)
            {
                ErrorLog.Add("Delete failed due to wrong type");
                return 0;
            }
            else
            {
                int successful = 0;
                foreach (string range in GetRanges(type,indices))
                {
                    if (ComCall("BLANK," + typeString + "," + range))
                        successful++;
                }

                return successful;
            }
            
        }

        /***************************************************/

        public int Delete(Type type, string tag = "")
        {
            string typeString = type.ToGsaString();

            if (typeString == null)
            {
                ErrorLog.Add("Delete failed due to wrong type"); //TODO: Can we something that only use tag and not type for deletion
                return 0;                                        //      If the tag is also empty, delete everything
            }
            else
            {
                throw new NotImplementedException();  // TODO: implement deltion of objects with a specific tag 
            }
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private List<string> GetRanges(Type type, List<int> indices)
        {
            List<string> ranges = new List<string>();
            if (indices == null || indices.Count < 1)
            {
                return new List<string>();
                //string str = "1," + Indexing.GetHighIndex(type, adapter);
                //ranges.Add(str);
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
                        ranges.Add(first + "," + indices[i]);
                        first = indices[i + 1];
                    }

                }

                ranges.Add(first + "," + indices.Last());

            }

            return ranges;
        }
    }
}
