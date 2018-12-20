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

using BH.oM.Geometry;
using BH.oM.Structure.Properties.Surface;
using System;
using System.Collections.Generic;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        private static LoadPanelSupportConditions GetLoadingConditionFromString(string str)
        {
            switch (str)
            {
                case "SUP_ALL":
                    return LoadPanelSupportConditions.AllSides;
                case "SUP_THREE":
                    return LoadPanelSupportConditions.ThreeSides;
                case "SUP_TWO":
                    return LoadPanelSupportConditions.TwoSides;
                case "SUP_TWO_ADJ":
                    return LoadPanelSupportConditions.TwoAdjacentSides;
                case "SUP_ONE":
                    return LoadPanelSupportConditions.OneSide;
                case "SUP_ONE_MOM":
                    return LoadPanelSupportConditions.Cantilever;
                default:
                    return LoadPanelSupportConditions.AllSides;
            }
        }

        private static string GetLoadPanelSupportConditions(LoadPanelSupportConditions cond)
        {
            switch (cond)
            {
                case LoadPanelSupportConditions.AllSides:
                    return "SUP_ALL";
                case LoadPanelSupportConditions.ThreeSides:
                    return "SUP_THREE";
                case LoadPanelSupportConditions.TwoSides:
                    return "SUP_TWO";
                case LoadPanelSupportConditions.TwoAdjacentSides:
                    return "SUP_TWO_ADJ";
                case LoadPanelSupportConditions.OneSide:
                    return "SUP_ONE";
                case LoadPanelSupportConditions.Cantilever:
                    return "SUP_ONE_MOM";
                default:
                    return "SUP_AUTO";
            }
        }

    }
}
