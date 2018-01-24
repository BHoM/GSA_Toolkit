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
        public static string[] ILoadPosition(this ILoad load)
        {
            return LoadPosition(load as dynamic);
        }

        public static string[] LoadPosition(BarPointLoad load)
        {
            string[] positions = { load.DistanceFromA.ToString(), "" };
            return positions;
        }

        public static string[] LoadPosition(BarUniformlyDistributedLoad load)
        {
            string[] positions = { "", "" };
            return positions;
        }

    }
}
