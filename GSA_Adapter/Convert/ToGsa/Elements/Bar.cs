/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Linq;
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

        private static string ToGsaString(this Edge edge, string index)
        {
            string command = "LINE.2";
            string name = edge.TaggedName().ToGSACleanName();
            string type = "LINE";

            List<int> topologies = edge.CustomData["NodeIndecies"] as List<int>;
            string startIndex = topologies[0].ToString();
            string endIndex = topologies[1].ToString();

            string axis = "GLOBAL";
            string nodalConstraint = "0, 0, 0, 0, 0, 0";
            string nodalStiffness = "0, 0, 0, 0, 0, 0";

            string step_definition = "USE_REGION_STEP_SIZE";
            string step_size = "1";
            string num_seg = "1";
            string step_ratio = "6";
            string tied_int = "NO";

            string str = command + ", " //LINE.2 
                + index + ","           //| ref |
                + name +                //name
                ", NO_RGB , "           //colour
                + type + " , "          //type
                + startIndex + ", "     //topology_1
                + endIndex + ", "       //topology_2
                + "0,"                  //topology_3
                + "0,"                  //radius
                + axis + ","             //axis
                + nodalConstraint + "," //| x | y | z | xx | yy | zz |
                + nodalStiffness + ","  //Kx | Ky | Kz | Kxx | Kyy | Kzz | 
                + step_definition + "," //step_definition
                + step_size + ","       //step_size
                + num_seg + ","         //num_seg
                + step_ratio + ","      //step_ratio
                + tied_int;             //tied_int

            return str;
        }

        /***************************************/

        private static string ToGsaString(this Opening opening, string index)
        {
            return AreaString(opening.Edges, opening.TaggedName().ToGSACleanName(), index, "VOID", "0");
        }

        /***************************************/

        public static List<string> ToGsaString(this Panel panel, string index, string areaIndex)
        {
            string areaString = AreaString(panel.ExternalEdges, panel.TaggedName().ToGSACleanName(), areaIndex, "TWO_WAY ", panel.Property.GSAId().ToString());
            string command = "REGION.2";

            string name = panel.TaggedName().ToGSACleanName();
            string colour = "NO_RGB";
            string grid_plane = "0";
            string region_type = "2D_MESH";
            string nodes = "";
            string lines = "";
            string areas = areaIndex + " " + panel.Openings.Select(x => x.GSAId().ToString()).Aggregate((a, b) => a + " " + b);
            string mesh_step_type = "LINEAR";
            string mesh_density = "FINE ";
            string rigorous = "RIGOROUS";
            string element_type = "QUAD4 ";
            string CAD_format = "YES ";
            string min_bandwidth = "YES ";
            string offset_mesh = "NO";
            string memb_elem = "NO";
            string soil_spr = "NO";
            string meshing_option = "QUAD_BUILD";

            string regionStr = command + ","          //	REGION.2
                        + index + ","	        //	ref
                        + name + ","	        //	name
                        + colour + ","	        //	colour
                        + grid_plane + ","	    //	grid_plane
                        + region_type + ","	    //	region_type
                        + nodes + ","	        //	nodes
                        + lines + ","	        //	lines
                        + areas + ","	        //	areas
                        + mesh_step_type + ","	//	mesh_step_type
                        + mesh_density + ","	//	mesh_density
                        + rigorous + ","	    //	rigorous
                        + element_type + ","	//	element_type
                        + CAD_format + ","	    //	CAD_format
                        + min_bandwidth + ","	//	min_bandwidth
                        + offset_mesh + ","	    //	offset_mesh
                        + memb_elem + ","	    //	memb_elem
                        + soil_spr + ","	    //	soil_spr
                        + meshing_option;	    //	meshing_option


            return new List<string> { areaString, regionStr };
        }

        /***************************************/

        private static string AreaString(this List<Edge> edges, string name, string index, string type, string property)
        {
            string command = "AREA.2";


            string span = "0";
            string group = "0";
            string lines = edges.Select(x => x.GSAId().ToString()).Aggregate((a, b) => a + " " + b);
            string coefficient = "0";

            string str = command + ", " //AREA.2
                + index + ","           //| ref |
                + name +                //name
                ", NO_RGB , "           //colour
                + type + " , "          //type
                + span + ", "           //span
                + property + ", "       //property
                + group + ", "          //group
                + lines + ","            //lines
                + coefficient;          //coefficient


            return str;
        }

        /***************************************/
    }
}


