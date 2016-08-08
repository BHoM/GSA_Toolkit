using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Structural;
using BHoM.Structural.Loads;
using BHoM.Materials;
using BHoM.Structural.SectionProperties;
using Interop.gsa_8_7;

namespace GSAToolkit
{
    public partial class GSAAdapter : IStructuralAdapter
    {
        private ComAuto GSA;
        private string Settings;

        public string Filename { get; }

        public GSAAdapter()
        {
            GSA = new ComAuto();
        }

        public GSAAdapter(string filePath)
        {
            GSA = new ComAuto();
            short result;
            if (filePath != "")
                 result = GSA.Open(filePath);
            else
                result = GSA.NewFile();           
        }

        /// <summary>
        /// Gets the bars in the robot model base on the input seleciton
        /// </summary>
        /// <param name="bars">output bar list</param>
        /// <param name="option"></param>
        /// <returns>true is successful</returns>
        public bool GetBars(out List<Bar> bars, string option = "all")
        {
            BarIO.GetBars(GSA, out bars);
            return true;
        }

        /// <summary>
        /// Sets the bars in GSA
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool CreateBars(List<Bar> bars, out List<string> ids)
        {
            BarIO.CreateBars(GSA, bars, out ids);
            return true;
        }

        public bool GetLoadcases(out List<ICase> cases)
        {
            throw new NotImplementedException();
        }

        public bool GetLoads(out List<ILoad> loads, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool GetNodes(out List<Node> nodes, string option = "all")
        {
            //NodeIO.GetNodes(Robot, out nodes, option);
            //return true;
            throw new NotImplementedException();
        }

        public bool GetPanels(out List<BHoM.Structural.Panel> panels, string option = "all")
        {
            //return PanelIO.GetPanels(Robot, out panels);
            throw new NotImplementedException();
        }


        public bool GetLevels(out List<Storey> levels, string options = "")
        {
            throw new NotImplementedException();
        }

        public bool GetOpenings(out List<Opening> opening, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool GetGrids(out List<Grid> grids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetNodes(List<Node> nodes, out List<string> ids, string option = "")
        {
            return NodeIO.CreateNodes(GSA, nodes, out ids);
        }

        public bool SetBars(List<Bar> bars, out List<string> ids, string option = "")
        {
            return BarIO.CreateBars(GSA, bars, out ids);
        }

        public bool SetPanels(List<Panel> panels, out List<string> ids, string option = "")
        {
            return PanelIO.CreatePanels(GSA, panels, out ids);
        }

        public bool SetOpenings(List<Opening> opening, out List<string> ids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetLevels(List<Storey> stores, out List<string> ids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetGrids(List<Grid> grids, out List<string> ids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetLoads(List<ILoad> loads, string option = "")
        {
            return LoadIO.AddLoads(GSA, loads);
        }

        public bool SetLoadcases(List<ICase> cases)
        {
            return LoadcaseIO.AddLoadCases(GSA, cases);
        }
    }

}
