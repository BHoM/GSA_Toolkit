using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.gsa_8_7;
using GSA_Adapter.Utility;
using BHP = BHoM.Structural.Properties;

namespace GSA_Adapter.Structural.Properties
{
    public static class LinkPropertyIO
    {
        /*******************************************************/
        /************** Link Property Get Methods **************/
        /*******************************************************/
        
        public static Dictionary<string, BHP.LinkConstraint> GetLinkConstraints(IComAuto gsa, bool nameAsKey = true)
        {
            Dictionary<string, BHP.LinkConstraint> constrs = new Dictionary<string, BHP.LinkConstraint>();
            IEnumerable<string> gsaLinkConstr = GetGsaLinkPropertyStrings(gsa);

            foreach (string gsaConstr in gsaLinkConstr)
            {
                BHP.LinkConstraint constr = GetLinkConstraintFromGsaString(gsaConstr);
                if (nameAsKey)
                {
                    if (!constrs.ContainsKey(constr.Name))
                        constrs.Add(constr.Name, constr);
                }
                else constrs.Add(constr.CustomData[Utils.ID].ToString(), constr);
            }

            return constrs;
        }

        public static BHP.LinkConstraint GetLinkConstraintFromGsaString(string gsaProp)
        {
            BHP.LinkConstraint constraint;
            string[] props = gsaProp.Split(',');
            string name = props[2];
            string type = props[4];
            string id = props[1];

            switch (type)
            {
                case "ALL":
                    constraint = BHP.LinkConstraint.Fixed;
                    break;
                case "PIN":
                    constraint = BHP.LinkConstraint.Pinned;
                    break;
                case "XY_PLANE":
                    constraint = BHP.LinkConstraint.XYPlane;
                    break;
                case "ZX_PLANE":
                    constraint = BHP.LinkConstraint.ZXPlane;
                    break;
                case "YZ_PLANE":
                    constraint = BHP.LinkConstraint.YZPlane;
                    break;
                case "XY_PLANE_PIN":
                    constraint = BHP.LinkConstraint.XYPlanePin;
                    break;
                case "ZX_PLANE_PIN":
                    constraint = BHP.LinkConstraint.ZXPlanePin;
                    break;
                case "YZ_PLANE_PIN":
                    constraint = BHP.LinkConstraint.YZPlanePin;
                    break;
                case "XY_PLATE":
                    constraint = BHP.LinkConstraint.ZPlate;
                    break;
                case "ZX_PLATE":
                    constraint = BHP.LinkConstraint.YPlate;
                    break;
                case "YZ_PLATE":
                    constraint = BHP.LinkConstraint.YPlate;
                    break;
                case "XY_PLATE_PIN":
                    constraint = BHP.LinkConstraint.ZPlatePin;
                    break;
                case "ZX_PLATE_PIN":
                    constraint = BHP.LinkConstraint.YPlatePin;
                    break;
                case "YZ_PLATE_PIN":
                    constraint = BHP.LinkConstraint.ZPlatePin;
                    break;
                default:
                    //String in format example: X:XYY-Y:YZZXX-Z:YY-XX:XX-YY:YY-ZZ:ZZ
                    constraint = new BHP.LinkConstraint();
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
            constraint.CustomData[Utility.Utils.ID] = id;

            return constraint;
        }

        /// <summary>
        /// Gets all section properties in a list of the format [PROP_LINK| num | name | colour | type
        /// </summary>
        /// <param name="gsa"></param>
        /// <returns>A list of strings for all section properties</returns>
        static public IEnumerable<string> GetGsaLinkPropertyStrings(IComAuto gsa)
        {

            string allProps = gsa.GwaCommand("GET_ALL, PROP_LINK").ToString();

            return string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');
        }


        /*******************************************************/
        /************** Link Property Set Methods **************/
        /*******************************************************/

        public static void CreateLinkProperties(IComAuto gsa, List<BHP.LinkConstraint> linkConstraints)
        {
            Dictionary<string, BHP.LinkConstraint> gsaContraints = GetLinkConstraints(gsa, true);

            int highestId = gsa.GwaCommand("HIGHEST, PROP_LINK") + 1;

            foreach (BHP.LinkConstraint constrs in linkConstraints)
            {
                object gsaIdobj;

                //Replace link property at position "id"
                if (constrs.CustomData.TryGetValue(Utils.ID, out gsaIdobj))
                {
                    string id = gsaIdobj.ToString();
                    SetLinkProperty(gsa, constrs, id);
                    if (gsaContraints.ContainsKey(constrs.Name))
                        gsaContraints[constrs.Name] = constrs;
                    else
                        gsaContraints.Add(constrs.Name, constrs);

                }
                //Check if exists, otherwhise add
                else if (!gsaContraints.ContainsKey(constrs.Name))
                {
                    string id = highestId.ToString();
                    highestId++;
                    SetLinkProperty(gsa, constrs, id);
                    constrs.CustomData.Add(Utils.ID, id);
                    gsaContraints.Add(constrs.Name, constrs);
                }
                else
                {
                    constrs.CustomData.Add(Utils.ID, gsaContraints[constrs.Name].CustomData[Utils.ID]);
                }
            }
        }

        public static bool SetLinkProperty(IComAuto gsa, BHP.LinkConstraint constr, string id)
        {
            string command = "PROP_LINK";
            string name = constr.Name;
            string restraint = GetRestraintString(constr);
            string str = command + ", " + id + ", " + name + ", NO_RGB, " + restraint;

            dynamic commandResult = gsa.GwaCommand(str);

            if (1 == (int)commandResult)
            {
            }
            else
            {
                Utils.SendErrorMessage("Application of command " + command + " error. Invalid arguments?");
                return false;
            }
            return true;
        }

        public static string GetRestraintString(BHP.LinkConstraint constr)
        {
            bool[] fixities = constr.GetBoolArray();

            string str = "";

            if (constr.XtoX || constr.XtoYY || constr.XtoZZ)
            {
                str += "X:";

                if (constr.XtoX)
                    str += "X";
                if (constr.XtoYY)
                    str += "YY";
                if (constr.XtoZZ)
                    str += "ZZ";

                str += "-";
            }

            if (constr.YtoY || constr.YtoZZ || constr.YtoZZ)
            {
                str += "Y:";

                if (constr.YtoY)
                    str += "Y";
                if (constr.YtoXX)
                    str += "XX";
                if (constr.YtoZZ)
                    str += "ZZ";

                str += "-";
            }

            if (constr.ZtoZ || constr.ZtoXX || constr.ZtoYY)
            {
                str += "Z:";

                if (constr.ZtoZ)
                    str += "Z";
                if (constr.ZtoXX)
                    str += "XX";
                if (constr.ZtoYY)
                    str += "YY";

                str += "-";
            }

            if (constr.XXtoXX)
                str += "XX:XX-";

            if (constr.YYtoYY)
                str += "YY:YY-";

            if (constr.ZZtoZZ)
                str += "ZZ:ZZ-";


            str = str.TrimEnd('-');

            return str;

        }

    }
}
