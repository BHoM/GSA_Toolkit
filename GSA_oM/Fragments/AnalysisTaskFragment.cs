/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

namespace BH.oM.Adapters.GSA
{
    [Description("Fragment to be put on a LoadCombination to help you control the analysis task type to use in GSA and for which stage the combination should be run.")]
    public class AnalysisTaskFragment : IFragment
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public virtual AnalysisType AnalysisType { get; set; } = AnalysisType.LinearStatic;

        [Description("The stage number for the combination to be run on.")]
        public virtual int Stage { get; set; } = 0;

        [Description("Allowed residual Force for convergence, only used for Non-linear analysis.")]
        public virtual double ResidualForce { get; set; } = 1.0;

        [Description("Allowed residual Moment for convergence, only used for Non-linear analysis.")]
        public virtual double ResidualMoment { get; set; } = 1.0;

        [Description("Reduce beam stiffness when in compression, only used for Non-linear analysis.")]
        public virtual bool BeamSlendernessEffect { get; set; } = true;

        /***************************************************/
    }
}





