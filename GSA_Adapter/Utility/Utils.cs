using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interop.gsa_8_7;
using BHB = BHoM.Base;

namespace GSA_Adapter.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        public const string NUM_KEY = "GSA Number";

        static public bool CommandFailed(string command)
        {
            SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
            return false;
        }

        static public void SendErrorMessage(string message)
        {
            MessageBox.Show(message, "Error");
        }


        static public int[] CreateIntSequence(int maxId)
        {
            int[] ids = new int[maxId];

            for (int i = 0; i < maxId; i++)
            {
                ids[i] = i + 1;
            }

            return ids;
        }

        static public double GetUnitFactor(ComAuto gsa, GsaEnums.UnitType unitType)
        {
            string iUnitFactor = gsa.GwaCommand("GET, UNIT_DATA, " + unitType.ToString());

            string[] unitArray = iUnitFactor.Split(',');

            double unitFactor = Convert.ToDouble(unitArray[unitArray.Length - 1].Trim());

            return unitFactor;
        }

        public static double[] GetUnitFactors(ComAuto gsa)
        {
            string iUnitFactor = gsa.GwaCommand("GET_ALL, UNIT_DATA");

            string[] unitStrings = iUnitFactor.Split('\n');

            double[] factors = new double[unitStrings.Length];

            for (int i = 0; i < unitStrings.Length; i++)
            {
                string[] row = unitStrings[i].Split(',');
                double d;
                if (double.TryParse(row[3], out d))
                    factors[i] = d;
            }

            return factors;
        }

        static public string GetSpaceSeparatedString(this IEnumerable<string> set)
        {
            string str = "";

            foreach (string s in set)
            {
                str += s + " ";
            }
            return str;
        }

        public static bool CheckAndGetGsaId(BHB.IBase obj, out string gsaId)
        {
            if (obj.CustomData.ContainsKey("GSA_id"))
            {
                gsaId = obj.CustomData["GSA_id"].ToString();
                return true;
            }
            gsaId = null;
            return false;
        }
    }
}
