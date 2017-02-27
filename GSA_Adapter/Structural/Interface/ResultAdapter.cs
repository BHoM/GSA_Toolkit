using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMBR = BHoM.Base.Results;
using BHoMSR = BHoM.Structural.Results;
using GSA_Adapter.Structural.Results;
using BHoM.Databases;

namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : BHoM.Structural.Interface.IResultAdapter
    {
        public bool GetBarForces(List<string> bars, List<string> cases, int divisions, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoM.Structural.Results.BarForce> resultServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.BarForce>();
            resultServer.OrderBy = orderBy;
            BarResults.GetBarForces(gsa, resultServer, bars, cases, divisions);
            results = resultServer.LoadData();

            return true;
        }

        public bool GetBarStresses(List<string> bars, List<string> cases, int divisions, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoM.Structural.Results.BarStress> resultServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.BarStress>();
            resultServer.OrderBy = orderBy;
            BarResults.GetBarStresses(gsa, resultServer, bars, cases, divisions);
            results = resultServer.LoadData();

            return true;
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
            BHoMBR.ResultServer<BHoMSR.NodeDisplacement> resultServer = new BHoMBR.ResultServer<BHoMSR.NodeDisplacement>();
            resultServer.OrderBy = orderBy;
            NodeResults.GetNodeDisplacements(gsa, resultServer, nodes, cases);
            results = resultServer.LoadData();

            return true;

            throw new NotImplementedException();
        }

        public bool GetNodeReactions(List<string> nodes, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoMSR.NodeReaction> resultServer = new BHoMBR.ResultServer<BHoMSR.NodeReaction>();
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

        public bool GetBarUtilisation(List<string> bars, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoM.Structural.Results.SteelUtilisation<int, string, int>> resultServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.SteelUtilisation<int, string, int>>();
            resultServer.OrderBy = orderBy;
            GSA_Adapter.Structural.Results.Utilisation.GetSteelBarUtilisation(gsa, resultServer, bars, cases);
            results = resultServer.LoadData();

            return true;

        }


        public bool StoreResults(string filename, List<BHoMBR.ResultType> resultTypes, List<string> loadcases, bool append = false)
        {
            foreach (BHoMBR.ResultType t in resultTypes)
            {
                switch (t)
                {
                    case BHoM.Base.Results.ResultType.BarForce:
                        BarResults.GetBarForces(gsa, new BHoMBR.ResultServer<BHoMSR.BarForce>(filename, append), null, loadcases, 3);
                        break;
                    case BHoM.Base.Results.ResultType.BarStress:
                        break;
                    case BHoMBR.ResultType.NodeReaction:
                        NodeResults.GetNodeReacions(gsa, new BHoMBR.ResultServer<BHoMSR.NodeReaction>(filename, append), null, loadcases);
                        break;
                    case BHoMBR.ResultType.NodeDisplacement:
                        NodeResults.GetNodeDisplacements(gsa, new BHoMBR.ResultServer<BHoMSR.NodeDisplacement>(filename, append), null, loadcases);
                        break;
                    case BHoMBR.ResultType.PanelForce:
                        break;
                    case BHoMBR.ResultType.PanelStress:
                        break;
                    case BHoMBR.ResultType.Utilisation:
                        Utilisation.GetSteelBarUtilisation(gsa, new BHoMBR.ResultServer<BHoMSR.SteelUtilisation<int, string, int>>(filename, append), null, loadcases);
                        break;
                    case BHoMBR.ResultType.NodeCoordinates:
                        NodeResults.GetNodeCoordinates(gsa, new BHoMBR.ResultServer<BHoMSR.NodeCoordinates>(filename), null);
                        break;
                    case BHoMBR.ResultType.BarCoordinates:
                        BarResults.GetBarCoordinates(gsa, new BHoMBR.ResultServer<BHoMSR.BarCoordinates>(filename), null);
                        break;
                }
            }
            return true;
        }

        public bool GetSlabReinforcement(List<string> panels, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            throw new NotImplementedException();
        }


        public bool GetNodeCoordinates(List<string> nodes, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoMSR.NodeCoordinates> resultServer = new BHoMBR.ResultServer<BHoMSR.NodeCoordinates>();
            resultServer.OrderBy = BHoM.Base.Results.ResultOrder.None;
            NodeResults.GetNodeCoordinates(gsa, resultServer, nodes);
            results = resultServer.LoadData();

            return true;
        }

        public bool GetBarCoordinates(List<string> bars, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoMSR.BarCoordinates> resultServer = new BHoMBR.ResultServer<BHoMSR.BarCoordinates>();
            resultServer.OrderBy = BHoM.Base.Results.ResultOrder.None;
            BarResults.GetBarCoordinates(gsa, resultServer, bars);
            results = resultServer.LoadData();

            return true;
        }

        public bool PushToDataBase(IDatabaseAdapter dbAdapter, List<BHoMBR.ResultType> resultTypes, List<string> loadcases, string key, bool append = false)
        {
            foreach (BHoMBR.ResultType t in resultTypes)
            {
                Dictionary<string, BHoMBR.IResultSet> results = new Dictionary<string, BHoM.Base.Results.IResultSet>(); ;
                switch (t)
                {
                    case BHoM.Base.Results.ResultType.BarForce:
                        BHoMBR.ResultServer<BHoM.Structural.Results.BarForce> resultServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.BarForce>();
                        BarResults.GetBarForces(gsa, resultServer, null, loadcases, 5);
                        dbAdapter.Push("BarForces", resultServer.ToList(), key);
                        break;
                    case BHoM.Base.Results.ResultType.BarStress:
                        BHoMBR.ResultServer<BHoM.Structural.Results.BarStress> stressServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.BarStress>();
                        BarResults.GetBarStresses(gsa, stressServer, null, loadcases, 5);
                        dbAdapter.Push("BarStresses", stressServer.ToList(), key);
                        break;
                    case BHoMBR.ResultType.NodeReaction:

                        break;
                    case BHoMBR.ResultType.NodeDisplacement:
                        break;
                    case BHoMBR.ResultType.PanelForce:
                        break;
                    case BHoMBR.ResultType.PanelStress:
                        break;
                    case BHoMBR.ResultType.Utilisation:
                        break;
                    case BHoMBR.ResultType.NodeCoordinates:
                        break;
                    case BHoMBR.ResultType.BarCoordinates:
                        break;
                }
            }
            return true;
        }


    }
}
