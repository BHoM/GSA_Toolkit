/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.Engine.Serialiser;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using System;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        /// <summary>Structure.Creates a BHoM section from a gsa string</summary>
        /// <param name="gsaString">
        /// <summary>
        /// Comma separated string on the format: [PROP_SEC | num | name | colour | mat | desc | principal | type | cost | 
        /// is_prop { | area | I11 | I22 | J | K1 | K2} | 
        /// is_mod { | area_to_by | area_m | I11_to_by | I11_m | I22_to_by | I22_m | J_to_by | J_m | K1_to_by | K1_m | K2_to_by | K2_m | mass | stress} | 
        /// plate_type | calc_J]
        /// </summary>
        /// </param>
        /// <param name="materials"></param>
        /// <returns></returns>
        public static ISectionProperty FromGsaSectionProperty(string gsaString, Dictionary<string, IMaterialFragment> materials)
        {
            ISectionProperty secProp = null;
            string message = "";

            if (gsaString == "")
            {
                return null;
            }

            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);

            string materialId = gsaStrings[4];

            IMaterialFragment mat;

            if (!materials.TryGetValue(materialId, out mat))
            {
                Engine.Reflection.Compute.RecordWarning(string.Format("Failed to extract material with id {0}. Section with Id {1} will not have any material applied to it.", materialId, id));
            }


            string description = gsaStrings[5];

            if (description == "EXP")
            {
                //prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;


                ExplicitSection expSecProp = new ExplicitSection();
                double a, iyy, izz, j, avy, avz;
                double.TryParse(gsaStrings[10], out a);
                double.TryParse(gsaStrings[11], out iyy);
                double.TryParse(gsaStrings[12], out izz);
                double.TryParse(gsaStrings[13], out j);
                double.TryParse(gsaStrings[14], out avy);
                double.TryParse(gsaStrings[15], out avz);

                expSecProp.Area = a;
                expSecProp.Iy = iyy;
                expSecProp.Iz = izz;
                expSecProp.J = j;
                expSecProp.Asy = avy;
                expSecProp.Asz = avz;

                secProp = expSecProp;
            }
            else
            {
                IProfile profile = null;

                if (description.StartsWith("CAT"))
                {
                    string[] desc = description.Split('%');

                    if (desc.Length < 3)
                    {
                        message += "Failed to parse the GSASection :" + description + "\n";
                    }

                    //Change from EA and UA to L for angles
                    if (desc[2].StartsWith("UA"))
                        desc[2] = desc[2].Replace("UA", "L");
                    else if (desc[2].StartsWith("EA"))
                        desc[2] = desc[2].Replace("EA", "L");
                    else if (desc[2].StartsWith("BP"))
                        desc[2] = desc[2].Replace("BP", "UBP");

                    secProp = Engine.Library.Query.Match("SectionProperties", desc[2]) as ISectionProperty;

                    if (secProp != null)
                    {
                        secProp = secProp.GetShallowClone() as ISectionProperty;
                        secProp.Material = mat;
                    }
                    else
                    {
                        if (desc[1] == "RHS" || desc[1] == "CHS")
                        {
                            description = "STD%" + desc[1] + "%";
                            string trim = desc[2].TrimStart(desc[1].ToCharArray());
                            string[] arr = trim.Split('x');

                            description += arr[0] + "%" + arr[1] + "%" + arr[2] + "%" + arr[2];

                            Engine.Reflection.Compute.RecordNote("Section of type: " + desc[2] + " not found in the library. Custom section will be used");
                        }
                        else
                        {
                            message += "Catalogue section of type " + desc[2] + " not found in library\n";
                        }
                    }
                }

                if (secProp == null && description.StartsWith("STD"))
                {
                    double D, W, T, t, Wt, Wb, Tt, Tb, Tw;
                    string type;
                    string[] desc = description.Split('%');
                    double factor;


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
                            profile = Engine.Geometry.Create.ISectionProfile(D, W, T, t, 0, 0);
                            break;
                        case "GI":
                            D = double.Parse(desc[2]) * factor;
                            Wt = double.Parse(desc[3]) * factor;
                            Wb = double.Parse(desc[4]) * factor;
                            Tw = double.Parse(desc[5]) * factor;
                            Tt = double.Parse(desc[6]) * factor;
                            Tb = double.Parse(desc[7]) * factor;
                            profile = Engine.Geometry.Create.FabricatedISectionProfile(D, Wt, Wb, Tw, Tt, Tb, 0);
                            break;
                        case "CHS":
                            D = double.Parse(desc[2]) * factor;
                            t = double.Parse(desc[3]) * factor;
                            profile = Engine.Geometry.Create.TubeProfile(D, t);
                            break;
                        case "RHS":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            if (T == t)
                                profile = Engine.Geometry.Create.BoxProfile(D, W, T, 0, 0); //TODO: Additional checks for fabricated/Standard
                            else
                                profile = Engine.Geometry.Create.FabricatedBoxProfile(D, W, T, t, t, 0);
                            break;
                        case "R":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            profile = Engine.Geometry.Create.RectangleProfile(D, W, 0);
                            break;
                        case "C":
                            D = double.Parse(desc[2]) * factor;
                            profile = Engine.Geometry.Create.CircleProfile(D);
                            break;
                        case "T":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            profile = Engine.Geometry.Create.TSectionProfile(D, W, T, t, 0, 0);
                            break;
                        case "A":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            profile = Engine.Geometry.Create.AngleProfile(D, W, T, t, 0, 0);
                            break;
                        case "CH":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            profile = Engine.Geometry.Create.ChannelProfile(D, W, T, t, 0, 0);
                            break;
                        default:
                            message += "Section convertion for the type: " + type + " is not implmented in the GSA adapter";
                            break;
                    }

                    //Creates a section based on the material type provided, with fallback to Generic
                    if(profile != null)
                        secProp = Engine.Structure.Create.SectionPropertyFromProfile(profile, mat, "");

                }
            }

            if (secProp == null)
            {
                secProp = new ExplicitSection { Material = mat };
                string error = "At least part of the section extraction failed and an empty explicit section has been returned in place of the section in GSA.";
                if (!string.IsNullOrWhiteSpace(message))
                    error += " The following error was reported by the adapter: " + message;
                Engine.Reflection.Compute.RecordError(error);
            }

            secProp.CustomData[AdapterIdName] = id;
            secProp.ApplyTaggedName(gsaStrings[2]);
            return secProp;
        }

        /***************************************************/

    }
}
