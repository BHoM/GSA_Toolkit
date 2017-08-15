using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using Interop.gsa_8_7;
using BH.Adapter.Strutural;

namespace BH.Adapter.GSA
{
    public partial class Convert
    {
        /***************************************/

        public static List<Bar> FromGsaBars(IEnumerable<GsaElement> gsaElements, Dictionary<string, SectionProperty> secProps, Dictionary<string, Node> nodes)
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

                string name;
                HashSet<string> tags = gsaBar.Name.GetTagsFromString(out name);

                Node n1, n2;
                nodes.TryGetValue(gsaBar.Topo[0].ToString(), out n1);
                nodes.TryGetValue(gsaBar.Topo[1].ToString(), out n2);

                Bar bar = new Bar(n1, n2, name);

                bar.Tags = tags;

                bar.FEAType = feType;

                bar.OrientationAngle = gsaBar.Beta;

                SectionProperty prop;
                secProps.TryGetValue(gsaBar.Property.ToString(), out prop);

                bar.SectionProperty = prop;

                bar.CustomData[GSAAdapter.ID] = gsaBar.Ref;

                barList.Add(bar);

            }
            return barList;
        }

        /***************************************/

        public static Material FromGsaMaterial(string gsaString)
        {
            if (string.IsNullOrWhiteSpace(gsaString))
                return null;

            string[] gStr = gsaString.Split(',');

            if (gStr.Length < 11)
                return null;

            MaterialType type = GetTypeFromString(gStr[2]);

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

            string name;
            HashSet<string> tags = gStr[3].GetTagsFromString(out name);

            Material mat = new Material(name, type, E, v, tC, G, rho);

            mat.Tags = tags;

            mat.CustomData.Add(GSAAdapter.ID, gStr[1].ToString());

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
        public static SectionProperty FromGsaSectionProperty(string gsaString, Dictionary<string, Material> materials)
        {
            SectionProperty secProp = null;

            if (gsaString == "")
            {
                return null;
            }

            string[] gsaStrings = gsaString.Split(',');

            int id;

            Int32.TryParse(gsaStrings[1], out id);

            string name;
            HashSet<string> tags = gsaStrings[2].GetTagsFromString(out name);

            string materialId = gsaStrings[4];
            string description = gsaStrings[5];

            if (description == "EXP")
            {
                //prop = "PROP," + area + "," + Iyy + "," + Izz + "," + J + "," + Avy + "," + Avz;


                ExplicitSectionProperty expSecProp = new ExplicitSectionProperty();
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
                //Temp to get compiling until secproperty is fixed in BHoM_Engine
                secProp = new ExplicitSectionProperty();

                //double D, W, T, t, Wt, Wb, Tt, Tb;
                //string[] desc = description.Split('%');
                //double factor;
                //string type;

                //if (desc[1].Contains("(cm)"))
                //{
                //    factor = 0.01;
                //    type = desc[1].Replace("(cm)", "");
                //}
                //else if (desc[1].Contains("(m)"))
                //{
                //    factor = 1;
                //    type = desc[1].Replace("(m)", "");
                //}
                //else
                //{
                //    factor = 0.001;
                //    type = desc[1];
                //}

                //switch (type)
                //{
                //    case "UC":
                //    case "UB":
                //    case "I":
                //        D = double.Parse(desc[2]) * factor;
                //        W = double.Parse(desc[3]) * factor;
                //        T = double.Parse(desc[4]) * factor;
                //        t = double.Parse(desc[5]) * factor;
                //        secProp = new SteelSection(ShapeType.ISection, /*SectionType.Steel,*/ D, W, T, t, 0, 0);
                //        break;
                //    case "GI":
                //        D = double.Parse(desc[2]) * factor;
                //        Wt = double.Parse(desc[3]) * factor;
                //        Wb = double.Parse(desc[4]) * factor;
                //        Tt = double.Parse(desc[5]) * factor;
                //        Tb = double.Parse(desc[6]) * factor;
                //        t = double.Parse(desc[7]) * factor;
                //        secProp = SectionProperty.CreateISection(BHoM.Materials.MaterialType.Steel, Wt, Wb, D, Tt, Tb, t, 0, 0);
                //        break;
                //    case "CHS":
                //        D = double.Parse(desc[2]) * factor;
                //        t = double.Parse(desc[3]) * factor;
                //        secProp = new SteelSection(ShapeType.Tube, /*SectionType.Steel,*/ D, D, t, t, 0, 0);
                //        break;
                //    case "RHS":
                //        D = double.Parse(desc[2]) * factor;
                //        W = double.Parse(desc[3]) * factor;
                //        T = double.Parse(desc[4]) * factor;
                //        t = double.Parse(desc[5]) * factor;
                //        secProp = new SteelSection(ShapeType.Box, /*SectionType.Steel,*/ D, W, T, t, 0, 0);
                //        break;
                //    case "R":
                //        D = double.Parse(desc[2]) * factor;
                //        W = double.Parse(desc[3]) * factor;
                //        secProp = new SteelSection(ShapeType.Rectangle, D, W, 0, 0, 0, 0);
                //        break;
                //    case "C":
                //        D = double.Parse(desc[2]) * factor;
                //        secProp = SectionProperty.CreateCircularSection(MaterialType.Steel, D);
                //        break;
                //    default:
                //        break;
                //}

            }

            secProp.CustomData.Add(GSAAdapter.ID, id);
            secProp.Name = name;
            secProp.Tags = tags;
            secProp.Material = materials[materialId];
            return secProp;
        }

        /***************************************/

        public static Node FromGsaNode(GsaNode gn)
        {

            string name;
            HashSet<string> tags = gn.Name.GetTagsFromString(out name);

            Node node = new Node(new Point(gn.Coor[0], gn.Coor[1], gn.Coor[2]), name);
            node.Tags = tags;
            node.CustomData.Add(GSAAdapter.ID, gn.Ref);

            //Check if the node is restrained in some way
            if (gn.Restraint != 0 || gn.Stiffness.Sum() != 0)
                node.Constraint = GetConstraint(gn.Restraint, gn.Stiffness);

            return node;
        }
    }
}
