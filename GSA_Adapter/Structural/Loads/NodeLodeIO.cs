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
    public static class NodeLodeIO
    {

        public static bool AddNodalLoad(ComAuto gsa, BHL.Load<BHE.Node> load, double factor)
        {
            BHG.Vector trans, rot;
            string command;
            List<string> forceStrings = new List<string>();

            switch (load.LoadType)
            {
                case BHL.LoadType.PointForce:
                    trans = ((BHL.PointForce)load).Force;
                    rot = ((BHL.PointForce)load).Moment;
                    command = "LOAD_NODE";
                    break;
                case BHL.LoadType.PointDisplacement:
                    trans = ((BHL.PointDisplacement)load).Translation;
                    rot = ((BHL.PointDisplacement)load).Rotation;
                    command = "DISP_NODE";
                    break;
                case BHL.LoadType.PointVelocity:
                case BHL.LoadType.PointAcceleration:
                case BHL.LoadType.PointMass:
                default:
                    LoadIO.LoadNotImplementedWarning(load.LoadType.ToString());
                    return false;
            }

            
            string name = load.Name;
            string str;

            string appliedTo;

            if (string.IsNullOrWhiteSpace(load.Objects.Name))
            {
                List<string> ids;
                if (!GSAE.NodeIO.GetOrCreateNodes(gsa, load.Objects, out ids)) { return false; }
                appliedTo = ids.GetSpaceSeparatedString();
            }
            else
            {
                appliedTo = "\"" + load.Objects.Name + "\"";
            }



            string caseNo;

            if(!LoadcaseIO.GetOrCreateLoadCaseId(gsa, load.Loadcase, out caseNo)) { return false; }

            str = command + "," + name + "," + appliedTo + "," + caseNo + "," + "GLOBAL";

            LoadIO.AddVectorDataToStringSingle(str, trans, ref forceStrings, factor, true);
            LoadIO.AddVectorDataToStringSingle(str, rot, ref forceStrings, factor, false);

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
