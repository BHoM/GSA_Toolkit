using BH.Engine.Serialiser;
using BH.Engine.Structure;
using BHM = BH.oM.Common.Materials;
using BHL = BH.oM.Structural.Loads;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structural.Results;

namespace BH.Engine.GSA
{
    public partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Bar> FromGsaBars(IEnumerable<GsaElement> gsaElements, Dictionary<string, ISectionProperty> secProps, Dictionary<string, Node> nodes)
        {
            List<Bar> barList = new List<Bar>();

            foreach (GsaElement gsaBar in gsaElements)
            {

                BarFEAType feType;

                switch (gsaBar.eType)
                {
                    case 1:
                        feType = BarFEAType.Axial;
                        break;
                    case 2:
                        feType = BarFEAType.Flexural;
                        break;
                    case 20:
                        feType = BarFEAType.CompressionOnly;
                        break;
                    case 21:
                        feType = BarFEAType.TensionOnly;
                        break;
                    default:
                        continue;
                }

                Node n1, n2;
                nodes.TryGetValue(gsaBar.Topo[0].ToString(), out n1);
                nodes.TryGetValue(gsaBar.Topo[1].ToString(), out n2);

                Bar bar = new Bar { StartNode = n1, EndNode = n2 };
                bar.ApplyTaggedName(gsaBar.Name);


                bar.FEAType = feType;

                bar.OrientationAngle = gsaBar.Beta;

                ISectionProperty prop;
                secProps.TryGetValue(gsaBar.Property.ToString(), out prop);

                bar.SectionProperty = prop;

                bar.CustomData[AdapterID] = gsaBar.Ref;

                barList.Add(bar);

            }
            return barList;
        }

        /***************************************/

        public static List<MeshFace> FromGsaMeshFace(IEnumerable<GsaElement> gsaElements, Dictionary<string, Property2D> props, Dictionary<string, Node> nodes)
        {
            List<MeshFace> faceList = new List<MeshFace>();

            foreach (GsaElement gsaMesh in gsaElements)
            {
                switch (gsaMesh.eType)
                {
                    case 5://Quad4      //TODO: Quad8 and Tri6
                    case 7://Tri3
                        break;
                    default:
                        continue;

                }

                MeshFace face = new MeshFace()
                {
                    Nodes = gsaMesh.Topo.Select(x => nodes[x.ToString()]).ToList(),
                    Property = props[gsaMesh.Property.ToString()]
                };

                face.ApplyTaggedName(gsaMesh.Name);
                face.CustomData[AdapterID] = gsaMesh.Ref;
                faceList.Add(face);

            }
            return faceList;
        }

        /***************************************/

        public static List<RigidLink> FromGsaRigidLinks(IEnumerable<GsaElement> gsaElements, Dictionary<string, LinkConstraint> constraints, Dictionary<string, Node> nodes)
        {
            List<RigidLink> linkList = new List<RigidLink>();

            foreach (GsaElement gsaLink in gsaElements)
            {
                if (gsaLink.eType != 9)
                    continue;

                RigidLink face = new RigidLink()
                {
                    MasterNode = nodes[gsaLink.Topo[0].ToString()],
                    SlaveNodes = new List<Node> { nodes[gsaLink.Topo[1].ToString()] },
                    Constraint = constraints[gsaLink.Property.ToString()]
                };

                face.ApplyTaggedName(gsaLink.Name);
                face.CustomData[AdapterID] = gsaLink.Ref;
                linkList.Add(face);

            }
            return linkList;
        }

        /***************************************/

        public static BHM.Material FromGsaMaterial(string gsaString)
        {
            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 11)
                return null;

            BHM.MaterialType type = GetTypeFromString(gStr[2]);

            double E, v, tC, G, rho;

            if (!double.TryParse(gStr[7], out E))
                return null;
            if (!double.TryParse(gStr[8], out v))
                return null;
            if (!double.TryParse(gStr[10], out tC))
                return null;
            if (!double.TryParse(gStr[11], out G))
                return null;
            if (!double.TryParse(gStr[9], out rho))
                return null;

            BHM.Material mat = Engine.Common.Create.Material("", type, E, v, tC, G, rho);
            mat.ApplyTaggedName(gStr[3]);

            mat.CustomData.Add(AdapterID, int.Parse(gStr[1]));

            return mat;
        }

        /***************************************/

        public static BHL.Loadcase FromGsaLoadcase(string gsaString)
        {

            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 5)
                return null;

            BHL.LoadNature loadNature = Query.BHoMLoadNature(gStr[3]);
            BHL.Loadcase lCase = Engine.Structure.Create.Loadcase(gStr[2], loadNature);

            int lCasenum = 0;

            if (Int32.TryParse(gStr[1], out lCasenum))
            {
                lCase.Number = lCasenum;
            }

            return lCase;            
        }

        /***************************************/

        public static BHL.LoadCombination FromGsaAnalTask(string gsaString, Dictionary<string, BHL.Loadcase> lCases)
        {

            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            List<Tuple<double, BHL.ICase>> lCasesForTask = new List<Tuple<double, BHL.ICase>>(); 
            string[] gStr = gsaString.Split(',');
            string[] lCaseArr = gStr[4].Split('+');

            if (gStr.Length < 5)
                return null;

            foreach (string str in lCaseArr)
            {
                string cleanStr = str.Replace(" ", "");
                cleanStr = cleanStr.Replace("L", ",");
                string[] lCaseParam = cleanStr.Split(',');

                if (lCaseParam.Length == 2)
                {
                    if (string.IsNullOrEmpty(lCaseParam[0]))
                        lCaseParam[0] = "1.0";

                    BHL.Loadcase templCase = lCases[lCaseParam[1]];
                    Tuple<double, BHL.ICase> loadCase = new Tuple<double, BHL.ICase>(double.Parse(lCaseParam[0]), templCase);
                    lCasesForTask.Add(loadCase);
                }
            }

            return Engine.Structure.Create.LoadCombination(gStr[2], lCasesForTask);
        }

        /***************************************/

        /// <summary>Creates a BHoM section from a gsa string</summary>
        /// <param name="gsaString">
        /// <summary>
        /// Comma separated string on the format: [PROP_SEC | num | name | colour | mat | desc | principal | type | cost | 
        /// is_prop { | area | I11 | I22 | J | K1 | K2} | 
        /// is_mod { | area_to_by | area_m | I11_to_by | I11_m | I22_to_by | I22_m | J_to_by | J_m | K1_to_by | K1_m | K2_to_by | K2_m | mass | stress} | 
        /// plate_type | calc_J]
        /// </summary>
        /// </param>
        /// <param name="materials"></param>
        /// <returns></returns>
        public static ISectionProperty FromGsaSectionProperty(string gsaString, Dictionary<string, BHM.Material> materials)
        {
            ISectionProperty secProp = null;

            if (gsaString == "")
            {
                return null;
            }

            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);

            string materialId = gsaStrings[4];
            string description = gsaStrings[5];

            if (description == "EXP")
            {
                //prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;


                ExplicitSection expSecProp = new ExplicitSection();
                double a, iyy, izz, j, avy, avz;
                double.TryParse(gsaStrings[10], out a);
                double.TryParse(gsaStrings[11], out iyy);
                double.TryParse(gsaStrings[12], out izz);
                double.TryParse(gsaStrings[13], out j);
                double.TryParse(gsaStrings[14], out avy);
                double.TryParse(gsaStrings[15], out avz);

                expSecProp.Area = a;
                expSecProp.Iy = iyy;
                expSecProp.Iz = izz;
                expSecProp.J = j;
                expSecProp.Asy = avy;
                expSecProp.Asz = avz;

                secProp = expSecProp;
            }
            else if (description.StartsWith("STD") /*|| description.StartsWith("CAT") */)
            {

                double D, W, T, t, Wt, Wb, Tt, Tb, Tw;
                string[] desc = description.Split('%');
                double factor;
                string type;

                if (desc[1].Contains("(cm)"))
                {
                    factor = 0.01;
                    type = desc[1].Replace("(cm)", "");
                }
                else if (desc[1].Contains("(m)"))
                {
                    factor = 1;
                    type = desc[1].Replace("(m)", "");
                }
                else
                {
                    factor = 0.001;
                    type = desc[1];
                }

                ISectionDimensions dimensions;

                switch (type)
                {
                    case "UC":
                    case "UB":
                    case "I":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        dimensions = new StandardISectionDimensions(D, W, T, t, 0, 0);
                        break;
                    case "GI":
                        D = double.Parse(desc[2]) * factor;
                        Wt = double.Parse(desc[3]) * factor;
                        Wb = double.Parse(desc[4]) * factor;
                        Tw = double.Parse(desc[5]) * factor;
                        Tt = double.Parse(desc[6]) * factor;
                        Tb = double.Parse(desc[7]) * factor;
                        dimensions = new FabricatedISectionDimensions(D, Wt, Wb, Tw, Tt, Tb, 0);
                        break;
                    case "CHS":
                        D = double.Parse(desc[2]) * factor;
                        t = double.Parse(desc[3]) * factor;
                        dimensions = new TubeDimensions(D, t);
                        break;
                    case "RHS":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        if (T == t)
                            dimensions = new StandardBoxDimensions(D, W, T, 0, 0); //TODO: Additional checks for fabricated/Standard
                        else
                            dimensions = new FabricatedBoxDimensions(D, W, T, t, t, 0);
                        break;
                    case "R":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        dimensions = new RectangleSectionDimensions(D, W, 0);
                        break;
                    case "C":
                        D = double.Parse(desc[2]) * factor;
                        dimensions = new CircleDimensions(D);
                        break;
                    case "T":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        dimensions = new StandardTeeSectionDimensions(D, W, T, t, 0, 0);
                        break;
                    case "A":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        dimensions = new StandardAngleSectionDimensions(D, W, T, t, 0, 0);
                        break;
                    case "CH":
                        D = double.Parse(desc[2]) * factor;
                        W = double.Parse(desc[3]) * factor;
                        T = double.Parse(desc[4]) * factor;
                        t = double.Parse(desc[5]) * factor;
                        dimensions = new StandardChannelSectionDimensions(D, W, T, t, 0, 0);
                        break;
                    default:
                        throw new NotImplementedException("Section convertion for the type: " + type + "is not implmented in the GSA adapter");
                }

                switch (materials[materialId].Type)
                {
                    case BHM.MaterialType.Steel:
                        secProp = Create.SteelSectionFromDimensions(dimensions);
                        break;
                    case BHM.MaterialType.Concrete:
                        secProp = Create.ConcreteSectionFromDimensions(dimensions);
                        break;
                    case BHM.MaterialType.Aluminium:
                    case BHM.MaterialType.Timber:
                    case BHM.MaterialType.Rebar:
                    case BHM.MaterialType.Tendon:
                    case BHM.MaterialType.Glass:
                    case BHM.MaterialType.Cable:
                    default:
                        throw new NotImplementedException("Material type for cross section not implemented");
                }

            }

            secProp.CustomData.Add(AdapterID, id);
            secProp.ApplyTaggedName(gsaStrings[2]);
            secProp.Material = materials[materialId];
            return secProp;
        }

        /***************************************/

        public static LinkConstraint FromGsaLinkConstraint(string gsaProp)
        {
            LinkConstraint constraint;
            string[] props = gsaProp.Split(',');
            string name = props[2];
            string type = props[4];
            string id = props[1];

            switch (type)
            {
                case "ALL":
                    constraint = Create.LinkConstraintFixed();
                    break;
                case "PIN":
                    constraint = Create.LinkConstraintPinned();
                    break;
                case "XY_PLANE":
                    constraint = Create.LinkConstraintXYPlane();
                    break;
                case "ZX_PLANE":
                    constraint = Create.LinkConstraintZXPlane();
                    break;
                case "YZ_PLANE":
                    constraint = Create.LinkConstraintYZPlane();
                    break;
                case "XY_PLANE_PIN":
                    constraint = Create.LinkConstraintXYPlanePin();
                    break;
                case "ZX_PLANE_PIN":
                    constraint = Create.LinkConstraintZXPlanePin();
                    break;
                case "YZ_PLANE_PIN":
                    constraint = Create.LinkConstraintYZPlanePin();
                    break;
                //case "XY_PLATE":
                //    constraint = BHP.LinkConstraint.ZPlate;
                //    break;
                //case "ZX_PLATE":
                //    constraint = BHP.LinkConstraint.YPlate;
                //    break;
                //case "YZ_PLATE":
                //    constraint = BHP.LinkConstraint.YPlate;
                //    break;                                            //TODO: CHECK CONSTRUCTOR NAMES IN BHOM_ENGINE
                //case "XY_PLATE_PIN":
                //    constraint = BHP.LinkConstraint.ZPlatePin;
                //    break;
                //case "ZX_PLATE_PIN":
                //    constraint = BHP.LinkConstraint.YPlatePin;
                //    break;
                //case "YZ_PLATE_PIN":
                //    constraint = BHP.LinkConstraint.ZPlatePin;
                //    break;
                default:
                    //String in format example: X:XYY-Y:YZZXX-Z:YY-XX:XX-YY:YY-ZZ:ZZ
                    constraint = new LinkConstraint();
                    string[] constraintProps = type.Split('-');

                    foreach (string c in constraintProps)
                    {
                        string[] fromTo = c.Split(':');
                        string from = fromTo[0];
                        string to = fromTo[1];
                        switch (from)
                        {
                            case "X":
                                if (to.Contains('X'))
                                    constraint.XtoX = true;
                                if (to.Contains('Y'))
                                    constraint.XtoYY = true;
                                if (to.Contains('Z'))
                                    constraint.XtoZZ = true;
                                break;
                            case "Y":
                                if (to.Contains('X'))
                                    constraint.YtoXX = true;
                                if (to.Contains('Y'))
                                    constraint.YtoY = true;
                                if (to.Contains('Z'))
                                    constraint.YtoZZ = true;
                                break;
                            case "Z":
                                if (to.Contains('X'))
                                    constraint.ZtoXX = true;
                                if (to.Contains('Y'))
                                    constraint.ZtoYY = true;
                                if (to.Contains('Z'))
                                    constraint.ZtoZ = true;
                                break;
                            case "XX":
                                if (to.Contains("XX"))
                                    constraint.XXtoXX = true;
                                break;
                            case "YY":
                                if (to.Contains("YY"))
                                    constraint.YYtoYY = true;
                                break;
                            case "ZZ":
                                if (to.Contains("ZZ"))
                                    constraint.ZZtoZZ = true;
                                break;
                        }
                    }
                    break;
            }

            constraint.Name = name;
            constraint.CustomData[AdapterID] = id;

            return constraint;
        }

        /***************************************/
        public static Property2D FromGsaProperty2d(string gsaString, Dictionary<string, BHM.Material> materials)
        {
            Property2D panProp = null;

            if (gsaString == "")
            {
                return null;
            }

            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);
            string name = gsaStrings[2];
            string materialId = gsaStrings[5];
            string description = gsaStrings[6];

            if (description == "SHELL")
            {
                panProp = new ConstantThickness();
                panProp.Material = materials[materialId];
                double t = double.Parse(gsaStrings[7]);
                panProp.Thickness = t;
            }
            else if (description == "LOAD")
            {
                panProp = new LoadingPanelProperty();
                ((LoadingPanelProperty)panProp).LoadApplication = GetLoadingConditionFromString(gsaStrings[7]);
                ((LoadingPanelProperty)panProp).ReferenceEdge = int.Parse(gsaStrings[8]);
            }

            panProp.CustomData.Add(AdapterID, id);
            panProp.Name = name;
            return panProp;
        }

        /***************************************/

        public static Node FromGsaNode(string gsaString)
        {
            if (gsaString == "")
            {
                return null;
            }

            string[] arr = gsaString.Split(',');

            int id = Int32.Parse(arr[1]);
            string name = arr[2];

            Point pos = new Point() { X = double.Parse(arr[4]), Y = double.Parse(arr[5]), Z = double.Parse(arr[6]) };

            Constraint6DOF con;
            if (arr.Length > 7)
            {
                bool[] fixities;
                double[] stiff;
                if (arr[9] == "REST")
                {
                    fixities = new bool[]
                    {
                        arr[10] == "1",
                        arr[11] == "1",
                        arr[12] == "1",
                        arr[13] == "1",
                        arr[14] == "1",
                        arr[15] == "1",
                    };
                    if (arr.Length > 16 && arr[16] == "STIFF")
                    {
                        stiff = new double[]
                            {
                                double.Parse(arr[17]),
                                double.Parse(arr[18]),
                                double.Parse(arr[19]),
                                double.Parse(arr[20]),
                                double.Parse(arr[21]),
                                double.Parse(arr[22])
                            };
                    }
                    else
                        stiff = new double[] { 0, 0, 0, 0, 0, 0 };
                }
                else
                {
                    fixities = new bool[] { false, false, false, false, false, false };
                    if (arr[10] == "STIFF")
                    {
                        stiff = new double[]
                            {
                                double.Parse(arr[11]),
                                double.Parse(arr[12]),
                                double.Parse(arr[13]),
                                double.Parse(arr[14]),
                                double.Parse(arr[15]),
                                double.Parse(arr[16])
                            };
                    }
                    else
                        stiff = new double[] { 0, 0, 0, 0, 0, 0 };
                }

                con = Create.Constraint6DOF("", fixities, stiff);
            }
            else
                con = null;

            Node node = new Node { Position = pos, Constraint = con };
            node.ApplyTaggedName(name);
            node.CustomData.Add(AdapterID, id);
            return node;
            //Node node = new Node { Position = new Point { X = gn.Coor[0], Y = gn.Coor[1], Z = gn.Coor[2] } };
            //node.ApplyTaggedName(gn.Name);
            //node.CustomData.Add(AdapterID, gn.Ref);

            ////Check if the node is restrained in some way
            //if (gn.Restraint != 0 || gn.Stiffness.Sum() != 0)
            //    node.Constraint = GetConstraint(gn.Restraint, gn.Stiffness);

            //return node;
        }

        /***************************************************/

        public static NodeDisplacement FromGsaNodeDisplacement(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {
            NodeDisplacement disp = new NodeDisplacement
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static NodeReaction FromGsaReaction(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {

            NodeReaction reac = new NodeReaction
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                FX = results.dynaResults[0],
                FY = results.dynaResults[1],
                FZ = results.dynaResults[2],
                MX = results.dynaResults[4],
                MY = results.dynaResults[5],
                MZ = results.dynaResults[6]
            };

            return reac;
        }

        /***************************************************/

        public static NodeVelocity FromGsaNodeVelocity(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {
            //TODO: Needs testing
            NodeVelocity disp = new NodeVelocity
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static NodeAcceleration FromGsaNodeAcceleration(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {
            //TODO: Needs testing
            NodeAcceleration disp = new NodeAcceleration
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static BarForce FromGsaBarForce(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarForce force = new BarForce
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                FX = results.dynaResults[0],
                FY = results.dynaResults[1],
                FZ = results.dynaResults[2],
                MX = results.dynaResults[4],
                MY = results.dynaResults[5],
                MZ = results.dynaResults[6]
            };
            return force;
        }

        /***************************************************/

        public static BarStress FromGsaBarStress(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarStress force = new BarStress
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                Axial = results.dynaResults[0],
                ShearY = results.dynaResults[1],
                ShearZ = results.dynaResults[2],
                BendingY_Bot = results.dynaResults[3],
                BendingY_Top = results.dynaResults[4],
                BendingZ_Bot = results.dynaResults[5],
                BendingZ_Top = results.dynaResults[6],
                CombAxialBendingPos = results.dynaResults[7],
                CombAxialBendingNeg = results.dynaResults[8]
            };
            return force;
        }

        /***************************************************/

        public static BarDeformation FromGsaBarDeformation(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarDeformation def = new BarDeformation
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return def;
        }

        /***************************************************/

        public static BarStrain FromGsaBarStrain(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarStrain strain = new BarStrain
            {
                ObjectId = id,
                Case = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                Axial = results.dynaResults[0]
            };
            return strain;
        }

        /***************************************************/

        public static GlobalReactions FromGsaGlobalReactions(string id, string force, string moment)
        {
            string[] fArr = force.Split(',');
            string[] mArr = moment.Split(',');

            return new GlobalReactions()
            {
                Case = "A" + fArr[1],
                FX = double.Parse(fArr[3]),
                FY = double.Parse(fArr[4]),
                FZ = double.Parse(fArr[5]),
                MX = double.Parse(mArr[3]),
                MY = double.Parse(mArr[4]),
                MZ = double.Parse(mArr[5])
            };
        }

        /***************************************************/

        public static ModalDynamics FromGsaModalDynamics(string id, string mode, string frequency, string mass, string stiffness, string damping, string effMassTran, string effMassRot)
        {
            string[] modeArr = mode.Split(',');
            string[] frArr = frequency.Split(',');
            string[] massArr = mass.Split(',');
            string[] stiArr = stiffness.Split(',');
            string[] tranArr = effMassTran.Split(',');
            string[] rotArr = effMassRot.Split(',');
            double damp;
            if (String.IsNullOrWhiteSpace(damping))
                damp = 0;
            else
                damp = double.Parse(damping.Split(',')[2]);

            double totMass = double.Parse(massArr[2]);
            //TODO: Modal damping
            return new ModalDynamics()
            {
                ObjectId = id,
                Case = "A" + modeArr[1],
                ModeNumber = int.Parse(modeArr[2]),
                Frequency = double.Parse(frArr[2]),
                ModalMass = totMass,
                ModalStiffness = double.Parse(stiArr[2]),
                MassRatioX = double.Parse(tranArr[3])/totMass,
                MassRatioY = double.Parse(tranArr[4]) / totMass,
                MassRatioZ = double.Parse(tranArr[5]) / totMass,
                InertiaRatioX = double.Parse(rotArr[3]) / totMass,
                InertiaRatioY = double.Parse(rotArr[4]) / totMass,
                InertiaRatioZ = double.Parse(rotArr[5]) / totMass,
                ModalDamping = damp
            };
        }

        /***************************************************/
    }
}
