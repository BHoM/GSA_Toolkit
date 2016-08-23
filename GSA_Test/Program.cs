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
            //TestNodes();
            //TestSetBars();
            TestGetBars();
                
        }


        private static void TestNodes()
        {
            GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");

            Point p1 = new Point(1, 2, 3);
            Node node = new Node(p1);
            List<Node> nodes = new List<Node>();
            nodes.Add(node);
            List<string> ids = null;
            app.SetNodes(nodes, out ids);
        }

        private static void TestGetBars()
        {
            GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");


            Bar bar;
            List<Bar> bars = new List<Bar>();
            app.GetBars(out bars);

            bar = bars[0];
            string name = bar.Name;

        }

        private static void TestSetBars()
        {
            GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");



            for (int i = 1; i < 10; i++)
            {
                Point p1 = new Point(0, 0, 0);
                Point p2 = new Point(10, i, 0);

                Bar bar = new Bar(p1, p2);
                List<Bar> bars = new List<Bar>();
                bars.Add(bar);
                BHoM.Structural.Properties.SectionProperty sec = new BHoM.Structural.Properties.SectionProperty(BHoM.Structural.Properties.ShapeType.Rectangle, BHoM.Structural.Properties.SectionType.Steel, 100, 50, 5, 5, 5, 5);
                bar.SetSectionProperty(sec);
                bar.Material = BHoM.Materials.Material.LoadFromDB("Steel");
                List<string> ids = null;
                bar.Name = i.ToString();
                app.CreateBars(bars, out ids);
            }

        }
    }
}
