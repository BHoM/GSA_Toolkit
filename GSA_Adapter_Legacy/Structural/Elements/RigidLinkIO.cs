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
using Interop.gsa_8_7;
using BHB = BHoM.Base;
using BHG = BHoM.Global;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;
using BHM = BHoM.Materials;
using GSA_Adapter.Structural.Properties;
using GSA_Adapter.Utility;

namespace GSA_Adapter.Structural.Elements
{
    public static class RigidLinkIO
    {
        public static bool CreateRigidLinks(ComAuto gsa, List<BHE.RigidLink> links, out List<string> ids)
        {
            ids = new List<string>();

            //Shallowclone the bars and their custom data
            links.ForEach(x => x = (BHE.RigidLink)x.ShallowClone());
            links.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            //Get unique section properties and clone the ones that does not contain a gsa ID
            Dictionary<Guid, BHP.LinkConstraint> linkConstraints = links.Select(x => x.Constraint).Distinct().ToDictionary(x => x.BHoM_Guid);
            Dictionary<Guid, BHP.LinkConstraint> clonedConstraints = Utils.CloneObjects(linkConstraints);

            //Create the section properties
            LinkPropertyIO.CreateLinkProperties(gsa, clonedConstraints.Values.ToList());

            //Assign newly created link properties to links
            links.ForEach(x => x.Constraint = clonedConstraints[x.Constraint.BHoM_Guid]);

            //Clone nodes
            List<string> nodeIds = new List<string>();
            List<BHE.Node> nodes = links.Select(x => x.MasterNode).ToList();
            links.ForEach(x => nodes.AddRange(x.SlaveNodes));
            Dictionary<Guid, BHE.Node> clonedNodes = Utils.CloneObjects(nodes.Distinct().ToDictionary(x => x.BHoM_Guid));

            links.ForEach(x => x.MasterNode = clonedNodes[x.MasterNode.BHoM_Guid]);

            foreach (BHE.RigidLink link in links)
            {
                List<BHE.Node> newNodes = new List<BHE.Node>();
                for (int i = 0; i < link.SlaveNodes.Count; i++)
                {
                    newNodes.Add(clonedNodes[link.SlaveNodes[i].BHoM_Guid]);
                }
                link.SlaveNodes = newNodes;
            }

            //Create nodes
            NodeIO.CreateNodes(gsa, clonedNodes.Values.ToList());


            int highestElemIndex = gsa.GwaCommand("HIGHEST, EL") + 1;
            int highestConstIndex = gsa.GwaCommand("HIGHEST, RIGID") + 1;


            foreach (BHE.RigidLink link in links)
            {
                if (link.SlaveNodes.Count < 1)
                    continue;
                else if (link.SlaveNodes.Count == 1)
                {
                    string propId = link.Constraint.CustomData[Utils.ID].ToString();
                    if (CreateRigidLink(gsa, link, highestElemIndex.ToString(), propId))
                        highestElemIndex++;
                    else return false;
                }
                else
                {
                    if (CreateRigidConstraint(gsa, link, highestConstIndex.ToString()))
                        highestConstIndex++;
                    else return false;
                }
            }

            gsa.UpdateViews();
            return true;
        }

        private static bool CreateRigidLink(ComAuto gsa, BHE.RigidLink link, string index, string propId)
        {
            string command = "EL_LINK";

            string name = link.Name;
            int group = 0;

            string startIndex = link.MasterNode.CustomData[Utils.ID].ToString();// nodeIds[0];

            string dummy = "";

            if (link.CustomData.ContainsKey("Dummy") && (bool)link.CustomData["Dummy"])
                dummy = "DUMMY";


            string endIndex = link.SlaveNodes[0].CustomData[Utils.ID].ToString();// nodeIds[1];

            string str = command + ", " + index + ", " + propId + ", " + group + ", " + startIndex + ", " + endIndex + ", " + dummy;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                return true;
            }
            else
            {
                return Utils.CommandFailed(command);
            }
        }

        private static bool CreateRigidConstraint(ComAuto gsa, BHE.RigidLink link, string index)
        {
            string command = "RIGID.2";

            string name = link.Name;

            string startIndex = link.MasterNode.CustomData[Utils.ID].ToString();// nodeIds[0];

            string slaves = RigidConstSlaveNodes(link.SlaveNodes);

            string type = LinkPropertyIO.GetRestraintString(link.Constraint);
            string stage = "all";

            //RIGID.2 | name | master | type | slaves | stage 

            string str = command + ", " + name + "," + startIndex + "," + type + "," + slaves + "," + stage;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
                return true;
            }
            else
            {
                return Utils.CommandFailed(command);
            }
        }

        private static string RigidConstSlaveNodes(List<BHE.Node> slaves)
        {
            string slaveStr = "";

            foreach (BHE.Node slave in slaves)
            {
                slaveStr += slave.CustomData[Utils.ID].ToString() + " ";
            }

            return slaveStr;
        }

    }
}
