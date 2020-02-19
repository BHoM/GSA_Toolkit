﻿/*
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


using BH.oM.Structure.Loads;
using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<string> ToGsaString(this LoadCombination loadComb)
        {
            //TODO: Implement pushing of combinations of combinations
            string desc = loadComb.CombinationString();
            string combNo = loadComb.Number.ToString();
            List<string> gsaStrings = new List<string>();
            gsaStrings.Add(AnalysisCase(combNo, loadComb.Name, combNo, desc));
            string type = TaskType(loadComb);
            gsaStrings.Add(AnalysisTask(combNo, loadComb.Name, type, "0", combNo));

            return gsaStrings;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string TaskType(LoadCombination comb)
        {
            if (comb.CustomData.ContainsKey("Task Type"))
                return comb.CustomData["Task Type"].ToString();

            return "STATIC";
        }

        /***************************************************/

        public static string CombinationString(this LoadCombination comb)
        {
            string str = "";
            for (int i = 0; i < comb.LoadCases.Count; i++)
            {
                str += comb.LoadCases[i].Item1.ToString() + "L" + ((Loadcase)comb.LoadCases[i].Item2).Number;

                if (i != comb.LoadCases.Count - 1)
                    str += " + ";
            }

            return str;
        }

        /***************************************************/

        private static string AnalysisTask(string taskNo, string name, string type, string stage, string anal_caseNo)
        {
            string addTask;
            string command = "TASK";
            string solution = "";
            string scheme = "";

            switch (type)
            {
                case "NL_STATIC":
                    solution = "SOL_BUCKLING_NL";
                    scheme = "SINGLE";
                    break;

                case "FORM_FIND":
                    solution = "SOL_FORM_FINDING";
                    //scheme = "SINGLE"; //GSA 8.7 build 27
                    scheme = "FORM_FIND"; //GSA 8.7 build 45    
                    break;

                case "SOAP_FILM":
                    solution = "SOL_FORM_FINDING";
                    scheme = "SOAP_FORM";
                    break;

                case "STATIC":
                    solution = "SOL_STATIC";
                    scheme = "SINGLE";
                    break;
            }

            if (type == "STATIC")
            {
                addTask = command
                     + "," + taskNo
                     + "," + name
                     + "," + stage
                     + ", GSS"
                     + "," + solution
                     + ", 1"                 //start 
                     + ", 0"                 //num_mode 
                     + ", 128"               //num 
                     + ", SELF"              //p_delta 
                     + ", none"              //p_delta_case
                     + ", none"              //prestress 
                     + ", DRCMEFNSU"         //result 
                     + ", MIN"               //front_op 
                     + ", AUTO"              //vector 
                     + ", 0,0,0"             //xyz
                     + ", NONE"              //prune
                     + ", ERROR"             //geometry
                     + ", NONE"              //lower
                     + ", NONE"              //upper 
                     + ", RAFT_LO"           //raft 
                     + ", RESID_NO"          //residual 
                     + ", 0"                 //shift 
                     + ", 1";                //stiff

                return addTask;
            }
            else
            {
                addTask = command
                    + "," + taskNo
                    + "," + name
                    + "," + stage
                    + ", GSRELAX"
                    + "," + solution
                    + "," + scheme
                    + ", 1"                     //num_case 
                    + ", BEAM_GEO_YES"
                    + ", SHELL_GEO_NO"
                    + ", 0.1"                   //first_inc
                    + ", 0.0001"                //min_inc 
                    + ", 0.1"                   //max_inc 
                    + ",CYCLE"
                    + ", 1000000"               //num_cycle 
                    + ", ABS"                   //rel_abs_residual 
                    + ", 1"                     // force_residual 
                    + ", 1"                     //moment_residual 
                    + ", DISP_CTRL_YES"
                    + ", 0"                     //disp_ctrl_node 
                    + ", 1"                     //disp_ctrl_dir 
                    + ", 0.01"                  //disp_ctrl_value
                    + ", LOAD_CTRL_YES"
                    + ", 1"                     //ctrl_load_factor 
                    + ", none"                  //elem_list 
                    + "," + anal_caseNo
                    + ", 500"                   //report_cycle 
                    + ", 10000"                 //graphic_cycle
                    + ", RESID_NOCONV"          //save_residual
                    + ", DAMP_VISCOUS"          //damping_type 
                    + ", 0"                     //percent_damp_disp
                    + ", 0"                     //percent_damp_rotn 
                    + ", 1"                     //dummy_mass_factor 
                    + ", 1"                     //dummy_inertia_factor 
                    + ", 1"                                                     //Fix for GSA Build 45
                    + ", 1"                                                     //Fix for GSA Build 45        
                    + ", AUTO_MASS_YES"
                    + ", AUTO_DAMP_YES"
                    + ", FF_SAVE_ELEM_FORCE_YES"                                //Fix for GSA Build 45
                    + ", FF_SAVE_SPACER_FORCE_TO_ELEM"                          //Fix for GSA Build 45                
                    + ", DRCEFNSU";             //results

                return addTask;
            }
        }

        /***************************************************/

        private static string AnalysisCase(string anal_caseNo, string name, string taskNo, string desc)
        {
            string addCase;
            string command = "ANAL";

            addCase = command
                + "," + anal_caseNo
                + "," + name
                + "," + taskNo
                + "," + desc;

            return addCase;
        }

        /***************************************************/

    }
}
