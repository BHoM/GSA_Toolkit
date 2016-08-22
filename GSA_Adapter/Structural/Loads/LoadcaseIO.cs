using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMG = BHoM.Geometry;
using BHoML = BHoM.Structural.Loads;
using BHoMB = BHoM.Base;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Loads
{
    class LoadcaseIO
    {
        static public bool AddLoadCases(ComAuto GSA, List<BHoML.ICase> cases)
        {
            foreach (BHoML.ICase icase in cases)
            {
                BHoML.Loadcase loadcase = icase as BHoML.Loadcase;
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

        private static string GSALoadType(BHoML.LoadNature loadNature)
        {
            if (loadNature == BHoML.LoadNature.Dead)
                return Utils.GsaEnums.LoadType.DEAD.ToString();
            if (loadNature == BHoML.LoadNature.Live)
                return Utils.GsaEnums.LoadType.IMPOSED.ToString();
            if (loadNature == BHoML.LoadNature.Other)
                return Utils.GsaEnums.LoadType.UNDEF.ToString();
            if (loadNature == BHoML.LoadNature.Seismic)
                return Utils.GsaEnums.LoadType.SEISMIC.ToString();
            if (loadNature == BHoML.LoadNature.Snow)
                return Utils.GsaEnums.LoadType.SNOW.ToString();
            if (loadNature == BHoML.LoadNature.Temperature)
                return Utils.GsaEnums.LoadType.IMPOSED.ToString();
            if (loadNature == BHoML.LoadNature.Wind)
                return Utils.GsaEnums.LoadType.WIND.ToString();
            return "";
        }
    }
}
