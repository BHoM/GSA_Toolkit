using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHG = BHoM.Geometry;
using BHE = BHoM.Structural.Elements;
using BHL = BHoM.Structural.Loads;
using BHB = BHoM.Base;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Loads
{
    public static class AreaLoadIO
    {
        /// <summary>
        /// ASSUMES Z DIRECTION CURRENTLY
        /// </summary>
        /// <param name="GSA"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        static public bool AddFaceLoad(ComAuto GSA, BHL.ILoad load)
        {
            BHL.AreaUniformalyDistributedLoad aLoad = load as BHL.AreaUniformalyDistributedLoad;

            string command = "LOAD_2D_FACE";
            string name = aLoad.Name;
            string list = "";//CreatePanelIDList(aLoad.Objects);
            string caseNo = "0"; // TODO: add loadcase.Number.ToString(); into BHoM = aLoad.Loadcase.Number.ToString();
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

    }
}
