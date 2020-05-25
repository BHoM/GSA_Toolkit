/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base;
using BH.Engine.Adapter;
using BH.oM.Adapter;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            bool success = true;

            if (typeof(Node).IsAssignableFrom(typeof(T)))
                success = CreateNodes(objects as IEnumerable<Node>);
            else if (typeof(RigidLink).IsAssignableFrom(typeof(T)))
                success = CreateLinks(objects as IEnumerable<RigidLink>);
            else
            {
                foreach (T obj in objects)
                {
                    if (typeof(FEMesh).IsAssignableFrom(typeof(T)))
                        success &= CreateFEMesh(obj as FEMesh);
                    else
                        success &= CreateObject((obj as dynamic));
                }
            }

            UpdateViews();
            return success;
        }

        /***************************************************/

        private bool CreateObject(BH.oM.Base.IBHoMObject obj)
        {
            return ComCall(Convert.IToGsaString(obj, obj.CustomData[AdapterIdName].ToString()));
        }

        /***************************************************/

        private bool CreateObject(ISectionProperty prop)
        {
            //Try creating a catalogue section
            string catString = prop.CreateCatalogueString();
            if (catString != null)
            {
                bool success = ComCall(catString, false);
                if (success)
                    return true;
            }

            return ComCall(Convert.IToGsaString(prop, prop.CustomData[AdapterIdName].ToString()));
        }

        /***************************************************/

        //
        private bool CreateLinks(IEnumerable<RigidLink> links)
        {
            
            bool success = true;
            foreach (RigidLink link in links)
            {
                success &= ComCall(Convert.ToGsaString(link, link.CustomData[AdapterIdName].ToString(), 0));
            }

            foreach (RigidLink link in links)
            {
                List<string> allIds = new List<string>();
                for (int i = 1; i < link.SlaveNodes.Count; i++)
                {
                    string id =  NextFreeId(link.GetType(), i == 1).ToString();
                    success &= ComCall(Convert.ToGsaString(link, id, i));
                    allIds.Add(id);
                }
                if (link.SlaveNodes.Count > 1)
                {
                    allIds.Add(link.CustomData[AdapterIdName].ToString());
                    link.CustomData[AdapterIdName + "-AllIds"] = allIds;
                }
            }
            return success;
        }

        /***************************************************/

        private bool CreateNodes(IEnumerable<Node> nodes)
        {
            //Method dealing with axis in GSA. GSA axes are handled as separate obejcts, but can not be dealt with via dependencytypes
            //as the BHoM equivalent is a basis on the node, which is not a BHoMObject but an IGeometry.

            BH.Engine.GSA.BasisComparer orientationComparer = new Engine.GSA.BasisComparer(6);
            Basis baseSystem = Basis.XY;

            List<Node> nonGlobalNodes = nodes.Where(x => x.Orientation != null && !orientationComparer.Equals(x.Orientation, baseSystem)).ToList();

            if (nonGlobalNodes.Count != 0)
            {
                List<CustomObject> existingAxes = ReadAxes();


            }

            bool success = true;
            //Create all nodes
            foreach (Node n in nodes)
            {
                success &= CreateObject(n);
            }
            return success;
        }

        /***************************************************/

        private bool CreateFEMesh(FEMesh mesh)
        {
            bool success = true;
            int id = (int)NextFreeId(mesh.GetType(), true);
            List<int> allIds = new List<int>();

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                success &= ComCall(Convert.ToGsaString(mesh,id,i));
                allIds.Add(id);
                id++;
                //mesh.CustomData[AdapterIdName] = allIds; //TODO: SOLVE THIS
            }

            return success;
        }

        /***************************************************/

        private bool CreateObject(LoadCombination loadComb)
        {
            bool success = true;

            foreach (string gsaString in loadComb.ToGsaString())
            {
                success &= ComCall(gsaString);
            }
            return success;
        }

        /***************************************************/

        private bool CreateObject(Loadcase loadCase)
        {
            return ComCall(loadCase.ToGsaString());
        }

        /***************************************************/

        private bool CreateObject(ILoad load)
        {
            bool success = true;
            double[] unitFactors = GetUnitFactors();

            foreach (string gsaString in load.IToGsaString(unitFactors))
            {
                success &= ComCall(gsaString);
            }
            return success;
        }

        /***************************************************/

        public double[] GetUnitFactors()
        {
            string iUnitFactor = m_gsaCom.GwaCommand("GET_ALL, UNIT_DATA");

            string[] unitStrings = iUnitFactor.Split('\n');

            double[] factors = new double[unitStrings.Length];

            for (int i = 0; i < unitStrings.Length; i++)
            {
                string[] row = unitStrings[i].Split(',');
                double d;
                if (double.TryParse(row[3], out d))
                    factors[i] = d;
            }

            return factors;
        }

        /***************************************************/
    }
}

