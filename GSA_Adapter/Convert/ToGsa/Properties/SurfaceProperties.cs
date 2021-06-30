/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using BH.Engine.Serialiser;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Structure;
using BH.Engine.Adapters.GSA;
using BH.oM.Adapters.GSA.SurfaceProperties;
using BH.oM.Adapters.GSA.FormFindingProperties;
using BH.Engine.Base;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/
       
        private static string ToGsaString(ConstantThickness panProp, string index)
        {
            panProp.Name = panProp.DescriptionOrName().ToGSACleanName();
            string name = panProp.TaggedName();
            string mat = panProp.Material.GSAId().ToString();

            string command = "PROP_2D";
            string colour = "NO_RGB";
            string type = "SHELL";
            string axis = "GLOBAL";
            string thick = panProp.Thickness.ToString();
            string mass = "100%";
            string bending = "100%";
            string inplane = "100%";
            string weight = "0";

            return command + "," + index + "," + name + "," + colour + "," + axis + "," + mat + "," + type + "," + thick + "," + weight + "," + mass + "," + bending + "," + inplane;

        }

        /***************************************************/

        private static string ToGsaString(LoadingPanelProperty panProp, string index)
        {
            string command = "PROP_2D";
            panProp.Name = panProp.DescriptionOrName().ToGSACleanName();
            string name = panProp.TaggedName();
            string colour = "NO_RGB";
            string axis = "0";
            string mat = "0";
            string type = "LOAD";
            string support = GetLoadPanelSupportConditions(panProp.LoadApplication);
            string edge = panProp.ReferenceEdge.ToString();

            return command + "," + index + "," + name + "," + colour + "," + axis + "," + mat + "," + type + "," + support + "," + edge;

        }


        /***************************************************/

        public static List<string> ToGsaStrings(this FabricPanelProperty panProp, string index)
        {
            string command = "PROP_2D";
            panProp.Name = panProp.DescriptionOrName().ToGSACleanName();
            string name = panProp.TaggedName();
            string colour = "NO_RGB";
            string axis = "GLOBAL";
            string mat = panProp.Material.GSAId().ToString();
            string type = "FABRIC";
            string thick = "0.1";
            string mass = panProp.AdditionalMass.ToString();
            string bending = "0";
            string inplane = "100%";
            string weight = "100%";

            string fabricString = command + "," + index + "," + name + "," + colour + "," + axis + "," + mat + "," + type + "," + thick + "," + mass + "," + bending + "," + inplane + "," + weight;

            List<string> gsaStrings = new List<string>();
            gsaStrings.Add(fabricString);

            SoapStress2D stress = panProp.SoapStress;

            if (stress != null)
                gsaStrings.Add(ToGsaString(stress, index));

            return gsaStrings;
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static LoadPanelSupportConditions GetLoadingConditionFromString(string str)
        {
            switch (str)
            {
                case "ALL":
                    return LoadPanelSupportConditions.AllSides;
                case "THREE":
                    return LoadPanelSupportConditions.ThreeSides;
                case "TWO":
                    return LoadPanelSupportConditions.TwoSides;
                case "TWO_ADJ":
                    return LoadPanelSupportConditions.TwoAdjacentSides;
                case "ONE":
                    return LoadPanelSupportConditions.OneSide;
                case "ONE_MOM":
                    return LoadPanelSupportConditions.Cantilever;
                default:
                    return LoadPanelSupportConditions.AllSides;
            }
        }

        /***************************************************/

        private static string GetLoadPanelSupportConditions(LoadPanelSupportConditions cond)
        {
            switch (cond)
            {
                case LoadPanelSupportConditions.AllSides:
                    return "SUP_ALL";
                case LoadPanelSupportConditions.Auto:
                    return "SUP_AUTO";
                case LoadPanelSupportConditions.ThreeSides:
                    return "SUP_THREE";
                case LoadPanelSupportConditions.TwoSides:
                    return "SUP_TWO";
                case LoadPanelSupportConditions.TwoAdjacentSides:
                    return "SUP_TWO_ADJ";
                case LoadPanelSupportConditions.OneSide:
                    return "SUP_ONE";
                case LoadPanelSupportConditions.Cantilever:
                    return "SUP_ONE_MOM";
                default:
                    return "SUP_AUTO";
            }
        }

        /***************************************************/

    }
}



