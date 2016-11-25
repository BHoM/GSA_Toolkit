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
    class LoadcaseIO
    {

        static public bool AddLoadCases(ComAuto gsa, List<BHL.ICase> cases)
        {
            int higestIndexComb = gsa.GwaCommand("HIGHEST, ANAL")+1;

            List<BHL.ICase> gsaCases;

            GetLoadcases(gsa, out gsaCases);

            foreach (BHL.ICase ca in cases)
            {
                if (ca is BHL.Loadcase)
                {
                    AddLoadCase(gsa, ca as BHL.Loadcase);
                }
                else if (ca is BHL.LoadCombination)
                {
                    string caseNo;
                    if (ca.Number <= 0)
                    {
                        caseNo = higestIndexComb.ToString();
                        higestIndexComb++;
                    }
                    else
                        caseNo = ca.Number.ToString();


                    AnalysisTaskIO.AddLoadCombination(gsa, ca as BHL.LoadCombination, caseNo);
   
                }
            }

            return true;
        }

        //public static bool GetOrCreateLoadCaseId(ComAuto gsa, BHL.ICase iCase, out string caseId)
        //{
        //    if (Utils.CheckAndGetGsaId(iCase, out caseId))
        //        return true;

        //    int highestIndex = gsa.GwaCommand("HIGHEST, LOAD_TITLE");

        //    caseId = highestIndex.ToString();

        //    return AddLoadCase(gsa, iCase as BHL.Loadcase, caseId);

        //}

        //public static bool GetOrCreateLoadCaseId(ComAuto gsa, BHL.ICase iCase, out string caseId, ref int highestCaseNo)
        //{
        //    if (Utils.CheckAndGetGsaId(iCase, out caseId))
        //        return true;

        //    caseId = highestCaseNo.ToString();

        //    if (AddLoadCase(gsa, iCase as BHL.Loadcase, caseId))
        //    {
        //        highestCaseNo++;
        //        return true;
        //    }

        //    return false;

        //}

        static public bool AddLoadCase(ComAuto GSA, BHL.Loadcase loadcase)
        {
            string title = loadcase.Name; ;
            string type = GSALoadType(loadcase.Nature);

            string str;
            string command = "LOAD_TITLE.1";
            string bridge = "BRIDGE_NO";

            if (type == "SUPERDEAD") type = "DEAD";

            str = command + "," + loadcase.Number + "," + title + "," + type + "," + bridge;

            dynamic commandResult = GSA.GwaCommand(str);

            if (1 == (int)commandResult) {

            }
            else
            {
                Utils.CommandFailed(command);
                return false;
            }
            return true;

        }
        


        private static string GSALoadType(BHL.LoadNature loadNature)
        {
            if (loadNature == BHL.LoadNature.Dead)
                return GsaEnums.LoadType.DEAD.ToString();
            if (loadNature == BHL.LoadNature.Live)
                return GsaEnums.LoadType.IMPOSED.ToString();
            if (loadNature == BHL.LoadNature.Other)
                return GsaEnums.LoadType.UNDEF.ToString();
            if (loadNature == BHL.LoadNature.Seismic)
                return GsaEnums.LoadType.SEISMIC.ToString();
            if (loadNature == BHL.LoadNature.Snow)
                return GsaEnums.LoadType.SNOW.ToString();
            if (loadNature == BHL.LoadNature.Temperature)
                return GsaEnums.LoadType.IMPOSED.ToString();
            if (loadNature == BHL.LoadNature.Wind)
                return GsaEnums.LoadType.WIND.ToString();
            return "";
        }


        private static BHL.LoadNature GetNatureFromString(string gsaNature)
        {
            //TODO: Add all loadtypes here

            switch (gsaNature)
            {
                case "LC_PERM_SELF":
                    return BHoM.Structural.Loads.LoadNature.Dead;
                case "LC_VAR_IMP":
                    return BHoM.Structural.Loads.LoadNature.Live;
                case "LC_PERM_SOIL":
                    return BHoM.Structural.Loads.LoadNature.Other;
                case "LC_VAR_WIND":
                    return BHoM.Structural.Loads.LoadNature.Wind;
                case "LC_PRESTRESS":
                    return BHoM.Structural.Loads.LoadNature.Prestress;
                case "LC_VAR_TEMP":
                    return BHoM.Structural.Loads.LoadNature.Temperature;
                case "LC_VAR_SNOW":
                    return BHoM.Structural.Loads.LoadNature.Snow;
                default:
                    return BHoM.Structural.Loads.LoadNature.Other;
                    break;
            }

            

        }

        public static List<string> GetLoadcases(ComAuto gsa, out List<BHL.ICase> cases)
        {
            List<string> gsaCaseStrings = GetGsaLoadCaseStrings(gsa);


            cases = CreateCasesFromGsaStrings(gsaCaseStrings);



            return null;
        }

        private static List<BHL.ICase> CreateCasesFromGsaStrings(List<string> gsaCaseStrings)
        {
            List<BHL.ICase> cases = new List<BHL.ICase>();

            foreach (string str in gsaCaseStrings)
            {
                string[] arr = str.Split(',');

                if (arr.Length < 3)
                    return null;

                BHL.Loadcase ca = new BHL.Loadcase();
                ca.Name = arr[2];
                ca.CustomData[Utils.ID] = arr[1];
                ca.Nature = GetNatureFromString(arr[3]);
                cases.Add(ca);
            }

            return cases;
        }

        /// <summary>
        /// Gets all loadcases in a gsamodel. Returns string on the form |LOAD_TITLE| index | name | type | group | Cathegory | Direction | Include
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all section properties</returns>
        static public List<string> GetGsaLoadCaseStrings(ComAuto gsa)
        {
            List<string> gsaCases = new List<string>();

            int i = 1;
            int chkCount = 0;
            int abortNum = 1000; //Amount of "" rows after which to abort

            while (chkCount < abortNum)
            {
                string gsaProp = gsa.GwaCommand("GET, LOAD_TITLE," + i).ToString();
                chkCount++;
                i++;

                if (gsaProp != "") //This check is to count the number of consecutive null rows and later abort at a certain number
                {
                    gsaCases.Add(gsaProp);
                    chkCount = 0;
                }
            }

            return gsaCases;
        }
    }
}
