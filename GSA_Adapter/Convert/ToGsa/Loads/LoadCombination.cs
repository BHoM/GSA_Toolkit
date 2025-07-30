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


using BH.oM.Structure.Loads;
using System.Collections.Generic;
using BH.oM.Adapters.GSA;
using BH.Engine.Base;
using BH.Engine.Adapter;

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
            AnalysisType type;
            int stage;
            double residualForce, residualMoment;
            string beamGeoStiffness;
            TaskTypeAndStage(loadComb, out type, out stage, out residualForce, out residualMoment, out beamGeoStiffness);
            gsaStrings.Add(AnalysisTask(combNo, loadComb.Name, type, stage, combNo, residualForce, residualMoment, beamGeoStiffness));

            loadComb.SetAdapterId(typeof(GSAId), "A" + loadComb.Number);

            return gsaStrings;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void TaskTypeAndStage(LoadCombination comb, out AnalysisType type, out int stage, out double residualForce, out double residualMoment, out string beamGeoStiffness)
        {
            AnalysisTaskFragment fragment = comb.FindFragment<AnalysisTaskFragment>() ?? new AnalysisTaskFragment();

            type = fragment.AnalysisType;
            stage = fragment.Stage;
            residualForce = fragment.ResidualForce;
            residualMoment = fragment.ResidualMoment;
            if (fragment.BeamSlendernessEffect)
                beamGeoStiffness = "BEAM_GEO_YES";
            else
                beamGeoStiffness = "BEAM_GEO_NO";

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

        private static string AnalysisTask(string taskNo, string name, AnalysisType type, int stage, string anal_caseNo, double residualForce, double residualMoment, string beamGeoStiffness)
        {
            string addTask;
            string command = "TASK";
            string solution = "";
            string scheme = "";

            switch (type)
            {
                case AnalysisType.NonLinearStatic:
#if GSA_10
                    solution = "STATIC_NL"; 
#else
                    solution = "SOL_BUCKLING_NL";
#endif
                    scheme = "SINGLE";
                    break;

                case AnalysisType.FormFinding:
                    solution = "SOL_FORM_FINDING";
                    //scheme = "SINGLE"; //GSA 8.7 build 27
                    scheme = "FORM_FIND"; //GSA 8.7 build 45    
                    break;

                case AnalysisType.SoapFilm:
                    solution = "SOL_FORM_FINDING";
                    scheme = "SOAP_FORM";
                    break;

                case AnalysisType.LinearStatic:
                    solution = "SOL_STATIC";
                    scheme = "SINGLE";
                    break;
            }


            if (type == AnalysisType.LinearStatic)
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
                    + ", " + beamGeoStiffness
                    + ", SHELL_GEO_NO"
                    + ", 0.1"                   //first_inc
                    + ", 0.0001"                //min_inc 
                    + ", 0.1"                   //max_inc 
                    + ",CYCLE"
                    + ", 1000000"               //num_cycle 
                    + ", ABS"                   //rel_abs_residual 
                    + ", " + residualForce                     // force_residual 
                    + ", " + residualMoment                     //moment_residual 
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






