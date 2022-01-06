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

using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Base;
using System;
using BH.Engine.Adapters.GSA;
using BH.Engine.Base;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static string ToGsaString(this Bar bar, string index)
        {
            string command = "EL.2";
            string name = bar.TaggedName().ToGSACleanName();
            string type = GetElementTypeString(bar);

            string sectionPropertyIndex = bar.SectionProperty != null ? bar.SectionProperty.GSAId().ToString() : "1";
            int group = 0;

            string startIndex = bar.StartNode.GSAId().ToString();
            string endIndex = bar.EndNode.GSAId().ToString();

            string orientationAngle = (bar.OrientationAngle * 180 / Math.PI).ToString();
            // TODO: Make sure that these are doing the correct thing. Release vs restraint corresponding to true vs false
            string startR = bar.Release != null ? CreateReleaseString(bar.Release.StartRelease) : "FFFFFF";
            string endR = bar.Release != null ? CreateReleaseString(bar.Release.EndRelease) : "FFFFFF";
            string dummy = CheckDummy(bar);

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static string GetElementTypeString(Bar bar)
        {
            switch (bar.FEAType)
            {
                case BarFEAType.Axial:
                    return "BAR";
                case BarFEAType.Flexural:
                    return "BEAM";
                case BarFEAType.TensionOnly:
                    return "TIE";
                case BarFEAType.CompressionOnly:
                    return "STRUT";
                default:
                    return "BEAM";
                    //Returning beam by default as it is the most generic type.
                    //Might be better flagging this as an error
            }
        }

        /***************************************/

        private static string CreateReleaseString(Constraint6DOF nodeConstraint)
        {
            bool[] fixities = nodeConstraint.Fixities();    //IsConstrained
            double[] stiffness = nodeConstraint.ElasticValues();

            string relStr = "";
            string stiffStr = "";

            for (int i = 0; i < fixities.Length; i++)
            {
                if (fixities[i])
                {
                    relStr += "F";
                }
                else
                {
                    if (stiffness[i] > 0)
                    {
                        relStr += "K";
                        stiffStr += "," + stiffness[i];
                    }
                    else
                        relStr += "R";
                }

            }

            return relStr + stiffStr;
        }

        /***************************************/

        private static string CheckDummy(BHoMObject obj)
        {
            DummyTag dummy = obj.FindFragment<DummyTag>();

            if (dummy != null && dummy.IsDummy)
                return "DUMMY";

            return "";
        }

        /***************************************/
    }
}



