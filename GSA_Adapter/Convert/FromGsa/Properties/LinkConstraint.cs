/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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


using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Constraints;
using System.Linq;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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
                    constraint = Engine.Structure.Create.LinkConstraintFixed();
                    break;
                case "PIN":
                    constraint = Engine.Structure.Create.LinkConstraintPinned();
                    break;
                case "XY_PLANE":
                    constraint = Engine.Structure.Create.LinkConstraintXYPlane();
                    break;
                case "ZX_PLANE":
                    constraint = Engine.Structure.Create.LinkConstraintZXPlane();
                    break;
                case "YZ_PLANE":
                    constraint = Engine.Structure.Create.LinkConstraintYZPlane();
                    break;
                case "XY_PLANE_PIN":
                    constraint = Engine.Structure.Create.LinkConstraintXYPlanePin();
                    break;
                case "ZX_PLANE_PIN":
                    constraint = Engine.Structure.Create.LinkConstraintZXPlanePin();
                    break;
                case "YZ_PLANE_PIN":
                    constraint = Engine.Structure.Create.LinkConstraintYZPlanePin();
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
            constraint.SetAdapterId(typeof(GSAId), int.Parse(id));

            return constraint;
        }

        /***************************************************/
    }
}

