using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHoMB = BHoM.Base;
using BHoME = BHoM.Structural.Elements;
using BHoMP = BHoM.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    public class NodeIO
    {
        public static bool CreateNodes(ComAuto GSA, List<BHoME.Node> nodes, out List<string> ids)
        {
            ids = new List<string>();

            int highestIndex = GSA.GwaCommand("HIGHEST, NODE") + 1;

            foreach (BHoME.Node n in nodes)
            {
                string cmd;
                string command = "NODE.2";
                string ID = n.Name;

                if (ID == null || ID == "" || ID == "0")
                {
                    ID = (highestIndex).ToString();
                    highestIndex++;
                }

                if (GSA.GwaCommand("EXIST, NODE, " + n.Name) == 0)
                {
                    string restraint = GetRestraintString(n);

                    string str = command + ", " + ID + ", , NO_RGB, " + n.X + " , " + n.Y + " , " + n.Z + ", NO_GRID, " + 0 + ", REST," + restraint + ", STIFF,0,0,0,0,0,0";
                    dynamic commandResult = GSA.GwaCommand(str); //"NODE.2, 1 , , NO_RGB,0 , 2 , 0, NO_GRID,0, REST,0,0,0,0,0,0, STIFF,0,0,0,0,0,0"

                    if (1 == (int)commandResult)
                    {
                        ids.Add(ID);
                        continue;
                    }
                    else
                    {
                        Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                        return false;
                    }
                }
                else
                    ids.Add(ID);
            }
            GSA.UpdateViews();
            return true;
        }


        public static bool CreateNodes(ComAuto gsa, List<BHoME.Node> nodes)
        {
            //ids = new List<string>();

            List<BHoME.Node> idNodes = nodes.Where(x => x.CustomData.ContainsKey("GSA_id")).ToList();
            List<BHoME.Node> nonIdNodes = nodes.Where(x => !x.CustomData.ContainsKey("GSA_id")).ToList();


            //Replace nodes in gsa with nodes that have a custom data GSA_id
            foreach (BHoME.Node n in idNodes)
            {
                string id = n.CustomData["GSA_id"].ToString();
                SetNode(gsa, n, id);
            }

            if (nonIdNodes.Count < 1)
                return true;

            //Hardcoded tolerance, need to be able to set this
            double tol = 0.001;

            int highestIndex = gsa.GwaCommand("HIGHEST, NODE");


            //Grab all nodes in gsa and store them in a matrix based on their position
            BHoM.Generic.PointMatrix<BHoME.Node> gsaNodes = GetNodes(gsa, Utils.CreateIntSequence(highestIndex));

            highestIndex++;

            foreach (BHoME.Node n in nonIdNodes)
            {

                //Check if there is a node at the same position as the previous ones
                BHoME.Node closeNode = gsaNodes.GetClosestPoint(n.Point, tol).Data;

                //If no node is found add a new one
                if (closeNode == null)
                {
                    string sectionPropertyIndex = highestIndex.ToString();
                    highestIndex++;
                    SetNode(gsa, n, sectionPropertyIndex);
                    n.CustomData.Add("GSA_id", sectionPropertyIndex);
                    gsaNodes.AddPoint(n.Point, n);
                }
                //Otherwhise add the found nodes GSA_id to the node
                else
                {
                    n.CustomData.Add("GSA_id", closeNode.CustomData["GSA_id"]);
                }


            }
            return true;
        }

        public static bool SetNode(ComAuto gsa, BHoME.Node n, string index)
        {
            string command = "NODE.2";
            string name = n.Name;

            string restraint = GetRestraintString(n);

            string str = command + ", " + index + ", "+name+" , NO_RGB, " + n.X + " , " + n.Y + " , " + n.Z + ", NO_GRID, " + 0 + ", REST," + restraint + ", STIFF,0,0,0,0,0,0";
            dynamic commandResult = gsa.GwaCommand(str); //"NODE.2, 1 , , NO_RGB,0 , 2 , 0, NO_GRID,0, REST,0,0,0,0,0,0, STIFF,0,0,0,0,0,0"

            if (1 == (int)commandResult)
            {
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            gsa.UpdateViews();
            return true;
        }

        //public static List<BHoME.Node> GetNodes(ComAuto gsa, int[] nodeNumbers)
        //{

        //    //Hardcoded cell size. Will need to be specified more carefully
        //    BHoM.Generic.PointMatrix<BHoME.Node> nodes = new BHoM.Generic.PointMatrix<BHoM.Structural.Elements.Node>(0.001);


        //    //List<BHoME.Node> nodes = new List<BHoME.Node>();

        //    //string gsaProp = gsa.GwaCommand("GET, NODE, " + nodeNumbers).ToString();

        //    GsaNode[] gsaNodes;

        //    gsa.Nodes(nodeNumbers, out gsaNodes);

        //    foreach (GsaNode gn in gsaNodes)
        //    {
        //        BHoME.Node node = new BHoME.Node(gn.Coor[0], gn.Coor[1], gn.Coor[2], gn.Name);
        //        node.CustomData.Add("GSA_id", gn.Ref);

        //        //TODO: Add restraints

        //        nodes.AddPoint(node.Point, node);
                
        //    }


        //    return nodes;
        //}

        public static BHoM.Generic.PointMatrix<BHoME.Node> GetNodes(ComAuto gsa, int[] nodeNumbers)
        {

            //Hardcoded cell size. Will need to be specified more carefully
            BHoM.Generic.PointMatrix<BHoME.Node> nodes = new BHoM.Generic.PointMatrix<BHoM.Structural.Elements.Node>(0.001);

            if (nodeNumbers.Length < 1)
                return nodes;

            //List<BHoME.Node> nodes = new List<BHoME.Node>();

            //string gsaProp = gsa.GwaCommand("GET, NODE, " + nodeNumbers).ToString();

            GsaNode[] gsaNodes;

            gsa.Nodes(nodeNumbers, out gsaNodes);

            foreach (GsaNode gn in gsaNodes)
            {
                BHoME.Node node = new BHoME.Node(gn.Coor[0], gn.Coor[1], gn.Coor[2], gn.Name);
                node.CustomData.Add("GSA_id", gn.Ref);

                //TODO: Add restraints

                nodes.AddPoint(node.Point, node);

            }


            return nodes;
        }

        public static string GetRestraintString(BHoME.Node node)
        {
            int X = 0;
            int Y = 0;
            int Z = 0;
            int XX = 0;
            int YY = 0;
            int ZZ = 0;

            if (node.IsConstrained)
            {
                X = ((node.Constraint.UX == BHoMP.DOFType.Fixed) ? 1 : 0);
                Y = ((node.Constraint.UY == BHoMP.DOFType.Fixed) ? 1 : 0);
                Z = ((node.Constraint.UZ == BHoMP.DOFType.Fixed) ? 1 : 0);
                XX = ((node.Constraint.RX == BHoMP.DOFType.Fixed) ? 1 : 0);
                YY = ((node.Constraint.RY == BHoMP.DOFType.Fixed) ? 1 : 0);
                ZZ = ((node.Constraint.RZ == BHoMP.DOFType.Fixed) ? 1 : 0);
            }

            return X + "," + Y + "," + Z + "," + XX + "," + YY + "," + ZZ;
        }
    }
}
