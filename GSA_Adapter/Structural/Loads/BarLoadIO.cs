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
using GSAE = GSA_Adapter.Structural.Elements;

namespace GSA_Adapter.Structural.Loads
{
    public static class BarLoadIO
    {
        public static bool AddBarLoad(ComAuto gsa, BHL.Load<BHE.Bar> load, double loadFactor, double lengthFactor)
        {
            List<string> forceStrings;// = new List<string>();


            List<string> ids;

            if (!GSAE.BarIO.GetOrCreateBars(gsa, load.Objects, out ids)) { return false; }

            string list = ids.GetSpaceSeparatedString();

            string caseNo;

            if (!LoadcaseIO.GetOrCreateLoadCaseId(gsa, load.Loadcase, out caseNo)) { return false; }

            switch (load.LoadType)
            {
                case BHL.LoadType.BarPointLoad:
                    CreateBarPointLoadStrings((BHL.BarPointLoad)load, loadFactor, lengthFactor, list, caseNo, out forceStrings);
                    break;
                case BHL.LoadType.BarUniformLoad:
                    CreateBarUniformLoadStrings((BHL.BarUniformlyDistributedLoad)load, loadFactor, list, caseNo, out forceStrings);
                    break;
                case BHL.LoadType.BarVaryingLoad:
                    BHL.BarVaryingDistributedLoad barLoad = load as BHL.BarVaryingDistributedLoad;
                    if (barLoad.DistanceFromA == 0 && barLoad.DistanceFromB == 0)
                        CreateBarLineLoadStrings(barLoad, loadFactor, list, caseNo, out forceStrings);
                    else
                        CreateBarTriLineLoadStrings(barLoad, loadFactor, lengthFactor, list, caseNo, out forceStrings);
                    break;
                default:
                    LoadIO.LoadNotImplementedWarning(load.LoadType.ToString());
                    return false;
            }

            foreach (string s in forceStrings)
            {
                dynamic commandResult = gsa.GwaCommand(s);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.CommandFailed(load.LoadType.ToString());
                    return false;
                }
            }

            return true;
        }

        private static bool CreateBarLineLoadStrings(BHL.BarVaryingDistributedLoad load, double loadFactor, string list, string caseNo, out List<string> forceStrings)
        {
            string command = "LOAD_BEAM_LINE";
            //LOAD_BEAM_LINE	name	none 	1	GLOBAL	NO	Z	30	40		

            string name = load.Name;

            string str = command + "," + name + "," + list + "," + caseNo + "," + "GLOBAL" + "," + "NO";

            forceStrings = new List<string>();

            AddDualForceVectorsStrings(ref forceStrings, str, load.ForceVectorA, load.ForceVectorB, loadFactor, true);
            AddDualForceVectorsStrings(ref forceStrings, str, load.MomentVectorA, load.MomentVectorB, loadFactor, false);


            return true;
        }

        private static void AddDualForceVectorsStrings(ref List<string> strings, string startString, BHG.Vector vec1, BHG.Vector vec2, double factor, bool translational)
        {
            List<string> loadStrings = GetDualForceVectorsStrings(vec1, vec2, factor, translational);

            foreach (string str in loadStrings)
            {
                strings.Add(startString + str);
            }

        }

        public static List<string> GetDualForceVectorsStrings(BHG.Vector vec1, BHG.Vector vec2, double factor, bool translational)
        {
            List<string> strings = new List<string>();

            if (vec1 != null && vec2 != null)
            {
                string[] dir = LoadIO.Directions(translational);

                if (vec1.X != 0 || vec2.X != 0)
                    strings.Add(dir[0] + "," + (factor * vec1.X).ToString() + "," + (factor * vec2.X).ToString());
                if (vec1.Y != 0 || vec2.Y != 0)
                    strings.Add(dir[1] + "," + (factor * vec1.Y).ToString() + "," + (factor * vec2.Y).ToString());
                if (vec1.Z != 0 || vec2.Z != 0)
                    strings.Add(dir[2] + "," + (factor * vec1.Z).ToString() + "," + (factor * vec2.Z).ToString());
            }

            return strings;
        }

        private static void CreateBarTriLineLoadStrings(BHL.BarVaryingDistributedLoad load, double loadFactor, double lengthFactor, string list, string caseNo, out List<string> forceStrings)
        {
            string command = "LOAD_BEAM_TRILIN";
            //LOAD_BEAM_TRILIN	name	none 	1	GLOBAL	NO	Z	1	30	100.00%	70

            throw new NotImplementedException();
        }

        private static bool  CreateBarUniformLoadStrings(BHL.BarUniformlyDistributedLoad load, double loadFactor, string list, string caseNo, out List<string> forceStrings)
        {
            forceStrings = new List<string>();

            string command = "LOAD_BEAM_UDL";
            //LOAD_BEAM_UDL	name	none 	1	GLOBAL	NO	Z	50			
            string name = load.Name;

            string str = command + "," + name + "," + list + "," + caseNo + "," + "GLOBAL" + "," + "NO";

            LoadIO.AddVectorDataToStringSingle(str, load.ForceVector, ref forceStrings, loadFactor, true);
            LoadIO.AddVectorDataToStringSingle(str, load.MomentVector, ref forceStrings, loadFactor, false);

            return true;
        }

        private static void CreateBarPointLoadStrings(BHL.BarPointLoad load, double loadFactor, double lengthFactor, string list, string caseNo, out List<string> forceStrings)
        {
            string command = "LOAD_BEAM_POINT";

            //LOAD_BEAM_POINT	name	2	1	GLOBAL	NO	Z	3	60		

            throw new NotImplementedException();
        }
    }
}
