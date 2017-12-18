using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter 
    {
        /***************************************************/
        /**** BHoM Adapter Interface                    ****/
        /***************************************************/

        protected override List<Type> GetDependencyTypes<T>()
        {
            Type type = typeof(T);

            if (m_DependencyTypes.ContainsKey(type))
                return m_DependencyTypes[type];

            else if (m_DependencyTypes.ContainsKey(type.BaseType))
                return m_DependencyTypes[type.BaseType];

            else
            {
                foreach (Type interType in type.GetInterfaces())
                {
                    if (m_DependencyTypes.ContainsKey(interType))
                        return m_DependencyTypes[interType];
                }
            }


            return new List<Type>();         
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Type, List<Type>> m_DependencyTypes = new Dictionary<Type, List<Type>>
        {
            {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } },
            {typeof(ISectionProperty), new List<Type> { typeof(Material) } }
        };

    }
}
