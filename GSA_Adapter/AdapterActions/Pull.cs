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

using System.Collections.Generic;
using System.Collections;
using BH.oM.Data.Requests;
using BH.oM.Adapter;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        public override IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault)
        {
            List<object> readresult = new List<object>();

            if (request == null || (request is FilterRequest && (request as FilterRequest)?.Type == null))
            {
                readresult.AddRange(ReadNodes());
                readresult.AddRange(ReadBars());
                readresult.AddRange(ReadFEMesh());

                BH.Engine.Reflection.Compute.RecordWarning("No request provided: only Nodes, Bars and FEMeshes have been pulled. \nTo To Pull other types, input an appropriate IRequest.");

                return readresult;
            }

            return base.Pull(request, pullType);
        }
    }
}
