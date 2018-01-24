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
        public static Vector[] IRotationVector(this ILoad load)
        {
            return RotationVector(load as dynamic);
        }

        public static Vector[] RotationVector(this PointForce load)
        {
            Vector[] momentVecs = { load.Moment };
            return momentVecs;
        }

        public static Vector[] RotationVector(this PointDisplacement load)
        {
            Vector[] rotVecs = { load.Rotation };
            return rotVecs;
        }

        public static Vector[] RotationVector(this BarPointLoad load)
        {
            Vector[] momentVecs = { load.Moment, BH.Engine.Geometry.Create.Vector() };
            return momentVecs;
        }

        public static Vector[] RotationVector(this BarUniformlyDistributedLoad load)
        {
            Vector[] momentVecs = { load.Moment, BH.Engine.Geometry.Create.Vector() };
            return momentVecs;
        }

        public static Vector[] RotationVector(this BarVaryingDistributedLoad load)
        {
            Vector[] momentVecs = { load.MomentA, load.MomentB};
            return momentVecs;
        }
    }
}
