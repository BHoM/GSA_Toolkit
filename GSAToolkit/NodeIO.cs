using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoM.Structural;
using BHoM.Global;

namespace GSAToolkit
{
    public class NodeIO
    {
        public static bool CreateNodes(ComAuto GSA, List<Node> nodes, out List<string> ids)
        {
            ids = new List<string>();

            foreach (Node n in nodes)
            {
                string cmd;
                int ID = int.Parse(n.Name);
                string restraint = GetRestraintString(n);
                ids.Add(n.Name.ToString());
                string command = "NODE.2";

                cmd = command + ", " + ID + ", , NO_RGB, " + n.X + " , " + n.Y + " , " + n.Z + ", NO_GRID, " + 0 + ", REST," + restraint + ", STIFF,0,0,0,0,0,0";
                //cmd = "NODE.2, 1 , , NO_RGB,0 , 2 , 0, NO_GRID,0, REST,0,0,0,0,0,0, STIFF,0,0,0,0,0,0";
                dynamic commandResult = GSA.GwaCommand(cmd);

                if (1 == (int)commandResult) continue;
                else
                {
                    Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }
            GSA.UpdateViews();
            return true;
        }      

        public static string GetRestraintString(Node node)
        {
            int X = 0;
            int Y = 0;
            int Z = 0;
            int XX = 0;
            int YY = 0;
            int ZZ = 0;

            if (node.IsConstrained)
            {
                X = ((node.Constraint.UX.Type == DOFType.Fixed) ? 1 : 0);
                Y = ((node.Constraint.UY.Type == DOFType.Fixed) ? 1 : 0);
                Z = ((node.Constraint.UZ.Type == DOFType.Fixed) ? 1 : 0);
                XX = ((node.Constraint.RX.Type == DOFType.Fixed) ? 1 : 0);
                YY = ((node.Constraint.RY.Type == DOFType.Fixed) ? 1 : 0);
                ZZ = ((node.Constraint.RZ.Type == DOFType.Fixed) ? 1 : 0);
            }

            return X + "," + Y + "," + Z + "," + XX + "," + YY + "," + ZZ;
        }
    }
}
