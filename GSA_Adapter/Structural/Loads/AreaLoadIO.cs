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
        /// <param name="gsa"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        static public bool AddFaceLoad(ComAuto gsa, BHL.AreaUniformalyDistributedLoad load, double factor)
        {

            string command = "LOAD_2D_FACE";
            string name = load.Name;
            string list = LoadIO.CreateIdListOrGroupName(gsa, load.Objects);
            string caseNo = load.Loadcase.Number.ToString();
            string axis = LoadIO.GetAxis(load);
            string type = "CONS";
            string proj = LoadIO.CheckProjected(load);
            //string dir = "Z";
            //string value = load.Pressure.U.ToString();
            string str;

            //str = command + ",," + list + "," + caseNo + "," + axis + "," + type + "," + proj + "," + dir + "," + value;

            List<string> forceStrings = new List<string>();

            str = command + "," + name + "," + list + "," + caseNo + "," + axis + "," + type + "," + proj;

            LoadIO.AddVectorDataToStringSingle(str, load.Pressure, ref forceStrings, factor, true);

            foreach (string s in forceStrings)
            {
                dynamic commandResult = gsa.GwaCommand(s);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.CommandFailed(command);
                    return false;
                }
            }

            return true;

        }

    }
}
