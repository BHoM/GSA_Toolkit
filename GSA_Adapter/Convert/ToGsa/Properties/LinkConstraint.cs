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

using BH.oM.Structure.Constraints;
using BH.Engine.Structure;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(LinkConstraint constraint, string index)
        {
            string command = "PROP_LINK";
            constraint.Name = constraint.DescriptionOrName().ToGSACleanName();
            string name = constraint.TaggedName();
            string restraint = GetRestraintString(constraint);
            return command + ", " + index + ", " + name + ", NO_RGB, " + restraint;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string GetRestraintString(LinkConstraint constr)
        {
            string str = "";

            if (constr.XtoX || constr.XtoYY || constr.XtoZZ)
            {
                str += "X:";

                if (constr.XtoX)
                    str += "X";
                if (constr.XtoYY)
                    str += "YY";
                if (constr.XtoZZ)
                    str += "ZZ";

                str += "-";
            }

            if (constr.YtoY || constr.YtoXX || constr.YtoZZ)
            {
                str += "Y:";

                if (constr.YtoY)
                    str += "Y";
                if (constr.YtoXX)
                    str += "XX";
                if (constr.YtoZZ)
                    str += "ZZ";

                str += "-";
            }

            if (constr.ZtoZ || constr.ZtoXX || constr.ZtoYY)
            {
                str += "Z:";

                if (constr.ZtoZ)
                    str += "Z";
                if (constr.ZtoXX)
                    str += "XX";
                if (constr.ZtoYY)
                    str += "YY";

                str += "-";
            }

            if (constr.XXtoXX)
                str += "XX:XX-";

            if (constr.YYtoYY)
                str += "YY:YY-";

            if (constr.ZZtoZZ)
                str += "ZZ:ZZ-";


            str = str.TrimEnd('-');

            return str;

        }

        /***************************************************/
    }
}






