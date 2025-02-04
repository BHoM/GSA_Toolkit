/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Collections.Generic;
using System.Linq;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.Engine.Structure;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using System;
using BH.Engine.Adapters.GSA;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Fragments;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(this ISectionProperty prop, string index)
        {
            string desc, props;
            if (!ICreateDescAndPropString(prop, out desc, out props))
                return "";
#if GSA_10

            desc = desc.Replace("%", " ");
#endif
            return ToGsaString(prop, index, desc, props);
        }

        /***************************************************/

        private static string ToGsaString(this ISectionProperty prop, string index, string description, string props)
        {
            prop.Name = prop.DescriptionOrName().ToGSACleanName();
            string name = prop.TaggedName();

            string colour = "NO_RGB";
            string principle = "NO";
            string type = prop.SectionType();
            string cost = "0";
            string plate_type = "FLAME_CUT";
            string calc_J = "NO_J";
            string mods = prop.ModifiersString();

            //PROP_SEC    2   Section 2   NO_RGB  1   CAT % UB % UB914x419x388 % 19990407   NO NA  0   NO_PROP NO_MOD_PROP FLAME_CUT NO_J

#if GSA_10

            string analNum, materialType, matNum;
            prop.Material.MaterialIdentifiers(out analNum, out materialType, out matNum);
            //SECTION_COMP | ref | name | matAnal | matType | matRef | desc | offset_y | offset_z | rotn | reflect | pool
            string sectionComp = "SECTION_COMP.4,," + analNum + "," + materialType + "," + matNum + "," + description + ",0,0,0,NONE,0,NONE,0";
            //SECTION.7 | ref | colour | name | memb | pool | point | refY | refZ | mass | fraction | cost | left | right | slab | num { <comp> }
            string str = "SECTION.7," + index + "," + colour + "," + name + ",1D_GENERIC,0,CENTROID,0,0,0,1,0,0,0,0,1," + sectionComp + "," + prop.ISectionMaterialComp();
#else
            string mat = prop.Material.GSAId().ToString();// materialId;  //"STEEL";// material.Name;
            string str = "PROP_SEC" + "," + index + "," + name + "," + colour + "," + mat + "," + description + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;
#endif
            return str;
        }

        /***************************************************/

#if GSA_10
        private static string ISectionMaterialComp(this ISectionProperty prop)
        {
            return SectionMaterialComp(prop as dynamic);
        }

        /***************************************************/

        private static string SectionMaterialComp(ISectionProperty prop)
        {
            return "0,0,NO_ENVIRON";
        }

        /***************************************************/

        private static string SectionMaterialComp(SteelSection prop)
        {

            return "SECTION_STEEL.2,0,1,1,1,0.4,NO_LOCK,UNDEF,UNDEF,0,0,NO_ENVIRON";
        }

        /***************************************************/

        private static string SectionMaterialComp(ConcreteSection prop)
        {
            return "SECTION_CONC.6,1,NO_SLAB,89.99999998,0.025,0,SECTION_LINK.3,0,0,DISCRETE,RECT,0,,SECTION_COVER.3,UNIFORM,0,0,NO_SMEAR,SECTION_TMPL.4,UNDEF,0,0,0,0,0,0,NO_ENVIRON";
        }

        /***************************************************/
#endif

        public static string CreateCatalogueString(this ISectionProperty secProp)
        {
            if (secProp is SteelSection)
            {
                SteelSection steel = secProp as SteelSection;
                string name = secProp.Name;

                //Try to push the section as a catalogue profile
                if (!string.IsNullOrWhiteSpace(name))
                {
                    string[] arr = name.Split(' ');

                    if (arr.Length >= 2)
                    {
                        if (steel.SectionProfile.Shape == ShapeType.Angle)
                        {
                            AngleProfile prof = steel.SectionProfile as AngleProfile;

                            if (prof.Height == prof.Width)
                                arr[0] = "EA";
                            else
                                arr[0] = "UA";
                        }
                        else if (steel.SectionProfile.Shape == ShapeType.Box || steel.SectionProfile.Shape == ShapeType.Tube)
                        {
                            //Add tailing .0 for closed sections
                            char[] chArr = arr[1].ToCharArray();
                            if (chArr.Length > 2 && chArr[arr[1].Length - 2] != '.')
                            {
                                arr[1] += ".0";
                            }
                        }
                        else if (steel.SectionProfile.Shape == ShapeType.ISection)
                        {
                            if (arr[0] == "UBP")
                                arr[0] = "BP";
                        }

                        string catProp = "CAT " + arr[0] + " ";
                        for (int i = 0; i < arr.Length; i++)
                        {
                            catProp += arr[i];
                        }

                        return ToGsaString(secProp, secProp.GSAId().ToString(), catProp, "NO_PROP");

                        //return "PROP_SEC," + secProp.GSAId().ToString() + ", " + name + ", NO_RGB," + secProp.Material.GSAId() + "," + catProp + ", NO," + secProp.SectionType() + ", 0, NO_PROP," + secProp.ModifiersString() + ", FLAME_CUT, NO_J";
                    }
                }
            }

            //Return null for any non-steel sections
            return null;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string ModifiersString(this ISectionProperty secProp)
        {
            string mods = "NO_MOD_PROP";

            double[] modifiers = Engine.Structure.Query.Modifiers(secProp);

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

        /****************************************************/
        /**** Private  Methods - Section type dispatcher ****/
        /****************************************************/


        private static bool ICreateDescAndPropString(ISectionProperty secProp, out string desc, out string prop)
        {
            return CreateDescAndPropString(secProp as dynamic, out desc, out prop);
        }

        /***************************************/

        private static bool CreateDescAndPropString(ExplicitSection secProp, out string desc, out string prop)
        {
            string area = secProp.Area.ToString();
            string Iyy = secProp.Iy.ToString();
            string Izz = secProp.Iz.ToString();
            string J = secProp.J.ToString();
            string Kvy = (secProp.Asy / secProp.Area).ToString();
            string Kvz = (secProp.Asz / secProp.Area).ToString();


#if GSA_10
            desc = "EXP(m) " + area + " " + Iyy + " " + Izz + " " + J + " " + Kvy + " " + Kvz;
#else
            desc = "EXP";
#endif
            prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Kvy + "," + Kvz;
            return true;
        }

        /***************************************/

        private static bool CreateDescAndPropString(CableSection secProp, out string desc, out string prop)
        {
            //TODO: Handle cables as non-explicit
            string area = secProp.Area.ToString();
            string Iyy = secProp.Iy.ToString();
            string Izz = secProp.Iz.ToString();
            string J = secProp.J.ToString();
            string Kvy = (secProp.Asy / secProp.Area).ToString();
            string Kvz = (secProp.Asz / secProp.Area).ToString();

#if GSA_10
            desc = "EXP(m) " + area + " " + Iyy + " " + Izz + " " + J + " " + Kvy + " " + Kvz;
#else
            desc = "EXP";
#endif
            prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Kvy + "," + Kvz;
            return true;
        }

        /***************************************/

        private static bool CreateDescAndPropString(IGeometricalSection secProp, out string desc, out string prop)
        {
            //This will handle Steel, Concrete, Aluminium, Timber and Generi section, i.e. all profile based sections, the same way.
            //If reinforement for concrete is to be added, this needs to be handled separately.
            prop = "NO_PROP";
            return ICreateDescString(secProp.SectionProfile, out desc);
        }

        /***************************************/

        private static bool CreateDescAndPropString(ISectionProperty secProp, out string desc, out string prop)
        {
            //Fallback method for section types not implemented.
            prop = "NO_PROP";
            desc = "";
            NotSupportedWarning(secProp.GetType(), "Section properties");
            return false;
        }

        /***************************************/

        private static bool ICreateDescString(IProfile dimensions, out string desc)
        {
            return CreateDescString(dimensions as dynamic, out desc);
        }

        /***************************************/

        private static bool CreateDescString(FabricatedBoxProfile dimensions, out string desc)
        {
            if (dimensions.TopFlangeThickness != dimensions.BotFlangeThickness)
                throw new NotSupportedException("Box sections with varying flange thickness between top and bottom are currently not supported in the GSA adapter");

            double h, w, wt, ft;

            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;
            wt = dimensions.WebThickness * 1000;
            ft = dimensions.BotFlangeThickness * 1000;

            desc = string.Format("STD%RHS%{0}%{1}%{2}%{3}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint(), wt.ToStringWithDecimalPoint(), ft.ToStringWithDecimalPoint());

            return true;
            //STD%RHS%800.%400.%12.%16.
        }

        /***************************************/

        private static bool CreateDescString(BoxProfile dimensions, out string desc)
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

        private static bool CreateDescString(ChannelProfile dimensions, out string desc)
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

        private static bool CreateDescString(AngleProfile dimensions, out string desc)
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

        private static bool CreateDescString(TSectionProfile dimensions, out string desc)
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

        private static bool CreateDescString(ISectionProfile dimensions, out string desc)
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

        private static bool CreateDescString(FabricatedISectionProfile dimensions, out string desc)
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

        private static bool CreateDescString(CircleProfile dimensions, out string desc)
        {
            double d;

            d = dimensions.Diameter * 1000;

            desc = string.Format("STD%C%{0}", d.ToStringWithDecimalPoint());

            return true;
        }

        /***************************************/

        private static bool CreateDescString(TubeProfile dimensions, out string desc)
        {
            double d, t;

            d = dimensions.Diameter * 1000;
            t = dimensions.Thickness * 1000;

            desc = string.Format("STD%CHS%{0}%{1}", d.ToStringWithDecimalPoint(), t.ToStringWithDecimalPoint());
            return true;
        }

        /***************************************/

        private static bool CreateDescString(RectangleProfile dimensions, out string desc)
        {
            double h, w;

            h = dimensions.Height * 1000;
            w = dimensions.Width * 1000;

            desc = string.Format("STD%R%{0}%{1}", h.ToStringWithDecimalPoint(), w.ToStringWithDecimalPoint());
            //STD%R%50.%20.
            return true;
        }

        /***************************************/

#if GSA_10

        private static bool CreateDescString(this TaperedProfile section, out string desc)
        {
            if (section.Profiles.Count == 1)
                return CreateDescString(section.Profiles.First().Value as dynamic, out desc);

            IProfile startProfile, endProfile;

            if (section.Profiles.Count == 2 && section.Profiles.TryGetValue(0, out startProfile) && section.Profiles.TryGetValue(1, out endProfile))
            {
                if (startProfile.GetType() == endProfile.GetType())
                {
                    string desc1, desc2;
                    if (CreateDescString(startProfile as dynamic, out desc1))
                    {
                        if (CreateDescString(endProfile as dynamic, out desc2))
                        {
                            desc2 = desc2.Split('%').Skip(2).Aggregate((a, b) => a + "%" + b);
                            desc = desc1 + "%:%" + desc2;
                            return true;
                        }
                    }
                }
            }

            Engine.Base.Compute.RecordWarning("The GSA adapter currently only support tapered sections with two profiles of the same type. Section set as explicit with 0-properties.");
            desc = "";
            return false;
        }
#endif
        /***************************************/
        private static bool CreateDescString(IProfile profile, out string desc)
        {
            NotSupportedWarning(profile.GetType(), "Section profiles");
            desc = "";
            return false;
        }

        /***************************************/

        private static string ToStringWithDecimalPoint(this double d)
        {
            return Math.Abs(d - (int)d) < double.Epsilon ? d.ToString() + "." : d.ToString();
        }

        /***************************************/
#if GSA_10
        private static string ToGsaString(this SectionModifier modifier, string index)
        {
            if (modifier == null)
                return "";

            //SECTION_MOD | ref | name | mod | centroid | stress | opArea | area | prin | opIyy | Iyy | opIzz | Izz | opJ | J | opKy | ky | opKz | kz | opVol | vol | mass

            return $"SECTION_ANAL.4, { index}, {index}, GEOM, SEC, NONE, BY, {modifier.Area}, YZ, BY, {modifier.Iy}, BY, {modifier.Iz}, BY, {modifier.J}, BY, {modifier.Asy}, BY, {modifier.Asz}, BY, 1, 0";
        }
#endif

        /***************************************/
    }
}






