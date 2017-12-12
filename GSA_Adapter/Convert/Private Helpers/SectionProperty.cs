using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.oM.Materials;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************/

        private static bool ICreateDescAndPropString(ISectionProperty secProp, out string desc, out string prop)
        {
            return CreateDescAndPropString(secProp as dynamic, out desc, out prop);
        }

        /***************************************/

        private static bool CreateDescAndPropString(ExplicitSection secProp, out string desc, out string prop)
        {
            desc = "EXP";
            string area = secProp.Area.ToString();
            string Iyy = secProp.Iy.ToString();
            string Izz = secProp.Iz.ToString();
            string J = secProp.J.ToString();
            string Kvy = (secProp.Asy/secProp.Area).ToString();
            string Kvz = (secProp.Asz / secProp.Area).ToString();

            prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Kvy + "," + Kvz;
            return true;
        }

        /***************************************/

        private static bool CreateDescAndPropString(CableSection secProp, out string desc, out string prop)
        {
            //TODO: Handle cables as non-explicit
            desc = "EXP";
            string area = secProp.Area.ToString();
            string Iyy = secProp.Iy.ToString();
            string Izz = secProp.Iz.ToString();
            string J = secProp.J.ToString();
            string Kvy = (secProp.Asy / secProp.Area).ToString();
            string Kvz = (secProp.Asz / secProp.Area).ToString();

            prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Kvy + "," + Kvz;
            return true;
        }

        /***************************************/
        private static bool CreateDescAndPropString(SteelSection secProp, out string desc, out string prop)
        {
            prop = "NO_PROP";
            if (secProp.SectionDimensions.Shape == ShapeType.Polygon)
                return CreateDescString(secProp.Edges, out desc);
            else
                return ICreateDescString(secProp.SectionDimensions, out desc);
        }

        /***************************************/
        private static bool CreateDescAndPropString(ConcreteSection secProp, out string desc, out string prop)
        {
            //TODO: Reinforcement???
            prop = "NO_PROP";
            if (secProp.SectionDimension.Shape == ShapeType.Polygon)
                return CreateDescString(secProp.Edges, out desc);
            else
                return ICreateDescString(secProp.SectionDimension, out desc);
        }

        /***************************************/



        private static bool ICreateDescString(ISectionDimensions dimensions, out string desc)
        {
            return CreateDescString(dimensions as dynamic, out desc);
        }

        /***************************************/

        private static bool CreateDescString(FabricatedBoxDimensions dimensions, out string desc)
        {
            if(dimensions.TopFlangeThickness != dimensions.BotFlangeThickness)
                throw new NotSupportedException("Box sections with varying flange thickness between top and bottom are currently not suported in the GSA adapter");

            double h, w, wt, ft;

            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;
            wt = dimensions.WebThickness * 1000;
            ft = dimensions.BotFlangeThickness * 1000;

            desc = string.Format("STD%RHS%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        private static bool CreateDescString(StandardBoxDimensions dimensions, out string desc)
        {
            double h, w, t;
            //TODO: Any way of accessing the catalogue sections?
            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;
            t = dimensions.Thickness * 1000;

            desc = string.Format("STD%RHS%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), t.ToStringWithDecimalPoint(), t.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        /***************************************/

        private static bool CreateDescString(StandardChannelSectionDimensions dimensions, out string desc)
        {
            double h, w, tw, tf;
            //TODO: Any way of accessing the catalogue sections?
            h = dimensions.Height * 1000;
            w = dimensions.FlangeWidth * 1000;
            tw = dimensions.WebThickness * 1000;
            tf = dimensions.FlangeThickness * 1000;

            desc = string.Format("STD%CH%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), tw.ToStringWithDecimalPoint(), tf.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        /***************************************/

        private static bool CreateDescString(StandardAngleSectionDimensions dimensions, out string desc)
        {
            double h, w, tw, tf;
            //TODO: Any way of accessing the catalogue sections?
            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;
            tw = dimensions.WebThickness * 1000;
            tf = dimensions.FlangeThickness * 1000;

            desc = string.Format("STD%A%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), tw.ToStringWithDecimalPoint(), tf.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        /***************************************/

        private static bool CreateDescString(StandardTeeSectionDimensions dimensions, out string desc)
        {
            double h, w, tw, tf;
            //TODO: Any way of accessing the catalogue sections?
            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;
            tw = dimensions.WebThickness * 1000;
            tf = dimensions.FlangeThickness * 1000;

            desc = string.Format("STD%T%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), tw.ToStringWithDecimalPoint(), tf.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        /***************************************/

        private static bool CreateDescString(StandardZedSectionDimensions dimensions, out string desc)
        {
            throw new NotSupportedException("Zed sections are currently not suported in the GSA adapter");
        }

        /***************************************/

        private static bool CreateDescString(StandardISectionDimensions dimensions, out string desc)
        {
            double h, w, tw, tf;
            //TODO: Any way of accessing the catalogue sections?
            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;
            tw = dimensions.WebThickness * 1000;
            tf = dimensions.FlangeThickness * 1000;

            desc = string.Format("STD%I%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), tw.ToStringWithDecimalPoint(), tf.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        /***************************************/

        private static bool CreateDescString(FabricatedISectionDimensions dimensions, out string desc)
        {
            if (dimensions.BotFlangeThickness == dimensions.TopFlangeThickness && dimensions.TopFlangeWidth == dimensions.BotFlangeWidth)
            {
                double h, w, tw, tf;
                //TODO: Any way of accessing the catalogue sections?
                h = dimensions.Height * 1000;
                w = dimensions.BotFlangeWidth * 1000;
                tw = dimensions.WebThickness * 1000;
                tf = dimensions.BotFlangeThickness * 1000;

                desc = string.Format("STD%I%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), tw.ToStringWithDecimalPoint(), tf.ToStringWithDecimalPoint());
            }
            else
            {
                double h, wft, wfb, tw, tft, tfb;
                h = dimensions.Height * 1000;
                wft = dimensions.TopFlangeWidth * 1000;
                wfb = dimensions.BotFlangeWidth * 1000;
                tw = dimensions.WebThickness * 1000;
                tft = dimensions.TopFlangeThickness * 1000;
                tfb = dimensions.BotFlangeThickness * 1000;
                desc = string.Format("STD%GI%{0}%{1}%{2}%{3}%{4}%{5}", h.ToStringWithDecimalPoint(), wft.ToStringWithDecimalPoint(), wfb.ToStringWithDecimalPoint(), tw.ToStringWithDecimalPoint(), tft.ToStringWithDecimalPoint(), tfb.ToStringWithDecimalPoint());
            }
            return true;
            //STD%RHS%800.%400.%12.%16.
        }
        /***************************************/

        private static bool CreateDescString(CircleDimensions dimensions, out string desc)
        {
            double d;

            d = dimensions.Diameter * 1000;

            desc = string.Format("STD%C%{0}", d.ToStringWithDecimalPoint());

            return true;
        }

        /***************************************/

        private static bool CreateDescString(TubeDimensions dimensions, out string desc)
        {
            double d, t;

            d = dimensions.Diameter * 1000;
            t = dimensions.Thickness * 1000;

            desc = string.Format("STD%CHS%{0}%{1}", d.ToStringWithDecimalPoint(), t.ToStringWithDecimalPoint());
            return true;
        }

        /***************************************/

        private static bool CreateDescString(RectangleSectionDimensions dimensions, out string desc)
        {
            double h, w;

            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;

            desc = string.Format("STD%R%{0}%{1}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint());
            //STD%R%50.%20.
            return true;
        }



        /***************************************/

        private static bool CreateDescString(IList<ICurve> polygon, out string desc)
        {
            throw new NotImplementedException();
        }



        /***************************************/

        private static string ToStringWithDecimalPoint(this double d)
        {
            return Math.Abs(d - (int)d) < double.Epsilon ? d.ToString() + "." : d.ToString();
        }


    }
}
