using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;
using BH.oM.Materials;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************/

        private static void CreateDescAndPropString(SectionProperty secProp, out string desc, out string prop)
        {

            if (secProp is SteelSection)
            {
                SteelSection steelSec = secProp as SteelSection;

                switch (steelSec.Shape)
                {
                    case ShapeType.Box:
                        CreateBoxString(steelSec, out desc, out prop);
                        return;
                    case ShapeType.Angle:
                        CreateAngleString(steelSec, out desc, out prop);
                        return;
                    case ShapeType.ISection:
                        CreateIString(steelSec, out desc, out prop);
                        return;
                    case ShapeType.Tee:
                        CreateTeeString(steelSec, out desc, out prop);
                        return;
                    case ShapeType.Tube:
                        CreateTubeString(steelSec, out desc, out prop);
                        return;
                }
            }

            switch (secProp.Shape)
            {
                case ShapeType.Rectangle:
                    CreateRectString(secProp, out desc, out prop);
                    return;
                case ShapeType.Channel:
                    CreateChannelString(secProp, out desc, out prop);
                    return;
                case ShapeType.Circle:
                    CreateCircleString(secProp, out desc, out prop);
                    return;
                case ShapeType.Zed:
                    CreateZedString(secProp, out desc, out prop);
                    return;
                case ShapeType.Polygon:
                    CreatePolygonString(secProp, out desc, out prop);
                    return;
                case ShapeType.DoubleAngle:
                    CreateDoubleAngleString(secProp, out desc, out prop);
                    return;
                case ShapeType.DoubleChannel:
                    CreateDoubleChannelString(secProp, out desc, out prop);
                    return;
                case ShapeType.Cable:
                default:
                    CreateCustomDataString(secProp, out desc, out prop);
                    return;
            }
        }

        /***************************************/

        private static void CreateCustomDataString(SectionProperty secProp, out string desc, out string prop)
        {
            desc = "EXP";
            string area = secProp.Area.ToString();
            string Iyy = secProp.Iy.ToString();
            string Izz = secProp.Iz.ToString();
            string J = secProp.J.ToString();
            string Avy = secProp.Asy.ToString();
            string Avz = secProp.Asz.ToString();

            prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;
        }

        /***************************************/

        private static void CreateDoubleChannelString(SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static void CreateCutIString(SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static void CreateDoubleAngleString(SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static void CreatePolygonString(SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static void CreateZedString(SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static void CreateCircleString(SectionProperty secProp, out string desc, out string prop)
        {
            //STD%C%60.
            prop = "NO_PROP";

            double d;

            d = secProp.TotalDepth * 1000;

            desc = string.Format("STD%C%{0}", d.ToStringWithDecimalPoint());

        }

        /***************************************/

        private static void CreateTubeString(SteelSection secProp, out string desc, out string prop)
        {
            //STD%CHS%60.%5.

            prop = "NO_PROP";

            double d, t;

            d = secProp.TotalDepth * 1000;
            t = secProp.Tw * 1000;

            desc = string.Format("STD%CHS%{0}%{1}", d.ToStringWithDecimalPoint(), t.ToStringWithDecimalPoint());
        }

        /***************************************/

        private static void CreateChannelString(SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static void CreateTeeString(SteelSection secProp, out string desc, out string prop)
        {
            //STD%T%150.%100.%5.%10.

            prop = "NO_PROP";

            double h, w, wt, ft;

            h = secProp.TotalDepth * 1000;
            w = secProp.TotalWidth * 1000;
            wt = secProp.Tw * 1000;
            ft = secProp.Tf1 * 1000;

            desc = string.Format("STD%T%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());

        }

        /***************************************/

        private static void CreateIString(SteelSection secProp, out string desc, out string prop)
        {
            //STD%I%150.%100.%5.%20.
            //STD%GI%150.%100.%50.%6.%10.%5.

            prop = "NO_PROP";

            double h, widthT, widthB, wt, ftT, ftB;

            h = secProp.TotalDepth * 1000;
            widthT = secProp.B1 * 1000;
            widthB = secProp.B2 * 1000;
            wt = secProp.Tw * 1000;
            ftT = secProp.Tf1 * 1000;
            ftB = secProp.Tf2 * 1000;

            if (widthB == widthT && ftB == ftT)
            {
                //STD%I%150.%100.%5.%20.
                desc = string.Format("STD%I%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), widthT.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ftT.ToStringWithDecimalPoint());
            }
            else
            {
                //STD%GI%150.%100.%50.%6.%10.%5.
                desc = string.Format("STD%GI%{0}%{1}%{2}%{3}%{4}%{5}", h.ToStringWithDecimalPoint(), widthT.ToStringWithDecimalPoint(), widthB.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ftT.ToStringWithDecimalPoint(), ftB.ToStringWithDecimalPoint());

            }
        }

        /***************************************/

        private static void CreateAngleString(SteelSection secProp, out string desc, out string prop)
        {
            //STD%A%150.%100.%5.%10.

            prop = "NO_PROP";

            double h, w, wt, ft;

            h = secProp.TotalDepth * 1000;
            w = secProp.TotalWidth * 1000;
            wt = secProp.Tw * 1000;
            ft = secProp.Tf1 * 1000;

            desc = string.Format("STD%A%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());
        }

        /***************************************/

        private static void CreateBoxString(SteelSection secProp, out string desc, out string prop)
        {
            prop = "NO_PROP";

            double h, w, wt, ft;

            h = secProp.TotalDepth * 1000;
            w = secProp.TotalWidth * 1000;
            wt = secProp.Tw * 1000;
            ft = secProp.Tf1 * 1000;

            desc = string.Format("STD%RHS%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());

            //STD%RHS%800.%400.%12.%16.

        }

        /***************************************/

        private static void CreateRectString(SectionProperty secProp, out string desc, out string prop)
        {
            prop = "NO_PROP";

            double h, w;

            h = secProp.TotalDepth * 1000;
            w = secProp.TotalWidth * 1000;

            desc = string.Format("STD%R%{0}%{1}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint());
            //STD%R%50.%20.
        }

        /***************************************/

        private static string ToStringWithDecimalPoint(this double d)
        {
            return Math.Abs(d - (int)d) < double.Epsilon ? d.ToString() + "." : d.ToString();
        }

        
    }
}
