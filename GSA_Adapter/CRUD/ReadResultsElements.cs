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

using BH.oM.Base;
using BH.oM.Common;
using BH.oM.Structure.Requests;
using BH.oM.Data.Requests;
using BH.oM.Adapter;
using Interop.gsa_8_7;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {

        public delegate IResult ForceConverter(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0);

        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IResult> ReadResults(IStructuralResultRequest request, ActionConfig actionConfig)
        {
            List<int> objectIds = CheckAndGetIds(request);
            List<string> loadCases = CheckAndGetAnalysisCases(request);
            CheckModes(request);

            ResHeader header;
            ForceConverter converter;
            string axis;
            double unitFactor;
            int divisions;
            int flags;
            if (!IGetExtractionParameters(request, out header, out converter, out axis, out unitFactor, out divisions, out flags))
                return new List<IResult>();

            flags += (int)Output_Init_Flags.OP_INIT_INFINITY;

            bool raiseDivisionsWarning = (request is BarResultRequest) && ((request as BarResultRequest).DivisionType == DivisionType.EvenlyDistributed);

            List<IResult> results = new List<IResult>();
            int midPoints = divisions == 1 ? divisions : divisions - 2;
            foreach (string loadCase in loadCases)
            {
                if (InitializeLoadextraction(header, loadCase, midPoints, axis, flags))
                {
                    foreach (int id in objectIds)
                    {
                        GsaResults[] gsaResults = GetResults(id, unitFactor);
                        if (gsaResults != null)
                        {
                            foreach (GsaResults gsaRes in gsaResults)
                            {
                                results.Add(converter.Invoke(gsaRes, id.ToString(), loadCase, gsaResults.Length));
                            }

                            if(raiseDivisionsWarning && gsaResults.Length != divisions)
                                Engine.Reflection.Compute.RecordWarning("Different number of results compared to the expected for object with id " + id + ", for loadcase: " + loadCase);

                        }
                        else
                        {
                            Engine.Reflection.Compute.RecordError("Unable to extract results for object with id " + id + ", for loadcase: " + loadCase);
                        }
                    }
                }
            }

            results.Sort();
            return results;
        }


        /***************************************************/

        private bool GetExtractionParameters(BarResultRequest request, out ResHeader header, out ForceConverter converter, out string axis, out double unitFactor, out int divisions, out int flags)
        {
            axis = Output_Axis.Local;
            divisions = request.Divisions;

            double[] unitFactors = GetUnitFactors();

            switch (request.DivisionType)
            {
                case DivisionType.ExtremeValues:
                    flags = (int)Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
                    break;
                case DivisionType.EvenlyDistributed:
                default:
                    flags = 0;
                    break;
            }

            switch (request.ResultType)
            {
                case BarResultType.BarForce:
                    converter = Convert.FromGsaBarForce;
                    header = ResHeader.REF_FORCE_EL1D;
                    unitFactor = unitFactors[(int)UnitType.FORCE];
                    break;
                case BarResultType.BarDisplacement:
                    axis = Output_Axis.Global;
                    converter = Convert.FromGsaBarDisplacement;
                    header = ResHeader.REF_DISP_EL1D;
                    unitFactor = unitFactors[(int)UnitType.LENGTH];
                    break;
                case BarResultType.BarDeformation:
                    converter = null;
                    header = ResHeader.REF_ACC;
                    unitFactor = 1;
                    Engine.Reflection.Compute.RecordError("Extraction of Localised BarDeformations is not supported in GSA. To get full displacements of the Bar in global coordinates, try pulling BarDisplacements");
                    return false;
                case BarResultType.BarStress:
                    converter = Convert.FromGsaBarStress;
                    header = ResHeader.REF_STRESS_EL1D;
                    unitFactor = unitFactors[(int)UnitType.STRESS];
                    break;
                case BarResultType.BarStrain:
                    converter = Convert.FromGsaBarStrain;
                    header = ResHeader.REF_STRAIN_EL1D;
                    unitFactor = 1;
                    break;
                default:
                    converter = null;
                    header = ResHeader.REF_ACC;
                    unitFactor = 1;
                    Engine.Reflection.Compute.RecordError("Result of type " + request.ResultType + " is not yet supported");
                    return false;
            }

            return true;
        }

        /***************************************************/

        private bool GetExtractionParameters(NodeResultRequest request, out ResHeader header, out ForceConverter converter, out string axis, out double unitFactor, out int divisions, out int flags)
        {

            switch (request.Axis)
            {
                case oM.Structure.Loads.LoadAxis.Local:
                    axis = Output_Axis.Local;
                    break;
                case oM.Structure.Loads.LoadAxis.Global:
                default:
                    axis = Output_Axis.Global;
                    break;
            }

            divisions = 1;
            double[] unitFactors = GetUnitFactors();

            flags = 0;

            switch (request.ResultType)
            {
                case NodeResultType.NodeReaction:
                    header = ResHeader.REF_REAC;
                    converter = Convert.FromGsaReaction;
                    unitFactor = unitFactors[(int)UnitType.FORCE];
                    break;
                case NodeResultType.NodeDisplacement:
                    converter = Convert.FromGsaNodeDisplacement;
                    header = ResHeader.REF_DISP;
                    unitFactor = unitFactors[(int)UnitType.LENGTH];
                    break;
                case NodeResultType.NodeVelocity:
                case NodeResultType.NodeAcceleration:
                default:
                    converter = null;
                    header = ResHeader.REF_ACC;
                    unitFactor = 1;
                    Engine.Reflection.Compute.RecordError("Result of type " + request.ResultType + " is not yet supported");
                    return false;
            }

            return true;
        }

        /***************************************************/

        private bool IGetExtractionParameters(IResultRequest request, out ResHeader header, out ForceConverter converter, out string axis, out double unitFactor, out int divisions, out int flags)
        {
            return GetExtractionParameters(request as dynamic, out header, out converter, out axis, out unitFactor, out divisions, out flags);
        }

        /***************************************************/

        private bool GetExtractionParameters(IResultRequest request, out ResHeader header, out ForceConverter converter, out string axis, out double unitFactor, out int divisions, out int flags)
        {
            axis = null;
            Engine.Reflection.Compute.RecordError("Request of type " + request.GetType().ToString() + " is not supported");
            header = ResHeader.REF_ACC;
            converter = null;
            unitFactor = 1;
            flags = 0;
            divisions = 0;
            return false;
        }

        /***************************************************/
        /**** Private  Methods - Result extraction      ****/
        /***************************************************/

        private bool InitializeLoadextraction(ResHeader header, string loadCase, int divisions, string axis, int flags)
        {
            if (m_gsaCom.Output_Init_Arr(flags, axis, loadCase, header, divisions) != 0)
            {
                Engine.Reflection.Compute.RecordError("Failed to initialize result extraction for loadcase: " + loadCase);
                return false;
            }
            return true;
        }

        /***************************************************/

        private GsaResults[] GetResults(int objectId, double unitFactor)
        {
            GsaResults[] results;
            int numOfComponents;
            try
            {
                m_gsaCom.Output_Extract_Arr(objectId, out results, out numOfComponents);
            }
            catch
            {
                Engine.Reflection.Compute.RecordError("Failed to extract results for item " + objectId);
                return null;
            }

            // Convert to SI
            if (unitFactor != 1)
            {
                foreach (GsaResults r in results)
                    for (int i = 0; i < r.dynaResults.Length; i++)
                        r.dynaResults[i] /= unitFactor;
            }

            return results;
        }


        /***************************************************/
        /**** Private  Methods - Index checking         ****/
        /***************************************************/

        private List<int> CheckAndGetIds(IResultRequest request)
        {
            IList ids = request.ObjectIds;
            if (ids == null || ids.Count == 0)
            {
                return GetAllIds(request as dynamic);
            }
            else if (ids is List<string>)
                return (ids as List<string>).Select(x => int.Parse(x)).ToList();
            else if (ids is List<int>)
                return ids as List<int>;
            else if (ids is List<double>)
                return (ids as List<double>).Select(x => (int)Math.Round(x)).ToList();
            else
            {
                List<int> idsOut = new List<int>();
                foreach (object o in ids)
                {
                    int id;
                    object idObj;
                    if (int.TryParse(o.ToString(), out id))
                    {
                        idsOut.Add(id);
                    }
                    else if (o is IBHoMObject && (o as IBHoMObject).CustomData.TryGetValue(AdapterIdName, out idObj) && int.TryParse(idObj.ToString(), out id))
                        idsOut.Add(id);
                }
                return idsOut;
            }

        }

        /***************************************************/

        private List<int> GetAllIds(BarResultRequest request)
        {
            string allBars = m_gsaCom.GwaCommand("GET_ALL, EL.2").ToString();
            string[] barArr = string.IsNullOrWhiteSpace(allBars) ? new string[0] : allBars.Split('\n');

            List<int> ids = new List<int>();
            bool containsDummies = false;
            foreach (string gsaBar in barArr)
            {

                string[] arr = gsaBar.Split(',');

                string index = arr[1];

                //Check that the element type is a bar
                switch (arr[4])
                {
                    case "BEAM":
                    case "BAR":
                    case "TIE":
                    case "STRUT":
                        break;
                    default:
                        continue;
                }

                //Check if dummy
                if (arr.Last().ToUpper() == "DUMMY")
                {
                    containsDummies = true;
                    continue;
                }

                ids.Add(int.Parse(index));
            }

            if(containsDummies)
                Engine.Reflection.Compute.RecordNote("Model contains 'dummy'-elements. The elements with this tag do not contain any results and will not have any results extracted.");

            return ids;
        }

        /***************************************************/

        private List<int> GetAllIds(NodeResultRequest request)
        {
            int highestIndex = m_gsaCom.GwaCommand("HIGHEST, NODE");

            GsaNode[] nodes;
            m_gsaCom.Nodes(CreateIntSequence(highestIndex), out nodes);

            return nodes.Select(x => x.Ref).ToList();
        }


        /***************************************************/

        static public int[] CreateIntSequence(int maxId)
        {
            int[] ids = new int[maxId];

            for (int i = 0; i < maxId; i++)
            {
                ids[i] = i + 1;
            }

            return ids;
        }


        /***************************************************/

        private List<int> GetAllIds(IResultRequest request)
        {
            return new List<int>();
        }

        /***************************************************/
    }
}

