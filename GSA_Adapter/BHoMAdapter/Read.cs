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

        protected override IEnumerable<object> Read(Type type, string tag = "")
        {
            return new List<object>(); // TODO: Implement this
        }

        
    }
}
