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
        public static bool GetNodeReacions(IComAuto gsa, BHBR.ResultServer<BHSR.NodeReaction<int, string, int>> resultServer, List<string> nodeNumbers, List<string> cases)
        {

            ResHeader resHeader = ResHeader.REF_REAC;

            List<int> nodeIds = CheckAndGetNodeIndecies(gsa, nodeNumbers);
            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);

            for (int i = 0; i < cases.Count; i++)
            {
                Dictionary<int, List<double>> res = GetNodalResults(gsa, cases[i], resHeader, nodeIds);

                if (res != null)
                {
                    List<BHSR.NodeReaction<int, string, int>> nodeReactions = new List<BHoM.Structural.Results.NodeReaction<int, string, int>>();

                    foreach (KeyValuePair<int, List<double>> kvp in res)
                    {
                        List<double> nodeRes = kvp.Value;
                        BHSR.NodeReaction<int, string, int> reak = new BHSR.NodeReaction<int, string, int>(kvp.Key, cases[i], 0, nodeRes[0], nodeRes[1], nodeRes[2], nodeRes[4], nodeRes[5], nodeRes[6]);
                        nodeReactions.Add(reak);
                    }
                    resultServer.StoreData(nodeReactions);
                }

            }

            return true;
        }

        public static bool GetNodeDisplacements(IComAuto gsa, BHBR.ResultServer<BHSR.NodeDisplacement<int, string, int>> resultServer, List<string> nodeNumbers, List<string> cases)
        {
            ResHeader resHeader = ResHeader.REF_DISP;

            List<int> nodeIds = CheckAndGetNodeIndecies(gsa, nodeNumbers);
            cases = ResultUtilities.CheckAndGetAnalysisCases(gsa, cases);

            for (int i = 0; i < cases.Count; i++)
            {
                Dictionary<int, List<double>> res = GetNodalResults(gsa, cases[i], resHeader, nodeIds);

                if (res != null)
                {
                    List<BHSR.NodeDisplacement<int, string, int>> nodeReactions = new List<BHSR.NodeDisplacement<int, string, int>>();

                    foreach (KeyValuePair<int, List<double>> kvp in res)
                    {
                        List<double> nodeRes = kvp.Value;
                        BHSR.NodeDisplacement<int, string, int> disp = new BHSR.NodeDisplacement<int, string, int>(kvp.Key, cases[i], 0, nodeRes[0], nodeRes[1], nodeRes[2], nodeRes[4], nodeRes[5], nodeRes[6]);
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
            /*for ()
            {
                nodeCoords.Add(new BHSR.NodeCoordinates(id, x, y, z));
            }*/

            resultServer.StoreData(nodeCoords);

            return true;
        }

        private static Dictionary<int, List<double>> GetNodalResults(IComAuto gsa, string loadCase, ResHeader resultType, List<int> nodeIndecies)
        {
            int inputFlags = (int)GsaEnums.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            string axis = GsaEnums.Output_Axis.Global();
            int num1dpos = 0;

            if (gsa.Output_Init_Arr(inputFlags, axis, loadCase, resultType, num1dpos) != 0)
                return null;

            Dictionary<int,List<double>> nodalResults = new Dictionary<int, List<double>>();

            for (int i = 0; i < nodeIndecies.Count; i++)
            {
                List<double> nodeRes = GetNodalResult(gsa, nodeIndecies[i]);
                if (nodeRes != null)
                    nodalResults.Add(nodeIndecies[i],nodeRes);
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

        private static List<int> CheckAndGetNodeIndecies(IComAuto gsa, List<string> ids)
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
