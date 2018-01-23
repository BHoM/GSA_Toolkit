﻿using BH.Engine.Structure;
using BH.oM.Base;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************/

        private static string GetElementTypeString(Bar bar)
        {
            switch (bar.FEAType)
            {
                case BarFEAType.Axial:
                    return "BAR";
                case BarFEAType.Flexural:
                    return "BEAM";
                case BarFEAType.TensionOnly:
                    return "TIE";
                case BarFEAType.CompressionOnly:
                    return "STRUT";
                default:
                    return "BEAM";
                    //Returning beam by default as it is the most generic type.
                    //Might be better flagging this as an error
            }
        }

        /***************************************/

        private static string CreateReleaseString(Constraint6DOF nodeConstraint)
        {
            bool[] fixities = nodeConstraint.Fixities();
            double[] stiffness = nodeConstraint.ElasticValues();

            string relStr = "";
            string stiffStr = "";

            for (int i = 0; i < fixities.Length; i++)
            {
                if (!fixities[i])
                {
                    relStr += "F";
                }
                else
                {
                    if (stiffness[i] > 0)
                    {
                        relStr += "K";
                        stiffStr += "," + stiffness[i];
                    }
                    else
                        relStr += "R";
                }

            }

            return relStr + stiffStr;
        }

        /***************************************/

        private static string CheckDummy(BHoMObject obj)
        {
            if (obj.CustomData.ContainsKey("Dummy"))
                return "DUMMY";

            return "";
        }

        /***************************************/
    }
}