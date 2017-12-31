using System.Collections.Generic;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public override bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            return ComCall(command);
        }


        /***************************************************/
    }
}
