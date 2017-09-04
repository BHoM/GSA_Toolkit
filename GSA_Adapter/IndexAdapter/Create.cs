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
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override bool Create(IEnumerable<object> objects)
        {
            bool success = true;
            foreach (BH.oM.Base.BHoMObject obj in objects)
            {
                success &= ComCall(Convert.ToGsaString(obj, obj.CustomData[AdapterId].ToString()));
            }
            UpdateViews();
            return success;
        }

    }
}
