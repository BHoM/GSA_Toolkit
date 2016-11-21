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
    public partial class GSAAdapter : IElementAdapter
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
        public List<string> GetBars(out List<BHE.Bar> bars, List<string> ids = null)
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
        public bool CreateBars(List<BHE.Bar> bars, out List<string> ids)
        {
            Structural.Elements.BarIO.CreateBars(gsa, bars, out ids);
            return true;
        }

        public List<string> GetLoadcases(out List<BHL.ICase> cases)
        {
            throw new NotImplementedException();
        }

        public bool GetLoads(out List<BHL.ILoad> loads, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNodes(out List<BHE.Node> nodes, List<string> ids = null)
        {
            //NodeIO.GetNodes(Robot, out nodes, option);
            //return true;
            throw new NotImplementedException();
        }

        public List<string> GetPanels(out List<BHE.Panel> panels, List<string> ids = null)
        {
            //return PanelIO.GetPanels(Robot, out panels);
            throw new NotImplementedException();
        }


        public List<string> GetLevels(out List<BHE.Storey> levels, string options = "")
        {
            throw new NotImplementedException();
        }

        public List<string> GetOpenings(out List<BHE.Opening> opening, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetGrids(out List<BHE.Grid> grids, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetRigidLinks(out List<BHE.RigidLink> links, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetGroups(out List<BHB.IGroup> groups, List<string> ids = null)
        {
            throw new NotImplementedException();
        }


        public bool SetNodes(List<BHE.Node> nodes, out List<string> ids)
        {
            //TODO: returning ids

            //Clone nodes
            List<BHE.Node> clones = nodes.Select(x => (BHE.Node)x.ShallowClone()).ToList();
            clones.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            ids = new List<string>();
            return Structural.Elements.NodeIO.CreateNodes(gsa, nodes);

            nodes = clones;
        }

        public bool SetBars(List<BHE.Bar> bars, out List<string> ids)
        {
            return Structural.Elements.BarIO.CreateBars(gsa, bars, out ids);
        }

        public bool SetPanels(List<BHE.Panel> panels, out List<string> ids)
        {
            return Structural.Elements.FaceIO.CreateFaces(gsa, panels, out ids);
        }

        public bool SetOpenings(List<BHE.Opening> opening, out List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetLevels(List<BHE.Storey> stores, out List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetGrids(List<BHE.Grid> grids, out List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool SetRigidLinks(List<BHE.RigidLink> rigidLinks, out List<string> ids)
        {
            return Structural.Elements.RigidLinkIO.CreateRigidLinks(gsa, rigidLinks, out ids);
        }

        public bool SetGroups(List<BHB.IGroup> groups, out List<string> ids)
        {
            return Elements.GroupIO.SetGroups(gsa, groups, out ids);
        }

        public bool SetLoads(List<BHL.ILoad> loads, List<string> ids = null)
        {
            return Structural.Loads.LoadIO.AddLoads(gsa, loads);
        }

        public bool SetLoadcases(List<BHL.ICase> cases)
        {
            return Structural.Loads.LoadcaseIO.AddLoadCases(gsa, cases);
        }

        public List<string> GetLevels(out List<BHE.Storey> levels, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public bool SetLoads(List<BHL.ILoad> loads)
        {
            return Structural.Loads.LoadIO.AddLoads(gsa, loads);
        }

        public List<string> GetFEMeshes(out List<BHE.FEMesh> meshes, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        public bool SetFEMeshes(List<BHE.FEMesh> meshes, out List<string> ids)
        {
            return Structural.Elements.MeshIO.CreateMeshes(gsa, meshes, out ids);
        }
    }

}
