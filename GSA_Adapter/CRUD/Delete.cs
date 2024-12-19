/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Adapter;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
#if GSA_10_1
    public partial class GSA101Adapter
#else
    public partial class GSA87Adapter
#endif
    {
        /***************************************************/
        /**** Index Adapter Methods                     ****/
        /***************************************************/

        protected override int IDelete(Type type, IEnumerable<object> indices, ActionConfig actionConfig = null)
        {
            //object source;
            //if (config != null && config.TryGetValue("Source", out source) && source.ToString() == "Replace")
                //return 0;

            string typeString = type.ToGsaString();

            if (typeString == null)
            {
                Engine.Base.Compute.RecordError("Delete failed due to wrong type");
                return 0;
            }
            else if (indices == null)
            {
                // Delete them all
                int maxIndex = m_gsaCom.GwaCommand("HIGHEST, " + typeString);
                if (ComCall("BLANK," + typeString + "," + 0 + "," + maxIndex))
                    return (maxIndex + 1); // TODO: Check that this is correct for Gsa
                else
                    return 0;
            }
            else
            {
                int successful = 0;
                foreach (Tuple<int, int> range in GetRanges(type, indices.Cast<int>().ToList()))
                {
                    if (ComCall("BLANK," + typeString + "," + range.Item1 + "," + range.Item2))
                        successful += (range.Item2 - range.Item1 + 1); // TODO: Check that this is correct for Gsa
                }

                return successful;
            }

        }


        /***************************************************/
        /**** Private  Helpers                          ****/
        /***************************************************/

        private List<Tuple<int, int>> GetRanges(Type type, List<int> indices)
        {
            List<Tuple<int, int>> ranges = new List<Tuple<int, int>>();
            if (indices == null || indices.Count < 1)
            {
                return new List<Tuple<int, int>>();
            }
            else
            {
                indices.Sort();
                int first = indices[0];

                for (int i = 0; i < indices.Count-1; i++)
                {
                    if (indices[i] == indices[i + 1] - 1)
                        continue;
                    else
                    {
                        ranges.Add(new Tuple<int, int>(first, indices[i]));
                        first = indices[i + 1];
                    }
                }

                ranges.Add(new Tuple<int, int>(first, indices.Last()));
            }

            return ranges;
        }

        /***************************************************/
    }
}






