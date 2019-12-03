/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using System;
using System.Collections.Generic;
using BH.Engine.Adapter;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************/

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

                        

                        return "PROP_SEC," + secProp.CustomData[AdapterID].ToString() + ", " + name + ", NO_RGB," + secProp.Material.CustomData[AdapterID] + "," + catProp + ", NO," + secProp.SectionType() + ", 0, NO_PROP," + secProp.ModifiersString() + ", FLAME_CUT, NO_J";


                    }
                }
            }

            //Return null for any non-steel sections
            return null;
        }

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
            return ICreateDescString(secProp.SectionProfile, out desc);
        }

        /***************************************/
        private static bool CreateDescAndPropString(ConcreteSection secProp, out string desc, out string prop)
        {
            //TODO: Reinforcement???
            prop = "NO_PROP";
            return ICreateDescString(secProp.SectionProfile, out desc);
        }

        /***************************************/

        private static bool ICreateDescString(IProfile dimensions, out string desc)
        {
            return CreateDescString(dimensions as dynamic, out desc);
        }

        /***************************************/

        private static bool CreateDescString(FabricatedBoxProfile dimensions, out string desc)
        {
            if(dimensions.TopFlangeThickness != dimensions.BotFlangeThickness)
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

        private static bool CreateDescString(ZSectionProfile dimensions, out string desc)
        {
            throw new NotSupportedException("Zed sections are currently not supported in the GSA adapter");
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

        private static bool CreateDescString(FreeFormProfile profile, out string desc)
        {
            throw new NotImplementedException();
        }

        /***************************************/

        private static string ToStringWithDecimalPoint(this double d)
        {
            return Math.Abs(d - (int)d) < double.Epsilon ? d.ToString() + "." : d.ToString();
        }

        /***************************************/
    }
}
