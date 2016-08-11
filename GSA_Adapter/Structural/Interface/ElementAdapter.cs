using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoME = BHoM.Structural.Elements;
using BHoMP = BHoM.Structural.Properties;
using BHoML = BHoM.Structural.Loads;


namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : BHoM.Structural.Interface.IElementAdapter
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
        public bool GetBars(out List<BHoME.Bar> bars, string option = "all")
        {
            Structural.Elements.BarIO.GetBars(GSA, out bars);
            return true;
        }

        /// <summary>
        /// Sets the bars in GSA
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool CreateBars(List<BHoME.Bar> bars, out List<string> ids)
        {
            Structural.Elements.BarIO.CreateBars(GSA, bars, out ids);
            return true;
        }

        public bool GetLoadcases(out List<BHoML.ICase> cases)
        {
            throw new NotImplementedException();
        }

        public bool GetLoads(out List<BHoML.ILoad> loads, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool GetNodes(out List<BHoME.Node> nodes, string option = "all")
        {
            //NodeIO.GetNodes(Robot, out nodes, option);
            //return true;
            throw new NotImplementedException();
        }

        public bool GetPanels(out List<BHoME.Panel> panels, string option = "all")
        {
            //return PanelIO.GetPanels(Robot, out panels);
            throw new NotImplementedException();
        }


        public bool GetLevels(out List<BHoME.Storey> levels, string options = "")
        {
            throw new NotImplementedException();
        }

        public bool GetOpenings(out List<BHoME.Opening> opening, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool GetGrids(out List<BHoME.Grid> grids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetNodes(List<BHoME.Node> nodes, out List<string> ids, string option = "")
        {
            return Structural.Elements.NodeIO.CreateNodes(GSA, nodes, out ids);
        }

        public bool SetBars(List<BHoME.Bar> bars, out List<string> ids, string option = "")
        {
            return Structural.Elements.BarIO.CreateBars(GSA, bars, out ids);
        }

        public bool SetPanels(List<BHoME.Panel> panels, out List<string> ids, string option = "")
        {
            return Structural.Elements.FaceIO.CreateFaces(GSA, panels, out ids);
        }

        public bool SetOpenings(List<BHoME.Opening> opening, out List<string> ids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetLevels(List<BHoME.Storey> stores, out List<string> ids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetGrids(List<BHoME.Grid> grids, out List<string> ids, string option = "")
        {
            throw new NotImplementedException();
        }

        public bool SetLoads(List<BHoML.ILoad> loads, string option = "")
        {
            return Structural.Loads.LoadIO.AddLoads(GSA, loads);
        }

        public bool SetLoadcases(List<BHoML.ICase> cases)
        {
            return Structural.Loads.LoadcaseIO.AddLoadCases(GSA, cases);
        }
    }

}
