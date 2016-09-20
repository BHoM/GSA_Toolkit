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
    public class LoadIO
    {
        static public bool AddLoads(ComAuto gsa, List<BHL.ILoad> loads)
        {
            double[] unitFactors = Utils.GetUnitFactors(gsa);

            foreach (BHL.ILoad load in loads)
            {
                switch (load.LoadType)
                {
                    case BHL.LoadType.Selfweight:
                        AddGravityLoad(gsa, load);
                        break;

                    case BHL.LoadType.PointForce:
                        NodeLodeIO.AddNodalLoad(gsa, (BHL.PointForce)load, unitFactors[(int)GsaEnums.UnitType.FORCE]);
                        break;

                    case BHL.LoadType.PointDisplacement:
                        NodeLodeIO.AddNodalLoad(gsa, (BHL.PointDisplacement)load, unitFactors[(int)GsaEnums.UnitType.DISP]);
                        break;

                    case BHL.LoadType.Pressure:
                        AddPreStressLoad(gsa, load);
                        break;

                    case BHL.LoadType.AreaUniformLoad:
                        AddFaceLoad(gsa, load);
                        break;

                    case BHL.LoadType.PointVelocity:
                        NodeLodeIO.AddNodalLoad(gsa, (BHL.PointVelocity)load, 0/*TODO: IImplement velocityfactor if needed*/);
                        break;
                    case BHL.LoadType.PointAcceleration:
                        NodeLodeIO.AddNodalLoad(gsa, (BHL.PointAcceleration)load, unitFactors[(int)GsaEnums.UnitType.ACCEL]);
                        break;
                    case BHL.LoadType.PointMass:
                    case BHL.LoadType.BarPointLoad:
                    case BHL.LoadType.BarUniformLoad:
                    case BHL.LoadType.BarVaryingLoad:
                    case BHL.LoadType.BarTemperature:
                    case BHL.LoadType.AreaVaryingLoad:
                    case BHL.LoadType.AreaTemperature:
                    case BHL.LoadType.Geometrical:
                        LoadNotImplementedWarning(load.LoadType.ToString());
                        break;
                    default:
                        break;
                }


            }

            gsa.UpdateViews();
            return true;

        }

        public static void LoadNotImplementedWarning(string loadType)
        {
            Utils.SendErrorMessage("Load of type " + loadType + "is not implemented yet.");
        }

        public static void AddVectorDataToStringSingle(string startStr, BHG.Vector vec, ref List<string> strings, double factor, bool translational)
        {
            if (vec != null)
            {
                string[] dir = LoadIO.Directions(translational);

                if (vec.X != 0)
                    strings.Add(startStr + "," + dir[0] + "," + (factor * vec.X).ToString());
                if (vec.Y != 0)
                    strings.Add(startStr + "," + dir[1] + "," + (factor * vec.Y).ToString());
                if (vec.Z != 0)
                    strings.Add(startStr + "," + dir[2] + "," + (factor * vec.Z).ToString());
            }
        }

        public static List<string> GetForceVectorsStrings(BHG.Vector vec, double factor, bool translational)
        {
            List<string> strings = new List<string>();

            if (vec != null)
            {
                string[] dir = LoadIO.Directions(translational);

                if (vec.X != 0)
                    strings.Add(dir[0] + "," + (factor * vec.X).ToString());
                if (vec.Y != 0)
                    strings.Add(dir[1] + "," + (factor * vec.Y).ToString());
                if (vec.Z != 0)
                    strings.Add(dir[2] + "," + (factor * vec.Z).ToString());
            }

            return strings;
        }

        public static string[] Directions(bool translations)
        {
            if (translations)
                return new string[] { "X", "Y", "Z" };
            else
                return new string[] { "XX", "YY", "ZZ" };
        }

        static public bool AddPreStressLoad(ComAuto GSA, BHL.ILoad load)
        {
            BHL.BarPrestressLoad psLoad = load as BHL.BarPrestressLoad;
            string name = psLoad.Name;
            string list = CreateBarIDList(psLoad.Objects);
            string caseNo = "0"; // TODO: add loadcase.Number.ToString(); into BHoM psLoad.Loadcase.Number.ToString();
            double value = psLoad.PrestressValue;

            string command = "LOAD_BEAM_PRE.2";
            string str;

            str = command + ",," + list + "," + caseNo + "," + value * GetUnitFactor(GSA, GsaEnums.UnitType.FORCE);

            dynamic commandResult = GSA.GwaCommand(str);
            GSA.UpdateViews();

            if (1 == (int)commandResult) return true;
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
        }




        static public bool AddGravityLoad(ComAuto GSA, BHL.ILoad load)
        {
            BHL.Load<BHE.Bar> gload = load as BHL.Load<BHE.Bar>;

            string command = "LOAD_GRAVITY.2";
            string name = gload.Name;
            string list = CreateBarIDList(gload.Objects);
            string caseNo = "0"; // TODO: add loadcase.Number.ToString(); into BHoM= gload.Loadcase.Number.ToString();
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
        static public bool AddFaceLoad(ComAuto GSA, BHL.ILoad load)
        {
            BHL.AreaUniformalyDistributedLoad aLoad = load as BHL.AreaUniformalyDistributedLoad;

            string command = "LOAD_2D_FACE";
            string name = aLoad.Name;
            string list = CreatePanelIDList(aLoad.Objects);
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

        static public double GetUnitFactor(ComAuto GSA, GsaEnums.UnitType unitType)
        {
            string iUnitFactor = GSA.GwaCommand("GET, UNIT_DATA, " + unitType.ToString());

            string[] unitArray = iUnitFactor.Split(',');

            double unitFactor = Convert.ToDouble(unitArray[unitArray.Length - 1].Trim());

            return unitFactor;
        }

        public static string CreateBarIDList(List<BHE.Bar> bars)
        {
            string str = "";
            foreach (BHE.Bar bar in bars)
            {
                str = str + " " + bar.Name;
            }
            return str;
        }

        public static string CreatePanelIDList(List<BHE.Panel> panels)
        {
            string str = "";
            foreach (BHE.Panel panel in panels)
            {
                str = str + " " + panel.Name;
            }
            return str;
        }
    }
}
