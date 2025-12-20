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


#if GSA_10_2
using Interop.Gsa_10_2;
#elif  GSA_10_1
using Interop.Gsa_10_1;
#else
using Interop.gsa_8_7;
#endif
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using System;
using System.Collections.Generic;
using BH.oM.Adapters.GSA.SpacerProperties;
using BH.oM.Adapters.GSA.Elements;
using BH.oM.Structure;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Spacer> FromGsaSpacers(IEnumerable<GsaElement> gsaElements, Dictionary<int, SpacerProperty> spaProps, Dictionary<int, Node> nodes)
        {
            List<Spacer> spacerList = new List<Spacer>();

            foreach (GsaElement gsaSpacer in gsaElements)
            {
                if (gsaSpacer.eType != 19)
                    continue;

                Node n1, n2;
                nodes.TryGetValue(gsaSpacer.Topo[0], out n1);
                nodes.TryGetValue(gsaSpacer.Topo[1], out n2);

                Spacer spacer = new Spacer { Start = n1, End = n2 };
                spacer.ApplyTaggedName(gsaSpacer.Name);

                SpacerProperty prop;
                spaProps.TryGetValue(gsaSpacer.Property, out prop);

                spacer.SpacerProperty = prop;

                int id = gsaSpacer.Ref;
                spacer.SetAdapterId(typeof(GSAId), id);

                spacerList.Add(spacer);

            }
            return spacerList;
        }

        /***************************************************/

    }
}






