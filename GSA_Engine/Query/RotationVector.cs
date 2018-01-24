using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Structural.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        public static Vector IRotationVector(this ILoad load)
        {
            return RotationVector(load as dynamic);
        }

        public static Vector RotationVector(this PointForce load)
        {
            return load.Moment;
        }

        public static Vector RotationVector(this PointDisplacement load)
        {
            return load.Rotation;
        }
    }
}
