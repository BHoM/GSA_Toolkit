using System.Collections.Generic;


namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Adapter Methods                           ****/
        /***************************************************/

        protected override bool UpdateObjects<T>(IEnumerable<T> objects)
        {
            return Create(objects, false);
        }

        /***************************************************/
    }
}
