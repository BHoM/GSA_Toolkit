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
        static public bool AddLoadCases(ComAuto GSA, List<BHL.ICase> cases)
        {
            foreach (BHL.ICase icase in cases)
            {
                BHL.Loadcase loadcase = icase as BHL.Loadcase;
                string caseNo = "0"; // TODO: add loadcase.Number.ToString(); into BHoM
                string title = loadcase.Name; ;
                string type = GSALoadType(loadcase.Nature);

                string str;
                string command = "LOAD_TITLE.1";
                string bridge = "BRIDGE_NO";

                if (type == "SUPERDEAD") type = "DEAD";

                str = command + "," + caseNo + "," + title + "," + type + "," + bridge;

                dynamic commandResult = GSA.GwaCommand(str);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }
            return true;
        }

        public static bool GetOrCreateLoadCaseId(ComAuto gsa, BHL.ICase iCase, out string caseId)
        {
            if (Utils.CheckAndGetGsaId(iCase, out caseId))
                return true;

            int highestIndex = gsa.GwaCommand("HIGHEST, LOAD_TITLE");

            caseId = highestIndex.ToString();

            return AddLoadCase(gsa, iCase, caseId);

        }

        static public bool AddLoadCase(ComAuto GSA, BHL.ICase iCase, string caseNo)
        {
            if (iCase is BHL.ICase)
            {
                BHL.Loadcase loadcase = iCase as BHL.Loadcase;
                //string caseNo = "0"; // TODO: add loadcase.Number.ToString(); into BHoM
                string title = loadcase.Name; ;
                string type = GSALoadType(loadcase.Nature);

                string str;
                string command = "LOAD_TITLE.1";
                string bridge = "BRIDGE_NO";

                if (type == "SUPERDEAD") type = "DEAD";

                str = command + "," + caseNo + "," + title + "," + type + "," + bridge;

                dynamic commandResult = GSA.GwaCommand(str);

                if (1 == (int)commandResult) { }
                else
                {
                    Utils.CommandFailed(command);
                    return false;
                }
                return true;
            }
            return false;

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
    }
}
