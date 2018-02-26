using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structural.Elements;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static string ElementType(this BHoMGroup<Node> group)
        {
            return "NODE";
        }

        /***************************************************/

        public static string ElementType(this BHoMGroup<Bar> group)
        {
            return "ELEMENT";
        }

        /***************************************************/

        public static string ElementType(this BHoMGroup<MeshFace> group)
        {
            return "ELEMENT";
        }

        /***************************************************/

        public static string ElementType(this BHoMGroup<RigidLink> group)
        {
            return "ELEMENT";
        }

        /***************************************************/

        public static string ElementType(this BHoMGroup<IBHoMObject> group)
        {
            if (group.Elements.Where(x => x.GetType() == typeof(Node)).Count() == group.Elements.Count)
                return "NODE";

            return "ELEMENT";
        }

        /***************************************************/

        public static string ElementType(this BHoMGroup<BHoMObject> group)
        {
            if (group.Elements.Where(x => x.GetType() == typeof(Node)).Count() == group.Elements.Count)
                return "NODE";

            return "ELEMENT";
        }

        /***************************************************/

        public static string ElementType(this BHoMGroup<IAreaElement> group)
        {
            return "ELEMENT";
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/
        public static string IElementType<T>(this BHoMGroup<T> group) where T : IBHoMObject
        {
            return ElementType(group as dynamic);
        }

        /***************************************************/

    }
}
