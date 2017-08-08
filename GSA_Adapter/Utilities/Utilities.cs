using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;

using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;

namespace BH.Adapter.GSA
{
    public static class Utilities
    {
      

        /***************************************/

        public static string GetGsaTypeString(this Type type)
        {
            if (type == typeof(Node))
                return "NODE";
            else if (type == typeof(Bar))
                return "EL";
            else if (type == typeof(Material))
                return "MAT";
            else if (type == typeof(SectionProperty))
                return "PROP_SEC";

            return null;
        }

        /***************************************/
    }
}

