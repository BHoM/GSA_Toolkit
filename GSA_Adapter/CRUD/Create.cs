/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.Engine.Base;
using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base;
using BH.oM.Adapter;
using BH.Engine.Adapters.GSA;
using BH.oM.Adapters.GSA.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Fragments;
using BH.oM.Adapters.GSA.Elements;

namespace BH.Adapter.GSA
{
#if GSA_10_1
    public partial class GSA101Adapter
#else 
    public partial class GSA87Adapter
#endif
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            bool success = true;

            if (typeof(RigidLink).IsAssignableFrom(typeof(T)))
                success = CreateLinks(objects as IEnumerable<RigidLink>);
#if GSA_10_1
            else if (typeof(IMaterialFragment).IsAssignableFrom(typeof(T)))
                success = CreateMaterials(objects as IEnumerable<IMaterialFragment>);
#endif
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
            return true;
        }

        /***************************************************/

#if GSA_10_1
        private bool CreateMaterials(IEnumerable<IMaterialFragment> materials)
        {
            bool success = true;
            foreach (IMaterialFragment material in materials)
            {
                string type = Convert.ToGsaString(material.GetType());
                int index = m_gsaCom.GwaCommand("HIGHEST, " + type) + 1;
                this.SetAdapterId(material, $"{type.Split('_').Last()}:{index}");
                success &= ComCall(Convert.IToGsaString(material, index.ToString()));
            }
            return success;
        }

        /***************************************************/

#endif

        private bool CreateObject(BH.oM.Base.IBHoMObject obj)
        {
            return ComCall(Convert.IToGsaString(obj, GetAdapterId<int>(obj).ToString()));
        }

        /***************************************************/

        private bool CreateObject(Node node)
        {
            bool success = true;
            foreach (string str in Convert.ToGsaString(node, GetAdapterId<int>(node).ToString()))
            {
                success &= ComCall(str);
            }
            return success;
        }

        /***************************************************/

        private bool CreateObject(ISectionProperty prop)
        {
            //Try creating a catalogue section
            string catString = prop.CreateCatalogueString();
            bool success = false;
            if (catString != null)
            {
                success = ComCall(catString, false);
#if GSA_10_1
                if (success)
                {
                    //GSA10 reports back a success for a failing section description, and just places "None" for the profile
                    //Pulling back section row to check this. A bit inefficient, but have not found a quicker way to resolve this.
                    string catPropRead = m_gsaCom.GwaCommand("GET, SECTION.7," + prop.GSAId() + ",").ToString();
                    string[] arr = catPropRead.Split(',');
                    if (arr.Length > 21 && arr[21].ToUpper() != "NONE")
                        success = true;
                    else
                        success = false;
                }
#else

                if (success)
                    return true;
#endif
            }

            if (!success)
                success = ComCall(Convert.IToGsaString(prop, GetAdapterId<int>(prop).ToString()));

#if GSA_10_1
            if (success)
            {
                SectionModifier modifier = prop.FindFragment<SectionModifier>();
                if (modifier != null)
                    success &= ComCall(Convert.IToGsaString(modifier, GetAdapterId<int>(prop).ToString()));
            }
#endif

            return success;
        }

        /***************************************************/

        //
        private bool CreateLinks(IEnumerable<RigidLink> links)
        {
            bool success = true;

            foreach (RigidLink link in links)
            {
                if (Convert.CheckRigCon(link) == false)
                {
                    if (link.SecondaryNodes.Count > 1)
                    {
                        List<int> secondaryIds = new List<int>();
                        foreach (Node secondaryNode in link.SecondaryNodes)
                        {
                            int id = (int)NextFreeId(link.GetType(), false);
                            int secondaryIndex = link.SecondaryNodes.IndexOf(secondaryNode);
                            success &= ComCall(Convert.ToGsaString(link, id.ToString(), secondaryIndex));
                            secondaryIds.Add(id);
                        }

                        var allIds = new List<int> { GetAdapterId<int>(link) };
                        allIds.AddRange(secondaryIds);
                        link.Fragments.Remove(typeof(GSAId));
                        link.SetAdapterId(typeof(GSAId), allIds);
                    }
                    else
                    {
                        int id = (int)NextFreeId(link.GetType(), false);
                        success &= ComCall(Convert.ToGsaString(link, id.ToString()));
                        link.Fragments.Remove(typeof(GSAId));
                        link.SetAdapterId(typeof(GSAId), id);
                    }
                }
                else
                {
                    success &= ComCall(Convert.ToGsaString(link, GetAdapterId<int>(link).ToString()));
                }
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
                success &= ComCall(Convert.ToGsaString(mesh, id, i));
                allIds.Add(id);
                id++;
            }

            SetAdapterId(mesh, allIds);

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
                if (gsaString != "")
                    success &= ComCall(gsaString);
            }

            SetAdapterId(load, load.Name ?? "");

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

        private bool CreateObject(FabricPanelProperty fabricProperty)
        {
            bool success = true;

            foreach (string gsaString in fabricProperty.ToGsaStrings(GetAdapterId<int>(fabricProperty).ToString()))
            {
                success &= ComCall(gsaString);
            }
            return success;
        }


        /***************************************************/
        /**** Fallback  Method                         ****/
        /***************************************************/

        private bool CreateObject(BHoMObject obj)
        {
            Compute.RecordError($"Pushing object of type {obj.GetType()} is not implemented for GSA_Toolkit.");
            return false;
        }

        /***************************************************/

    }
}




