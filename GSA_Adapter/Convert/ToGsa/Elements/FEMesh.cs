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

using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Elements;
using BH.Engine.Adapters.GSA;
using System;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this FEMesh mesh, int index, int faceID)
        {

            string command = "EL.2";
            string type;

            FEMeshFace face = mesh.Faces[faceID];
            face.SetAdapterId(typeof(GSAId), index);

            //TODO: Implement QUAD8 and TRI6
            if (face.NodeListIndices.Count == 3)
                type = "TRI3";
            else if (face.NodeListIndices.Count == 4)
                type = "QUAD4";
            else
                return "";

            string name = mesh.TaggedName().ToGSACleanName();

            string propertyIndex = mesh.Property.GSAId().ToString();
            int group = 0;

            string topology = "";

            foreach (int nodeIndex in face.NodeListIndices)
            {
                topology += mesh.Nodes[nodeIndex].GSAId().ToString() + ",";
            }

            string orientationAngle = (face.OrientationAngle * 180 / Math.PI).ToString();

            string dummy = CheckDummy(face);
            //EL	1	gfdgdf	NO_RGB	QUAD4	1	1	1	2	3	4	0	0	NO_RLS	NO_OFFSET	DUMMY
            //EL  2       NO_RGB TRI3    1   1   1   2   5   0   0   NO_RLS NO_OFFSET   DUMMY

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + propertyIndex + ", " + group + ", " + topology + " 0 , " + orientationAngle + ", NO_RLS" + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************************/

    }
}





