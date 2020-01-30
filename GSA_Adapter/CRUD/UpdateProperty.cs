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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Base;
using BH.Engine.GSA;
using BH.oM.Adapter;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        protected override int IUpdateTags(Type type, IEnumerable<object> ids, IEnumerable<HashSet<string>> newTags, ActionConfig actionConfig = null)
        {
            List<string> indecies = ids.Select(x => x.ToString()).ToList();
            if (indecies.Count < 1)
                return 0;

            List<HashSet<string>> tags = newTags.ToList();

            List<IBHoMObject> objects = IRead(type, indecies.ToList(), actionConfig).ToList();

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Tags = tags[i];
            }

            if (ICreate(objects, actionConfig))
                return objects.Count;

            return 0;

        }
    }
}

