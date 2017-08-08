using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMB = BHoM.Base;
using BHoMG = BHoM.Geometry;
using BHoME = BHoM.Structural.Elements;
using BHoMP = BHoM.Structural.Properties;
using BHoMM = BHoM.Materials;
using GSA_Adapter.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    /// <summary>
    /// GSA panel class, for all panel objects and operations
    /// </summary>
    public class FaceIO
    {
        /// <summary>
        /// Create GSA Panels
        /// </summary>
        /// <returns></returns>
        public static bool CreateFaces(ComAuto GSA, List<BHoME.Panel> panels, out List<string> ids)
        {
            ids = new List<string>();

            List<string> props = PropertyIO.Get2DPropertyStringList(GSA);
            int highestIndex = GSA.GwaCommand("HIGHEST, EL") + 1;

            foreach (BHoME.Panel panel in panels)
            {
                panel.MeshAsSingleFace();
                BHoME.Face face = panel.GetMeshFaces[0];
                string command = "";
                if (face.Nodes.Count == 4)
                    command = "EL_QUAD4";
                if (face.Nodes.Count == 3)
                    command = "EL_TRI3";

                string index = face.Name;
                if (index == null || index == "" || index == "0")
                {
                    index = highestIndex.ToString();
                    highestIndex++;
                }

                string propertyIndex = PropertyIO.GetOrCreate2DPropertyIndex(GSA, panel, props);
                string group = "0";
                string indexString = GetPanelNodeIndexString(GSA, face);

                string str = command + "," + index + "," + propertyIndex + "," + group + "," + indexString + "0,REAL";
                dynamic commandResult = GSA.GwaCommand(str);

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    continue;
                }
                else
                {
                    Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }

            GSA.UpdateViews();
            return true;
        }

        public static string GetPanelNodeIndexString(ComAuto GSA, BHoME.Face face)
        {
            string str = "";
            List<string> IDs;
            //List<Node> edgeNodes = new List<Node>();
             
            //foreach (Curve edge in panel.Edges)
            //    edgeNodes.Add(new Node(edge.StartPoint));

            //foreach (Node vertex in panel.Vertices)
            //    edgeNodes.Add(vertex);

            NodeIO.CreateNodes(GSA, face.Nodes, out IDs);

            foreach (string ID in IDs)
                str = str + ID + ",";

            return str;
        }
    }
}
