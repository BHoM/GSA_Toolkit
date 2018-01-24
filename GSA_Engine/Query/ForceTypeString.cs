using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structural.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        public static string IForceTypeString(this ILoad load)
        {
            return ForceTypeString(load as dynamic);
        }

        public static string ForceTypeString(this PointForce load)
        {
            return "LOAD_NODE";
        }

        public static string ForceTypeString(this BarPointLoad load)
        {
            return "LOAD_BEAM_POINT";
        }

        public static string ForceTypeString(this BarUniformlyDistributedLoad load)
        {
            return "LOAD_BEAM_UDL";
        }

        public static string ForceTypeString(this BarVaryingDistributedLoad load)
        {
            return "LOAD_BEAM_TRILIN";
        }
    }
}
