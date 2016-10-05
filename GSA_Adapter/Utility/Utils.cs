using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHP = BHoM.Structural.Properties;

namespace GSA_Adapter.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        public const string NUM_KEY = "GSA Number";
        public const string ID = "GSA_id";

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
            if (obj.CustomData.ContainsKey(Utils.ID))
            {
                gsaId = obj.CustomData[Utils.ID].ToString();
                return true;
            }
            gsaId = null;
            return false;
        }

        public static string GetName(BHB.BHoMObject obj)
        {
            return !string.IsNullOrWhiteSpace(obj.Name) ? obj.Name : obj.ToString(); 
        }

        public static Dictionary<Guid, T> CloneObjects<T>(Dictionary<Guid, T> dict) where T : BHB.BHoMObject
        {
            Dictionary<Guid, T> clones = new Dictionary<Guid, T>();

            foreach (KeyValuePair<Guid,T> kvp in dict)
            {
                if (kvp.Value.CustomData.ContainsKey(Utils.ID))
                    clones.Add(kvp.Key, kvp.Value);
                else
                {
                    T obj = (T)kvp.Value.ShallowClone();
                    obj.CustomData = new Dictionary<string, object>(kvp.Value.CustomData);
                    clones.Add(kvp.Key, obj);
                }
            }

            return clones;
        }

        public static Dictionary<Guid, T> CloneSectionProperties<T>(Dictionary<Guid, T> dict) where T:BHB.BHoMObject
        {
            bool isSecProp;

            if (isSecProp =(typeof(T) != typeof(BHP.SectionProperty)) && typeof(T) != typeof(BHP.PanelProperty))
                return null;

            Dictionary<Guid, T> clones = new Dictionary<Guid, T>();

            foreach (KeyValuePair<Guid, T> kvp in dict)
            {
                if (kvp.Value.CustomData.ContainsKey(Utils.ID) &&
                    isSecProp ? (kvp.Value as BHP.SectionProperty).Material.CustomData.ContainsKey(Utils.ID)
                    : (kvp.Value as BHP.PanelProperty).Material.CustomData.ContainsKey(Utils.ID))
                {
                    clones.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    T obj = (T)kvp.Value.ShallowClone();
                    obj.CustomData = new Dictionary<string, object>(kvp.Value.CustomData);
                    clones.Add(kvp.Key, obj);
                }
            }

            return clones;
        }

        public static T TagWithIdAndClone<T>(T objToTag, string id) where T : BHB.BHoMObject
        {
            if (objToTag.CustomData.ContainsKey(Utils.ID))
                return objToTag;

            T clone = (T)objToTag.ShallowClone();
            clone.CustomData = new Dictionary<string, object>(clone.CustomData);
            clone.CustomData.Add(Utils.ID, id);
            return clone;
        }

        public static string CheckDummy(BHB.BHoMObject obj)
        {
            if (obj.CustomData.ContainsKey("Dummy"))
                return "DUMMY";

            return "";
        }


        /// <summary>
        /// Generates a string of all the Id:s for a group. Assumes that the ids have been sorted
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string GeterateIdString(IEnumerable<int> ids)
        {
            string str = "";

            int counter = 0;
            int prev = -10;

            foreach (int i in ids)
            {
                if (i - 1 == prev)
                {
                    counter++;
                }
                else
                {
                    if (counter > 1)
                        str += "to " + prev + " ";
                    else if (counter > 0)
                        str += prev + " ";

                    str += i.ToString() + " ";
                    counter = 0;
                }

                prev = i;
            }

            if (counter > 1)
                str += "to " + prev + " ";
            else if (counter > 0)
                str += prev + " ";

            return str;
        }
    }
}
