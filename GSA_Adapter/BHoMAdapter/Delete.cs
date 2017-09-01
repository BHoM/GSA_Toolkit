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
        /**** BHoM Adapter Methods                      ****/
        /***************************************************/

        protected override int Delete(Type type, string tag = "")
        {
            string typeString = type.ToGsaString();

            if (typeString == null)
            {
                ErrorLog.Add("Delete failed due to wrong type"); //TODO: Can we something that only use tag and not type for deletion
                return 0;                                        //      If the tag is also empty, delete everything
            }
            else
            {
                if (tag == "")
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
                    // Delete all with tag
                    IEnumerable<BHoMObject> withTag = Pull<BHoMObject>(new List<IQuery> { new FilterQuery(type, tag) });

                    // Delete all with that tag only
                    List<int> indices = withTag.Where(x => x.Tags.Count == 1).Select(x => (int)x.CustomData[AdapterId]).OrderBy(x => x).ToList();
                    Delete(type, indices);

                    // Remove tag if other tags as well
                    IEnumerable<BHoMObject> multiTags = withTag.Where(x => x.Tags.Count > 1);
                    UpdateTags(multiTags);

                    return indices.Count;
                }

            }
        }

    }
}
