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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Base;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.ComponentModel;
using BH.oM.Adapters.GSA.SpacerProperties;
using BH.oM.Adapters.GSA.Elements;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.GSA.MaterialFragments;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this Type type)
        {
            if (type == typeof(Node))
                return "NODE";
            else if (type == typeof(Bar))
                return "EL";
#if GSA_10_1
            else if (type == typeof(Steel)) // Does not support pushing of non orthoropic, non - generic materials
                return "MAT_STEEL";
            else if (type == typeof(Concrete))
                return "MAT_CONCRETE";
            else if (type == typeof(Timber))
                return "MAT_ANAL";//return "MAT_TIMBER";
            else if (type == typeof(Aluminium))
                return "MAT_ANAL";//return "MAT_ALUMINIUM";
            else if (type == typeof(Fabric))
                return "MAT_FABRIC";//return "MAT_ALUMINIUM";
            else if (type == typeof(GenericIsotropicMaterial) || type == typeof(GenericOrthotropicMaterial))
                return "MAT_ANAL";
#else
            else if (typeof(IMaterialFragment).IsAssignableFrom(type))
                return "MAT";
#endif
            else if (typeof(ISectionProperty).IsAssignableFrom(type))
                return "PROP_SEC";
            else if (typeof(ISurfaceProperty).IsAssignableFrom(type))
                return "PROP_2D";
            else if (type == typeof(FEMesh))
                return "EL";
            else if (type == typeof(RigidLink))
                return "EL";
            else if (type == typeof(LinkConstraint))
                return "PROP_LINK";
            else if (type == typeof(Spacer))
                return "EL";
            else if (type == typeof(SpacerProperty))
                return "PROP_SPACER";
            else if (type.IsGenericType && type.Name == typeof(BHoMGroup<IBHoMObject>).Name)
                return "List";
#if GSA_10_1
            else if (type == typeof(Panel))
                return "MEMB";
            else if (type == typeof(Constraint6DOF))
                return "PROP_SPR";
#endif
            return null;
        }

        /***************************************************/

        [Description("Removing disallowed characters like ',' from the string to make it possible to push using GSA commands.")]
        public static string ToGSACleanName(this string name)
        {
            if(name == null)
                return "";

            if (name.Contains(","))
            {
                Engine.Base.Compute.RecordNote("Any ',' in the name of the object pushed to GSA has been replaced by a ';' due to naming limitations through the GSA API.");
                name = name.Replace(',', ';');
            }
            return name;
        }

        /***************************************************/

        [Description("Raises a warning for types not supported.")]
        [Input("type", "The type not supported.")]
        [Input("category", "The object category to raise a warning for. Defaults to object.")]
        public static void NotSupportedWarning(Type type, string category = "Objects")
        {
            Engine.Base.Compute.RecordWarning(category + " of type " + type.FullName + " are not supported in the GSA Adapter");
        }

        /***************************************************/
        /**** Public Interface Methods                  ****/
        /***************************************************/

        public static string IToGsaString(this object obj, string index)
        {
            return ToGsaString(obj as dynamic, index);
        }

        /***************************************************/
        /**** Private fallback                          ****/
        /***************************************************/

        public static string ToGsaString(this object obj, string index)
        {
            NotSupportedWarning(obj.GetType());
            return "";
        }

        /***************************************/


    }
}




