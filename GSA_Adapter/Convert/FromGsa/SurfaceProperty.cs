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

using BH.Engine.Serialiser;
using BH.Engine.Structure;
using BHM = BH.oM.Structure.MaterialFragments;
using BHL = BH.oM.Structure.Loads;
using BHMF = BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using Interop.gsa_8_7;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.Engine.Adapter;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
      
        public static ISurfaceProperty FromGsaSurfaceProperty(string gsaString, Dictionary<string, BHM.IMaterialFragment> materials)
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
            string materialId = gsaStrings[5];
            string description = gsaStrings[6];

            if (description == "SHELL")
            {
                panProp = new ConstantThickness();
                panProp.Material = materials[materialId];
                double t = double.Parse(gsaStrings[7]);
                ((ConstantThickness)panProp).Thickness = t;
            }
            else if (description == "LOAD")
            {
                panProp = new LoadingPanelProperty();
                ((LoadingPanelProperty)panProp).LoadApplication = GetLoadingConditionFromString(gsaStrings[7]);
                ((LoadingPanelProperty)panProp).ReferenceEdge = int.Parse(gsaStrings[8]);
            }

            panProp.CustomData.Add(AdapterIdName, id);
            panProp.Name = name;
            return panProp;
        }

        /***************************************************/

    }
}
