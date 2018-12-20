/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = BH.oM.Structural.Elements;
using BHP = BH.oM.Structural.Properties;
using Interop.gsa_8_7;
using GSA_Adapter.Structural.Properties;
using GSA_Adapter.Structural.Elements;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.DesignMembers
{
    public static class BarMemberIO
    {
        public static bool SetBarMember(IComAuto gsa, List<BHE.Bar> bars, out List<string> ids)
        {

            ids = new List<string>();

            DesignRestraintIO.CreateDefaultDesignRestraintProperties(gsa);
            Dictionary<string, string> designPropRefs = DesignPropertyIO.CreateSteelGradeDesignProperties(gsa, bars);

            //Get unique section properties and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHP.SectionProperty> sectionProperties = bars.Select(x => x.SectionProperty).Distinct().ToDictionary(x => x.BHoM_Guid);
            Dictionary<Guid, BHP.SectionProperty> clonedSecProps = Utils.CloneSectionProperties(sectionProperties);


            //Create the section properties
            PropertyIO.CreateSectionProperties(gsa, clonedSecProps.Values.ToList());

            //Get unique nodes and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHE.Node> nodes = bars.SelectMany(x => new List<BHE.Node> { x.StartNode, x.EndNode }).Distinct().ToDictionary(x => x.BHoM_Guid);
            //Dictionary<Guid, BHE.Node> clonedNodes = nodes.Select(x => x.Value.CustomData.ContainsKey(Utils.ID) ? x : new KeyValuePair<Guid, BHE.Node>(x.Key, (BHE.Node)x.Value.ShallowClone())).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<Guid, BHE.Node> clonedNodes = Utils.CloneObjects(nodes);


            //Assign the clones section properties to the abrs
            bars.ForEach(x => x.SectionProperty = clonedSecProps[x.SectionProperty.BHoM_Guid]);

            //Assign the cloned nodes to the bars
            bars.ForEach(x => x.StartNode = clonedNodes[x.StartNode.BHoM_Guid]);
            bars.ForEach(x => x.EndNode = clonedNodes[x.EndNode.BHoM_Guid]);

            //Create nodes
            NodeIO.CreateNodes(gsa, clonedNodes.Values.ToList());

            int highestIndex = gsa.GwaCommand("HIGHEST, MEMB") + 1;

            foreach (BHE.Bar bar in bars)
            {
                if (bar.Material.Type != BHoM.Materials.MaterialType.Steel)
                    continue;

                string command = "MEMB";
                string index = highestIndex.ToString();
                highestIndex++;
                

                string name = bar.Name;
                string type = GetTypeString(bar.StructuralUsage);
                string sectionPropertyIndex = bar.SectionProperty.CustomData[Utils.ID].ToString();
                int group = 0;

                string restraint = GetRestraintGroup(bar);
                

                string startIndex = bar.StartNode.CustomData[Utils.ID].ToString();
                string endIndex = bar.EndNode.CustomData[Utils.ID].ToString();

                string orientationAngle = bar.OrientationAngle.ToString();
                string startR = bar.Release != null ? BarIO.CreateReleaseString(bar.Release.StartConstraint) : "FFFFFF";
                string endR = bar.Release != null ? BarIO.CreateReleaseString(bar.Release.EndConstraint) : "FFFFFF";

                //MEMB.2 | num | name | colour | MT_STEEL | type | section | design | restraint | group | topo(2) | node | angle | is_rls { | rls} is_offset { | Ox | Oy | Oz } 
                string str = command + "," + index + "," + name + "," + "NO_RGB," + "MT_STEEL" + "," + type + "," + sectionPropertyIndex + "," + designPropRefs[bar.Material.Name] + "," + restraint + "," + group + "," + startIndex + "," + endIndex + "," + "0" + "," + "0" + "," + orientationAngle + ",RLS," + startR + "," + endR + ", NO_OFFSET,";


                //MEMB.1 | num | MT_STEEL | type | section | design | restraint | group | topo(2) | node | angle | rls(2) | { Ox | Oy | Oz } 
                //string str = command + "," + index + "," + "MT_STEEL" + "," + type + "," + sectionPropertyIndex + "," +designPropRefs[bar.Material.Name] + "," + restraint + "," + group + "," + startIndex + "," + endIndex + "," + "0" + "," + orientationAngle + ",NRLS,"+ startR + "," +endR + ", NO_OFFSET,";

                //string str = command + "," + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET,";

                dynamic commandResult = gsa.GwaCommand(str); 

                if (1 == (int)commandResult)
                {
                    ids.Add(index);
                    continue;
                }
                else
                {
                    return Utils.CommandFailed(command);
                }
            }

            gsa.UpdateViews();
            return true;

            throw new NotImplementedException();
        }


        private static string GetRestraintGroup(BHE.Bar bar)
        {
            switch (bar.FEAType)
            {
                case BHE.BarFEAType.Axial:
                case BHE.BarFEAType.CompressionOnly:
                case BHE.BarFEAType.TensionOnly:
                    return "2";
                case BHE.BarFEAType.Flexural:
                default:
                    //TODO: Check restraints for how this should be set
                    return "1";
            }
        }


        private static string GetTypeString(BHE.BarStructuralUsage strucUse)
        {
            switch (strucUse)
            {
                case BHE.BarStructuralUsage.Beam:
                    return "MB_BEAM";
                case BHE.BarStructuralUsage.Column:
                    return "MB_COL";
                case BHE.BarStructuralUsage.Undefined:
                case BHE.BarStructuralUsage.Brace:
                case BHE.BarStructuralUsage.Cable:
                case BHE.BarStructuralUsage.Pile:
                default:
                    return "MB_UNDEF";
            }
        }


        public static bool GetBarMembers(ComAuto gsa, out List<BHE.Bar> barList, List<string> barNumbers = null)
        {

            string barStr = gsa.GwaCommand("GET_ALL, MEMB");

            Dictionary<string, BHP.SectionProperty> secProps = PropertyIO.GetSections(gsa, false);

            barList = new List<BHE.Bar>();

            foreach (string str in barStr.Split('\n'))
            {
                string[] barProps = str.Split(',');
                if (barProps.Length < 1)
                    continue;

                

                GsaNode[] gsaNodes;

                int[] topo = new int[] { int.Parse(barProps[10]), int.Parse(barProps[11]) };

                gsa.Nodes(topo, out gsaNodes);
                BHE.Node n1 = new BHE.Node(gsaNodes[0].Coor[0], gsaNodes[0].Coor[1], gsaNodes[0].Coor[2]);
                BHE.Node n2 = new BHE.Node(gsaNodes[1].Coor[0], gsaNodes[1].Coor[1], gsaNodes[1].Coor[2]);


                BHE.Bar bar = new BHE.Bar(n1, n2, barProps[2]);



                bar.OrientationAngle = double.Parse(barProps[14]);



                bar.SectionProperty = secProps[barProps[6]];

                bar.CustomData[Utils.ID] = barProps[1];

                barList.Add(bar);


            }

            return true;
        }

    }
}
