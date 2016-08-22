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
        private ComAuto GSA;
        private string Settings;

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
        public List<string> GetBars(out List<BHoME.Bar> bars, List<string> ids = null)
        {
            Structural.Elements.BarIO.GetBars(GSA, out bars);
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
            Structural.Elements.BarIO.CreateBars(GSA, bars, out ids);
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
            return Structural.Elements.NodeIO.CreateNodes(GSA, nodes, out ids);
        }

        public bool SetBars(List<BHoME.Bar> bars, out List<string> ids)
        {
            return Structural.Elements.BarIO.CreateBars(GSA, bars, out ids);
        }

        public bool SetPanels(List<BHoME.Panel> panels, out List<string> ids)
        {
            return Structural.Elements.FaceIO.CreateFaces(GSA, panels, out ids);
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
            return Structural.Loads.LoadIO.AddLoads(GSA, loads);
        }

        public bool SetLoadcases(List<BHoML.ICase> cases)
        {
            return Structural.Loads.LoadcaseIO.AddLoadCases(GSA, cases);
        }

        public List<string> GetLevels(out List<BHoME.Storey> levels, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        bool IElementAdapter.GetLoads(out List<BHoML.ILoad> loads, List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetLoads(List<BHoML.ILoad> loads)
        {
            throw new NotImplementedException();
        }
    }

}
