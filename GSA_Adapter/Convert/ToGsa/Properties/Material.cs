/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Reflection;
using System.Linq;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;
using BH.oM.Adapters.GSA.MaterialFragments;
using BH.Engine.Base;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(this IIsotropic material, string index)
        {
            string num = index;
            string mModel = "MAT_ELAS_ISO";
            material.Name = material.DescriptionOrName().ToGSACleanName();
            string name = material.TaggedName();
            string colour = "NO_RGB";
            string type = GetMaterialType(material).ToString();
            string E = material.YoungsModulus.ToString();
            string nu = material.PoissonsRatio.ToString();
            string rho = material.Density.ToString();
            string alpha = material.ThermalExpansionCoeff.ToString();
            string G = material.ShearModulus().ToString();
            string damp = material.DampingRatio.ToString();
            string str = "";

#if GSA_10_1
            string matType = type.Substring(0, 1) + type.Substring(1).ToLower();

            if (type == "STEEL")
            {
                Steel steel = material as Steel;

                string fy = steel.YieldStress.ToString();
                string fu = steel.UltimateStress.ToString();

                string uls = "MAT_CURVE_PARAM.2,,ELAS_PLAS,1,1";
                string sls = "MAT_CURVE_PARAM.2,,ELAS_PLAS,1,1";
                string prop = "MAT_ANAL.1,,0,MAT_ELAS_ISO,6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp;
                string mat = "MAT.10," + name + "," + E + "," + fy + "," + nu + "," + G + "," + rho + "," + alpha + "," + prop + ",0,0,0,0,0,0,0.05," + uls + "," + sls + ",0,Steel"; // MAT_10 to include G

                str = "MAT_" + type + ".3," + num + "," + mat + "," + fy + "," + fu + ",0,0";
            }
            else if (type == "CONCRETE")
            {
                Concrete concrete = material as Concrete;

                double fck = concrete.CylinderStrength; // concrete strength
                string fcd = (fck * 1.0 / 1.5).ToString(); // design strength
                string fcdc = (fck * 0.2667).ToString(); // cracked strength
                string fcdt = (fck * 0.1 * 1.0 / 1.5).ToString(); // tensile strength, using 10% of design strength as currently undefined in BHoM
                string fcfib = (fck * 2 / 30).ToString(); // peak strength for FIB/Popovics curves
                fck.ToString();

                string uls = "MAT_CURVE_PARAM.2,,RECTANGLE+NO_TENSION,1.5,1";
                string sls = "MAT_CURVE_PARAM.2,,LINEAR+NO_TENSION,1,1";
                string prop = "MAT_ANAL.1,,0,MAT_ELAS_ISO,6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp;
                string mat = "MAT.10," + name + "," + E + "," + fcd + "," + nu + "," + G + "," + rho + "," + alpha + "," + prop + ",0,0,0,0,0,0,0," + uls + "," + sls + ",0,Steel"; // MAT_10 to include G

                str = "MAT_" + type + "," + num + "," + mat + "," + "CYLINDER,N," + fck + "," + fcd + "," + fcdc + "," + fcdt + "," + fcfib + ",0,0,2,0.003,0.003,0.0006,0.003,0.003,0.002,0.003,NO,0.02,0,1,1,0,0,0,0,0";
            }
            else if (type == "ALUMINIUM")
            {
                Aluminium aluminium = material as Aluminium;
                string F = ""; // Strenght

                string uls = "MAT_CURVE_PARAM.2,,UNDEF,1,1";
                string sls = "MAT_CURVE_PARAM.2,,UNDEF,1,1";
                string prop = "MAT_ANAL.1,,0,MAT_ELAS_ISO,6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp;

                str = "MAT_" + type + ".9" + "," + num + "," + name + "," + E + "," + F + "," + nu + "," + G + "," + rho + "," + alpha + "," + prop + "0,0,0,0,0,0,0," + uls + "," + sls + ",0," + type;
            }
            else if (type == "TIMBER")
            {
                Timber timber = material as Timber;
                string F = ""; // Strenght

                string uls = "MAT_CURVE_PARAM.2,,UNDEF,1,1";
                string sls = "MAT_CURVE_PARAM.2,,UNDEF,1,1";
                string prop = "MAT_ANAL.1,,0,MAT_ELAS_ISO,6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp;

                str = "MAT_" + type + ".9" + "," + num + "," + name + "," + E + "," + F + "," + nu + "," + G + "," + rho + "," + alpha + "," + prop + "0,0,0,0,0,0,0," + uls + "," + sls + ",0," + type;
            }
            else if (type == "UNDEF")
            { str = "MAT_ANAL.1" + "," + num + "," + mModel + "," + name + "," + colour + ",6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0"; }
#else
            str = "MAT" + "," + num + "," + "MT_" + mModel + "," + name + "," + colour + "," + type + ",6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
#endif
            return str;
        }

        /***************************************************/

        private static string ToGsaString(this IOrthotropic material, string index)
        {
            string num = index;
            string mModel = "MAT_ELAS_ORTHO";
            material = CheckMaterialVectors(material);
            material.Name = material.DescriptionOrName().ToGSACleanName();
            string name = material.TaggedName();
            string colour = "NO_RGB";
            string type = GetMaterialType(material).ToString();
            string E = CommaSeparatedValues(material.YoungsModulus);
            string nu = CommaSeparatedValues(material.PoissonsRatio);
            string rho = material.Density.ToString();
            string alpha = CommaSeparatedValues(material.ThermalExpansionCoeff);
            string G = CommaSeparatedValues(material.ShearModulus);
            string damp = material.DampingRatio.ToString();
            string str;

#if GSA_10_1
            str = "MAT_ANAL.1" + "," + num + "," + mModel + "," + name + "," + colour + ",14," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0";
#else
            str = "MAT" + "," + num + "," + mModel + "," + name + "," + colour + "," + type + ",14," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
#endif
                 
return str;

        }

        /***************************************************/

        private static string ToGsaString(this Fabric material, string index)
        {
            string command = "MAT";
            string num = index;
            string mModel = "MAT_FABRIC";
            material.Name = material.DescriptionOrName().ToGSACleanName();
            string name = material.TaggedName();
            string colour = "NO_RGB";
            string type = GetMaterialType(material).ToString();
            string Ex = material.WarpModulus.ToString();
            string Ey = material.WeftModulus.ToString();
            string nu = material.PoissonsRatio.ToString();
            string G = material.ShearModulus.ToString();
            string rho = material.Density.ToString();
            string damp = material.DampingRatio.ToString();

#if GSA_10_1
            string uls = "MAT_CURVE_PARAM.2,,UNDEF,1,1";
            string sls = "MAT_CURVE_PARAM.2,,UNDEF,1,1";
            string prop = "MAT_ANAL.1," + name + ",,MAT_FABRIC,5," + Ex + "," + Ey + "," + nu + "," + G + "," + rho + ",1,0,00,0,0,0,0," + uls + "," + sls;
            
            string str = "MAT_" + type + "," + num + "," + prop + ",0," + type;
#else
            string str = command + "," + num + "," + mModel + "," + name + "," + colour + ",4," + Ex + "," + Ey + "," + nu + "," + G + ",1," + ",0,NO_ENV";
#endif
            return str;
        }

        /***************************************************/

        private static string ToGsaString(this IMaterialFragment material, string index)
        {
            Engine.Base.Compute.RecordWarning("GSA_Toolkit does currently only suport Isotropic and Orthotropic materials. Material with name " + material.Name + " has NOT been pushed");
            return "";
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string CommaSeparatedValues(Vector v)
        {
            return v.X + "," + v.Y + "," + v.Z;
        }

        /***************************************************/

        private static MaterialType GetMaterialType(IMaterialFragment material)
        {

            if (material is Steel)
                return MaterialType.STEEL;
            else if (material is Concrete)
                return MaterialType.CONCRETE;
            else if (material is Aluminium)
                return MaterialType.ALUMINIUM;
            else if (material is Timber)
                return MaterialType.TIMBER;
            else
                return MaterialType.UNDEF;

        }

        /***************************************************/

        private static IOrthotropic CheckMaterialVectors(this IOrthotropic material)
        {
            List<string> failingProperties = new List<string>();
            IOrthotropic clone = material.ShallowClone();

            if (material.YoungsModulus == null)
            {
                clone.YoungsModulus = new Vector();
                failingProperties.Add("YoungsModulus");
            }

            if (material.PoissonsRatio == null)
            {
                clone.PoissonsRatio = new Vector();
                failingProperties.Add("PoissonsRatio");
            }

            if (material.ShearModulus == null)
            {
                clone.ShearModulus = new Vector();
                failingProperties.Add("ShearModulus");
            }

            if (material.ThermalExpansionCoeff == null)
            {
                clone.ThermalExpansionCoeff = new Vector();
                failingProperties.Add("ThermalExpansionCoeff");
            }

            if (failingProperties.Count > 0)
            {
                string message;
                string isAre;
                if (failingProperties.Count == 1)
                {
                    message = "The vector property " + failingProperties[0];
                    isAre = "is";
                }
                else
                {
                    isAre = "are";
                    message = "The vector properties ";
                    for (int i = 0; i < failingProperties.Count - 1; i++)
                    {
                        message += failingProperties[i] + ", ";
                    }
                    message = message.TrimEnd(' ', ',');
                    message += " and " + failingProperties[failingProperties.Count - 1];
                }

                message += " " + isAre + " unset (null) for a material of type " + material.GetType().Name;
                if (!string.IsNullOrWhiteSpace(material.Name))
                    message += ", with the name " + material.Name;

                message += ".\nAll unset (null) properties have been replaced with empty (zero length) vectors. Please check the input data!";

                Engine.Base.Compute.RecordError(message);
            }

            return clone;
        }

        /***************************************************/

    }
}

