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


#if GSA_10_1
using Interop.Gsa_10_1;
#else
using Interop.gsa_8_7;
#endif
using BH.oM.Base;
using BH.oM.Analytical.Results;
using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Requests;
using BH.oM.Data.Requests;
using BH.oM.Structure.Results;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
#if GSA_10_1
    public partial class GSA101Adapter
#else
    public partial class GSA87Adapter
#endif
    {

        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IResult> ReadResults(GlobalResultRequest request, ActionConfig actionConfig)
        {
            List<int> caseNumbers = CheckAndGetAnalysisCaseNumbers(request.Cases);
            CheckModes(request);

            List<IResult> results;

            switch (request.ResultType)
            {
                case GlobalResultType.Reactions:
                    results = ExtractGlobalReaction(caseNumbers).ToList();
                    break;
                case GlobalResultType.ModalDynamics:
                    results = ExtractGlobalDynamics(caseNumbers).ToList();
                    break;
                default:
                    Engine.Base.Compute.RecordError("Result of type " + request.ResultType + " is not yet supported");
                    results = new List<IResult>();
                    break;
            }
            results.Sort();
            return results;
        }

        /***************************************************/

        private IEnumerable<IResult> ExtractGlobalDynamics(List<int> cases)
        {
            List<ModalDynamics> dynamics = new List<ModalDynamics>();

            string id = ""; //TODO: Strategy for ids for full model

            foreach (int loadCase in cases)
            {
                string mode = m_gsaCom.GwaCommand("GET, MODE, " + loadCase);

                if (String.IsNullOrWhiteSpace(mode))
                    continue;
                string frequency = m_gsaCom.GwaCommand("GET, FREQ, " + loadCase);
                string mass = m_gsaCom.GwaCommand("GET, MASS, " + loadCase);
                string inertia = m_gsaCom.GwaCommand("GET, INERTIA, " + loadCase);
                string modalMass = m_gsaCom.GwaCommand("GET, MODAL_MASS, " + loadCase);
                string damping = m_gsaCom.GwaCommand("GET, MODAL_DAMP, " + loadCase);
                string stiffness = m_gsaCom.GwaCommand("GET, MODAL_STIFF, " + loadCase);
                string effMassTran = m_gsaCom.GwaCommand("GET, EFF_MASS, " + loadCase + ",TRAN");
                string effMassRot = m_gsaCom.GwaCommand("GET, EFF_MASS, " + loadCase + ",ROT");
                dynamics.Add(BH.Adapter.GSA.Convert.FromGsaModalDynamics(id, mode, frequency, mass, inertia, modalMass, stiffness, damping, effMassTran, effMassRot));
            }

            return dynamics;
        }

        /***************************************************/

        private IEnumerable<IResult> ExtractGlobalReaction(List<int> cases)
        {
            List<GlobalReactions> reactions = new List<GlobalReactions>();

            string id = ""; //TODO: Strategy for ids for full model

            foreach (int loadCase in cases)
            {
                List<string> forceStrings = new List<string>();
                List<string> momentStrings = new List<string>();

                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",REACT"));
                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",SUPPORT"));
                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",SPRING"));
                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",SOIL"));

                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",REACT"));
                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",SUPPORT"));
                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",SPRING"));
                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",SOIL"));

                //string force = m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",REACT");
                //string moment = m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",REACT");
                //reactions.Add(BH.Adapter.GSA.Convert.FromGsaGlobalReactions(id, force, moment));

                reactions.Add(BH.Adapter.GSA.Convert.FromGsaGlobalReactions(id, forceStrings, momentStrings));
            }

            return reactions;
        }

        /***************************************************/
    }
}






