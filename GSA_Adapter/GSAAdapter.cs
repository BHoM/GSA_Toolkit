using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Structural;
using BH.Adapter.Queries;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;



namespace BH.Adapter.GSA
{
    public partial class GSAAdapter : BHoMAdapter//IStructuralAdapter, INodeAdapter, IBarAdapter
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

            m_gsa = new ComAuto();
        }

        /***************************************************/

        public GSAAdapter(string filePath) : this()
        {
            short result;
            if (!string.IsNullOrWhiteSpace(filePath))
                result = m_gsa.Open(filePath);
            else
                result = m_gsa.NewFile();
        }


        /***************************************************/
        /**** Private  Methods - Com Interop            ****/
        /***************************************************/

        private bool ComCall(string str)
        {
            dynamic commandResult = m_gsa.GwaCommand(str);

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
            dynamic commandResult = m_gsa.GwaCommand(str);

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
            m_gsa.UpdateViews();
        }


        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private ComAuto m_gsa;
    }
}
