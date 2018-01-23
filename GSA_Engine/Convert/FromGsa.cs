using BH.Engine.Serialiser;
using BH.Engine.Structure;
using BHM = BH.oM.Common.Materials;
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
                        throw new NotImplementedException();
                }

            }

            secProp.CustomData.Add(AdapterID, id);
            secProp.ApplyTaggedName(gsaStrings[2]);
            secProp.Material = materials[materialId];
            return secProp;
        }

        /***************************************/

        public static Node FromGsaNode(GsaNode gn)
        {
            Node node = new Node { Position = new Point { X = gn.Coor[0], Y = gn.Coor[1], Z = gn.Coor[2] } };
            node.ApplyTaggedName(gn.Name);
            node.CustomData.Add(AdapterID, gn.Ref);

            //Check if the node is restrained in some way
            if (gn.Restraint != 0 || gn.Stiffness.Sum() != 0)
                node.Constraint = GetConstraint(gn.Restraint, gn.Stiffness);

            return node;
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
    }
}
