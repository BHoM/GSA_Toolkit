using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structural.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> ForceVectorsStrings(BH.oM.Geometry.Vector[] vec, double factor, bool translational, string[] pos)
        {
            List<string> strings = new List<string>();

            if (vec != null)
            {
                string[] dir = Directions(translational);

                if (vec[0].X != 0 || vec[1].X != 0)
                    strings.Add(dir[0] + pos[0] + (factor * vec[0].X).ToString() + pos[1] + (factor * vec[1].X).ToString());
                if (vec[0].Y != 0 || vec[1].Y != 0)
                    strings.Add(dir[1] + pos[0] + (factor * vec[0].Y).ToString() + pos[1] + (factor * vec[1].Y).ToString());
                if (vec[0].Z != 0 || vec[1].Z != 0)
                    strings.Add(dir[2] + pos[0] + (factor * vec[0].Z).ToString() + pos[1] + (factor * vec[1].Z).ToString());
            }
            return strings;
        }
    }
}
