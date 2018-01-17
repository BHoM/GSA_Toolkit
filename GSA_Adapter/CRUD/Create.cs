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

            if (typeof(BH.oM.Base.IObject).IsAssignableFrom(typeof(T)))
            {
                foreach (T obj in objects)
                {
                    success &= ComCall(Engine.GSA.Convert.IToGsaString(obj, (obj as BH.oM.Base.IObject).CustomData[AdapterId].ToString()));
                }
            }
            UpdateViews();
            return success;
        }

        /***************************************************/
    }
}
