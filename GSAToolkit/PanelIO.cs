using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoM.Structural;
using BHoM.Geometry;

namespace GSAToolkit
{
    /// <summary>
    /// GSA panel class, for all panel objects and operations
    /// </summary>
    public class PanelIO
    {
        /// <summary>
        /// Create GSA Panels
        /// </summary>
        /// <returns></returns>
        public static bool CreatePanels(ComAuto GSA, List<Panel> str_panels, out List<string> ids)
        {
            ids = new List<string>();

            List<string> props = PropertyIO.Get2DPropertyStringList(GSA);
            int highestIndex = GSA.GwaCommand("HIGHEST, EL") + 1;

            foreach (Panel panel in str_panels)
            {
                string command = "";
                if (panel.Vertices.Count == 4)
                    command = "EL_QUAD4";
                if (panel.Vertices.Count == 3)
                    command = "EL_TRI3";

                string index = panel.Name;
                if (index == null || index == "" || index == "0")
                {
                    index = highestIndex.ToString();
                    highestIndex++;
                }

                string propertyIndex = PropertyIO.GetOrCreate2DPropertyIndex(GSA, panel, props);
                string group = "0";
                string indexString = GetPanelNodeIndexString(GSA, panel);

                string str = command + "," + index + "," + propertyIndex + "," + group + "," + indexString + "0,REAL";
                dynamic commandResult = GSA.GwaCommand(str);

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    continue;
                }
                else
                {
                    GSAUtils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                    return false;
                }
            }

            GSA.UpdateViews();
            return true;
        }

        public static string GetPanelNodeIndexString(ComAuto GSA, Panel panel)
        {
            string str = "";
            List<string> IDs;
            //List<Node> edgeNodes = new List<Node>();
             
            //foreach (Curve edge in panel.Edges)
            //    edgeNodes.Add(new Node(edge.StartPoint));

            //foreach (Node vertex in panel.Vertices)
            //    edgeNodes.Add(vertex);

            NodeIO.CreateNodes(GSA, panel.Vertices, out IDs);

            foreach (string ID in IDs)
                str = str + ID + ",";

            return str;
        }
    }
}
