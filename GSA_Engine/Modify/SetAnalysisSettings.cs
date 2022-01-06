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

using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Structure.Loads;
using BH.Engine.Base;

namespace BH.Engine.Adapters.GSA
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the analysis type and stage for a LoadCombination")]
        [Input("loadcombination", "The load combination to set stage and analysis type for.")]
        [Input("stage", "The stage number for the combination to be run on.")]
        [Input("residualForce", "Allowed residual Force for convergence, only used for Non-linear analysis.")]
        [Input("residualMoment", "Allowed residual Moment for convergence, only used for Non-linear analysis.")]
        [Input("beamSlendernessEffect", "Reduce beam stiffness when in compression, only used for Non-linear analysis.")]
        [Output("loadCombination", "The loadcombination with set analysis type and stage.")]
        public static LoadCombination SetAnalysisSettings(this LoadCombination loadcombination, AnalysisType analysisType = AnalysisType.LinearStatic, int stage = 0, double residualForce = 1.0, double residualMoment = 1.0, bool beamSlendernessEffect = true)
        {
            if (loadcombination == null)
            {
                Reflection.Compute.RecordError("Loadcombination is null. Cannot assign settings.");
                return null;
            }
            AnalysisTaskFragment fragment = new AnalysisTaskFragment { AnalysisType = analysisType, Stage = stage, ResidualForce = residualForce, ResidualMoment = residualMoment, BeamSlendernessEffect = beamSlendernessEffect };
            LoadCombination clone = loadcombination.ShallowClone();

            clone.Fragments.AddOrReplace(fragment);
            return clone;
        }

        /***************************************************/
    }
}

