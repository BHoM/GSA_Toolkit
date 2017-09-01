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
        /**** IStructuralAdapter Interface              ****/
        /***************************************************/

        protected override bool UpdateTags(IEnumerable<object> objects)
        {
            return Create(objects);
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/
    }
}
