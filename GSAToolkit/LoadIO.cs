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
        static public bool AddLoads(ComAuto GSA, List<ILoad> loads)
        {
            foreach (ILoad load in loads)
            {
                if (load is BarPrestressLoad)
                    AddPreStressLoad(GSA, load);
                if (load is AreaUniformalyDistributedLoad)
                    AddFaceLoad(GSA, load);
                else
                    AddGravityLoad(GSA, load);

            }
            return true;

        }
        static public bool AddPreStressLoad(ComAuto GSA, ILoad load)
        {
            BarPrestressLoad psLoad = load as BarPrestressLoad;
            string name = psLoad.Name;
            string list = CreateBarIDList(psLoad.Objects);
            string caseNo = psLoad.Loadcase.Number.ToString();
            double value = psLoad.PrestressValue;

            string command = "LOAD_BEAM_PRE.2";
            string str;

            str = command + ",," + list + "," + caseNo + "," + value * GetUnitFactor(GSA, GSAUtils.GsaEnums.UnitType.FORCE);

            dynamic commandResult = GSA.GwaCommand(str);
            GSA.UpdateViews();

            if (1 == (int)commandResult) return true;
            else
            {
                GSAUtils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }



        static public bool AddGravityLoad(ComAuto GSA, ILoad load)
        {
            Load<Bar> gload = load as Load<Bar>;

            string command = "LOAD_GRAVITY.2";
            string name = gload.Name;
            string list = CreateBarIDList(gload.Objects);
            string caseNo = gload.Loadcase.Number.ToString();
            string x = "0.00";
            string y = "0.00";
            string z = "-1.00";
            string str;

            str = command + ",," + list + "," + caseNo + "," + x + "," + y + "," + z;

            dynamic commandResult = GSA.GwaCommand(str);

            if (1 == (int)commandResult) return true;
            else
            {
                GSAUtils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }

        static public bool AddFaceLoad(ComAuto GSA, ILoad load)
        {
            AreaUniformalyDistributedLoad aLoad = load as AreaUniformalyDistributedLoad;

            string command = "LOAD_2D_FACE";
            string name = aLoad.Name;
            string list = CreatePanelIDList(aLoad.Objects);
            string caseNo = aLoad.Loadcase.Number.ToString();
            string axis = "LOCAL";
            string type = "CONS";
            string proj = "NO";
            string dir = "Z";
            string value = aLoad.PressureValue.ToString();
            string str;

            str = command + ",," + list + "," + caseNo + "," + axis + "," + type + "," + proj + "," + dir + "," + value;

            dynamic commandResult = GSA.GwaCommand(str);

            if (1 == (int)commandResult) return true;
            else
            {
                GSAUtils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }

        static public double GetUnitFactor(ComAuto GSA, GSAUtils.GsaEnums.UnitType unitType)
        {
            string iUnitFactor = GSA.GwaCommand("GET, UNIT_DATA, " + unitType.ToString());

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

        public static string CreatePanelIDList(List<Panel> panels)
        {
            string str = "";
            foreach (Panel panel in panels)
            {
                str = str + " " + panel.Name;
            }
            return str;
        }
    }
}
