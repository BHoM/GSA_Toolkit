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

        public static string SectionType(this ISectionProperty prop)
        {
            if (prop is SteelSection)
            {
                SteelSection steel = prop as SteelSection;

                switch (steel.Fabrication)
                {
                    case SteelFabrication.Welded:
                        return "WELDED";
                    case SteelFabrication.HotRolled:
                        return "ROLLED";
                    case SteelFabrication.HotFormed:
                    case SteelFabrication.ColdFormed:
                        return "FORMED";
                    default:
                        return "NA";
                }
            }

            return "NA";
        }

        /***************************************************/
    }
}
