/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.Engine.Structure;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************/
        private static MaterialType GetMaterialType(IMaterialFragment material)
        {

            if (material is Steel)
                return MaterialType.MT_STEEL;
            else if (material is Concrete)
                return MaterialType.MT_CONCRETE;
            else if (material is Aluminium)
                return MaterialType.MT_ALUMINIUM;
            else if (material is Timber)
                return MaterialType.MT_TIMBER;
            else
                return MaterialType.MT_UNDEF;

        }
        
        /***************************************/

        //private static MaterialType GetTypeFromString(string gsaString)
        //{
        //    switch (gsaString)
        //    {
        //        case "MT_ALUMINIUM":
        //            return MaterialType.Aluminium;

        //        case "MT_CONCRETE":
        //            return MaterialType.Concrete;

        //        case "MT_GLASS":
        //            return MaterialType.Glass;

        //        case "MT_STEEL":
        //            return BHM.MaterialType.Steel;

        //        case "MT_TIMBER":
        //            return BHM.MaterialType.Timber;

        //        case "MT_REBAR":
        //            return BHM.MaterialType.Rebar;
        //        //Undef set to steel for now. Need to implement an undef material enum.
        //        case "MT_UNDEF":
        //        default:
        //            return BHM.MaterialType.Steel;
        //    }
        //}

        /***************************************/
    }
}

