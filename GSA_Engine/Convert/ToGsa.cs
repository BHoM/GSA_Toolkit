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

using BH.Engine.Serialiser;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Common;
using BH.Engine.Adapter;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static string ToGsaString(this Type type)
        {
            if (type == typeof(Node))
                return "NODE";
            else if (type == typeof(Bar))
                return "EL";
            else if (type == typeof(IMaterialFragment))
                return "MAT";
            else if (type == typeof(ISectionProperty))
                return "PROP_SEC";
            else if (type == typeof(ISurfaceProperty))
                return "PROP_2D";
            else if (type == typeof(FEMesh))
                return "EL";
            else if (type == typeof(RigidLink))
                return "EL";
            else if (type == typeof(LinkConstraint))
                return "PROP_LINK";
            else if (type.IsGenericType && type.Name == typeof(BHoMGroup<IBHoMObject>).Name)
                return "List";
            return null;
        }

        /***************************************************/

        public static List<string> ToGsaString(this LoadCombination loadComb)
        {
            //TODO: Implement pushing of combinations of combinations
            string desc = loadComb.CombinationString();
            string combNo = loadComb.Number.ToString();
            List<string> gsaStrings = new List<string>();
            gsaStrings.Add(AnalysisCase(combNo, loadComb.Name, combNo, desc));
            string type = Query.TaskType(loadComb);
            gsaStrings.Add(AnalysisTask(combNo, loadComb.Name, type, "0", combNo));

            return gsaStrings;
        }

        /***************************************************/

        public static List<string> ToGsaString(this Load<Node> nodeLoad, double[] unitFactors)
        {
            string command;
            List<string> forceStrings = new List<string>();
            double factor;

            Vector[] force = nodeLoad.ITranslationVector();
            command = nodeLoad.IForceTypeString();
            Vector[] moment = nodeLoad.IRotationVector();
            factor = nodeLoad.IFactor(unitFactors);

            string name = nodeLoad.Name;
            string str;
            string appliedTo = nodeLoad.CreateIdListOrGroupName();
            string caseNo = nodeLoad.Loadcase.Number.ToString();
            string axis = Query.IsGlobal(nodeLoad);
            string[] pos = { ("," + nodeLoad.ILoadPosition()[0]), ("," + nodeLoad.ILoadPosition()[1]) };

            str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis;

            VectorDataToString(str, force, ref forceStrings, factor, true, pos);
            VectorDataToString(str, moment, ref forceStrings, factor, false, pos);

            return forceStrings;
        }

        /***************************************************/

        public static List<string> ToGsaString(this Load<Bar> barLoad, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();

            string name = barLoad.Name;
            string projection = barLoad.IsProjectedString();
            string axis = barLoad.IsGlobal();
            double factor = barLoad.IFactor(unitFactors);
            string appliedTo = barLoad.CreateIdListOrGroupName();
            Vector[] force = { barLoad.ITranslationVector()[0], barLoad.ITranslationVector()[1]};
            Vector[] moment = { barLoad.IRotationVector()[0], barLoad.IRotationVector()[1] };
            string caseNo = barLoad.Loadcase.Number.ToString();
            string command = barLoad.IForceTypeString();
            string[] pos = { ("," + barLoad.ILoadPosition()[0]), ("," + barLoad.ILoadPosition()[1]) };

            if (appliedTo == null)
                return null;

            string str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis + "," + projection;

            VectorDataToString(str, force, ref forceStrings, factor, true, pos);
            VectorDataToString(str, moment, ref forceStrings, factor, false, pos);

            return forceStrings;
        }

        /***************************************************/

        public static List<string> ToGsaString(this Load<IAreaElement> areaLoad, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();

            string name = areaLoad.Name;
            string projection = areaLoad.IsProjectedString();
            string axis = areaLoad.IsGlobal();
            double factor = areaLoad.IFactor(unitFactors);
            string appliedTo = areaLoad.CreateIdListOrGroupName();
            Vector[] force = { areaLoad.ITranslationVector()[0], areaLoad.ITranslationVector()[1] };
            string caseNo = areaLoad.Loadcase.Number.ToString();
            string command = areaLoad.IForceTypeString();
            string[] pos = { ("," + areaLoad.ILoadPosition()[0]), ("," + areaLoad.ILoadPosition()[1]) };
            string type = areaLoad.IAreaLoadTypeString();
            if (appliedTo == null)
                return null;

            string str = command + "," + name + "," + appliedTo + "," + caseNo + "," + axis + ","  + type + "," + projection;

            VectorDataToString(str, force, ref forceStrings, factor, true, pos);

            return forceStrings;
        }

        /***************************************************/
        public static List<string> ToGsaString(this GravityLoad load, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();
            string command = load.IForceTypeString();
            string name = load.Name;
            string list = load.CreateIdListOrGroupName();

            string caseNo = load.Loadcase.Number.ToString();

            string x = load.GravityDirection.X.ToString();
            string y = load.GravityDirection.Y.ToString();
            string z = load.GravityDirection.Z.ToString();

            string str = command + ",," + list + "," + caseNo + "," + x + "," + y + "," + z;
            forceStrings.Add(str);
            return forceStrings;
        }

        /***************************************************/
        public static List<string> ToGsaString(this BarPrestressLoad load, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();
            string command = load.IForceTypeString();
            string name = load.Name;
            string list = load.CreateIdListOrGroupName();
            string caseNo = load.Loadcase.Number.ToString();
            double value = load.Prestress;

            string str = command + ",," + list + "," + caseNo + "," + value * unitFactors[(int)UnitType.FORCE];
            forceStrings.Add(str);
            return forceStrings;
        }

        /***************************************************/
        public static List<string> ToGsaString(this BarTemperatureLoad load, double[] unitFactors)
        {
            List<string> forceStrings = new List<string>();
            string command = load.IForceTypeString();
            string name = load.Name;
            string list = load.CreateIdListOrGroupName();
            string caseNo = load.Loadcase.Number.ToString();
            string type = "CONS";
            string value = load.TemperatureChange.ToString();

            string str = command + ",," + list + "," + caseNo + "," + type + "," + value;
            forceStrings.Add(str);
            return forceStrings;
        }

        /***************************************************/

        public static List<string> IToGsaString(this ILoad load, double[] unitFactors)
        {
            return ToGsaString(load as dynamic, unitFactors);
        }

        /***************************************************/

        static public string ToGsaString(this Loadcase loadCase)
        {
            string title = loadCase.Name; ;
            string type = Query.LoadNatureString(loadCase.Nature);

            string str;
            string command = "LOAD_TITLE.1";
            string bridge = "BRIDGE_NO";

            str = command + "," + loadCase.Number + "," + title + "," + type + "," + bridge;
            return str;
        }

        /***************************************************/

        private static string ToGsaString(this IMaterialFragment material, string index)
        {


            if (material is IIsotropic)
            {
                IIsotropic isotropic = material as IIsotropic;
                string command = "MAT";
                string num = index;
                string mModel = "MAT_ELAS_ISO";
                string name = material.TaggedName();
                string colour = "NO_RGB";
                string type = GetMaterialType(material).ToString();
                string E = isotropic.YoungsModulus.ToString();
                string nu = isotropic.PoissonsRatio.ToString();
                string rho = isotropic.Density.ToString();
                string alpha = isotropic.ThermalExpansionCoeff.ToString();
                string G = isotropic.ShearModulus().ToString();
                string damp = isotropic.DampingRatio.ToString();

                string str = command + "," + num + "," + mModel + "," + name + "," + colour + "," + type + ",6," + E + "," + nu + "," + rho + "," + alpha + "," + G + "," + damp + ",0,0,NO_ENV";
                return str;
            }
           else
            {
                Engine.Reflection.Compute.RecordWarning("GSA_Toolkit does currently only suport Isotropic material. Material with name " + material.Name + " have been NOT been pushed");
                return "";
            }
        }

        /***************************************/

        private static string ToGsaString(this Bar bar, string index)
        {
            string command = "EL.2";
            string name = bar.TaggedName();
            string type = GetElementTypeString(bar);

            string sectionPropertyIndex = bar.SectionProperty != null ? bar.SectionProperty.CustomData[AdapterIdName].ToString() : "1";
            int group = 0;

            string startIndex = bar.StartNode.CustomData[AdapterIdName].ToString();
            string endIndex = bar.EndNode.CustomData[AdapterIdName].ToString();

            string orientationAngle = (bar.OrientationAngle * 180 / Math.PI).ToString();
            // TODO: Make sure that these are doing the correct thing. Release vs restraint corresponding to true vs false
            string startR = bar.Release != null ? CreateReleaseString(bar.Release.StartRelease) : "FFFFFF";
            string endR = bar.Release != null ? CreateReleaseString(bar.Release.EndRelease) : "FFFFFF";
            string dummy = CheckDummy(bar);

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + sectionPropertyIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0 ," + orientationAngle + ", RLS, " + startR + " , " + endR + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************/

        private static string ToGsaString(this Node node, string index)
        {
            string command = "NODE.2";
            string name = node.TaggedName();

            string restraint = GetRestraintString(node);
            Point position = node.Position();
            string str = command + ", " + index + ", " + name + " , NO_RGB, " + position.X + " , " + position.Y + " , " + position.Z + ", NO_GRID, " + 0 + "," + restraint;
            return str;
        }

        /***************************************/

        private static string ToGsaString(this ISectionProperty prop, string index)
        {
            string name = prop.TaggedName();

            string mat = prop.Material.CustomData[AdapterIdName].ToString();// materialId;  //"STEEL";// material.Name;

            string desc;
            string props;

            if (!ICreateDescAndPropString(prop, out desc, out props))
                return "";

            string command = "PROP_SEC";
            string colour = "NO_RGB";
            string principle = "NO";
            string type = prop.SectionType();
            string cost = "0";
            string plate_type = "FLAME_CUT";
            string calc_J = "NO_J";
            string mods = prop.ModifiersString();

            //PROP_SEC    2   Section 2   NO_RGB  1   CAT % UB % UB914x419x388 % 19990407   NO NA  0   NO_PROP NO_MOD_PROP FLAME_CUT NO_J

            string str = command + "," + index + "," + name + "," + colour + "," + mat + "," + desc + "," + principle + "," + type + "," + cost + "," + props + "," + mods + "," + plate_type + "," + calc_J;
            return str;
        }

        /***************************************/

        private static string ToGsaString(ConstantThickness panProp, string index)
        {

            string name = panProp.TaggedName();
            string mat = panProp.Material.CustomData[AdapterIdName].ToString();


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

        /***************************************/

        private static string ToGsaString(LinkConstraint constraint, string index)
        {

            string command = "PROP_LINK";
            string name = constraint.Name;
            string restraint = GetRestraintString(constraint);
            return command + ", " + index + ", " + name + ", NO_RGB, " + restraint;
        }

        /***************************************/

        private static string ToGsaString(LoadingPanelProperty panProp, string index)
        {
            string command = "PROP_2D";
            string name = panProp.TaggedName();
            string colour = "NO_RGB";
            string axis = "0";
            string mat = "0";
            string type = "LOAD";
            string support = GetLoadPanelSupportConditions(panProp.LoadApplication);
            string edge = panProp.ReferenceEdge.ToString();

            return command + "," + index + "," + name + "," + colour + "," + axis + "," + mat + "," + type + "," + support + "," + edge;

        }

        /***************************************/

        public static string ToGsaString(this RigidLink link, string index, int slaveIndex = 0)
        {
            string command = "EL.2";
            string name = link.TaggedName();
            string type = "LINK";

            string constraintIndex = link.Constraint.CustomData[AdapterIdName].ToString();
            string group = "0";

            string startIndex = link.MasterNode.CustomData[AdapterIdName].ToString();

            string endIndex = link.SlaveNodes[slaveIndex].CustomData[AdapterIdName].ToString();  

            string dummy = CheckDummy(link);


            //EL	1	gfdgfdg	NO_RGB	LINK	1	1	1	2	0	0	NO_RLS	NO_OFFSET	DUMMY
            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + constraintIndex + ", " + group + ", " + startIndex + ", " + endIndex + " , 0" + ",0" + ", NO_RLS" + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************/


        public static string ToGsaString(this FEMesh mesh, int index, int faceID)
        {

            string command = "EL.2";
            string type;

            FEMeshFace face = mesh.Faces[faceID];
            face.CustomData[AdapterIdName] = index;

            //TODO: Implement QUAD8 and TRI6
            if (face.NodeListIndices.Count == 3)
                type = "TRI3";
            else if (face.NodeListIndices.Count == 4)
                type = "QUAD4";
            else
                return "";

            string name = mesh.TaggedName();

            string propertyIndex = mesh.Property.CustomData[AdapterIdName].ToString();
            int group = 0;

            string topology = "";

            foreach (int nodeIndex in face.NodeListIndices)
            {
                topology += mesh.Nodes[nodeIndex].CustomData[AdapterIdName].ToString() + ",";
            }

            string dummy = CheckDummy(face);
            //EL	1	gfdgdf	NO_RGB	QUAD4	1	1	1	2	3	4	0	0	NO_RLS	NO_OFFSET	DUMMY
            //EL  2       NO_RGB TRI3    1   1   1   2   5   0   0   NO_RLS NO_OFFSET   DUMMY

            string str = command + ", " + index + "," + name + ", NO_RGB , " + type + " , " + propertyIndex + ", " + group + ", " + topology + " 0 , 0" + ", NO_RLS" + ", NO_OFFSET," + dummy;
            return str;
        }

        /***************************************/

        private static string ToGsaString<T>(this BH.oM.Base.BHoMGroup<T> group, string index) where T: BH.oM.Base.IBHoMObject
        {
            string command = "LIST";
            string name = group.Name;
            string type = group.IElementType();
            string desc = group.Elements.Select(x => int.Parse(x.CustomData[AdapterIdName].ToString())).GeterateIdString();

            return command + ", " + index + ", " + name + ", " + type +", " + desc;
        }

        /***************************************************/
        /**** Public Interface Methods                  ****/
        /***************************************************/

        public static string IToGsaString(this object obj, string index)
        {
            return ToGsaString(obj as dynamic, index);
        }

        /***************************************************/
        /**** Private fallback                          ****/
        /***************************************************/

        public static string ToGsaString(this object obj, string index)
        {
            Engine.Reflection.Compute.RecordWarning("Objects of type " + obj.GetType().Name + " are not suppoerted in this Adapter");
            return "";
        }

        /***************************************/

        public static string ToGsaString(this Panel obj, string index)
        {
            Engine.Reflection.Compute.RecordWarning("GSA has no meshing capabilities and does therefor not support Panel objects. Try meshing the panel and create a FEMesh and then push again");
            return "";
        }

        /***************************************/
    }
}
