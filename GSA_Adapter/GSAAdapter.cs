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

using Interop.gsa_8_7;
using BH.oM.GSA;
using System.ComponentModel;
using System;
using BH.oM.Adapter;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter : StructuralAnalysisAdapter
    {
        public override Type ExternalIdFragmentType { get; set; } = typeof(GSAIdFragment);

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSAAdapter(string filePath = "", GSAConfig gsaConfig = null, bool active = false)
        {
            AdapterId = BH.Engine.GSA.Convert.AdapterID;

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
            }
        }


        /***************************************************/
        /**** Private  Methods - Com Interop            ****/
        /***************************************************/

        private bool ComCall(string str, bool raiseError = true)
        {
            dynamic commandResult = m_gsaCom.GwaCommand(str);

            if (1 == (int)commandResult)
                return true;
            else
            {
                if(raiseError)
                    Engine.Reflection.Compute.RecordError("Failure calling the command: " + str);

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
                Engine.Reflection.Compute.RecordError("Failure calling the command: " + str);
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
                            BH.Engine.Reflection.Compute.RecordWarning("No specific steel code of the type " + steelConfig + " existing for the country. Default values used.");
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
                            BH.Engine.Reflection.Compute.RecordWarning("No specific steel code of the type " + steelConfig + " existing for the country. Default values used.");
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
                            BH.Engine.Reflection.Compute.RecordWarning("No specific concrete code of the type " + concConfig + " existing for the country. Default values used.");
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
                            BH.Engine.Reflection.Compute.RecordWarning("No specific Concrete code of the type " + concConfig + " existing for the country. Default values used.");
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
