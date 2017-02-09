using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSA_Adapter.Structural.Interface;
using GSA_Adapter.Structural.Results;
using Mongo_Adapter;
using BHoMBR = BHoM.Base.Results;
using BHoMSR = BHoM.Structural.Results;

namespace GSA_Mongo.Structural.Interface
{
    public static class MongoAdapter
    {

        public static bool StoreResults(MongoLink mongo, GSAAdapter gsaAdapter, List<BHoMBR.ResultType> resultTypes, List<string> loadcases, bool append = false)
        {
            foreach (BHoMBR.ResultType t in resultTypes)
            {
                Dictionary<string, BHoMBR.IResultSet> results = new Dictionary<string, BHoM.Base.Results.IResultSet>(); ;
                switch (t)
                {
                    case BHoM.Base.Results.ResultType.BarForce:
                        gsaAdapter.GetBarForces(null, loadcases, 5, BHoM.Base.Results.ResultOrder.Name, out results); ;
                        break;
                    case BHoM.Base.Results.ResultType.BarStress:
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

                foreach (var kvp in results)
                {
                    mongo.Push(kvp.Value.ToListData(), kvp.Key);
                }
            }
            return true;

        }

    }
}
