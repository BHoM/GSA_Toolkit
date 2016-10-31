using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMBR = BHoM.Base.Results;
using BHoMSR = BHoM.Structural.Results;
using GSA_Adapter.Structural.Results;

namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : BHoM.Structural.Interface.IResultAdapter
    {
        public bool GetBarForces(List<string> bars, List<string> cases, int divisions, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoM.Structural.Results.BarForce<int, string, int>> resultServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.BarForce<int, string, int>>();
            resultServer.OrderBy = orderBy;
            BarResults.GetBarForces(gsa, resultServer, bars, cases, divisions);
            results = resultServer.LoadData();

            return true;
        }

        public bool GetBarStresses()
        {
            throw new NotImplementedException();
        }

        public bool GetModalResults()
        {
            throw new NotImplementedException();
        }

        public bool GetNodeAccelerations(List<string> nodes, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetNodeDisplacements(List<string> nodes, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoMSR.NodeDisplacement<int, string, int>> resultServer = new BHoMBR.ResultServer<BHoMSR.NodeDisplacement<int, string, int>>();
            resultServer.OrderBy = orderBy;
            NodeResults.GetNodeDisplacements(gsa, resultServer, nodes, cases);
            results = resultServer.LoadData();

            return true;

            throw new NotImplementedException();
        }

        public bool GetNodeReactions(List<string> nodes, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoMSR.NodeReaction<int,string,int>> resultServer = new BHoMBR.ResultServer<BHoMSR.NodeReaction<int, string, int>>();
            resultServer.OrderBy = orderBy;
            NodeResults.GetNodeReacions(gsa, resultServer, nodes, cases);
            results = resultServer.LoadData();

            return true;
        }

        public bool GetNodeVelocities(List<string> nodes, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetPanelForces(List<string> panels, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetPanelStress(List<string> panels, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool StoreResults(string filename, List<BHoMBR.ResultType> resultTypes, List<string> loadcases, bool append = false)
        {
            foreach (BHoMBR.ResultType t in resultTypes)
            {
                switch (t)
                {
                    case BHoM.Base.Results.ResultType.BarForce:
                        BarResults.GetBarForces(gsa, new BHoMBR.ResultServer<BHoMSR.BarForce<int, string, int>>(filename, append), null, loadcases, 3);
                        break;
                    case BHoM.Base.Results.ResultType.BarStress:
                        break;
                    case BHoMBR.ResultType.NodeReaction:
                        NodeResults.GetNodeReacions(gsa, new BHoMBR.ResultServer<BHoMSR.NodeReaction<int, string, int>>(filename, append), null, loadcases);
                        break;
                    case BHoMBR.ResultType.NodeDisplacement:
                        NodeResults.GetNodeDisplacements(gsa, new BHoMBR.ResultServer<BHoMSR.NodeDisplacement<int, string, int>>(filename, append), null, loadcases);
                        break;
                    case BHoMBR.ResultType.PanelForce:
                        break;
                    case BHoMBR.ResultType.PanelStress:
                        break;

                }
            }
            return true;
        }
    }
}
