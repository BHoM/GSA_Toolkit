/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Adapters.GSA.MaterialFragments;
using BH.oM.Adapters.GSA.Analysis;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IMaterialFragment FromGsaMaterial(string gsaString)
        {
            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 11)
                return null;

            IMaterialFragment mat;

            if (gsaString.Contains("MAT_ELAS_ISO"))
            {
                double E, v, tC, G, rho;
                string material, taggedName;
                IsotropicMaterialProperties(gsaString, out E, out v, out tC, out G, out rho, out material, out taggedName);

                //BHMF.MaterialType type = GetTypeFromString(gStr[5]);

                switch (material)
                {
                    case "ALUMINIUM":
                        mat = Engine.Structure.Create.Aluminium("", E, v, tC, rho);
                        break;
                    case "CONCRETE":
                        mat = Engine.Structure.Create.Concrete("", E, v, tC, rho);
                        break;
                    case "STEEL":
                    case "REBAR":
                        mat = Engine.Structure.Create.Steel("", E, v, tC, rho); // add yield/ ultimate stress
                        break;
                    case "TIMBER":
                        mat = Engine.Structure.Create.Timber("", new Vector { X = E, Y = E, Z = E }, new Vector { X = v, Y = v, Z = v }, new Vector { X = G, Y = G, Z = G }, new Vector { X = tC, Y = tC, Z = tC }, rho, 0);
                        break;
                    default:
                        mat = new GenericIsotropicMaterial { YoungsModulus = E, Density = rho, PoissonsRatio = v, ThermalExpansionCoeff = tC };
                        Engine.Base.Compute.RecordWarning(string.Format("Material with id {0} and name {1} is of a type not currently fully supported or has no type defined. A generic isotropic material will be assumed", gStr[1], taggedName));
                        break;
                }

                mat.ApplyTaggedName(taggedName);
            }
            else if (gsaString.Contains("MAT_ELAS_ORTHO"))
            {
                double E1, E2, E3, v1, v2, v3, G1, G2, G3, tC1, tC2, tC3, rho;
                string material, taggedName;
                OrthotropicMaterialProperties(gsaString, out E1, out E2, out E3, out v1, out v2, out v3, out G1, out G2, out G3, out tC1, out tC2, out tC3, out rho, out material, out taggedName);

                Vector e = new Vector { X = E1, Y = E2, Z = E3 };
                Vector v = new Vector { X = v1, Y = v2, Z = v3 };
                Vector tC = new Vector { X = tC1, Y = tC2, Z = tC3 };
                Vector g = new Vector { X = G1, Y = G2, Z = G3 };

                switch (material)
                {
                    case "TIMBER":
                        mat = Engine.Structure.Create.Timber("", e, v, g, tC, rho, 0);
                        break;
                    default:
                        mat = new GenericOrthotropicMaterial { YoungsModulus = e, ShearModulus = g, Density = rho, ThermalExpansionCoeff = tC, PoissonsRatio = v };
                        Engine.Base.Compute.RecordWarning(string.Format("Material with id {0} and name {1} is of a type not currently fully supported or has no orthotropic type defined. A generic orthotropic material will be assumed", gStr[1], taggedName));
                        break;
                }
                mat.ApplyTaggedName(taggedName);
            }
            else if (gsaString.Contains("FABRIC"))
            {
                double Ex, Ey, v, G;
                string taggedName;
                FabricMaterialProperties(gsaString, out Ex, out Ey, out v, out G, out taggedName);

                mat = new Fabric { WarpModulus = Ex, WeftModulus = Ey, PoissonsRatio = v, ShearModulus = G };
                mat.ApplyTaggedName(taggedName);
            }
            else
            {
                return null;
            }



#if GSA_10
            string id = gStr[1];
            id = gsaString.Split(("_.").ToCharArray())[1] + ":" + id;
            mat.SetAdapterId(typeof(GSAId), id);
#else
            int id = int.Parse(gStr[1]);
            mat.SetAdapterId(typeof(GSAId), id);
#endif
            return mat;
        }

        /***************************************************/

        private static void IsotropicMaterialProperties(string gsaString, out double e, out double v, out double tC, out double g, out double rho, out string material, out string taggedName)
        {
            string[] gStr = gsaString.Split(',');

            //Separate data extractions specific to each GSA version
#if GSA_10
            if (gStr[0].Contains("ANAL"))
            {
                e = double.Parse(gStr[6]);
                v = double.Parse(gStr[7]);
                tC = double.Parse(gStr[9]);
                g = double.Parse(gStr[10]);
                rho = double.Parse(gStr[8]);

                material = null;
                taggedName = gStr[3];
            }
            else if (gStr[0].Contains("STEEL") || gStr[0].Contains("CONCRETE") || gStr[0].Contains("FRP") || gStr[0].Contains("REBAR"))
            {
                e = double.Parse(gStr[4]);
                v = double.Parse(gStr[6]);
                tC = double.Parse(gStr[9]);
                g = double.Parse(gStr[7]);
                rho = double.Parse(gStr[8]);

                material = gStr[0].Split(("_.".ToCharArray()))[1];
                taggedName = gStr[3];
            }
            else if (gStr[0].Contains("ALUMINIUM") || gStr[0].Contains("TIMBER") || gStr[0].Contains("GLASS"))
            {
                e = double.Parse(gStr[3]);
                v = double.Parse(gStr[5]);
                tC = double.Parse(gStr[8]);
                g = double.Parse(gStr[6]);
                rho = double.Parse(gStr[7]);

                material = gStr[0].Split("_.".ToCharArray())[1];
                taggedName = gStr[2];
            }
            else
            {
                e = v = tC = g = rho = 0;
                material = taggedName = null;
            }
#else
            e = double.Parse(gStr[7]);
            v = double.Parse(gStr[8]);
            tC = double.Parse(gStr[10]);
            g = double.Parse(gStr[11]);
            rho = double.Parse(gStr[9]);

            material = gStr[5].Split('_')[1];
            taggedName = gStr[3];
#endif
        }

        /***************************************************/

        private static void OrthotropicMaterialProperties(string gsaString, out double e1, out double e2, out double e3, out double v1, out double v2, out double v3, out double g1, out double g2, out double g3, out double tC1, out double tC2, out double tC3, out double rho, out string material, out string taggedName)
        {
            string[] gStr = gsaString.Split(',');

            //Separate data extractions speficic to each GSA version
#if GSA_10
            e1 = double.Parse(gStr[6]);
            e2 = double.Parse(gStr[7]);
            e3 = double.Parse(gStr[8]);

            v1 = double.Parse(gStr[9]);
            v2 = double.Parse(gStr[10]);
            v3 = double.Parse(gStr[11]);

            g1 = double.Parse(gStr[13]);
            g2 = double.Parse(gStr[14]);
            g3 = double.Parse(gStr[15]);

            tC1 = double.Parse(gStr[16]);
            tC2 = double.Parse(gStr[17]);
            tC3 = double.Parse(gStr[18]);

            rho = double.Parse(gStr[12]);

            material = null;
            taggedName = gStr[3];
#else
           e1 = double.Parse(gStr[7]);
           e2 = double.Parse(gStr[8]);
           e3 = double.Parse(gStr[9]);

           v1 = double.Parse(gStr[10]);
           v2 = double.Parse(gStr[11]);
           v3 = double.Parse(gStr[12]);

           g1 = double.Parse(gStr[17]);
           g2 = double.Parse(gStr[18]);
           g3 = double.Parse(gStr[19]);

           tC1 = double.Parse(gStr[14]);
           tC2 = double.Parse(gStr[15]);
           tC3 = double.Parse(gStr[16]);

           rho = double.Parse(gStr[13]);

           material = gStr[5].Split('_')[1];
           taggedName = gStr[3];
#endif
        }

        /***************************************************/

        private static void FabricMaterialProperties(string gsaString, out double ex, out double ey, out double v, out double g, out string taggedName)
        {
            string[] gStr = gsaString.Split(',');

            //Separate data extractions specific to each GSA version
#if GSA_10
            ex = double.Parse(gStr[3]);
            ey = double.Parse(gStr[4]);
            v = double.Parse(gStr[5]);
            g = double.Parse(gStr[6]);

            taggedName = gStr[2];
#else
            ex = double.Parse(gStr[6]);
            ey = double.Parse(gStr[7]);
            v = double.Parse(gStr[8]);
            g = double.Parse(gStr[9]);

            taggedName = gStr[3];
#endif
        }

        /***************************************************/
    }
}






