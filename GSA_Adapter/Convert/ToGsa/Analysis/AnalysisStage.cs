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

using BH.Engine.Adapters.GSA;
using BH.oM.Adapters.GSA.Analysis;
using BH.oM.Base;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections.Generic;
using BH.Engine.Adapter;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(this AnalysisStage stage, string index)
        {
            string command = "ANAL_STAGE";
            string name = stage.Name;
            index = stage.Number.ToString();

            stage.SetAdapterId(typeof(BH.oM.Adapters.GSA.GSAId), stage.Number);

            List<Object> elements = stage.Elements;
            string objectIds = "";

            for (int i = 0; i < elements.Count; i++)
            {
                string id = "";
                int idInt; 

                // String type
                if (int.TryParse(elements[i].ToString(), out idInt))
                {
                    id = idInt.ToString();
                }

                // Property Type
                else if (elements[i] is ISectionProperty)
                {
                    id = "PB" + (elements[i] as ISectionProperty).GSAId().ToString();
                }
                else if (elements[i] is LinkConstraint)
                {
                    id = "PL" + (elements[i] as LinkConstraint).GSAId().ToString();
                }
                else if (elements[i] is ISurfaceProperty)
                {
                    id = "PA" + (elements[i] as ISurfaceProperty).GSAId().ToString();
                }

                // Element type
                else if (elements[i] is IBHoMObject) 
                {
                    id = (elements[i] as IBHoMObject).GSAId().ToString();
                }
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("Unable to extract GSA Id. Supported types are doubles, ints, strings and BHoMObjects.");
                    continue;
                }

                objectIds += " " + id;
            }

            string str = command + ", " + index + " , " + name + ", NO_RGB , " + objectIds + " , 0 ,";
            return str;
        }

        /***************************************/
    }
}
