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


using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Base;
using BH.Engine.Adapters.GSA;
using BH.Engine.Base;
using BH.oM.Adapters.GSA.Fragments;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

#if GSA_10

        private static string ToGsaString(this Panel panel, string index)
        {

            PanelBoundaryNodeFragment fragment = panel.FindFragment<PanelBoundaryNodeFragment>();

            if (fragment == null)
                return "";

            string topo = fragment.ExternalNodes.Select(x => x.GSAId().ToString()).Aggregate((a, b) => a + " " + b);

            foreach (List<Node> openingNodes in fragment.OpeningNodes)
            {
                topo += " V(" + openingNodes.Select(x => x.GSAId().ToString()).Aggregate((a, b) => a + " " + b) + ")";
            }

            //MEMB.8	1		NO_RGB	2D_GENERIC	ALL	1	1	1 2 3 4 V(5 6 7 8)	0	35.8	0	YES	LINEAR	0	0	0	0	0	0	ACTIVE	0	NO	REBAR_2D.1	0.03	0.03	0
            return $"MEMB.8, {index}, {panel.Name}, NO_RGB, 2D_GENERIC, ALL,{panel.Property.GSAId()}, 1, {topo}, 0, {panel.OrientationAngle * 180 / Math.PI}, 0, YES,LINEAR,0,0,0,0,0,0,ACTIVE,0,NO,REBAR_2D.1,0.03,0.03,0";
        }

        /***************************************************/
#else

        public static string ToGsaString(this Panel obj, string index)
        {
            Engine.Base.Compute.RecordWarning("GSA has no meshing capabilities and does therefore not support Panel objects. \n"+
                                                    "To be able to push a Panel it first needs to be meshed and turned into a FEMesh.");
            return "";
        }
#endif
        /***************************************/
    }
}






