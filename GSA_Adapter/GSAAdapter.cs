using Interop.gsa_8_7;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSAAdapter()
        {
            m_gsaCom = new ComAuto();

            AdapterId = BH.Engine.GSA.Convert.AdapterID;

            Config.SeparateProperties = true;
            Config.MergeWithComparer = true;
            Config.ProcessInMemory = false;
            Config.CloneBeforePush = true;
        }

        /***************************************************/

        public GSAAdapter(string filePath) : this()
        {
            short result;
            if (!string.IsNullOrWhiteSpace(filePath))
                result = m_gsaCom.Open(filePath);
            else
                result = m_gsaCom.NewFile();
        }


        /***************************************************/
        /**** Private  Methods - Com Interop            ****/
        /***************************************************/

        private bool ComCall(string str)
        {
            dynamic commandResult = m_gsaCom.GwaCommand(str);

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
            dynamic commandResult = m_gsaCom.GwaCommand(str);

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
            m_gsaCom.UpdateViews();
        }


        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private ComAuto m_gsaCom;


        /***************************************************/
    }
}
