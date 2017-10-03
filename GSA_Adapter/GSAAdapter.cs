using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Queries;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;



namespace BH.Adapter.GSA
{
    public partial class GSAAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public static fields                      ****/
        /***************************************************/

        public const string ID = "GSA_id";


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSAAdapter()
        {
            AdapterId = ID;
            gsaCom = new ComAuto();

            Config.SeparateProperties = true;
            Config.MergeWithComparer = true;
            Config.ProcessInMemory = true;
        }

        /***************************************************/

        public GSAAdapter(string filePath) : this()
        {
            short result;
            if (!string.IsNullOrWhiteSpace(filePath))
                result = gsaCom.Open(filePath);
            else
                result = gsaCom.NewFile();
        }


        /***************************************************/
        /**** Private  Methods - Com Interop            ****/
        /***************************************************/

        private bool ComCall(string str)
        {
            dynamic commandResult = gsaCom.GwaCommand(str);

            if (1 == (int)commandResult)
                return true;
            else
            {
                ErrorLog.Add("Failure calling the command: " + str);
                return false;
            }
        }

        /***************************************************/

        private T ReturnComCall<T>(string str)
        {
            dynamic commandResult = gsaCom.GwaCommand(str);

            T returnVar = (T)commandResult;

            if (returnVar != null)
                return returnVar;
            else
            {
                ErrorLog.Add("Failure calling the command: " + str);
                return default(T);
            }
        }

        /***************************************************/

        private void UpdateViews()
        {
            gsaCom.UpdateViews();
        }


        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private ComAuto gsaCom;
    }
}
