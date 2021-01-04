/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

            if (gStr[2] == "MAT_ELAS_ISO")
            {
                //BHMF.MaterialType type = GetTypeFromString(gStr[5]);

                double E, v, tC, G, rho;

                if (!double.TryParse(gStr[7], out E))
                    return null;
                if (!double.TryParse(gStr[8], out v))
                    return null;
                if (!double.TryParse(gStr[10], out tC))
                    return null;
                if (!double.TryParse(gStr[11], out G))
                    return null;
                if (!double.TryParse(gStr[9], out rho))
                    return null;

                switch (gStr[5])
                {
                    case "MT_ALUMINIUM":
                        mat = Engine.Structure.Create.Aluminium("", E, v, tC, rho);
                        break;
                    case "MT_CONCRETE":
                        mat = Engine.Structure.Create.Concrete("", E, v, tC, rho);
                        break;
                    case "MT_STEEL":
                    case "MT_REBAR":
                        mat = Engine.Structure.Create.Steel("", E, v, tC, rho);
                        break;
                    case "MT_TIMBER":
                        mat = Engine.Structure.Create.Timber("", new Vector { X = E, Y = E, Z = E }, new Vector { X = v, Y = v, Z = v }, new Vector { X = G, Y = G, Z = G }, new Vector { X = tC, Y = tC, Z = tC }, rho, 0);
                        break;
                    default:
                        mat = new GenericIsotropicMaterial { YoungsModulus = E, Density = rho, PoissonsRatio = v, ThermalExpansionCoeff = tC };
                        Engine.Reflection.Compute.RecordWarning(string.Format("Material with id {0} and name {1} is of a type not currently fully supported or has no type defined. A generic isotropic material will be assumed", gStr[1], gStr[3]));
                        break;

                }
            }
            else if (gStr[2] == "MAT_ELAS_ORTHO")
            {
                double E1, E2, E3, v1, v2, v3, tC1, tC2, tC3, G1, G2, G3, rho;

                if (!double.TryParse(gStr[7], out E1))
                    return null;
                if (!double.TryParse(gStr[8], out E2))
                    return null;
                if (!double.TryParse(gStr[9], out E3))
                    return null;

                if (!double.TryParse(gStr[10], out v1))
                    return null;
                if (!double.TryParse(gStr[11], out v2))
                    return null;
                if (!double.TryParse(gStr[12], out v3))
                    return null;

                if (!double.TryParse(gStr[13], out rho))
                    return null;

                if (!double.TryParse(gStr[14], out tC1))
                    return null;
                if (!double.TryParse(gStr[15], out tC2))
                    return null;
                if (!double.TryParse(gStr[16], out tC3))
                    return null;

                if (!double.TryParse(gStr[17], out G1))
                    return null;
                if (!double.TryParse(gStr[18], out G2))
                    return null;
                if (!double.TryParse(gStr[19], out G3))
                    return null;

                Vector e = new Vector { X = E1, Y = E2, Z = E3 };
                Vector v = new Vector { X = v1, Y = v2, Z = v3 };
                Vector tC = new Vector { X = tC1, Y = tC2, Z = tC3 };
                Vector g = new Vector { X = G1, Y = G2, Z = G3 };

                switch (gStr[5])
                {

                    case "MT_TIMBER":
                        mat = Engine.Structure.Create.Timber("", e, v, g, tC, rho, 0);
                        break;
                    default:
                        mat = new GenericOrthotropicMaterial { YoungsModulus = e, ShearModulus = g, Density = rho, ThermalExpansionCoeff = tC, PoissonsRatio = v };
                        Engine.Reflection.Compute.RecordWarning(string.Format("Material with id {0} and name {1} is of a type not currently fully supported or has no orthotropic type defined. A generic orthotropic material will be assumed", gStr[1], gStr[3]));
                        break;

                }
            }
            else
            {
                return null;
            }


            mat.ApplyTaggedName(gStr[3]);

            int id = int.Parse(gStr[1]);
            mat.SetAdapterId(typeof(GSAId), id);

            return mat;
        }

        /***************************************************/

    }
}

