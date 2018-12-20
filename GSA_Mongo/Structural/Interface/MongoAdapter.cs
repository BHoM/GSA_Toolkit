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
