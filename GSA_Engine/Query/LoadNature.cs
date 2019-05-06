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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Loads;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string LoadNatureString(this LoadNature loadNature)
        {
            if (loadNature == LoadNature.Dead || loadNature == LoadNature.SuperDead)
                return "LC_PERM_SELF";
            if (loadNature == LoadNature.Live)
                return "LC_VAR_IMP";
            if (loadNature == LoadNature.Other)
                return "LC_UNDEF";
            if (loadNature == LoadNature.Seismic)
                return "LC_EQE_ACC";
            if (loadNature == LoadNature.Snow)
                return "LC_VAR_SNOW";
            if (loadNature == LoadNature.Temperature)
                return "LC_VAR_TEMP";
            if (loadNature == LoadNature.Wind)
                return "LC_VAR_WIND";
            if (loadNature == LoadNature.Prestress)
                return "LC_PRESTRESS";
            if (loadNature == LoadNature.Accidental)
                return "LC_ACCIDENTAL";
            else
                return "";
        }

        public static LoadNature BHoMLoadNature(string loadNature)
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
    }
}
