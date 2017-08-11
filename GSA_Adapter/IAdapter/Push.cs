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

        public bool Push(IEnumerable<object> objects, string key = "", Dictionary<string, string> config = null)
        {
            return StructuralPusher.PushByType(this, objects, key, config);
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        
    }
}
