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
        public static double IFactor(this ILoad load, double[] unitType)
        {
            return Factor(load as dynamic, unitType);
        }

        public static double Factor(this PointForce load, double[] unitType)
        {
            return unitType[0];
        }

        public static double Factor(this PointDisplacement load, double[] unitType)
        {
            return unitType[2];
        }

        public static double Factor(this BarPointLoad load, double[] unitType)
        {
            return unitType[1];
        }

        public static double Factor(this BarUniformlyDistributedLoad load, double[] unitType)
        {
            return unitType[1];
        }
    }
}
