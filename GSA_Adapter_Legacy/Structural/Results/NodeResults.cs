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
using Interop.gsa_8_7;
using BHBR = BHoM.Base.Results;
using BHSR = BHoM.Structural.Results;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Results
{
    public static class NodeResults
    {
        public static bool GetNodeReacions(IComAuto gsa, BHBR.ResultServer<BHSR.NodeReaction> resultServer, List<string> nodeNumbers, List<string> cases)
        {

            ResHeader resHeader = ResHeader.REF_REAC;

            List<int> nodeIds = CheckAndGetNodeIndices(gsa, nodeNumbers);
            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);

            for (int i = 0; i < cases.Count; i++)
            {
                Dictionary<int, List<double>> res = GetNodalResults(gsa, cases[i], resHeader, nodeIds);

                if (res != null)
                {
                    List<BHSR.NodeReaction> nodeReactions = new List<BHoM.Structural.Results.NodeReaction>();

                    foreach (KeyValuePair<int, List<double>> kvp in res)
                    {
                        List<double> nodeRes = kvp.Value;
                        BHSR.NodeReaction reak = new BHSR.NodeReaction(kvp.Key.ToString(), cases[i], "0", nodeRes[0], nodeRes[1], nodeRes[2], nodeRes[4], nodeRes[5], nodeRes[6]);
                        nodeReactions.Add(reak);
                    }
                    resultServer.StoreData(nodeReactions);
                }

            }

            return true;
        }

        public static bool GetNodeDisplacements(IComAuto gsa, BHBR.ResultServer<BHSR.NodeDisplacement> resultServer, List<string> nodeNumbers, List<string> cases)
        {
            ResHeader resHeader = ResHeader.REF_DISP;

            List<int> nodeIds = CheckAndGetNodeIndices(gsa, nodeNumbers);
            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);

            for (int i = 0; i < cases.Count; i++)
            {
                Dictionary<int, List<double>> res = GetNodalResults(gsa, cases[i], resHeader, nodeIds);

                if (res != null)
                {
                    List<BHSR.NodeDisplacement> nodeReactions = new List<BHSR.NodeDisplacement>();

                    foreach (KeyValuePair<int, List<double>> kvp in res)
                    {
                        List<double> nodeRes = kvp.Value;
                        BHSR.NodeDisplacement disp = new BHSR.NodeDisplacement(kvp.Key.ToString(), cases[i], "0", nodeRes[0], nodeRes[1], nodeRes[2], nodeRes[4], nodeRes[5], nodeRes[6]);
                        nodeReactions.Add(disp);
                    }
                    resultServer.StoreData(nodeReactions);
                }

            }

            return true;
        }

        public static bool GetNodeCoordinates(IComAuto gsa, BHBR.ResultServer<BHSR.NodeCoordinates> resultServer, List<string> nodes)
        {


            List<BHSR.NodeCoordinates> nodeCoords = new List<BHSR.NodeCoordinates>();

            // TODO for Isak
            int highest = gsa.GwaCommand("HIGHEST, NODE");

            int[] possibleIndices = Utility.Utils.CreateIntSequence(highest);

            GsaNode[] gsaNodes;
            
            gsa.Nodes(possibleIndices, out gsaNodes);

            foreach (GsaNode n in gsaNodes)
            {
                nodeCoords.Add(new BHSR.NodeCoordinates(n.Ref.ToString(), n.Coor[0], n.Coor[1], n.Coor[2]));
            }

            resultServer.StoreData(nodeCoords);

            return true;
        }

        private static Dictionary<int, List<double>> GetNodalResults(IComAuto gsa, string loadCase, ResHeader resultType, List<int> nodeIndices)
        {
            int inputFlags = (int)GsaEnums.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            string axis = GsaEnums.Output_Axis.Global();
            int num1dpos = 0;

            if (gsa.Output_Init_Arr(inputFlags, axis, loadCase, resultType, num1dpos) != 0)
                return null;

            Dictionary<int,List<double>> nodalResults = new Dictionary<int, List<double>>();

            for (int i = 0; i < nodeIndices.Count; i++)
            {
                List<double> nodeRes = GetNodalResult(gsa, nodeIndices[i]);
                if (nodeRes != null)
                    nodalResults.Add(nodeIndices[i],nodeRes);
            }

            return nodalResults;
        }

        private static List<double> GetNodalResult(IComAuto gsa, int nodeId)
        {
            GsaResults[] results;
            int numOfComponents;
            try
            {
                gsa.Output_Extract_Arr(nodeId, out results, out numOfComponents);
            }
            catch
            {
                return null;
            }
            List<double> nodeRes = new List<double>();

            for (int i = 0; i < numOfComponents; i++)
            {
                nodeRes.Add(results[0].dynaResults[i]);
            }

            return nodeRes;

        }

        private static List<int> CheckAndGetNodeIndices(IComAuto gsa, List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                int highestIndex = gsa.GwaCommand("HIGHEST, NODE");

                GsaNode[] nodes;
                gsa.Nodes(Utils.CreateIntSequence(highestIndex), out nodes);

                return nodes.Select(x => x.Ref).ToList();
            }
            else
            {
                return ids.Select(x => int.Parse(x)).ToList();
            }
        }


        //private static List<double> GetNodalResult(IComAuto gsa, string loadCase, ResHeader resultType, int nodeId)
        //{
        //    int inputFlags = (int)GsaEnums.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
        //    string axis = GsaEnums.Output_Axis.Global();
        //    int num1dpos = 0;

        //    if (gsa.Output_Init_Arr(inputFlags, axis, loadCase, resultType, num1dpos) != 0)
        //        return null;

        //    GsaResults[] results;
        //    int numOfComponents;
        //    gsa.Output_Extract_Arr(nodeId, out results, out numOfComponents);

        //    List<double> nodeRes = new List<double>();

        //    for (int i = 0; i < numOfComponents; i++)
        //    {
        //        nodeRes.Add(results[0].dynaResults[i]);
        //    }

        //    return nodeRes;
        //}
    }
}
