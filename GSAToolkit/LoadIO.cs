using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoM.Structural.Loads;
using BHoM.Structural;

namespace GSAToolkit
{
    public class LoadIO
    {
        static public bool AddLoads(ComAuto gsa, List<ILoad> loads)
        {
            foreach (ILoad load in loads)
            {
                if (load is BarPrestressLoad)
                    AddPreStressLoad(gsa, load);
                if (load is BarGravityLoad)
                    AddGravityLoad(gsa, load);

            }
            return true;

        }
        static public bool AddPreStressLoad(ComAuto gsa, ILoad load)
        {
            BarPrestressLoad psLoad = load as BarPrestressLoad;
            string name = psLoad.Name;
            string list = CreateBarIDList(psLoad.Objects);
            string caseNo = psLoad.Loadcase.Number.ToString();
            double value = psLoad.PrestressValue;

            string command = "LOAD_BEAM_PRE.2";
            string str;

            str = command + ",," + list + "," + caseNo + "," + value * GetUnitFactor(gsa, Utils.GsaEnums.UnitType.FORCE);

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }



        static public bool AddGravityLoad(ComAuto gsa, ILoad load)
        {
            BarGravityLoad gLoad = load as BarGravityLoad;

            string command = "LOAD_GRAVITY.2";
            string name = gLoad.Name;
            string list = CreateBarIDList(gLoad.Objects);
            string caseNo = gLoad.Loadcase.Number.ToString();
            string x = "0.00";
            string y = "0.00";
            string z = "-1.00";
            string str;

            str = command + ",," + list + "," + caseNo + "," + x + "," + y + "," + z;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }

        static public double GetUnitFactor(ComAuto gsa, Utils.GsaEnums.UnitType unitType)
        {
            string iUnitFactor = gsa.GwaCommand("GET, UNIT_DATA, " + unitType.ToString());

            string[] unitArray = iUnitFactor.Split(',');

            double unitFactor = Convert.ToDouble(unitArray[unitArray.Length - 1].Trim());

            return unitFactor;
        }

        public static string CreateBarIDList(List<Bar> bars)
        {
            string str = "";
            foreach (Bar bar in bars)
            {
                str = str + " " + bar.Name;
            }
            return str;
        }
    }
}
