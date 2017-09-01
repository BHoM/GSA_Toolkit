using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;
using BH.Adapter.Queries;
using BH.Adapter.Strutural;
using System.Collections;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        protected bool PushType(List<Bar> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<Bar> comparer = EqualityComparer<Bar>.Default;
            List<Type> dependencyTypes = new List<Type> { typeof(SectionProperty), typeof(Node) };
            return PushType(objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }

        /***************************************************/

        protected bool PushType(List<Node> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<Node> comparer = new BH.Engine.Structure.NodeDistanceComparer(3);
            List<Type> dependencyTypes = new List<Type>();
            return PushType(objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }

        /***************************************************/

        protected bool PushType(List<SectionProperty> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<SectionProperty> comparer = new BH.Engine.Base.BHoMObjectNameOrToStringComparer();
            List<Type> dependencyTypes = new List<Type> { typeof(Material) };
            return PushType(objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }

        /***************************************************/

        protected bool PushType(List<Material> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<Material> comparer = new BH.Engine.Base.BHoMObjectNameComparer();
            List<Type> dependencyTypes = new List<Type>();
            return PushType(objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }
    }
}
