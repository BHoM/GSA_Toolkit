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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Base;
using System;


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
            else if (type == typeof(IMaterialFragment))
                return "MAT";
            else if (type == typeof(ISectionProperty))
                return "PROP_SEC";
            else if (type == typeof(ISurfaceProperty))
                return "PROP_2D";
            else if (type == typeof(FEMesh))
                return "EL";
            else if (type == typeof(RigidLink))
                return "EL";
            else if (type == typeof(LinkConstraint))
                return "PROP_LINK";
            else if (type.IsGenericType && type.Name == typeof(BHoMGroup<IBHoMObject>).Name)
                return "List";
            return null;
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

        public static string ToGsaString(this Panel obj, string index)
        {
            Engine.Reflection.Compute.RecordWarning("GSA has no meshing capabilities and does therefore not support Panel objects. \n"+
                                                    "To be able to push a Panel it first needs to be meshed and turned into a FEMesh.");
            return "";
        }

        /***************************************/
    }
}
