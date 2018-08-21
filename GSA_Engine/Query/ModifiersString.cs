using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Properties;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ModifiersString(this ISectionProperty secProp)
        {
            string mods = "NO_MOD_PROP";

            double[] modifiers = Structure.Query.Modifiers(secProp);

            if (modifiers != null)
            {
                if (modifiers.Length == 6)
                {
                    mods = "MOD_PROP";
                    for (int i = 0; i < 6; i++)
                    {
                        mods += ",BY," + modifiers[i];
                    }
                    mods += ",NO,NO_MOD";
                }

            }

            return mods;
        }

        /***************************************************/
    }
}
