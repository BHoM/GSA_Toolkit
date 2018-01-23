using System.Collections.Generic;
using System.Linq;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.oM.Structural.Properties;
using BH.Engine.Structure;
using BH.Adapter.GSA;
using BH.oM.Common.Materials;
using BH.oM.Queries;
using BH.oM.Structural.Results;

namespace GSA_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExtractForces();
            //TestPushBars();
            //TestDelete();
            //TestPushMaterials();
        }

        private static void TestExtractForces()
        {
            //C: \Users\inaslund\Documents\GSA sandbox\SimpleBeam Pt load.gwb
            GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\GSA sandbox\SimpleBeam Pt load.gwb");
            FilterQuery query = new FilterQuery(typeof(BarDeformation));
            app.Pull(query);
        }

        //private static void TestPushMaterials()
        //{
        //    List<Material> materials = new List<Material> {
        //     new BH.oM.Materials.Material("TestPushBars1", BH.oM.Materials.MaterialType.Steel, 10, 10, 10, 10, 10),
        //     new BH.oM.Materials.Material("TestPushBars1", BH.oM.Materials.MaterialType.Steel, 10, 10, 10, 10, 10),
        //     new BH.oM.Materials.Material("TestPushBars1", BH.oM.Materials.MaterialType.Steel, 10, 10, 10, 10, 10),
        //     new BH.oM.Materials.Material("TestPushBars1", BH.oM.Materials.MaterialType.Steel, 10, 10, 10, 10, 10),
        //     new BH.oM.Materials.Material("TestPushBars2", BH.oM.Materials.MaterialType.Steel, 10, 10, 10, 10, 10),
        //     new BH.oM.Materials.Material("TestPushBars2", BH.oM.Materials.MaterialType.Steel, 10, 10, 10, 10, 10) };

        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\GSA sandbox\EmptyFile.gwb");
        //    List<string> ids;
        //    app.PushMaterials(materials, out ids, "Test");

        //}

        private static void TestPushBars()
        {
            //
            //string key = "Test Push Bar Key";
            GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");
            //app = new GSAAdapter(@"‪C:\Users\phesari\Desktop\Gsa2.gwb");
            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = new Point { X = 1, Y = 0, Z = 0 };
            Point p3 = new Point { X = 1, Y = 1, Z = 0 };
            Point p4 = new Point { X = 0, Y = 1, Z = 0 };
            Point p5 = new Point { X = 0, Y = 0, Z = 1 };
            Point p6 = new Point { X = 1, Y = 0, Z = 1 };
            Point p7 = new Point { X = 1, Y = 1, Z = 1 };
            Point p8 = new Point { X = 0, Y = 1, Z = 1 };

            Point p5b = new Point { X = 0, Y = 0, Z = 2 };
            Point p6b = new Point { X = 1, Y = 0, Z = 2 };
            Point p7b = new Point { X = 1, Y = 1, Z = 2 };
            Point p8b = new Point { X = 0, Y = 1, Z = 2 };

            Constraint6DOF pin = Create.PinConstraint6DOF();
            Constraint6DOF fix = Create.FixConstraint6DOF();

            List<Node> nodesA = new List<Node>();

            Node n1a = new Node { Position = p5, Name = "1" };
            Node n2a = new Node { Position = p6, Name = "2" };
            Node n3a = new Node { Position = p7, Name = "3" };
            Node n4a = new Node { Position = p8, Name = "4" };
        
            n1a.Constraint = pin;
            n2a.Constraint = pin;
            n3a.Constraint = fix;
            n4a.Constraint = fix;

            nodesA.Add(n1a);
            nodesA.Add(n2a);
            nodesA.Add(n3a);
            nodesA.Add(n4a);



            List<Node> nodesB = new List<Node>();

            Node n1b = new Node { Position = p5b, Name = "1" };
            Node n2b = new Node { Position = p6b, Name = "2" };
            Node n3b = new Node { Position = p7b, Name = "3" };
            Node n4b = new Node { Position = p8b, Name = "4" };

            n1b.Constraint = pin;
            n2b.Constraint = pin;
            n3b.Constraint = Create.FullReleaseConstraint6DOF();
            n4b.Constraint = fix;

            nodesB.Add(n1b);
            nodesB.Add(n2b);
            nodesB.Add(n3b);
            nodesB.Add(n4b);

            Bar bar1 = Create.Bar(new Node { Position = p1 }, new Node { Position = p2 });
            Bar bar2 = Create.Bar(new Node { Position = p2 }, new Node { Position = p3 });
            Bar bar3 = Create.Bar(new Node { Position = p3 }, new Node { Position = p4 });
            Bar bar4 = Create.Bar(new Node { Position = p4 }, new Node { Position = p1 });

            Bar bar5 = Create.Bar(new Node { Position = p5 }, new Node { Position = p6 });
            Bar bar6 = Create.Bar(new Node { Position = p6 }, new Node { Position = p7 });
            Bar bar7 = Create.Bar(new Node { Position = p7 }, new Node { Position = p8 });
            Bar bar8 = Create.Bar(new Node { Position = p8 }, new Node { Position = p5 });

            Bar bar9 = Create.Bar(new Node { Position = p1 }, new Node { Position = p5 });
            Bar bar10 = Create.Bar(new Node { Position = p2 }, new Node { Position = p6 });
            Bar bar11 = Create.Bar(new Node { Position = p3 }, new Node { Position = p7 });
            Bar bar12 = Create.Bar(new Node { Position = p4 }, new Node { Position = p8 });

            Bar bar5b = Create.Bar(new Node { Position = p5b }, new Node { Position = p6b });
            Bar bar6b = Create.Bar(new Node { Position = p6b }, new Node { Position = p7b });
            Bar bar7b = Create.Bar(new Node { Position = p7b }, new Node { Position = p8b });
            Bar bar8b = Create.Bar(new Node { Position = p8b }, new Node { Position = p5b });

            Bar bar9b = Create.Bar(new Node { Position = p1 }, new Node { Position = p5b });
            Bar bar10b = Create.Bar(new Node { Position = p2 }, new Node { Position = p6b });
            Bar bar11b = Create.Bar(new Node { Position = p3 }, new Node { Position = p7b });
            Bar bar12b = Create.Bar(new Node { Position = p4 }, new Node { Position = p8b });

            List<Bar> bars1 = new List<Bar>();
            List<Bar> bars2a = new List<Bar>();
            List<Bar> bars2b = new List<Bar>();

            bars1.Add(bar1);
            bars1.Add(bar2);
            bars1.Add(bar3);
            bars1.Add(bar4);

            bars2a.Add(bar5);
            bars2a.Add(bar6);
            bars2a.Add(bar7);
            bars2a.Add(bar8);
            bars2a.Add(bar9);
            bars2a.Add(bar10);
            bars2a.Add(bar11);
            bars2a.Add(bar12);

            bars2b.Add(bar5b);
            bars2b.Add(bar6b);
            bars2b.Add(bar7b);
            bars2b.Add(bar8b);
            bars2b.Add(bar9b);
          
            bars2b.Add(bar10b);
            bars2b.Add(bar11b);
            bars2b.Add(bar12b);

            ISectionProperty sec1a = new ExplicitSection();
            sec1a.Material = BH.Engine.Common.Create.Material("Material1", MaterialType.Concrete, 10, 10, 10, 10, 10);
            sec1a.Name = "Section 1";

            ISectionProperty sec1b = new ExplicitSection();
            sec1b.Material = BH.Engine.Common.Create.Material("Material1", MaterialType.Concrete, 10, 10, 10, 10, 10);
            sec1b.Name = "Section 1";

            ISectionProperty sec2 = new ExplicitSection();
            sec2.Material = BH.Engine.Common.Create.Material("Material2", MaterialType.Concrete, 10, 10, 10, 10, 10);
            sec2.Name = "Section 2";

            ISectionProperty sec3 = new ExplicitSection();
            sec3.Material = BH.Engine.Common.Create.Material("Material2", MaterialType.Concrete, 10, 10, 10, 10, 10);
            sec3.Name = "Section 3";

            foreach (Bar b in bars1.Concat(bars2a).Concat(bars2b))
            {
                b.SectionProperty = sec1a;
            }


            app.Push(nodesA, "Nodes");

            app.Push(bars1,  "Bars1");

            app.Push(bars2a, "Bars2");
            app.Push(bars2b, "Bars2");

            app.Push(nodesB, "Nodes");

            app.Delete(new BH.oM.Queries.FilterQuery(typeof(Node), "Nodes"));

            //List<string> ids;
            //app.PushObjects(nodesA, out ids, "Nodes");

            //app.PushObjects(bars1, out ids, "Bars1");

            //app.PushObjects(bars2a, out ids, "Bars2");
            //app.PushObjects(bars2b, out ids, "Bars2");

            //app.PushObjects(nodesB, out ids, "Nodes");
        }

        //private static void TestPushBar()
        //{
        //    Point p1 = new Point { X = 0, Y = 0, Z = 0 };
        //    Point p2 = new Point { X = 1, Y = 0, Z = 0 };
        //    Point p3 = new Point { X = 1, Y = 1 , Z = 0 };
        //    Point p4 = new Point { X = 0, Y = 1, Z = 0 };


        //    Bar bar1 = Create.Bar(new Node { Position = p1 }, new Node { Position = p2 });
        //    Bar bar2 = Create.Bar(new Node { Position = p1 }, new Node { Position = p3 });
        //    Bar bar3 = Create.Bar(new Node { Position = p1 }, new Node { Position = p4 });

        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\GSA sandbox\EmptyFile.gwb");

        //    BH.oM.Structural.Properties.SectionProperty sec1a = new BH.oM.Structural.Properties.ExplicitSectionProperty();
        //    sec1a.Material = new BH.oM.Materials.Material("Material1", BH.oM.Materials.MaterialType.Concrete, 10, 10, 10, 10, 10);
        //    sec1a.Name = "Section 1";

        //    bar1.SectionProperty = sec1a;
        //    bar2.SectionProperty = sec1a;
        //    bar3.SectionProperty = sec1a;

        //    List<string> ids;
        //    app.PushBars(new List<Bar> { bar1 }, out ids, "Bars");
        //    app.PushBars(new List<Bar> { bar2 }, out ids, "Bars");
        //    app.PushBars(new List<Bar> { bar3 }, out ids, "Bars");
        //}

        //private static void TestDelete()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\GSA sandbox\Test.gwb");

        //    BH.Adapter.Queries.FilterQuery query = new BH.Adapter.Queries.FilterQuery();
        //    app.Delete(query);
        //}


        //private static void TestGetFEMeshes()
        //{
        //    List<BHoM.Structural.Elements.FEMesh> meshes;
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\GSA sandbox\2delems 2.gwb");
        //    app.GetFEMeshes(out meshes);
        //}

        //private static void TestLinkProperty()
        //{
        //    BHoM.Structural.Properties.LinkConstraint constr = BHoM.Structural.Properties.LinkConstraint.XYPlanePin;
        //    GSA_Adapter.Structural.Properties.LinkPropertyIO.GetRestraintString(constr);
        //}
        //private static void TestExtractBarStress()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\QF Stadium\170127 QF SD Roof Model_for Review.gwb");
        //    List<BHoM.Structural.Elements.Bar> bars;
        //    Dictionary<string, BHoM.Base.Results.IResultSet> res;
        //    app.GetBarForces(null, null, 5, BHoM.Base.Results.ResultOrder.Name, out res);
        //}

        //private static void TestGetMembers()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\QF Stadium\161109 -  QF Stadium Roof.gwb");
        //    List<BHoM.Structural.Elements.Bar> bars;
        //    app.GetBarDesignElement(out bars);
        //}

        //private static void TestExtractBarUtil()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\inaslund\Documents\QF Stadium\161109 -  QF Stadium Roof.gwb");
        //    Dictionary<string, BHoM.Base.Results.IResultSet> res;
        //    app.GetBarForces(null, null,3, BHoM.Base.Results.ResultOrder.Loadcase, out res);
        //    app.GetBarUtilisation(null, null, BHoM.Base.Results.ResultOrder.Loadcase, out res);
        //}

        //private static void TestNodes()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");

        //    Point p1 = new Point { X = 1, Y = 2, Z = 3 };
        //    Node node = new Node { Position = p1 };
        //    List<Node> nodes = new List<Node>();
        //    nodes.Add(node);
        //    List<string> ids = null;
        //    app.SetNodes(nodes, out ids);
        //}

        //private static void TestGetBars()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");


        //    Bar bar;
        //    List<Bar> bars = new List<Bar>();
        //    app.GetBars(out bars);

        //    bar = bars[0];
        //    string name = bar.Name;

        //}

        ////private static void TestSetBars()
        ////{
        ////    GSAAdapter app = new GSAAdapter(@"C:\Users\afisher\Desktop\Project Shortcuts\_Test\Al_Test.gwb");




        ////    Point p1 = new Point { X = 10, Y = 10, Z = 10 };
        ////    Point p2 = new Point { X = 5, Y = 5, Z = 5 };

        ////    Bar bar = Create.Bar(p1, p2);
        ////    List<Bar> bars = new List<Bar>();
        ////    bars.Add(bar);
        ////    BHoM.Structural.Properties.SectionProperty sec = new BHoM.Structural.Properties.SectionProperty(BHoM.Structural.Properties.ShapeType.Rectangle, /*BHoM.Structural.Properties.SectionType.Steel,*/ 100, 50, 5, 5, 5, 5);
        ////    sec.Description = "EXP";
        ////    bar.SetSectionProperty(sec);
        ////    BHoM.Materials.Material material = BHoM.Materials.Material.Default(BHoM.Materials.MaterialType.Steel);

        ////    BHoM.Materials.Material material2 = new BHoM.Materials.Material("Test", BHoM.Materials.MaterialType.Steel, 210, 0, 0, 0, 7840);


        ////    bar.Material = material2;    

        ////    List<string> ids = null;
        ////    bar.Name = "10";
        ////    app.CreateBars(bars, out ids);


        ////}

        //private static void TestExtractBarForces()
        //{
        //    GSAAdapter app = new GSAAdapter(@"C:\Users\nsmithie\Desktop\NS_Test.gwb");
        //    List<String> bars = new List<String> { "1" };
        //    List<String> cases = new List<String> { "C1" };
        //    Dictionary<string, BHoMBR.IResultSet> testResults = new Dictionary<string, BHoMBR.IResultSet>();
        //    BHoMBR.IResultSet test;


        //    app.GetBarForces(bars, cases, 0, BHoMBR.ResultOrder.Name, out testResults);

        //    if (testResults.TryGetValue("1", out test)) // Returns true.
        //    {
        //        Console.WriteLine("Hello World"); // This is the value at "1".
        //    }

        //    Console.WriteLine("end");

        //}
    }
}
