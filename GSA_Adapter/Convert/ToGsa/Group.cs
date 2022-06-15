/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.Engine.Adapters.GSA;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString<T>(this BH.oM.Base.BHoMGroup<T> group, string index) where T : BH.oM.Base.IBHoMObject
        {
            string command = "LIST";
            string name = group.Name;
            string type = group.IElementType();
            string desc = group.Elements.Select(x => int.Parse(x.GSAId().ToString())).GeterateIdString();

            return command + ", " + index + ", " + name + ", " + type + ", " + desc;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        public static string CreateIdListOrGroupName<T>(this IElementLoad<T> load) where T : IBHoMObject
        {
            //For a named group, appy loads to the group name
            if (!string.IsNullOrWhiteSpace(load.Objects.Name))
                return "\"" + load.Objects.Name + "\"";

            //Otherwise apply to the corresponding indecies
            return load.Objects.Elements.SelectMany(x => x.GSAIds()).OrderBy(x => x).GeterateIdString();

        }

        /***************************************************/

        private static IEnumerable<int> GSAIds(this IBHoMObject obj)
        {
            //If FEMesh, then get ID for each face
            if (obj is FEMesh)
            {
                return (obj as FEMesh).Faces.Select(x => x.GSAId());
            }
            else
                return new List<int> { obj.GSAId() };
        }

        /***************************************************/

        public static string GeterateIdString(this IEnumerable<int> ids)
        {
            string str = "";

            int counter = 0;
            int prev = -10;

            foreach (int i in ids)
            {
                if (i - 1 == prev)
                {
                    counter++;
                }
                else
                {
                    if (counter > 1)
                        str += "to " + prev + " ";
                    else if (counter > 0)
                        str += prev + " ";

                    str += i.ToString() + " ";
                    counter = 0;
                }

                prev = i;
            }

            if (counter > 1)
                str += "to " + prev + " ";
            else if (counter > 0)
                str += prev + " ";

            return str;
        }

        /***************************************************/
        /**** Private Methods - Element Type            ****/
        /***************************************************/

        private static string ElementType(this BHoMGroup<Node> group)
        {
            return "NODE";
        }

        /***************************************************/

        private static string ElementType(this BHoMGroup<Bar> group)
        {
            return "ELEMENT";
        }


        /***************************************************/

        private static string ElementType(this BHoMGroup<RigidLink> group)
        {
            return "ELEMENT";
        }

        /***************************************************/

        private static string ElementType(this BHoMGroup<IBHoMObject> group)
        {
            if (group.Elements.Where(x => x.GetType() == typeof(Node)).Count() == group.Elements.Count)
                return "NODE";

            return "ELEMENT";
        }

        /***************************************************/

        private static string ElementType(this BHoMGroup<BHoMObject> group)
        {
            if (group.Elements.Where(x => x.GetType() == typeof(Node)).Count() == group.Elements.Count)
                return "NODE";

            return "ELEMENT";
        }

        /***************************************************/

        private static string ElementType(this BHoMGroup<IAreaElement> group)
        {
            return "ELEMENT";
        }

        /***************************************************/
        /**** private Methods - Interfaces               ****/
        /***************************************************/

        private static string IElementType<T>(this BHoMGroup<T> group) where T : IBHoMObject
        {
            return ElementType(group as dynamic);
        }

        /***************************************************/
    }
}


