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

using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using BH.Engine.Serialiser;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(this IIsotropic material, string index)
        {
            string command = "MAT";
            string num = index;
            string mModel = "MAT_ELAS_ISO";
            material.Name = material.DescriptionOrName();
            string name = material.TaggedName();
            string colour = "NO_RGB";
            string type = GetMaterialType(material).ToString();
            string E = material.YoungsModulus.ToString();
            string nu = material.PoissonsRatio.ToString();
            string rho = material.Density.ToString();
            string alpha = material.ThermalExpansionCoeff.ToString();
            string G = material.ShearModulus().ToString();
            string damp = material.DampingRatio.ToString();

            string str = command + "," + num + "," + mModel + "," + name + "," + colour + "," + type + ",6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
            return str;

        }

        /***************************************************/

        private static string ToGsaString(this IOrthotropic material, string index)
        {
            string command = "MAT";
            string num = index;
            string mModel = "MAT_ELAS_ORTHO";
            material = CheckMaterialVectors(material);
            material.Name = material.DescriptionOrName();
            string name = material.TaggedName();
            string colour = "NO_RGB";
            string type = GetMaterialType(material).ToString();
            string E = CommaSeparatedValues(material.YoungsModulus);
            string nu = CommaSeparatedValues(material.PoissonsRatio);
            string rho = material.Density.ToString();
            string alpha = CommaSeparatedValues(material.ThermalExpansionCoeff);
            string G = CommaSeparatedValues(material.ShearModulus);
            string damp = material.DampingRatio.ToString();

            string str = command + "," + num + "," + mModel + "," + name + "," + colour + "," + type + ",14," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
            return str;

        }

        /***************************************************/

        private static string ToGsaString(this IMaterialFragment material, string index)
        {
            Engine.Reflection.Compute.RecordWarning("GSA_Toolkit does currently only suport Isotropic and Orthotropic materials. Material with name " + material.Name + " has NOT been pushed");
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
                return MaterialType.MT_STEEL;
            else if (material is Concrete)
                return MaterialType.MT_CONCRETE;
            else if (material is Aluminium)
                return MaterialType.MT_ALUMINIUM;
            else if (material is Timber)
                return MaterialType.MT_TIMBER;
            else
                return MaterialType.MT_UNDEF;

        }

        /***************************************************/

        private static IOrthotropic CheckMaterialVectors(this IOrthotropic material)
        {
            List<string> failingProperties = new List<string>();
            IOrthotropic clone = material.GetShallowClone() as IOrthotropic;

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
                if (failingProperties.Count == 1)
                {
                    message = "The vector property " + failingProperties[0];
                }
                else
                {
                    message = "The vector properties ";
                    for (int i = 0; i < failingProperties.Count - 1; i++)
                    {
                        message += failingProperties[i] + " ,";
                    }
                    message += " and " + failingProperties[failingProperties.Count - 1];
                }

                message += " is/are unset (null) for a material of type " + material.GetType().Name;
                if (!string.IsNullOrWhiteSpace(material.Name))
                    message += " , named " + material.Name;

                message += ". All unset (null) properties have been replaced with empty (zero length) vectors. Please check your input data!";

                Engine.Reflection.Compute.RecordError(message);
            }

            return clone;
        }

        /***************************************************/

    }
}
