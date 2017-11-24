using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHG = BHoM.Geometry;
using BHL = BHoM.Structural.Loads;
using BHB = BHoM.Base;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Loads
{
    public class AnalysisTaskIO
    {

        public static bool AddLoadCombination(ComAuto gsa, BHL.LoadCombination comb, string combNo)
        {
            string name = comb.Name;
            string desc = GenerateCombinationString(gsa, comb);

            if (!AddAnalysisCase(gsa, combNo, name, combNo, desc))
                return false;

            string type = GetTaskType(comb);

            if (!AddAnalysisTask(gsa, combNo, name, type, "0", combNo, comb))
                return false;

            return true;
        }

        static public bool AddAnalysisCase(ComAuto gsa, string anal_caseNo, string name, string taskNo, string desc)
        {
            string addCase;
            string command = "ANAL";

            addCase = command
                + "," + anal_caseNo
                + "," + name
                + "," + taskNo
                + "," + desc;

            dynamic commandResult = gsa.GwaCommand(addCase);
            if (1 == (int)commandResult) return true;
            return Utils.CommandFailed(command);
        }

        static public bool AddAnalysisTask(ComAuto gsa, string taskNo, string name, string type, string stage, string anal_caseNo, BHL.LoadCombination comb)
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

                dynamic commandResult = gsa.GwaCommand(addTask);
                if (1 == (int)commandResult) return true;
                return Utils.CommandFailed(command);
            }
            else
            {
                string bemGeoStiff;
                object outObj;
                if (comb.CustomData.TryGetValue("BeamGeoStiff", out outObj) && !(bool)outObj)
                    bemGeoStiff = "BEAM_GEO_NO";
                else
                    bemGeoStiff = "BEAM_GEO_YES";

                addTask = command
                    + "," + taskNo
                    + "," + name
                    + "," + stage
                    + ", GSRELAX"
                    + "," + solution
                    + "," + scheme
                    + ", 1"                     //num_case 
                    + ", " + bemGeoStiff        //Geometric stiffness of beam, lowers the stiffness of beam elements under high compression
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

                dynamic commandResult = gsa.GwaCommand(addTask);
                if (1 == (int)commandResult) return true;
                return Utils.CommandFailed(command);
            }
        }

        private static string GetTaskType(BHL.LoadCombination comb)
        {
            if (comb.CustomData.ContainsKey("Task Type"))
                return comb.CustomData["Task Type"].ToString();

            return "STATIC";
        }

        private static string GenerateCombinationString(ComAuto gsa, BHL.LoadCombination comb)
        {
            if (comb.Loadcases.Count != comb.LoadFactors.Count)
            {
                Utils.SendErrorMessage("The number of Loadfactors and Loadcases must match up");
                return null;
            }
            string str = "";
            for (int i = 0; i < comb.Loadcases.Count; i++)
            {
                str += comb.LoadFactors[i].ToString() +"L"+ ((BHL.Loadcase)comb.Loadcases[i]).Number;

                if (i != comb.Loadcases.Count-1)
                    str += " + ";
            }

            return str;
        }
    }
}
