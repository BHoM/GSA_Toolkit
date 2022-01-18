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

using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SurfaceProperties;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Collections.Generic;
using BH.oM.Adapters.GSA.SurfaceProperties;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISurfaceProperty FromGsaSurfaceProperty(string gsaString, Dictionary<string, IMaterialFragment> materials)
        {
            ISurfaceProperty panProp = null;

            if (gsaString == "")
            {
                return null;
            }

            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);
            string name = gsaStrings[2];

            string description, materialId, loadCondition;
            double t;
            int refEdge;
            FromGSAString(gsaString, out description, out materialId, out t, out loadCondition, out refEdge);

            if (description == "SHELL")
            {
                panProp = new ConstantThickness();
                panProp.Material = materials[materialId];
                ((ConstantThickness)panProp).Thickness = t;
            }
            else if (description == "LOAD")
            {
                panProp = new LoadingPanelProperty();
                ((LoadingPanelProperty)panProp).LoadApplication = GetLoadingConditionFromString(loadCondition);
                ((LoadingPanelProperty)panProp).ReferenceEdge = refEdge;
            }
            else if (description == "FABRIC")
            {
                panProp = new FabricPanelProperty();
                panProp.Material = materials[materialId];
            }
            else
            {
                Engine.Base.Compute.RecordWarning(string.Format("2D Property with id {0} and name {1} is of a type currently not supported. Will return a null object.",  gsaStrings[1], name));
                return null;
            }

            panProp.SetAdapterId(typeof(GSAId), id);
            panProp.Name = name;
            return panProp;
        }

        /***************************************************/

        private static void FromGSAString(string gsaString, out string description, out string materialId, out double t, out string loadCondition, out int refEdge)
        {
            string[] gsaStrings = gsaString.Split(',');

            //Separate data extractions specific to each GSA version
#if GSA_10_1
            description = gsaStrings[4];

            string matType;
            string matId;

            if (gsaStrings[6] == "0")
            {
                matId = gsaStrings[8];
                matType = gsaStrings[7];
            }
            else
            {
                matId = gsaStrings[6];
                matType = "ANAL";
            }

            materialId = matType + ":" + matId;

            if (description == "SHELL")
            {
                t = double.Parse(gsaStrings[10]);
                loadCondition = null;
                refEdge = 0;
            }
            else if (description == "LOAD")
            {
                t = 0;
                loadCondition = gsaStrings[5];
                refEdge = int.Parse(gsaStrings[6]);
            }
            else
            {
                t = 0;
                loadCondition = null;
                refEdge = 0;
            }
#else
            description = gsaStrings[6];
            materialId = gsaStrings[5];

            if (description == "SHELL")
            {
                t = double.Parse(gsaStrings[7]);
                loadCondition = null;
                refEdge = 0;
            }
            else if (description == "LOAD")
            {
                t = 0;
                loadCondition = gsaStrings[7].TrimStart("SUP_".ToCharArray());
                refEdge = int.Parse(gsaStrings[8]);
            }
            else
            {
                t = 0;
                loadCondition = null;
                refEdge = 0;
            }
#endif
        }

        /***************************************************/

    }
}


