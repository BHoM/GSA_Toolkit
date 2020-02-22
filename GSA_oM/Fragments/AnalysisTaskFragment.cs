﻿/*
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


using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.External.GSA
{
    [Description("Fragment to be put on a LoadCombination to help you control the analysis task type to use in GSA and for which stage the combination should be run.")]
    public class AnalysisTaskFragment : IFragment
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public AnalysisType AnalysisType { get; set; } = AnalysisType.LinearStatic;

        [Description("The stage number for the combination to be run on.")]
        public int Stage { get; set; } = 0;

        /***************************************************/
    }
}

