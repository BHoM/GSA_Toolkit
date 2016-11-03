using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using GSA_Adapter.Utility;
using BHE = BHoM.Structural.Elements;
using BHM = BHoM.Materials;

namespace GSA_Adapter.Structural.DesignMembers
{
    public static class DesignPropertyIO
    {

        public static Dictionary<string, string> CreateSteelGradeDesignProperties(IComAuto gsa, List<BHE.Bar> bars)
        {
            Dictionary<string, string> gradePropRefs = new Dictionary<string, string>();

            List<BHM.Material> materials = bars.Select(x => x.Material).Distinct().ToList();

            int counter = 1;

            foreach (BHM.Material mat in materials)
            {
                if (mat.Type != BHM.MaterialType.Steel)
                    continue;

                if (!gradePropRefs.ContainsKey(mat.Name))
                {
                    CreateDefaultSteelDesignProperty(gsa, mat.Name, counter.ToString());
                    gradePropRefs.Add(mat.Name, counter.ToString());
                    counter++;
                }
                
            }


            return gradePropRefs;
        }

        public static bool CreateDefaultSteelDesignProperty(IComAuto gsa, string steelGrade, string num)
        {
            return CreateSteelDesignProperty(gsa, "Default " + steelGrade + " property", num, steelGrade, "1", "1.2", "CALC", "100%", "100%", "100%");
        }

        public static bool CreateSteelDesignProperty(IComAuto gsa, string name, string num, string steelGrade, string areaRatio, string plastElastRatio, string effLengthSetting, string effLengthMajor, string effLegnthMinor, string effLengthLatTor)
        {
            string command = "STEEL_BEAM_DES";
            string temp = "20";
            string expoType = "EXPO_ALL";
            string expo = "";


            string str = command + "," + num + "," + name + "," + steelGrade + "," + areaRatio + "," + plastElastRatio + "," + effLengthSetting + "," + effLengthMajor + "," + effLegnthMinor + "," + effLengthLatTor + "," + temp + "," + expoType + "," + expo;

            dynamic commandResult = gsa.GwaCommand(str); 

            if (1 == (int)commandResult)
            {
                return true;
            }
            else
            {
                return Utils.CommandFailed(command);
            }
        }
    }
}
