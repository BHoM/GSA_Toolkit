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

        protected override IEnumerable<IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            if (typeof(StructuralGlobalResult).IsAssignableFrom(type))
                return ExtractGlobalresults(type, cases);
            else
                return ExtractObjectResults(type, ids, cases, divisions);

        }

        /***************************************************/
        /**** Global Results  Methods                   ****/
        /***************************************************/

        private IEnumerable<IResult> ExtractGlobalresults(Type type, IList cases)
        {
            List<int> caseNumbers = CheckAndGetAnalysisCaseNumbers(cases);

            if (typeof(GlobalReactions).IsAssignableFrom(type))
                return ExtractGlobalReaction(caseNumbers);
            else if (typeof(ModalDynamics).IsAssignableFrom(type))
                return ExtractGlobalDynamics(caseNumbers);
            else
                ErrorLog.Add("Force type " + type.Name + " not suported");


            return new List<IResult>();

        }

        /***************************************************/

        private IEnumerable<IResult> ExtractGlobalReaction(List<int> cases)
        {
            List<GlobalReactions> reactions = new List<GlobalReactions>();

            string id = ""; //TODO: Strategy for ids for full model

            foreach (int loadCase in cases)
            {
                List<string> forceStrings = new List<string>();
                List<string> momentStrings = new List<string>();

                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",REACT"));
                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",SUPPORT"));
                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",SPRING"));
                forceStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",SOIL"));

                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",REACT"));
                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",SUPPORT"));
                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",SPRING"));
                momentStrings.Add(m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",SOIL"));

                //string force = m_gsaCom.GwaCommand("GET, TOTAL_FORCE, " + loadCase + ",REACT");
                //string moment = m_gsaCom.GwaCommand("GET, TOTAL_MOMENT, " + loadCase + ",REACT");
                //reactions.Add(BH.Engine.GSA.Convert.ToBHoMGlobalReactions(id, force, moment));

                reactions.Add(BH.Engine.GSA.Convert.ToBHoMGlobalReactions(id, forceStrings, momentStrings));
            }

            return reactions;
        }

        /***************************************************/

        private IEnumerable<IResult> ExtractGlobalDynamics(List<int> cases)
        {
            List<ModalDynamics> dynamics = new List<ModalDynamics>();

            string id = ""; //TODO: Strategy for ids for full model

            foreach (int loadCase in cases)
            {
                string mode = m_gsaCom.GwaCommand("GET, MODE, " + loadCase);

                if (String.IsNullOrWhiteSpace(mode))
                    continue;
                string frequency = m_gsaCom.GwaCommand("GET, FREQ, " + loadCase);
                string mass = m_gsaCom.GwaCommand("GET, MASS, " + loadCase);
                string damping = m_gsaCom.GwaCommand("GET, MODAL_DAMP, " + loadCase);
                string stiffness = m_gsaCom.GwaCommand("GET, MODAL_STIFF, " + loadCase);
                string effMassTran = m_gsaCom.GwaCommand("GET, EFF_MASS, " + loadCase + ",TRAN");
                string effMassRot = m_gsaCom.GwaCommand("GET, EFF_MASS, " + loadCase + ",ROT");
                dynamics.Add(BH.Engine.GSA.Convert.ToBHoMModalDynamics(id, mode, frequency, mass, stiffness, damping, effMassTran, effMassRot));
            }

            return dynamics;
        }

        /***************************************************/
        /**** Object Results  Methods                   ****/
        /***************************************************/

        private IEnumerable<IResult> ExtractObjectResults(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            List<int> objectIds = CheckAndGetIds(ids, type);
            List<string> loadCases = CheckAndGetAnalysisCases(cases);

            ResHeader header;// = type.ResultHeader();
            ForceConverter converter;
            string axis;
            double unitFactor;
            if (!GetExtractionParameters(type, out header, out converter, out axis, out unitFactor, ref divisions))
                return new List<IResult>();

            List<IResult> results = new List<IResult>();
            int midPoints = divisions == 1 ? divisions : divisions - 2;
            foreach (string loadCase in loadCases)
            {
                if (InitializeLoadextraction(header, loadCase, midPoints, axis))
                {
                    foreach (int id in objectIds)
                    {
                        GsaResults[] gsaResults = GetResults(id, unitFactor);
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
            }

            return results;
            
        }

        /***************************************************/

        private bool GetExtractionParameters(Type type, out ResHeader header, out ForceConverter converter, out string axis, out double unitFactor, ref int divisions)
        {
            double[] unitFactors = GetUnitFactors();
            if (typeof(NodeReaction).IsAssignableFrom(type))
            {
                header = ResHeader.REF_REAC;
                axis = BH.Engine.GSA.Output_Axis.Global();
                converter = BH.Engine.GSA.Convert.ToBHoMReaction;
                divisions = 1;
                unitFactor = unitFactors[(int)BH.Engine.GSA.UnitType.FORCE];
            }
            else if (typeof(NodeDisplacement).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Output_Axis.Global();
                converter = BH.Engine.GSA.Convert.ToBHoMNodeDisplacement;
                header = ResHeader.REF_DISP;
                divisions = 1;
                unitFactor = unitFactors[(int)BH.Engine.GSA.UnitType.LENGTH];
            }
            else if (typeof(BarForce).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.ToBHoMBarForce;
                header = ResHeader.REF_FORCE_EL1D;
                unitFactor = unitFactors[(int)BH.Engine.GSA.UnitType.FORCE];
            }
            else if (typeof(BarDeformation).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.ToBHoMBarDeformation;
                header = ResHeader.REF_DISP_EL1D;
                unitFactor = unitFactors[(int)BH.Engine.GSA.UnitType.LENGTH];
            }
            else if (typeof(BarStress).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.ToBHoMBarStress;
                header = ResHeader.REF_STRESS_EL1D;
                unitFactor = unitFactors[(int)BH.Engine.GSA.UnitType.STRESS];
            }
            else if (typeof(BarStrain).IsAssignableFrom(type))
            {
                axis = BH.Engine.GSA.Output_Axis.Local();
                converter = BH.Engine.GSA.Convert.ToBHoMBarStrain;
                header = ResHeader.REF_STRAIN_EL1D;
                unitFactor = 1;
            }
            else
            {
                axis = null;
                ErrorLog.Add("Force type " + type.Name + " not suported");
                header = ResHeader.REF_ACC;
                converter = null;
                unitFactor = 1;
                return false;
            }
            return true;
        }

        /***************************************************/
        /**** Private  Methods - Result extraction      ****/
        /***************************************************/

        private bool InitializeLoadextraction(ResHeader header, string loadCase, int divisions, string axis)
        {
            int inputFlags = (int)BH.Engine.GSA.Output_Init_Flags.OP_INIT_1D_AUTO_PTS;
            if (m_gsaCom.Output_Init_Arr(inputFlags, axis, loadCase, header, divisions) != 0)
            {
                ErrorLog.Add("Failed to initialize result extraction for loadcase: " + loadCase);
                return false;
            }
            return true;
        }

        private GsaResults[] GetResults(int objectId, double unitFactor)
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

            // Convert to SI
            if (unitFactor != 1)
            {
                foreach (GsaResults r in results)
                    for (int i = 0; i < r.dynaResults.Length; i++)
                        r.dynaResults[i] /= unitFactor;
            }

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
            else if (ids is List<double>)
                return (ids as List<double>).Select(x => (int)Math.Round(x)).ToList();
            else
            {
                List<int> idsOut = new List<int>();
                foreach (object o in ids)
                {
                    int id;
                    object idObj;
                    if (int.TryParse(o.ToString(), out id))
                    {
                        idsOut.Add(id);
                    }
                    else if (o is IBHoMObject && (o as IBHoMObject).CustomData.TryGetValue(AdapterId, out idObj) && int.TryParse(idObj.ToString(), out id))
                        idsOut.Add(id);
                }
                return idsOut;
            }

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
        private List<int> CheckAndGetAnalysisCaseNumbers(IList cases)
        {
            List<int> loadCases;
            if (cases == null || cases.Count == 0)
            {
                loadCases = new List<int>();
                string sResult;
                int maxIndex = m_gsaCom.GwaCommand("HIGHEST, ANAL");

                for (int i = 1; i <= maxIndex; i++)
                {
                    try
                    {
                        sResult = m_gsaCom.GwaCommand("GET, ANAL," + i).ToString();
                        int number;
                        if (!string.IsNullOrWhiteSpace(sResult) && int.TryParse(m_gsaCom.Arg(1, sResult), out number))
                            loadCases.Add(number);
                    }
                    catch
                    {
                        ErrorLog.Add("Analysis task " + i + "could not be found in the model.");
                    }
                }

            }
            else if (cases is List<int>)
                loadCases = cases as List<int>;
            else if (cases is List<string>)
            {
                loadCases = new List<int>();
                foreach (string ac in cases)
                {
                    string descriptionCase = ac;
                    int idCase = System.Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                    loadCases.Add(idCase);
                }
            }
            else
            {
                loadCases = new List<int>();

                foreach (object o in cases)
                {
                    int id;
                    if (int.TryParse(o.ToString(), out id))
                    {
                        loadCases.Add(id);
                    }
                    else if (o is string)
                    {
                        string s = o as string;
                        if (s.StartsWith("A") && int.TryParse(s.TrimStart('A'), out id))
                            loadCases.Add(id);
                    }
                    else if (o is BH.oM.Structural.Loads.LoadCombination)
                    {
                        loadCases.Add((o as oM.Structural.Loads.LoadCombination).Number);
                    }
                }
            }

            return loadCases.Where(x => CheckAnalysisCaseExists(x, "A" + x.ToString()) && CheckAnalysisCaseResultsExists(x, "A"+x)).ToList();
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
                        if(!string.IsNullOrWhiteSpace(sResult))
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
                        if (!string.IsNullOrWhiteSpace(sResult))
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
            {
                loadCases = new List<string>();
                foreach (object o in cases)
                {
                    int idInt;
                    if (int.TryParse(o.ToString(), out idInt))
                    {
                        loadCases.Add("A" + idInt);
                    }
                    else if (o is string)
                    {
                        string s = o as string;
                        if (s.StartsWith("A") || s.StartsWith("C"))
                            loadCases.Add(s);
                    }
                    else if (o is BH.oM.Structural.Loads.LoadCombination)
                    {
                        loadCases.Add("A" + (o as oM.Structural.Loads.LoadCombination).Number);
                    }

                }
                return loadCases;
            }

            return CheckAnalysisCasesExist(loadCases);
        }

        /***************************************************/
        private List<string> CheckAnalysisCasesExist(List<string> cases, bool checkResults = true)
        {
            List<string> checkedCases = new List<string>();
            foreach (string ac in cases)
            {
                string descriptionCase = ac;
                int idCase = System.Convert.ToInt32(Char.IsLetter(ac[0]) ? ac.Trim().Substring(1) : ac.Trim());
                if (CheckAnalysisCaseExists(idCase, ac) && (!checkResults || CheckAnalysisCaseResultsExists(idCase, ac)))
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

            return true;

        }

        /***************************************************/

        private bool CheckAnalysisCaseResultsExists(int caseId, string caseDescription)
        {

            if (m_gsaCom.CaseResultsExist(caseDescription[0].ToString(), caseId, 0) != 1)
            {
                ErrorLog.Add("Error, analysis case " + caseDescription + " has no results.");
                return false;
            }

            return true;

        }

        /***************************************************/
    }
}
