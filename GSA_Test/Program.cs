using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSA_Adapter.Structural.Interface;
using BHoM.Structural.Elements;
using BHoM.Geometry;

namespace GSA_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestNodes();
        }


        private static void TestNodes()
        {
            GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");

            Point p1 = new Point(0, 0, 0);
            Node node = new Node(p1);
            List<Node> nodes = new List<Node>();
            nodes.Add(node);
            List<string> ids = null;
            app.SetNodes(nodes, out ids);
        }

        private static void TestBars()
        {
            GSAAdapter app = new GSAAdapter();

            Point p1 = new Point(10, 0, 0);
            Point p2 = new Point(0, 10, 0);

            Bar bar = new Bar(p1, p2);
            List<Bar> bars = new List<Bar>();
            bars.Add(bar);
            List<string> ids = null;     
            app.CreateBars(bars, out ids);

        }
    }
}
