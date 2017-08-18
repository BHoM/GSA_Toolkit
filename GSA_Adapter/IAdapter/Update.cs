using BH.Adapter.Queries;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public int Update(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null)
        {
            Type propertyType = filter.Type.GetProperty(property).PropertyType;
            MethodInfo method = typeof(Merge).GetMethod("Update");
            MethodInfo generic = method.MakeGenericMethod(new Type[] { filter.Type, propertyType });
            return (int)generic.Invoke(null, new object[] { filter, property, newValue, config});

        }

        /***************************************************/

        public int Update<T, P>(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null) where T: BHoMObject
        {
            // Pull the objects to update
            List<T> objects = Pull<T>(new List<IQuery> { filter }).ToList();

            // Set their property
            Action<T, P> setProp = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T>), typeof(T).GetProperty(property).GetSetMethod());
            if (newValue is IEnumerable<P>)
            {
                // Case of a list of properties
                List<P> values = ((IEnumerable<P>)newValue).ToList();
                if (values.Count == objects.Count)
                {
                    for (int i = 0; i < values.Count; i++)
                        setProp(objects[i], values[i]);
                }
            }
            else
            {
                // Case of a single common property
                P value = (P)newValue;
                foreach(T obj in objects)
                    setProp(obj, value);
            }

            // Push the objects back
            Create(objects);

            return objects.Count;
        }


        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/
    }
}
