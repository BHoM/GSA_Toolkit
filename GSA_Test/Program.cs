using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSA_Adapter.Structural.Interface;
using BHoM.Structural.Elements;
using BHoM.Geometry;
using BHoMBR = BHoM.Base.Results;

namespace GSA_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestNodes();
            //TestSetBars();
            //TestGetBars();
            TestExtractBarForces();
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

        private static void TestExtractBarForces()
        {
            GSAAdapter app = new GSAAdapter(@"C:\Users\nsmithie\Desktop\NS_Test.gwb");
            List<String> bars = new List<String> { "1", "2" };
            List<String> cases = new List<String> { "C1" };
            Dictionary<string, BHoMBR.IResultSet> testResults = new Dictionary<string, BHoMBR.IResultSet>();
            BHoMBR.IResultSet test;


            app.GetBarForces(bars, cases, 0, BHoMBR.ResultOrder.Loadcase, out testResults);

            if (testResults.TryGetValue("1", out test)) // Returns true.
            {
                Console.WriteLine("Hello World"); // This is the value at "1".
            }

            Console.WriteLine("end");

        }
    }
}
