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

        public bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null)
        {
            return ComCall(command);
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/
    }
}
