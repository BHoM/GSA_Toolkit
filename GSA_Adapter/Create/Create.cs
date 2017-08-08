using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/
        public bool CreateObject(object obj, string index)
        {
            return ComCall(Convert.GetGsaString(obj, index));
        }

        /***************************************************/

        public bool CreateObjects(IEnumerable<object> obj)
        {
            return _CreateObjects(obj as dynamic);
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private bool _CreateObjects(IEnumerable<BH.oM.Base.BHoMObject> objects)
        {
            bool success = true;
            foreach (BH.oM.Base.BHoMObject obj in objects)
            {
                success &= ComCall(Convert.GetGsaString(obj, obj.CustomData[ID].ToString()));
            }
            UpdateViews();
            return success;
        }

        /***************************************************/
    }
}
