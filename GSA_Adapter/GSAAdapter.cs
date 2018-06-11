﻿using Interop.gsa_8_7;
using BH.oM.GSA;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSAAdapter(string filePath = "", GSAConfig gsaConfig = null, bool active = false)
        {
            AdapterId = BH.Engine.GSA.Convert.AdapterID;

            Config.SeparateProperties = true;
            Config.MergeWithComparer = true;
            Config.ProcessInMemory = false;
            Config.CloneBeforePush = true;

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

        private bool ComCall(string str)
        {
            dynamic commandResult = m_gsaCom.GwaCommand(str);

            if (1 == (int)commandResult)
                return true;
            else
            {
                ErrorLog.Add("Failure calling the command: " + str);
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
                ErrorLog.Add("Failure calling the command: " + str);
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
