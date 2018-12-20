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
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Base;
using BH.Engine.GSA;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        public override int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {

            if (property == "Tags")
            {
                List<string> indecies = ids.Select(x => x.ToString()).ToList();
                if (indecies.Count < 1)
                    return 0;

                List<HashSet<string>> tags = (newValue as IEnumerable<HashSet<string>>).ToList();
                return UpdateDateTags(type, indecies, tags);
            }

            return 0;
        }

        private int UpdateDateTags(Type t, List<string> indecies, List<HashSet<string>> tags)
        {
            
            List<IBHoMObject> objects = Read(t, indecies.ToList()).ToList();

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Tags = tags[i];
            }

            if (Create(objects))
                return objects.Count;

            return 0;
        }
    }
}
