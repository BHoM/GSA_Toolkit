using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    public class NodeIO
    {

        /***************************************/

        public static bool CreateNodes(IComAuto gsa, List<BHE.Node> nodes)
        {
            List<string> ids;
            return CreateNodes(gsa, nodes, out ids);
        }


        /***************************************/

        public static bool CreateNodes(IComAuto gsa, List<BHE.Node> nodes, out List<string> ids)
        {
            ids = new List<string>();



            List<BHE.Node> idNodes = nodes.Where(x => x.CustomData.ContainsKey(Utils.ID)).ToList();
            List<BHE.Node> nonIdNodes = nodes.Where(x => !x.CustomData.ContainsKey(Utils.ID)).ToList();


            //Replace nodes in gsa with nodes that have a custom data GSA_id
            foreach (BHE.Node n in idNodes)
            {
                string id = n.CustomData[Utils.ID].ToString();
                SetNode(gsa, n, id);
            }

            if (nonIdNodes.Count < 1)
                return true;

            //Hardcoded tolerance, need to be able to set this
            double tol = 0.001;

            int highestIndex = gsa.GwaCommand("HIGHEST, NODE");


            //Grab all nodes in gsa and store them in a matrix based on their position
            BHoM.Generic.PointMatrix<BHE.Node> gsaNodes = GetNodes(gsa, Utils.CreateIntSequence(highestIndex));

            highestIndex++;

            foreach (BHE.Node n in nonIdNodes)
            {

                //Check if there is a node at the same position as the previous ones
                BHE.Node closeNode = gsaNodes.GetClosestPoint(n.Point, tol).Data;


                string nodeIndex;

                //If no node is found add a new one
                if (closeNode == null)
                {
                    nodeIndex = highestIndex.ToString();
                    highestIndex++;
                    SetNode(gsa, n, nodeIndex);
                    n.CustomData.Add(Utils.ID, nodeIndex);
                    gsaNodes.AddPoint(n.Point, n);
                }
                //Otherwhise add the found nodes GSA_id to the node
                else
                {

                    nodeIndex = closeNode.CustomData[Utils.ID].ToString();

                    //If the name of the node has been uppdated or if the provided node is constrained,
                    //the current existing node is overwritten
                    if (!(string.IsNullOrWhiteSpace(n.Name) && n.Name != closeNode.Name) || n.IsConstrained)
                    {
                        SetNode(gsa, n, nodeIndex);
                    }

                    n.CustomData.Add(Utils.ID, nodeIndex);
                }

                ids.Add(nodeIndex);
            }
            gsa.UpdateViews();
            return true;

        }

        /***************************************/

        public static bool SetNode(IComAuto gsa, BHE.Node n, string index)
        {
            string command = "NODE.2";
            string name = n.Name;

            string restraint = GetRestraintString(n);

            string str = command + ", " + index + ", " + name + " , NO_RGB, " + n.X + " , " + n.Y + " , " + n.Z + ", NO_GRID, " + 0 + ", REST," + restraint + ", STIFF,0,0,0,0,0,0";
            dynamic commandResult = gsa.GwaCommand(str); //"NODE.2, 1 , , NO_RGB,0 , 2 , 0, NO_GRID,0, REST,0,0,0,0,0,0, STIFF,0,0,0,0,0,0"

            if (1 == (int)commandResult)
            {
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }


            return true;
        }

        /***************************************/

        public static string GetRestraintString(BHE.Node node)
        {
            int X = 0;
            int Y = 0;
            int Z = 0;
            int XX = 0;
            int YY = 0;
            int ZZ = 0;

            if (node.IsConstrained)
            {
                X = ((node.Constraint.UX == BHP.DOFType.Fixed) ? 1 : 0);
                Y = ((node.Constraint.UY == BHP.DOFType.Fixed) ? 1 : 0);
                Z = ((node.Constraint.UZ == BHP.DOFType.Fixed) ? 1 : 0);
                XX = ((node.Constraint.RX == BHP.DOFType.Fixed) ? 1 : 0);
                YY = ((node.Constraint.RY == BHP.DOFType.Fixed) ? 1 : 0);
                ZZ = ((node.Constraint.RZ == BHP.DOFType.Fixed) ? 1 : 0);
            }

            return X + "," + Y + "," + Z + "," + XX + "," + YY + "," + ZZ;
        }

        /***************************************/

        public static BHoM.Generic.PointMatrix<BHE.Node> GetNodes(IComAuto gsa, int[] nodeNumbers)
        {

            //Hardcoded cell size. Will need to be specified more carefully
            BHoM.Generic.PointMatrix<BHE.Node> nodes = new BHoM.Generic.PointMatrix<BHoM.Structural.Elements.Node>(0.001);

            if (nodeNumbers.Length < 1)
                return nodes;

            GsaNode[] gsaNodes;

            gsa.Nodes(nodeNumbers, out gsaNodes);

            foreach (GsaNode gn in gsaNodes)
            {
                BHE.Node node = ToBHoMNode(gn);
                nodes.AddPoint(node.Point, node);
            }


            return nodes;
        }

        /***************************************/

        public static List<string> GetNodes(IComAuto gsa, out List<BHE.Node> nodes, List<string> nodeIds = null)
        {

            nodes = new List<BHE.Node>();

            GsaNode[] gsaNodes;

            gsa.Nodes(NodeIdIndecies(gsa, nodeIds), out gsaNodes);

            List<string> outIds = new List<string>();

            foreach (GsaNode gn in gsaNodes)
            {
                BHE.Node node = ToBHoMNode(gn);
                nodes.Add(node);
                outIds.Add(gn.Ref.ToString());
            }


            return outIds;
        }

        /***************************************/

        public static Dictionary<string, BHE.Node> GetNodes(IComAuto gsa)
        {

            Dictionary<string, BHE.Node> nodes = new Dictionary<string, BHE.Node>();

            GsaNode[] gsaNodes;

            gsa.Nodes(NodeIdIndecies(gsa), out gsaNodes);

            List<string> outIds = new List<string>();

            foreach (GsaNode gn in gsaNodes)
            {
                BHE.Node node = ToBHoMNode(gn);
                nodes.Add(gn.Ref.ToString(), node);
            }

            return nodes;
        }

        /***************************************/


        public static BHE.Node ToBHoMNode(GsaNode gn)
        {
            BHE.Node node = new BHE.Node(gn.Coor[0], gn.Coor[1], gn.Coor[2], gn.Name);
            node.CustomData.Add(Utils.ID, gn.Ref);

            //Check if the node is restrained in some way
            if (gn.Restraint != 0 || gn.Stiffness.Sum() != 0)
                node.Constraint = GetConstraint(gn.Restraint, gn.Stiffness);

            return node;
        }

        /***************************************/

        private static BHP.NodeConstraint GetConstraint(int gsaConst, double[] stiffnesses)
        {
            //Construct the constraint
            BitArray arr = new BitArray(new int[] { gsaConst });
            bool[] fixities = new bool[6];

            for (int i = 0; i < 6; i++)
            {
                fixities[i] = arr[i];
            }

            BHP.NodeConstraint con = new BHoM.Structural.Properties.NodeConstraint("", fixities, stiffnesses);

            return con;

        }

        /***************************************/

        private static int[] NodeIdIndecies(IComAuto gsa, List<string> ids = null)
        {
            if (ids == null || ids.Count < 1)
            {
                int highestIndex = gsa.GwaCommand("HIGHEST, NODE");
                return Utils.CreateIntSequence(highestIndex);
            }
            else
            {
                int id;
                int[] idArr = new int[ids.Count];

                for (int i = 0; i < ids.Count; i++)
                    if (int.TryParse(ids[i], out id))
                        idArr[i] = id;

                return idArr;
            }
        }

        /***************************************/
    }
}
