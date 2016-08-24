using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMBR = BHoM.Base.Results;
using GSA_Adapter.Structural.Results;

namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : BHoM.Structural.Interface.IElementAdapter
    {
        public bool GetBarForces(List<string> bars, List<string> cases, int divisions, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            BHoMBR.ResultServer<BHoM.Structural.Results.BarForce> resultServer = new BHoM.Base.Results.ResultServer<BHoM.Structural.Results.BarForce>();
            resultServer.OrderBy = orderBy;
            BarResults.GetBarForces(GSA, resultServer, bars, cases, divisions);
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
            throw new NotImplementedException();
        }

        public bool GetNodeReactions(List<string> nodes, List<string> cases, BHoMBR.ResultOrder orderBy, out Dictionary<string, BHoMBR.IResultSet> results)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
