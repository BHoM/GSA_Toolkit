using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoME = BHoM.Structural.Elements;
using BHoMP = BHoM.Structural.Properties;
using BHoML = BHoM.Structural.Loads;
using BHoM.Structural.Interface;

namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : BHoM.Structural.Interface.IElementAdapter
    {
        private ComAuto gsa;
        private string settings;

        public string Filename { get; }

        public ObjectSelection Selection
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public GSAAdapter()
        {
            gsa = new ComAuto();
        }

        public GSAAdapter(string filePath)
        {
            gsa = new ComAuto();
            short result;
            if (filePath != "")
                 result = gsa.Open(filePath);
            else
                result = gsa.NewFile();           
        }

        /// <summary>
        /// Gets the bars in the robot model base on the input seleciton
        /// </summary>
        /// <param name="bars">output bar list</param>
        /// <param name="option"></param>
        /// <returns>true is successful</returns>
        public List<string> GetBars(out List<BHoME.Bar> bars, List<string> ids = null)
        {
            Structural.Elements.BarIO.GetBars(gsa, out bars);
            return null; //TODO: Return list of bar ids
        }

        /// <summary>
        /// Sets the bars in GSA
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool CreateBars(List<BHoME.Bar> bars, out List<string> ids)
        {
            Structural.Elements.BarIO.CreateBars(gsa, bars, out ids);
            return true;
        }

        public List<string> GetLoadcases(out List<BHoML.ICase> cases)
        {
            throw new NotImplementedException();
        }

        public List<string> GetLoads(out List<BHoML.ILoad> loads, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNodes(out List<BHoME.Node> nodes, List<string> ids = null)
        {
            //NodeIO.GetNodes(Robot, out nodes, option);
            //return true;
            throw new NotImplementedException();
        }

        public List<string> GetPanels(out List<BHoME.Panel> panels, List<string> ids = null)
        {
            //return PanelIO.GetPanels(Robot, out panels);
            throw new NotImplementedException();
        }


        public List<string> GetLevels(out List<BHoME.Storey> levels, string options = "")
        {
            throw new NotImplementedException();
        }

        public List<string> GetOpenings(out List<BHoME.Opening> opening, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetGrids(out List<BHoME.Grid> grids, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public bool SetNodes(List<BHoME.Node> nodes, out List<string> ids)
        {
            //TODO: returning ids
            ids = new List<string>();
            return Structural.Elements.NodeIO.CreateNodes(gsa, nodes);
        }

        public bool SetBars(List<BHoME.Bar> bars, out List<string> ids)
        {
            return Structural.Elements.BarIO.CreateBars(gsa, bars, out ids);
        }

        public bool SetPanels(List<BHoME.Panel> panels, out List<string> ids)
        {
            return Structural.Elements.FaceIO.CreateFaces(gsa, panels, out ids);
        }

        public bool SetOpenings(List<BHoME.Opening> opening, out List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetLevels(List<BHoME.Storey> stores, out List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetGrids(List<BHoME.Grid> grids, out List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetLoads(List<BHoML.ILoad> loads, List<string> ids = null)
        {
            return Structural.Loads.LoadIO.AddLoads(gsa, loads);
        }

        public bool SetLoadcases(List<BHoML.ICase> cases)
        {
            return Structural.Loads.LoadcaseIO.AddLoadCases(gsa, cases);
        }

        public List<string> GetLevels(out List<BHoME.Storey> levels, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public bool SetLoads(List<BHoML.ILoad> loads)
        {
            return Structural.Loads.LoadIO.AddLoads(gsa, loads);
        }

        bool IElementAdapter.GetLoads(out List<BHoML.ILoad> loads, List<string> ids)
        {
            throw new NotImplementedException();
        }
    }

}
