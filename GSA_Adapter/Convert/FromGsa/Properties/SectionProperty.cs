/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Base;
using System;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProperty FromGsaSectionProperty(string gsaString, Dictionary<string, IMaterialFragment> materials)
        {
            int id;
            IMaterialFragment mat;
            string description;
            string taggedName;
            char splitChar;
            FromGSAString(gsaString, materials, out id, out mat, out description, out taggedName, out splitChar);

            ISectionProperty secProp = null;
            string message = "";

            if (gsaString == "")
            {
                return null;
            }

            if (description.StartsWith("EXP"))
            {
                //prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;

                ExplicitSection expSecProp = new ExplicitSection();

                double a, iyy, izz, j, avy, avz;

                FromGSAString(gsaString, out a, out iyy, out izz, out j, out avy, out avz);

                expSecProp.Area = a;
                expSecProp.Iy = iyy;
                expSecProp.Iz = izz;
                expSecProp.J = j;
                expSecProp.Asy = avy;
                expSecProp.Asz = avz;

                expSecProp.Material = mat;

                secProp = expSecProp;
            }
            else
            {
                IProfile profile = null;

                if (description.StartsWith("CAT"))
                {
                    string[] desc = description.Split(splitChar);

                    if (desc.Length < 3)
                    {
                        message += "Failed to parse the GSASection :" + description + "\n";
                    }

                    //Get section type
                    string orgSecType = desc[1];
                    if (orgSecType.Contains("-"))
                    {
                        orgSecType = orgSecType.Split('-')[1];
                    }

                    string secType = orgSecType;
                    //Change from EA and UA to L for angles
                    if (secType == "UA")
                        secType = "L";
                    else if (secType == "EA")
                        secType = "L";
                    else if (secType == "BP")
                        secType = "UBP";

                    string secName = secType + desc[2].TrimStart(orgSecType.ToCharArray());

                    secProp = Engine.Library.Query.Match("Structure\\SectionProperties", secName) as ISectionProperty;

                    // Test need to add .0 to section property name
                    if (secProp == null)
                    {
                        secName += ".0";
                        secProp = Engine.Library.Query.Match("Structure\\SectionProperties", secName) as ISectionProperty;
                    }

                    if (secProp != null)
                    {
                        secProp = secProp.ShallowClone();
                        secProp.Material = mat;
                    }
                    else
                    {
                        secName = secName.TrimEnd((".0").ToCharArray());

                        if (desc[1].Contains("CHS"))
                        {
                            description = "STD" + splitChar + secType + splitChar;
                            string trim = desc[2].TrimStart(secType.ToCharArray());
                            string[] arr = trim.Split('x');

                            description += arr[0] + splitChar + arr[1];

                            Engine.Base.Compute.RecordNote("Section of type: " + secName + " not found in the library. Custom section will be used");
                        }
                        else if (desc[1].Contains("RHS"))
                        {
                            description = "STD" + splitChar + secType + splitChar;
                            string trim = desc[2].TrimStart(secType.ToCharArray());
                            string[] arr = trim.Split('x');

                            description += arr[0] + splitChar + arr[1] + splitChar + arr[2] + splitChar + arr[2];

                            Engine.Base.Compute.RecordNote("Section of type: " + secName + " not found in the library. Custom section will be used");
                        }
                        else if (desc[1].Contains("CHS"))
                        {
                            description = "STD%" + secType + "%";
                            string trim = desc[2].TrimStart(secType.ToCharArray());
                            string[] arr = trim.Split('x');

                            description += arr[0] + "%" + arr[1];

                            Engine.Base.Compute.RecordNote("Section of type: " + secName + " not found in the library. Custom section will be used");
                        }
                        else
                        {
                            message += "Catalogue section of type " + secName + " not found in library\n";
                        }
                    }
                }

                if (secProp == null && description.StartsWith("STD"))
                {
                    double D, W, T, t, Wt, Wb, Tt, Tb, Tw;
                    string type;
                    string[] desc = description.Split(splitChar);
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
                            profile = Engine.Spatial.Create.ISectionProfile(D, W, T, t, 0, 0);
                            break;
                        case "GI":
                            D = double.Parse(desc[2]) * factor;
                            Wt = double.Parse(desc[3]) * factor;
                            Wb = double.Parse(desc[4]) * factor;
                            Tw = double.Parse(desc[5]) * factor;
                            Tt = double.Parse(desc[6]) * factor;
                            Tb = double.Parse(desc[7]) * factor;
                            profile = Engine.Spatial.Create.FabricatedISectionProfile(D, Wt, Wb, Tw, Tt, Tb, 0);
                            break;
                        case "CHS":
                            D = double.Parse(desc[2]) * factor;
                            t = double.Parse(desc[3]) * factor;
                            profile = Engine.Spatial.Create.TubeProfile(D, t);
                            break;
                        case "RHS":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            if (T == t)
                                profile = Engine.Spatial.Create.BoxProfile(D, W, T, 0, 0); //TODO: Additional checks for fabricated/Standard
                            else
                                profile = Engine.Spatial.Create.FabricatedBoxProfile(D, W, T, t, t, 0);
                            break;
                        case "R":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            profile = Engine.Spatial.Create.RectangleProfile(D, W, 0);
                            break;
                        case "C":
                            D = double.Parse(desc[2]) * factor;
                            profile = Engine.Spatial.Create.CircleProfile(D);
                            break;
                        case "T":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            profile = Engine.Spatial.Create.TSectionProfile(D, W, T, t, 0, 0);
                            break;
                        case "A":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            profile = Engine.Spatial.Create.AngleProfile(D, W, T, t, 0, 0);
                            break;
                        case "CH":
                            D = double.Parse(desc[2]) * factor;
                            W = double.Parse(desc[3]) * factor;
                            T = double.Parse(desc[4]) * factor;
                            t = double.Parse(desc[5]) * factor;
                            profile = Engine.Spatial.Create.ChannelProfile(D, W, T, t, 0, 0);
                            break;
                        default:
                            message += "Section convertion for the type: " + type + " is not implemented in the GSA adapter";
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
                Engine.Base.Compute.RecordWarning(error);
            }

            secProp.SetAdapterId(typeof(GSAId), id);
            secProp.ApplyTaggedName(taggedName);
            return secProp;
        }

        /***************************************************/

        private static void FromGSAString(string gsaString, Dictionary<string, IMaterialFragment> materials, out int id, out IMaterialFragment mat, out string description, out string taggedName, out char splitChar)
        {
            string[] gsaStrings = gsaString.Split(',');
            string materialId;

            Int32.TryParse(gsaStrings[1], out id);

            //Separate data extractions specific to each GSA version
#if GSA_10_1
            description = gsaStrings[21];
            taggedName = gsaStrings[3];
            splitChar = ' '; //To split gsaString

            string matType;
            string matId;

            if (gsaStrings[18] == "0")
            {
                matId = gsaStrings[20];
                matType = gsaStrings[19];
            }
            else
            {
                matId = gsaStrings[18];
                matType = "ANAL";
            }

            materialId = matType + ":" + matId;
#else
            description = gsaStrings[5];
            taggedName = gsaStrings[2];
            materialId = gsaStrings[4];
            splitChar = '%';
#endif

            if (!materials.TryGetValue(materialId, out mat))
            {
                Engine.Base.Compute.RecordWarning(string.Format("Failed to extract material with id {0}. Section with Id {1} will not have any material applied to it.", materialId, id));
            }
        }

        private static void FromGSAString(string gsaString, out double a, out double iyy, out double izz, out double j, out double avy, out double avz)
        {
            string[] gsaStrings = gsaString.Split(',');

            //Separate data extractions specific to each GSA version
#if GSA_10_1
            string[] desc = gsaStrings[21].Split(' ');

            double.TryParse(desc[1], out a);
            double.TryParse(desc[2], out iyy);
            double.TryParse(desc[3], out izz);
            double.TryParse(desc[4], out j);
            double.TryParse(desc[5], out avy);
            double.TryParse(desc[6], out avz);
#else
            double.TryParse(gsaStrings[10], out a);
            double.TryParse(gsaStrings[11], out iyy);
            double.TryParse(gsaStrings[12], out izz);
            double.TryParse(gsaStrings[13], out j);
            double.TryParse(gsaStrings[14], out avy);
            double.TryParse(gsaStrings[15], out avz);
#endif
        }

        /***************************************************/
    }
}


