using System;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            bool success = true;

            if (typeof(BH.oM.Base.BHoMObject).IsAssignableFrom(typeof(T)))
            {
                foreach (T obj in objects)
                {
                    success &= ComCall(Engine.GSA.Convert.IToGsaString(obj, (obj as BH.oM.Base.BHoMObject).CustomData[AdapterId].ToString()));
                }
            }
            UpdateViews();
            return success;
        }

        /***************************************************/
    }
}
