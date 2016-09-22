using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHG = BHoM.Global;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;
using BHM = BHoM.Materials;
using GSA_Adapter.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    public static class RigidLinkIO
    {
        public static bool CreateRigidLinks(ComAuto gsa, List<BHE.RigidLink> links, out List<string> ids)
        {
            ids = new List<string>();

            List<string> nodeIds = new List<string>();
            List<BHE.Node> nodes = links.Select(x => x.MasterNode).ToList();
            links.ForEach(x => nodes.AddRange(x.SlaveNodes));
            nodes = nodes.Distinct().ToList();

            NodeIO.CreateNodes(gsa, nodes);

            string propId;

            if (!LinkPropertyIO.CreateLinkProperty(gsa, out propId))
                return false;



            int highestIndex = gsa.GwaCommand("HIGHEST, EL") + 1;
            string command = "EL_LINK";

            foreach (BHE.RigidLink link in links)
            {
                string name = link.Name;
                int group = 0;

                string startIndex = link.MasterNode.CustomData["GSA_id"].ToString();// nodeIds[0];

                string dummy = "";

                if (link.CustomData.ContainsKey("Dummy") && (bool)link.CustomData["Dummy"])
                    dummy = "DUMMY";

                foreach (BHE.Node slave in link.SlaveNodes)
                {
                    string index = highestIndex.ToString();
                    highestIndex++;

                    string endIndex = slave.CustomData["GSA_id"].ToString();// nodeIds[1];

                    string str = command + ", " + index + ", " + propId + ", " + group + ", " + startIndex + ", " + endIndex + ", " + dummy;

                    dynamic commandResult = gsa.GwaCommand(str);

                    if (1 == (int)commandResult)
                    {
                        ids.Add(index);
                        continue;
                    }
                    else
                    {
                        return Utils.CommandFailed(command);
                    }

                }
            }

            gsa.UpdateViews();
            return true;
        }
    }
}
