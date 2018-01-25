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
        public static Vector[] ITranslationVector(this ILoad load)
        {
            return TranslationVector(load as dynamic);
        }

        public static Vector[] TranslationVector(this PointForce load)
        {
            Vector[] loadVec = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return loadVec;
        }

        public static Vector[] TranslationVector(this PointDisplacement load)
        {
            Vector[] transVecs = { load.Translation, BH.Engine.Geometry.Create.Vector() };
            return transVecs;
        }

        public static Vector[] TranslationVector(this BarPointLoad load)
        {
            Vector[] forceVecs = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        public static Vector[] TranslationVector(this BarUniformlyDistributedLoad load)
        {
            Vector[] forceVecs = { load.Force, BH.Engine.Geometry.Create.Vector() };
            return forceVecs;
        }

        public static Vector[] TranslationVector(this BarVaryingDistributedLoad load)
        {
            Vector[] forceVecs = { load.ForceA, load.ForceB };
            return forceVecs;
        }
    }
}
