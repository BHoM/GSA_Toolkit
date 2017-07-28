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

        public bool ClearModel()
        {
            throw new NotImplementedException();
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

        public bool ScreenCapture(string fileName = null, List<string> cases = null, List<string> viewNames = null)
        {
            List<string> OrigImgNames = new List<string>();
            List<string> NewImgNames = new List<string>();
            
            
            List<int> viewIndecies = new List<int>();
            
            int highView = gsa.HighestView("SGV");
            for (int i = 0; i < highView; i++)
            {
                if (gsa.ViewExist("SGV", i + 1) == 1)
                {
                    string viewName = gsa.ViewName("SGV", i + 1);

                    if (viewNames.Contains(viewName))
                    {
                        viewIndecies.Add(i + 1);
                    }
                }
            }

            foreach (string casename in cases)
            {

                for (int i = 0; i < viewIndecies.Count; i++)
                {
                    gsa.SetViewCaseList("SGV", viewIndecies[i], "A" + casename);
                }
                for (int i = 0; i < viewNames.Count; i++)
                {
                    gsa.SaveViewToFile(viewNames[i], "PNG");
                }

                // Change names of the created images to something more sensible
                
                foreach (string viewname in viewNames)
                {
                    // Get the name of the created image 
                    string origname = fileName.Split('.')[0] + "_" + viewname + "(0).png";

                    // create the new imgpath and name

                    string newname = fileName.Split('.')[0] + "_" + casename + "_" + viewname + ".png";
                   
                    System.IO.File.Move(origname, newname);
                    
                }
            }

            return true;

        }
    
    }
}
