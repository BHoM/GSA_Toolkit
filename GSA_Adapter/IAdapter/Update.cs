using BH.Adapter.Queries;
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
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public int Update(FilterQuery filter, Dictionary<string, object> changes, Dictionary<string, string> config = null)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/
    }
}
