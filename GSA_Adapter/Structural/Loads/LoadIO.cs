using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMG = BHoM.Geometry;
using BHoME = BHoM.Structural.Elements;
using BHoML = BHoM.Structural.Loads;
using BHoMB = BHoM.Base;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Loads
{
    public class LoadIO
    {
        static public bool AddLoads(ComAuto GSA, List<BHoML.ILoad> loads)
        {
            foreach (BHoML.ILoad load in loads)
            {
                if (load is BHoML.BarPrestressLoad)
                    AddPreStressLoad(GSA, load);
                if (load is BHoML.AreaUniformalyDistributedLoad)
                    AddFaceLoad(GSA, load);
                else
                    AddGravityLoad(GSA, load);

            }
            return true;

        }
        static public bool AddPreStressLoad(ComAuto GSA, BHoML.ILoad load)
        {
            BHoML.BarPrestressLoad psLoad = load as BHoML.BarPrestressLoad;
            string name = psLoad.Name;
            string list = CreateBarIDList(psLoad.Objects);
            string caseNo = psLoad.Loadcase.Number.ToString();
            double value = psLoad.PrestressValue;

            string command = "LOAD_BEAM_PRE.2";
            string str;

            str = command + ",," + list + "," + caseNo + "," + value * GetUnitFactor(GSA, Utils.GsaEnums.UnitType.FORCE);

            dynamic commandResult = GSA.GwaCommand(str);
            GSA.UpdateViews();

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }



        static public bool AddGravityLoad(ComAuto GSA, BHoML.ILoad load)
        {
            BHoML.Load<BHoME.Bar> gload = load as BHoML.Load<BHoME.Bar>;

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
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }

        /// <summary>
        /// ASSUMES Z DIRECTION CURRENTLY
        /// </summary>
        /// <param name="GSA"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        static public bool AddFaceLoad(ComAuto GSA, BHoML.ILoad load)
        {
            BHoML.AreaUniformalyDistributedLoad aLoad = load as BHoML.AreaUniformalyDistributedLoad;

            string command = "LOAD_2D_FACE";
            string name = aLoad.Name;
            string list = CreatePanelIDList(aLoad.Objects);
            string caseNo = aLoad.Loadcase.Number.ToString();
            string axis = "LOCAL";
            string type = "CONS";
            string proj = "NO";
            string dir = "Z";
            string value = aLoad.Pressure.U.ToString();
            string str;

            str = command + ",," + list + "," + caseNo + "," + axis + "," + type + "," + proj + "," + dir + "," + value;

            dynamic commandResult = GSA.GwaCommand(str);

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }

        static public double GetUnitFactor(ComAuto GSA, Utils.GsaEnums.UnitType unitType)
        {
            string iUnitFactor = GSA.GwaCommand("GET, UNIT_DATA, " + unitType.ToString());

            string[] unitArray = iUnitFactor.Split(',');

            double unitFactor = Convert.ToDouble(unitArray[unitArray.Length - 1].Trim());

            return unitFactor;
        }

        public static string CreateBarIDList(List<BHoME.Bar> bars)
        {
            string str = "";
            foreach (BHoME.Bar bar in bars)
            {
                str = str + " " + bar.Name;
            }
            return str;
        }

        public static string CreatePanelIDList(List<BHoME.Panel> panels)
        {
            string str = "";
            foreach (BHoME.Panel panel in panels)
            {
                str = str + " " + panel.Name;
            }
            return str;
        }
    }
}
