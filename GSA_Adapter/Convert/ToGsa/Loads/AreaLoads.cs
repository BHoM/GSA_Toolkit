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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Adapters.GSA;
using BH.oM.Base;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<string> ToGsaString(this IElementLoad<IAreaElement> areaLoad, double[] unitFactors)
        {
            double factor = areaLoad.IFactor(unitFactors);
            if(double.IsNaN(factor))
                return new List<string>();

            string name = areaLoad.Name;
            string projection = areaLoad.IsProjectedString();
            string axis = areaLoad.IsGlobal();
            string appliedTo = areaLoad.CreateIdListOrGroupNameAreaLoad();
            Vector[] force = { areaLoad.ITranslationVector()[0], areaLoad.ITranslationVector()[1] };
            string caseNo = areaLoad.Loadcase.Number.ToString();
            string command = areaLoad.IForceTypeString();
            string[] pos = { ("," + areaLoad.ILoadPosition()[0]), ("," + areaLoad.ILoadPosition()[1]) };
            string type = areaLoad.IAreaLoadTypeString();
            if (appliedTo == null)
                return null;

            string str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis + "," + type + "," + projection;

            List<string> forceStrings = new List<string>();
            VectorDataToString(str, force, ref forceStrings, factor, true, pos);

            return forceStrings;
        }


        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static string AreaLoadTypeString(this AreaUniformlyDistributedLoad load)
        {
            return "CONS";
        }

        /***************************************************/

        private static string CreateIdListOrGroupNameAreaLoad(this IElementLoad<IAreaElement> areaLoad)
        {
            //For a named group, appy loads to the group name
            if (!string.IsNullOrWhiteSpace(areaLoad.Objects.Name))
                return "\"" + areaLoad.Objects.Name + "\"";

            List<int> indecies = new List<int>();

            //Add id for each face
            foreach (FEMeshFace face in areaLoad.Objects.Elements.OfType<FEMesh>().SelectMany(x => x.Faces))
            {
                indecies.Add(face.GSAId());
            }

            //Add ids for non-FEMesh objects (should not be needed, but added to future proof.
            indecies.AddRange(areaLoad.Objects.Elements.Where(x => !(x is FEMesh)).Select(x => x.GSAId()));

            //Otherwise apply to the corresponding indecies
            return indecies.GenerateIDString();
        }

        /***************************************************/

        //public static string AreaLoadTypeString(this AreaVaryingDistributedLoad load)
        //{
        //    return "GEN";
        //}

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        private static string IAreaLoadTypeString(this IElementLoad<IAreaElement> load)
        {
            return AreaLoadTypeString(load as dynamic);
        }

    }
}







