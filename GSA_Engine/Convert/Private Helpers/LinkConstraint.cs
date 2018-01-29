using BH.oM.Geometry;
using BH.oM.Structural.Properties;
using BH.Engine.Structure;
using System;
using System.Collections.Generic;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {

        public static string GetRestraintString(LinkConstraint constr)
        {
            string str = "";

            if (constr.XtoX || constr.XtoYY || constr.XtoZZ)
            {
                str += "X:";

                if (constr.XtoX)
                    str += "X";
                if (constr.XtoYY)
                    str += "YY";
                if (constr.XtoZZ)
                    str += "ZZ";

                str += "-";
            }

            if (constr.YtoY || constr.YtoZZ || constr.YtoZZ)
            {
                str += "Y:";

                if (constr.YtoY)
                    str += "Y";
                if (constr.YtoXX)
                    str += "XX";
                if (constr.YtoZZ)
                    str += "ZZ";

                str += "-";
            }

            if (constr.ZtoZ || constr.ZtoXX || constr.ZtoYY)
            {
                str += "Z:";

                if (constr.ZtoZ)
                    str += "Z";
                if (constr.ZtoXX)
                    str += "XX";
                if (constr.ZtoYY)
                    str += "YY";

                str += "-";
            }

            if (constr.XXtoXX)
                str += "XX:XX-";

            if (constr.YYtoYY)
                str += "YY:YY-";

            if (constr.ZZtoZZ)
                str += "ZZ:ZZ-";


            str = str.TrimEnd('-');

            return str;

        }
    }
}
