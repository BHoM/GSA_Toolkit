using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoMP = BHoM.Structural.Properties;
using BHoMM = BHoM.Materials;
using BHoM.Structural.Databases;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Properties
{

    /// <summary>
    /// Static class responsible for generating section strings from BHoM cross sections
    /// </summary>
    internal static class PropertyStringGenerators
    {
        internal static void CreateDescAndPropString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            switch (secProp.Shape)
            {
                case BHoMP.ShapeType.Rectangle:
                    CreateRectString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Box:
                    CreateBoxString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Angle:
                    CreateAngleString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.ISection:
                    CreateIString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Tee:
                    CreateTeeString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Channel:
                    CreateChannelString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Tube:
                    CreateTubeString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Circle:
                    CreateCircleString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Zed:
                    CreateZedString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Polygon:
                    CreatePolygonString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.DoubleAngle:
                    CreateDoubleAngleString(secProp, out desc, out prop);
                    break;
                //case BHoMP.ShapeType.DoubleISection:
                    //CreateCutIString(secProp, out desc, out prop);
                    //break;
                case BHoMP.ShapeType.DoubleChannel:
                    CreateDoubleChannelString(secProp, out desc, out prop);
                    break;
                case BHoMP.ShapeType.Cable:
                default:
                    CreateCustomDataString(secProp, out desc, out prop);
                    break;
            }
        }

        private static void CreateCustomDataString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            desc = "EXP";
            string area = secProp.GrossArea.ToString();
            string Iyy = secProp.Iy.ToString();
            string Izz = secProp.Ix.ToString();
            string J = secProp.J.ToString();
            string Avy = secProp.Asx.ToString();
            string Avz = secProp.Asy.ToString();

            prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;
        }

        private static void CreateDoubleChannelString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        private static void CreateCutIString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        private static void CreateDoubleAngleString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        private static void CreatePolygonString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        private static void CreateZedString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        private static void CreateCircleString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            //STD%C%60.
            prop = "NO_PROP";

            double d;

            d = secProp.SectionData[(int)SteelSectionData.Height] * 1000;

            desc = string.Format("STD%C%{0}", d.ToStringWithDecimalPoint());

        }

        private static void CreateTubeString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            //STD%CHS%60.%5.

            prop = "NO_PROP";

            double d, t;

            d = secProp.SectionData[(int)SteelSectionData.Height] * 1000;
            t = secProp.SectionData[(int)SteelSectionData.TW] * 1000;

            desc = string.Format("STD%CHS%{0}%{1}", d.ToStringWithDecimalPoint(), t.ToStringWithDecimalPoint());
        }

        private static void CreateChannelString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            throw new NotImplementedException();
        }

        private static void CreateTeeString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            //STD%T%150.%100.%5.%10.

            prop = "NO_PROP";

            double h, w, wt, ft;

            h = secProp.SectionData[(int)SteelSectionData.Height] * 1000;
            w = secProp.SectionData[(int)SteelSectionData.Width] * 1000;
            wt = secProp.SectionData[(int)SteelSectionData.TW] * 1000;
            ft = secProp.SectionData[(int)SteelSectionData.TF1] * 1000;

            desc = string.Format("STD%T%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());

        }

        private static void CreateIString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            //STD%I%150.%100.%5.%20.
            //STD%GI%150.%100.%50.%6.%10.%5.

            prop = "NO_PROP";

            double h, widthT, widthB, wt, ftT, ftB;

            h = secProp.SectionData[(int)SteelSectionData.Height] * 1000;
            widthT = secProp.SectionData[(int)SteelSectionData.B1] * 1000;
            widthB = secProp.SectionData[(int)SteelSectionData.B2] * 1000;
            wt = secProp.SectionData[(int)SteelSectionData.TW] * 1000;
            ftT = secProp.SectionData[(int)SteelSectionData.TF1] * 1000;
            ftB = secProp.SectionData[(int)SteelSectionData.TF2] * 1000;

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

        private static void CreateAngleString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            //STD%A%150.%100.%5.%10.

            prop = "NO_PROP";

            double h, w, wt, ft;

            h = secProp.SectionData[(int)SteelSectionData.Height] * 1000;
            w = secProp.SectionData[(int)SteelSectionData.Width] * 1000;
            wt = secProp.SectionData[(int)SteelSectionData.TW] * 1000;
            ft = secProp.SectionData[(int)SteelSectionData.TF1] * 1000;

            desc = string.Format("STD%A%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());
        }

        private static void CreateBoxString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            prop = "NO_PROP";

            double h, w, wt, ft;

            h = secProp.SectionData[(int)SteelSectionData.Height] * 1000;
            w = secProp.SectionData[(int)SteelSectionData.Width] * 1000;
            wt = secProp.SectionData[(int)SteelSectionData.TW] * 1000;
            ft = secProp.SectionData[(int)SteelSectionData.TF1] * 1000;

            desc = string.Format("STD%RHS%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());

            //STD%RHS%800.%400.%12.%16.

        }

        private static void CreateRectString(BHoMP.SectionProperty secProp, out string desc, out string prop)
        {
            prop = "NO_PROP";

            double h, w;

            h = secProp.SectionData[(int)SteelSectionData.Height] * 1000;
            w = secProp.SectionData[(int)SteelSectionData.Width] * 1000;

            desc = string.Format("STD%R%{0}%{1}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint());
            //STD%R%50.%20.


        }

        private static string ToStringWithDecimalPoint(this double d)
        {
            return Math.Abs(d - (int)d) < double.Epsilon ? d.ToString() + "." : d.ToString();
        }

        /// <summary>Creates a BHoM section from a gsa string</summary>
        internal static BHoMP.SectionProperty GetSectionFromGsaString(string gsaString, Dictionary<string, BHoMM.Material> materials)
        {
            BHoMP.SectionProperty secProp = null;

            if (gsaString == "")
            {
                return null;
            }

            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);
            string name = gsaStrings[3];
            string materialId = gsaStrings[4];
            string description = gsaStrings[5];

            if (description == "EXP")
            {
                //prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;


                BHoMP.ExplicitSectionProperty expSecProp = new BHoMP.ExplicitSectionProperty();
                double a, iyy, izz, j, avy, avz;
                double.TryParse(gsaStrings[6], out a);
                double.TryParse(gsaStrings[7], out iyy);
                double.TryParse(gsaStrings[8], out izz);
                double.TryParse(gsaStrings[9], out j);
                double.TryParse(gsaStrings[10], out avy);
                double.TryParse(gsaStrings[11], out avz);

                expSecProp.GrossArea = a;
                expSecProp.Ix = iyy;
                expSecProp.Iy = izz;
                expSecProp.J = j;
                expSecProp.Asx = avy;
                expSecProp.Asy = avz;

                secProp = expSecProp;
            }

            if (description.StartsWith("STD") /*|| description.StartsWith("CAT") */)
            {
                double D, W, T, t, Wt, Wb, Tt, Tb;
                string[] desc = description.Split('%');
                double factor;
                string type;

                if (desc[1].Contains("(cm)"))
                {
                    factor = 0.01;
                    type = desc[1].Replace("(cm)", "");
                }
                else if (desc[1].Contains("(m)"))
                {
                    factor = 1;
                    type = desc[1].Replace("(m)", "");
                }
                else
                {
                    factor = 0.001;
                    type = desc[1];
                }

                switch (type)
                {
                    case "UC":
                    case "UB":
                    case "I":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        secProp = new BHoMP.SteelSection(BHoMP.ShapeType.ISection, /*BHoMP.SectionType.Steel,*/ D, W, T, t, 0, 0);
                        break;
                    case "GI":
                        D = double.Parse(desc[2]) * factor;
                        Wt = double.Parse(desc[3]) * factor;
                        Wb = double.Parse(desc[4]) * factor;
                        Tt = double.Parse(desc[5]) * factor;
                        Tb = double.Parse(desc[6]) * factor;
                        t = double.Parse(desc[7]) * factor;
                        secProp = BHoMP.SectionProperty.CreateISection(BHoM.Materials.MaterialType.Steel, Wt, Wb, D, Tt, Tb, t, 0, 0);
                        break;
                    case "CHS":
                        D = double.Parse(desc[2]) * factor;
                        t = double.Parse(desc[3]) * factor;
                        secProp = new BHoMP.SteelSection(BHoMP.ShapeType.Tube, /*BHoMP.SectionType.Steel,*/ D, D, t, t, 0, 0);
                        break;
                    case "RHS":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        secProp = new BHoMP.SteelSection(BHoMP.ShapeType.Box, /*BHoMP.SectionType.Steel,*/ D, W, T, t, 0, 0);
                        break;
                    case "R":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        secProp = new BHoMP.SteelSection(BHoMP.ShapeType.Rectangle, D, W, 0, 0, 0, 0);
                        break;
                    default:
                        break;
                }

            }

            secProp.CustomData.Add(Utils.ID, id);
            secProp.Name = name;
            secProp.Material = materials[materialId];
            return secProp;
        }

        public static BHoMP.PanelProperty GetPanelPropertyFromGsaString(string gsaProp, Dictionary<string, BHoMM.Material> materials)
        {
            throw new NotImplementedException();
        }
    }
}
