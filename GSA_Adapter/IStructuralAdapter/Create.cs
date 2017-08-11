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
        /**** IStructuralAdapter Interface              ****/
        /***************************************************/

        public bool Create(IEnumerable<object> obj)
        {
            return _Create(obj as dynamic);
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public bool Create(object obj, string index)
        {
            return ComCall(Convert.ToGsaString(obj, index));
        }


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private bool _Create(IEnumerable<BH.oM.Base.BHoMObject> objects)
        {
            bool success = true;
            foreach (BH.oM.Base.BHoMObject obj in objects)
            {
                success &= ComCall(Convert.ToGsaString(obj, obj.CustomData[AdapterId].ToString()));
            }
            UpdateViews();
            return success;
        }

        /***************************************************/
    }
}
