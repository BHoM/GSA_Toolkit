using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {

        public bool ComCall(string str)
        {
            dynamic commandResult = m_gsa.GwaCommand(str); 

            if (1 == (int)commandResult)
            {
                return true;
            }
            else
            {
                ErrorLog.Add("Failure calling the command: " + str);
                return false;
            }
        }
        public T ReturnComCall<T>(string str)
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

        public void UpdateViews()
        {
            m_gsa.UpdateViews();
        }

    }
}
