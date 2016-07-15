using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoM.Structural.Loads;
using System.Globalization;

namespace GSAToolkit
{
    class LoadcaseIO
    {
        static public bool AddLoadCases(ComAuto gsa, List<ICase> cases)
        {
            foreach (ICase icase in cases)
            {
                Loadcase loadcase = icase as Loadcase;
                string caseNo = loadcase.Number.ToString();
                string title = loadcase.Name; ;
                string type = GSALoadType(loadcase.Nature);

                string str;
                string command = "LOAD_TITLE.1";
                string bridge = "BRIDGE_NO";

                if (type == "SUPERDEAD") type = "DEAD";

                str = command + "," + caseNo + "," + title + "," + type + "," + bridge;

                dynamic commandResult = gsa.GwaCommand(str);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }
            return true;
        }

        private static string GSALoadType(LoadNature loadNature)
        {
            if (loadNature == LoadNature.Dead)
                return Utils.GsaEnums.LoadType.DEAD.ToString();
            if (loadNature == LoadNature.Live)
                return Utils.GsaEnums.LoadType.IMPOSED.ToString();
            if (loadNature == LoadNature.Other)
                return Utils.GsaEnums.LoadType.UNDEF.ToString();
            if (loadNature == LoadNature.Seismic)
                return Utils.GsaEnums.LoadType.SEISMIC.ToString();
            if (loadNature == LoadNature.Snow)
                return Utils.GsaEnums.LoadType.SNOW.ToString();
            if (loadNature == LoadNature.Temperature)
                return Utils.GsaEnums.LoadType.IMPOSED.ToString();
            if (loadNature == LoadNature.Wind)
                return Utils.GsaEnums.LoadType.WIND.ToString();
            return "";
        }
    }
}
