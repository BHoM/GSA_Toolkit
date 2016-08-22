using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMB = BHoM.Base;
using BHoME = BHoM.Structural.Elements;
using BHoMP = BHoM.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    public class NodeIO
    {
        public static bool CreateNodes(ComAuto GSA, List<BHoME.Node> nodes, out List<string> ids)
        {
            ids = new List<string>();

            int highestIndex = GSA.GwaCommand("HIGHEST, NODE") + 1;

            foreach (BHoME.Node n in nodes)
            {
                string cmd;
                string command = "NODE.2";
                string ID = n.Name;

                if (ID == null || ID == "" || ID == "0")
                {
                    ID = (highestIndex + 1).ToString();
                    highestIndex++;
                }

                if (GSA.GwaCommand("EXIST, NODE, " + n.Name) == 0)
                {
                    string restraint = GetRestraintString(n);

                    string str = command + ", " + ID + ", , NO_RGB, " + n.X + " , " + n.Y + " , " + n.Z + ", NO_GRID, " + 0 + ", REST," + restraint + ", STIFF,0,0,0,0,0,0";
                    dynamic commandResult = GSA.GwaCommand(str); //"NODE.2, 1 , , NO_RGB,0 , 2 , 0, NO_GRID,0, REST,0,0,0,0,0,0, STIFF,0,0,0,0,0,0"

                    if (1 == (int)commandResult)
                    {
                        ids.Add(ID);
                        continue;
                    }
                    else
                    {
                        Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                        return false;
                    }
                }
                else
                    ids.Add(ID);
            }
            GSA.UpdateViews();
            return true;
        }      

        public static string GetRestraintString(BHoME.Node node)
        {
            int X = 0;
            int Y = 0;
            int Z = 0;
            int XX = 0;
            int YY = 0;
            int ZZ = 0;

            if (node.IsConstrained)
            {
                X = ((node.Constraint.UX == BHoMP.DOFType.Fixed) ? 1 : 0);
                Y = ((node.Constraint.UY == BHoMP.DOFType.Fixed) ? 1 : 0);
                Z = ((node.Constraint.UZ == BHoMP.DOFType.Fixed) ? 1 : 0);
                XX = ((node.Constraint.RX == BHoMP.DOFType.Fixed) ? 1 : 0);
                YY = ((node.Constraint.RY == BHoMP.DOFType.Fixed) ? 1 : 0);
                ZZ = ((node.Constraint.RZ == BHoMP.DOFType.Fixed) ? 1 : 0);
            }

            return X + "," + Y + "," + Z + "," + XX + "," + YY + "," + ZZ;
        }
    }
}
