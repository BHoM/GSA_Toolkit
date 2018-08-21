using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Loads;
using BH.oM.Base;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        public static string CreateIdListOrGroupName<T>(this Load<T> load) where T : IBHoMObject
        {
            //For a named group, appy loads to the group name
            if (!string.IsNullOrWhiteSpace(load.Objects.Name))
                return "\"" + load.Objects.Name + "\"";

            //Otherwise apply to the corresponding indecies
            return load.Objects.Elements.Select(x => int.Parse(x.CustomData[AdapterID].ToString())).GeterateIdString();

        }

        public static string GeterateIdString(this IEnumerable<int> ids)
        {
            string str = "";

            int counter = 0;
            int prev = -10;

            foreach (int i in ids)
            {
                if (i - 1 == prev)
                {
                    counter++;
                }
                else
                {
                    if (counter > 1)
                        str += "to " + prev + " ";
                    else if (counter > 0)
                        str += prev + " ";

                    str += i.ToString() + " ";
                    counter = 0;
                }

                prev = i;
            }

            if (counter > 1)
                str += "to " + prev + " ";
            else if (counter > 0)
                str += prev + " ";

            return str;
        }
    }
}
