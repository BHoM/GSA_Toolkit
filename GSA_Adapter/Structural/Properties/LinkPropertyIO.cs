using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Properties
{
    public static class LinkPropertyIO
    {

        public static bool CreateLinkProperty(ComAuto gsa, out string id, string restraint = "ALL")
        {
            //Hardcoded linkproperty to use for rigid links.
            //Could be expanded upon if we find a need

            string command = "PROP_LINK";
            id = "1";
            string str = command + ", " + id + ", " + "Link property 1, NO_RGB, " + restraint;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
            return true;
        }
    }
}
