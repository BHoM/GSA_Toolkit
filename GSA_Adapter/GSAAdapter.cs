/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

#if GSA_10_1
using Interop.Gsa_10_1;
#else
using Interop.gsa_8_7;
#endif
using BH.oM.Adapters.GSA;
using System.ComponentModel;
using System;
using BH.Engine.Adapter;
using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Base.Objects;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Base.Attributes;
using BH.Engine.Structure;
using BH.oM.Adapters.GSA.SpacerProperties;
using BH.oM.Adapters.GSA.Elements;

namespace BH.Adapter.GSA
{
#if GSA_10_1
    public partial class GSA101Adapter : BHoMAdapter
#else
    public partial class GSA87Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

#if GSA_10_1
        public GSA101Adapter(string filePath = "", GSAConfig gsaConfig = null, bool active = false)
#else
        public GSA87Adapter(string filePath = "", GSAConfig gsaConfig = null, bool active = false)
#endif
        {
            AdapterIdFragmentType = typeof(GSAId);

            Modules.Structure.ModuleLoader.LoadModules(this);

            this.AdapterModules.Add(new BH.Engine.Adapters.GSA.Modules.GetPanelNodes());
            this.AdapterModules.Add(new BH.Engine.Adapters.GSA.Modules.GetNodeSpings());

            AdapterComparers = new Dictionary<Type, object>
            {
                {typeof(Bar), new BarEndNodesDistanceComparer(3) },
                {typeof(Node), new NodeDistanceComparer(3) },
                {typeof(ISectionProperty), new NameOrDescriptionComparer() },
                {typeof(ISurfaceProperty), new NameOrDescriptionComparer() },
                {typeof(IMaterialFragment), new NameOrDescriptionComparer() },
                {typeof(LinkConstraint), new NameOrDescriptionComparer() },
                {typeof(SpacerProperty), new NameOrDescriptionComparer() },
#if GSA_10_1
                {typeof(Constraint6DOF), new NameOrDescriptionComparer() },
#endif
            };

            DependencyTypes = new Dictionary<Type, List<Type>>
            {
                {typeof(BH.oM.Structure.Loads.IElementLoad<Node>), new List<Type> { typeof(Node), typeof(BH.oM.Structure.Loads.Loadcase) } },
                {typeof(BH.oM.Structure.Loads.IElementLoad<Bar>), new List<Type> { typeof(Bar), typeof(BH.oM.Structure.Loads.Loadcase) } },
                {typeof(BH.oM.Structure.Loads.ILoad), new List<Type> { typeof(BH.oM.Structure.Loads.Loadcase) } },
                {typeof(BH.oM.Structure.Loads.LoadCombination), new List<Type> { typeof(BH.oM.Structure.Loads.Loadcase) } },
                {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } },
                {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(RigidLink), new List<Type> { typeof(LinkConstraint), typeof(Node) } },
                {typeof(FEMesh), new List<Type> { typeof(ISurfaceProperty), typeof(Node) } },
                {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(Spacer), new List<Type> { typeof(SpacerProperty), typeof(Node) } },
                {typeof(Panel), new List<Type> { typeof(ISurfaceProperty), typeof(Node) } },
                {typeof(RigidConstraint), new List<Type> { typeof(Node) } },
#if GSA_10_1
                {typeof(Node), new List<Type>{typeof(Constraint6DOF) } }
#endif
            };

            if (active)
            {
                m_gsaCom = new ComAuto();

                short result;
                if (!string.IsNullOrWhiteSpace(filePath))
                    result = m_gsaCom.Open(filePath);
                else
                    result = m_gsaCom.NewFile();

                if (gsaConfig != null)
                    SetConfig(gsaConfig);

#if GSA_10_1
                m_gsaCom.DisplayGsaWindow(true);
#endif
            }
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/



        /***************************************************/
        /**** Private  Methods - Com Interop            ****/
        /***************************************************/

        private bool ComCall(string str, bool raiseError = true)
        {
            dynamic commandResult;

            if (!string.IsNullOrWhiteSpace(str))
                commandResult = m_gsaCom.GwaCommand(str);
            else
                return false;

            if (1 == (int)commandResult)
                return true;
            else
            {
                if (raiseError)
                    Engine.Base.Compute.RecordError("Failure calling the command: " + str);

                return false;
            }
        }

        /***************************************************/

        private T ReturnComCall<T>(string str)
        {
            dynamic commandResult = m_gsaCom.GwaCommand(str);

            T returnVar = (T)commandResult;

            if (returnVar != null)
                return returnVar;
            else
            {
                Engine.Base.Compute.RecordError("Failure calling the command: " + str);
                return default(T);
            }
        }

        /***************************************************/

        private void SetConfig(GSAConfig config)
        {
            SetSteelDesign(config);
            SetConcreteDesign(config);
        }

        /***************************************************/

        private void SetSteelDesign(GSAConfig config)
        {
            string steelConfig = "";
            switch (config.SteelDesign)
            {
                case SteelDesignSpecification.Eurocode1993:
                    steelConfig = "EN 1993-1-1:2005 Eurocode 3";
                    switch (config.Country)
                    {
                        case Country.UK:
                            steelConfig += " (UK)";
                            break;
                        case Country.NL:
                            steelConfig += " (NL)";
                            break;
                        case Country.France:
                            steelConfig += " (FR)";
                            break;
                        case Country.Singapore:
                        case Country.Italy:
                        case Country.Denmark:
                        case Country.US:
                        case Country.Germany:
                            BH.Engine.Base.Compute.RecordWarning("No specific steel code of the type " + steelConfig + " existing for the country. Default values used.");
                            break;
                        case Country.Undefined:
                        default:
                            break;
                    }
                    break;
                case SteelDesignSpecification.ASCI:
                    steelConfig = "AISC10_LRFD";
                    switch (config.Country)
                    {
                        case Country.US:
                        case Country.Undefined:
                            break;
                        default:
                            BH.Engine.Base.Compute.RecordWarning("No specific steel code of the type " + steelConfig + " existing for the country. Default values used.");
                            break;
                    }
                    break;
                default:
                    return;
            }

            m_gsaCom.GwaCommand("SPEC_STEEL_DESIGN," + steelConfig);
        }

        /***************************************************/

        private void SetConcreteDesign(GSAConfig config)
        {
            string concConfig = "";
            string country = "";
            switch (config.ConcreteDesign)
            {
                case ConcreteDesignSpecification.Eurocode1992:
                    concConfig = "EC2_04";
                    switch (config.Country)
                    {
                        case Country.UK:
                            country = "GB";
                            break;
                        case Country.NL:
                            country = "NL";
                            break;
                        case Country.France:
                            country = "FR";
                            break;
                        case Country.Singapore:
                            country = "SG";
                            break;
                        case Country.Italy:
                            country = "IT";
                            break;
                        case Country.Denmark:
                            country = "DK";
                            break;
                        case Country.Germany:
                            country = "DE";
                            break;
                        case Country.US:
                            country = "EU";
                            BH.Engine.Base.Compute.RecordWarning("No specific concrete code of the type " + concConfig + " existing for the country. Default values used.");
                            break;
                        case Country.Undefined:
                        default:
                            country = "EU";
                            break;
                    }
                    break;
                case ConcreteDesignSpecification.ACI:
                    concConfig = "ACI318_11";
                    country = "US";
                    switch (config.Country)
                    {
                        case Country.US:
                        case Country.Undefined:
                            break;
                        default:
                            BH.Engine.Base.Compute.RecordWarning("No specific Concrete code of the type " + concConfig + " existing for the country. Default values used.");
                            break;
                    }
                    break;
                default:
                    return;
            }

            m_gsaCom.GwaCommand("SPEC_CONC_DESIGN," + concConfig + "," + country);
        }

        /***************************************************/
        private void UpdateViews()
        {
            m_gsaCom.UpdateViews();
        }


        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private ComAuto m_gsaCom;

        /***************************************************/
    }
}




