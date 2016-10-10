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
    public static class GroupIO
    {
        public static bool SetGroups(ComAuto gsa, List<BHB.IGroup> groups, out List<string> ids)
        {
            ids = new List<string>();

            int highestId = gsa.GwaCommand("HIGHEST, LIST") + 1;

            string command = "LIST";

            Dictionary<string, BHB.IGroup> gsaGroups = GetGroups(gsa, false);

            foreach (BHB.IGroup group in groups)
            {

                string groupStr = CreateGroupString(gsa, group);
                
                if (groupStr == null)
                    continue;

                string id;
                if (Utils.CheckAndGetGsaId(group, out id))
                { }
                else if (gsaGroups.ContainsKey(group.Name))
                {
                    id = gsaGroups[group.Name].CustomData[Utils.ID].ToString();
                }
                else
                {
                    id = highestId.ToString();
                    highestId++;
                    group.CustomData[Utils.ID] = id;
                    gsaGroups.Add(group.Name, group);
                }

                string name = group.Name;

                string str = command + ", " + id + ", " + name + ", " + groupStr;

                dynamic commandResult = gsa.GwaCommand(str);

                if (1 == (int)commandResult)
                {
                    ids.Add(id);
                    continue;
                }
                else
                {
                    Utils.CommandFailed(command);
                    return false;
                }

            }

            gsa.UpdateViews();
            return true;
        }

        public static string CreateGroupString(ComAuto gsa, BHB.IGroup group)
        {
            if (group.ObjectType == typeof(BHE.Node))
            {
                return CreateNodeGroupString(gsa, group);
            }
            else if (group.ObjectType == typeof(BHE.Bar))
            {
                return CreateElementGroupString(gsa, group);
            }
            else if (group.ObjectType == typeof(BHE.FEMesh))
            {
                return CreateElementGroupString(gsa, group);
            }

            return null;
        }

        private static string CreateElementGroupString(ComAuto gsa, BHB.IGroup group)
        {
            List<string> strIds;

            string str = "ELEMENT, ";

            if (group is BHB.Group<BHE.Bar>)
            {
                if (!BarIO.GetOrCreateBars(gsa, ((BHB.Group<BHE.Bar>)group).Data, out strIds))
                    return null;


                IEnumerable<int> ids = strIds.Select(x => int.Parse(x));

                str += Utils.GeterateIdString(ids);

                return str;

            }
            else if (group is BHB.Group<BHE.FEMesh>)
            {
                List<int> idItems = new List<int>();
                foreach (BHE.FEMesh mesh in group as BHB.Group<BHE.FEMesh>)
                {
                    foreach (BHE.FEFace face in mesh.Faces)
                    {
                        string id;
                        if (Utils.CheckAndGetGsaId(face, out id))
                            idItems.Add(int.Parse(id));
                        else
                        {
                            Utils.SendErrorMessage("All faces in the mesh needs to have an GSA_id to be able to be assigned to a group");
                            return null;
                        }
                    }
                }

                str += Utils.GeterateIdString(idItems);

                return str;
            }



            return null;
        }

        private static string CreateNodeGroupString(ComAuto gsa, BHB.IGroup group)
        {
            BHB.Group<BHE.Node> nodeGroup = group as BHB.Group<BHE.Node>;
            string str = "NODE, ";

            NodeIO.CreateNodes(gsa, nodeGroup);

            IEnumerable<int> ids = nodeGroup.Select(x => int.Parse(x.CustomData[Utils.ID].ToString())).OrderBy(x => x);

            str += Utils.GeterateIdString(ids);

            return str;
        }


        public static Dictionary<string, BHB.IGroup> GetGroups(ComAuto gsa, bool fillGroups = true)
        {

            Dictionary<string, BHB.IGroup> groups = new Dictionary<string, BHB.IGroup>();


            //LIST,1,List 1,ELEMENT,1 to 2
            List<string> gsaStrings = GetGsaGroupStrings(gsa);

            foreach (string str in gsaStrings)
            {
                BHB.IGroup group = CreateGroupFromString(str, fillGroups);

                groups.Add(group.Name, group);
            }

            return groups;
        }

        private static BHB.IGroup CreateGroupFromString(string str, bool fillGroups)
        {
            string[] arr = str.Split(',');


            BHB.IGroup group = CreateGroupType(arr[3]);

            group.Name = arr[2];
            group.CustomData[Utils.ID] = arr[1];

            if(fillGroups)
                throw new NotImplementedException();

            return group;
        }

        private static BHB.IGroup CreateGroupType(string type)
        {
            switch (type)
            {
                case "NODE":
                    return new BHB.Group<BHE.Node>();
                case "ELEMENT":
                    //Need to be able to check if the elements are panels and/or bars
                    return new BHB.Group<BHE.Bar>();
                default:
                    break;
            }
            return null;
        }




        /// <summary>
        /// Returns a string in the format LIST,'Id','Name','Type','element/node numbers'
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns></returns>
        static public List<string> GetGsaGroupStrings(ComAuto gsa)
        {
            List<string> gsaProps = new List<string>();

            int i = 1;
            int chkCount = 0;
            int abortNum = 1000; //Amount of "" rows after which to abort

            while (chkCount < abortNum)
            {
                string gsaProp = gsa.GwaCommand("GET, LIST," + i).ToString();
                chkCount++;
                i++;

                if (gsaProp != "") //This check is to count the number of consecutive null rows and later abort at a certain number
                {
                    gsaProps.Add(gsaProp);
                    chkCount = 0;
                }
            }

            return gsaProps;
        }

    }
}
