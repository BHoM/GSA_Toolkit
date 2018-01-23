using BH.Engine.GSA;
using BH.oM.Base;
using BH.oM.Common;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Structural.Results;
using Interop.gsa_8_7;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {

        /***************************************************/
        /**** Adaptor  Methods                          ****/
        /***************************************************/

        public delegate IResult ForceConverter(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0);

        protected override IEnumerable<IResult> Extract(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            List<int> objectIds = CheckAndGetIds(ids, type);
            List<string> loadCases = CheckAndGetAnalysisCases(cases);

            ResHeader header;// = type.ResultHeader();
            ForceConverter converter;
            string axis;
            if (!GetExtractionParameters(type, out header, out converter, out axis, ref divisions))
                return new List<IResult>();


            List<IResult> results = new List<IResult>();
            int midPoints = divisions == 1 ? divisions : divisions - 2;
            foreach (string loadCase in loadCases)
            {
                foreach (int id in objectIds)
                {
                    GsaResults[] gsaResults = GetResults(header, loadCase, id, midPoints, axis);
                    if (gsaResults != null && gsaResults.Length == divisions)
                    {
                        foreach (GsaResults gsaRes in gsaResults)
                        {
                            results.Add(converter.Invoke(gsaRes, id.ToString(), loadCase, divisions));
                        }
                    }
                    else
                    {
                        ErrorLog.Add("Different number of results compared to the expected for object with id " + id + ", for loadcase: " + loadCase);
                    }
                }
            }

            return results;
            
        }

        /***************************************************/

        private bool GetExtractionParameters(Type type, out ResHeader header, out ForceConverter converter, out string axis, ref int divisions)
        {
            if (typeof(NodeReaction).IsAssignableFrom(type))
            {
                header = ResHeader.REF_REAC;
                axis = BH.Engine.GSA.Convert.Output_Axis.Global();
                converter = BH.Engine.GSA.Convert.FromGsaReaction;
                divisions = 1;
            }
            else if (typeof(NodeDisplacement).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Convert.Output_Axis.Global();
                converter = BH.Engine.GSA.Convert.FromGsaNodeDisplacement;
                header = ResHeader.REF_DISP;
                divisions = 1;
            }
            else if (typeof(BarForce).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Convert.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.FromGsaBarForce;
                header = ResHeader.REF_FORCE_EL1D;
            }
            else if (typeof(BarDeformation).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Convert.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.FromGsaBarDeformation;
                header = ResHeader.REF_DISP_EL1D;
            }
            else if (typeof(BarStress).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Convert.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.FromGsaBarStress;
                header = ResHeader.REF_STRESS_EL1D;
            }
            else if (typeof(BarStrain).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Convert.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.FromGsaBarStrain;
                header = ResHeader.REF_STRAIN_EL1D;
            }
            else
            {
                axis = null;
                ErrorLog.Add("Force type " + type.Name + " not suported");
                header = ResHeader.REF_ACC;
                converter = null;
                return false;
            }
            return true;
        }

        /***************************************************/
        /**** Private  Methods - Result extraction      ****/
        /***************************************************/

        private GsaResults[] GetResults(ResHeader header, string loadCase, int objectId, int divisions, string axis, double unitFactor = 1)
        {
            int inputFlags = (int)BH.Engine.GSA.Convert.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            if (m_gsaCom.Output_Init_Arr(inputFlags, axis, loadCase, header, divisions) != 0)
            {
                ErrorLog.Add("Failed to initialize result extraction for object with id: " + objectId + ", for loadcase: " + loadCase);
                return null;
            }

            GsaResults[] results;
            int numOfComponents;
            try
            {
                m_gsaCom.Output_Extract_Arr(objectId, out results, out numOfComponents);
            }
            catch
            {
                ErrorLog.Add("Failed to extract results for item " + objectId);
                return null;
            }

            // Convert to SI
            foreach (GsaResults r in results)
                for (int i = 0; i < r.dynaResults.Length; i++)
                    r.dynaResults[i] /= unitFactor;

            return results;
        }

       

        /***************************************************/
        /**** Private  Methods - Index checking         ****/
        /***************************************************/

        private List<int> CheckAndGetIds(IList ids, Type type)
        {
            if (ids == null || ids.Count == 0)
            {
                if (typeof(BarResult).IsAssignableFrom(type))
                    return GetAllBarIds();
                else if (typeof(NodeResult).IsAssignableFrom(type))
                    return GetAllNodeIds();

            }
            else if (ids is List<string>)
                return (ids as List<string>).Select(x => int.Parse(x)).ToList();
            else if (ids is List<int>)
                return ids as List<int>;        

            return new List<int>();
        }

        /***************************************************/

        private List<int> GetAllBarIds()
        {
            List<int> ids = new List<int>();
            int maxIndex = m_gsaCom.GwaCommand("HIGHEST, EL");
            GsaElement[] gsaElems;
            m_gsaCom.Elements(CreateIntSequence(maxIndex), out gsaElems);

            foreach (GsaElement elem in gsaElems)
            {
                int gsaType = elem.eType;

                //Check that the element type is a bar
                if (gsaType == 1 || gsaType == 2 || gsaType == 20 || gsaType == 21)
                    ids.Add(elem.Ref);

            }

            return ids;
        }

        /***************************************************/
        private List<int> GetAllNodeIds()
        {

            int highestIndex = m_gsaCom.GwaCommand("HIGHEST, NODE");

            GsaNode[] nodes;
            m_gsaCom.Nodes(CreateIntSequence(highestIndex), out nodes);

            return nodes.Select(x => x.Ref).ToList();

        }

        /***************************************************/
        static public int[] CreateIntSequence(int maxId)
        {
            int[] ids = new int[maxId];

            for (int i = 0; i < maxId; i++)
            {
                ids[i] = i+1;
            }

            return ids;
        }

        /***************************************************/

        private List<string> CheckAndGetAnalysisCases(IList cases)
        {
            List<string> loadCases;
            if (cases == null || cases.Count == 0)
            {
                loadCases = new List<string>();
                string sResult;
                int maxIndex = m_gsaCom.GwaCommand("HIGHEST, ANAL");
                int maxCombIndex = m_gsaCom.GwaCommand("HIGHEST, COMBINATION");

                for (int i = 1; i <= maxIndex; i++)
                {
                    try
                    {
                        sResult = m_gsaCom.GwaCommand("GET, ANAL," + i).ToString();
                        loadCases.Add("A" + m_gsaCom.Arg(1, sResult));
                    }
                    catch
                    {
                        ErrorLog.Add("Analysis task " + i + "could not be found in the model.");
                    }
                }

                for (int i = 1; i <= maxCombIndex; i++)
                {
                    try
                    {
                        sResult = m_gsaCom.GwaCommand("GET, COMBINATION," + i).ToString();
                        loadCases.Add("C" + m_gsaCom.Arg(1, sResult));
                    }
                    catch
                    {
                        ErrorLog.Add("Analysis task " + i + "could not be found in the model.");
                    }
                }
            }
            else if (cases is List<string>)
                loadCases = cases as List<string>;
            else
                return new List<string>();

            return CheckAnalysisCasesExist(loadCases);
        }

        /***************************************************/
        private List<string> CheckAnalysisCasesExist(List<string> cases)
        {
            List<string> checkedCases = new List<string>();
            foreach (string ac in cases)
            {
                string descriptionCase = ac;
                int idCase = System.Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                if (CheckAnalysisCaseExists(idCase, ac))
                {
                    checkedCases.Add(ac);
                }
            }
            return checkedCases;
        }

        /***************************************************/
        private bool CheckAnalysisCaseExists(int caseId, string caseDescription)
        {

            if (m_gsaCom.CaseExist(caseDescription[0].ToString(), caseId) != 1)
            {
                ErrorLog.Add("Error, analysis case " + caseDescription + " does not exist.");
                return false;
            }

            if (m_gsaCom.CaseResultsExist(caseDescription[0].ToString(), caseId, 0) != 1)
            {
                ErrorLog.Add("Error, analysis case " + caseDescription + " has no results.");
                return false;
            }

            return true;

        }

        /***************************************************/
        /**** Methods to be deleted                     ****/
        /***************************************************/

        private Dictionary<int, List<List<double>>> GetResults(string loadCase, ResHeader resultType, List<int> nodeIndices, int divisions)
        {
            int inputFlags = (int)BH.Engine.GSA.Convert.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            string axis = BH.Engine.GSA.Convert.Output_Axis.Global();

            if (m_gsaCom.Output_Init_Arr(inputFlags, axis, loadCase, resultType, divisions) != 0)
                return null;

            Dictionary<int, List<List<double>>> nodalResults = new Dictionary<int, List<List<double>>>();

            for (int i = 0; i < nodeIndices.Count; i++)
            {
                List<List<double>> nodeRes = GetResult(nodeIndices[i]);
                if (nodeRes != null)
                    nodalResults.Add(nodeIndices[i], nodeRes);
            }

            return nodalResults;
        }

        /***************************************************/

        private List<List<double>> GetResult(int objectId, double unitFactor = 1)
        {
            GsaResults[] results;
            int numOfComponents;
            try
            {
                m_gsaCom.Output_Extract_Arr(objectId, out results, out numOfComponents);
            }
            catch
            {
                ErrorLog.Add("Failed to extract results for item " + objectId);
                return null;
            }
            List<List<double>> resList = new List<List<double>>();

            foreach (GsaResults r in results)
            {
                List<double> innerList = new List<double>();
                for (int i = 0; i < r.dynaResults.Length; i++)
                    innerList.Add(r.dynaResults[i] / unitFactor);

                resList.Add(innerList);
            }

            return resList;

        }

        private bool ExtractBeamResults(int bId, string caseDescription, ResHeader header, double unitFactor, int divisions, string sAxis, out GsaResults[] GSAresults)
        {
            int inputFlags = (int)BH.Engine.GSA.Convert.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            //int inputFlags = 0x40;
            int nComp = 0;

            // Get unit factor for extracted results.

            if (m_gsaCom.Output_Init_Arr(inputFlags, sAxis, caseDescription, header, divisions - 2) != 0)
            {
                ErrorLog.Add("Initialisation failed");
                GSAresults = null;
                return false;
            }

            try
            {
                if (m_gsaCom.Output_Extract_Arr(bId, out GSAresults, out nComp) != 0)
                {
                    ErrorLog.Add("Extraction failed");
                    return false;
                }
            }

            catch (Exception e)
            {
                ErrorLog.Add(e.Message);
                ErrorLog.Add("Extraction failed on element " + bId);

                GSAresults = new GsaResults[0];

                return false;
            }

            // Convert to SI
            foreach (GsaResults r in GSAresults)
                for (int i = 0; i < r.dynaResults.Length; i++)
                    r.dynaResults[i] /= unitFactor;

            return true;
        }


        /***************************************************/

        //public List<NodeReaction> GetNodeReacions(List<int> nodeIds, List<string> cases)
        //{
        //    ResHeader resHeader = ResHeader.REF_REAC;
        //    List<NodeReaction> nodeReactions = new List<NodeReaction>();
        //    for (int i = 0; i < cases.Count; i++)
        //    {
        //        Dictionary<int, List<double>> res = GetResults(cases[i], resHeader, nodeIds, 0);

        //        if (res != null)
        //        {
        //            foreach (KeyValuePair<int, List<double>> kvp in res)
        //            {
        //                List<double> nodeRes = kvp.Value;
        //                NodeReaction reac = new NodeReaction
        //                {
        //                    ObjectId = kvp.Key.ToString(),
        //                    Case = cases[i],
        //                    FX = nodeRes[0],
        //                    FY = nodeRes[1],
        //                    FZ = nodeRes[2],
        //                    MX = nodeRes[4],
        //                    MY = nodeRes[5],
        //                    MZ = nodeRes[6]
        //                };
        //                nodeReactions.Add(reac);
        //            }
        //        }

        //    }

        //    return nodeReactions;
        //}

        /***************************************************/

        //public List<NodeDisplacement> GetNodeDisplacements(List<int> nodeIds, List<string> cases)
        //{
        //    ResHeader resHeader = ResHeader.REF_DISP;
        //    List<NodeDisplacement> nodeReactions = new List<NodeDisplacement>();
        //    for (int i = 0; i < cases.Count; i++)
        //    {
        //        Dictionary<int, List<double>> res = GetResults(cases[i], resHeader, nodeIds, 0);

        //        if (res != null)
        //        {
        //            foreach (KeyValuePair<int, List<double>> kvp in res)
        //            {
        //                List<double> nodeRes = kvp.Value;
        //                NodeDisplacement reac = new NodeDisplacement
        //                {
        //                    ObjectId = kvp.Key.ToString(),
        //                    Case = cases[i],
        //                    UX = nodeRes[0],
        //                    UY = nodeRes[1],
        //                    UZ = nodeRes[2],
        //                    RX = nodeRes[4],
        //                    RY = nodeRes[5],
        //                    RZ = nodeRes[6]
        //                };
        //                nodeReactions.Add(reac);
        //            }
        //        }

        //    }

        //    return nodeReactions;
        //}
    }
}
