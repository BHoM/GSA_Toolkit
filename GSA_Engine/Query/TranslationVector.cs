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
        public static Vector ITranslationVector(this ILoad load)
        {
            return TranslationVector(load as dynamic);
        }

        public static Vector TranslationVector(this PointForce load)
        {
            return load.Force;
        }

        public static Vector TranslationVector(this PointDisplacement load)
        {
            return load.Translation;
        }
    }
}
