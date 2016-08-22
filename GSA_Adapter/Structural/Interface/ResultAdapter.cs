using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Base.Results;
using BHoM.Structural.Results;

namespace GSA_Adapter.Structural.Interface
{
    public partial class ResultAdapter : BHoM.Structural.Interface.IResultAdapter
    {
        public bool GetBarForces(List<string> bars, List<string> cases, int divisions, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetBarStresses()
        {
            throw new NotImplementedException();
        }

        public bool GetModalResults()
        {
            throw new NotImplementedException();
        }

        public bool GetNodeAccelerations(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetNodeDisplacements(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetNodeReactions(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetNodeVelocities(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetPanelForces(List<string> panels, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetPanelStress(List<string> panels, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool StoreResults(string filename, List<ResultType> resultTypes, List<string> loadcases, bool append = false)
        {
            throw new NotImplementedException();
        }
    }
}
