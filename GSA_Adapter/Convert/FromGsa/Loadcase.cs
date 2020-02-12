﻿/*
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
using BH.oM.Structure.Loads;
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

        public static Loadcase FromGsaLoadcase(string gsaString)
        {

            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 5)
                return null;

            LoadNature loadNature = BHoMLoadNature(gStr[3]);
            Loadcase lCase = Engine.Structure.Create.Loadcase(gStr[2], int.Parse(gStr[1]), loadNature);

            int lCasenum = 0;

            if (Int32.TryParse(gStr[1], out lCasenum))
            {
                lCase.Number = lCasenum;
            }

            return lCase;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static LoadNature BHoMLoadNature(string loadNature)
        {
            if (loadNature == "LC_PERM_SELF")
                return LoadNature.Dead;
            if (loadNature == "LC_VAR_IMP")
                return LoadNature.Live;
            if (loadNature == "LC_UNDEF")
                return LoadNature.Other;
            if (loadNature == "LC_EQE_ACC")
                return LoadNature.Seismic;
            if (loadNature == "LC_VAR_SNOW")
                return LoadNature.Snow;
            if (loadNature == "LC_VAR_TEMP")
                return LoadNature.Temperature;
            if (loadNature == "LC_VAR_WIND")
                return LoadNature.Wind;
            if (loadNature == "LC_PRESTRESS")
                return LoadNature.Prestress;
            if (loadNature == "LC_ACCIDENTAL")
                return LoadNature.Accidental;
            else
                return LoadNature.Other;
        }

        /***************************************************/
    }
}
