using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;
using BHL = BHoM.Structural.Loads;
using BHoM.Structural.Interface;

namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : ICommandAdapter
    {
        public bool Analyse(List<string> cases = null)
        {
            short res;

            if (cases == null)
            {
                res = gsa.Analyse();
            }
            else
            {
                res = 0;

                foreach (string c in cases)
                {
                    int num;

                    if(int.TryParse(c, out num))
                        res += gsa.Analyse(num);
                }
            }

            return res == 0;

        }

        public bool ClearResults()
        {
            return gsa.Delete("RESULTS") == 0;
        }

        public bool Close()
        {
            return gsa.Close() == 0;
        }

        public bool Save(string fileName = null)
        {
            if (fileName == null)
                return gsa.Save() == 0;
            else
                return gsa.SaveAs(fileName) == 0;
        }
    }
}
