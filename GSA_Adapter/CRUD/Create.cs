using System;
using System.Collections.Generic;
using BH.oM.Structural.Loads;
using BH.oM.Structural.Elements;
using BH.oM.Base;
using BH.Engine.GSA;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** Index Adapter Interface                   ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            bool success = true;

            if (typeof(T).IsAssignableFrom(typeof(RigidLink)))
                success = CreateLinks(objects as List<RigidLink>);
            else
            {
                foreach (T obj in objects)
                {
                    success &= Create((obj as dynamic));
                }
            }

            UpdateViews();
            return success;
        }

        /***************************************************/

        private bool Create(BH.oM.Base.IObject obj)
        {
            return ComCall(Engine.GSA.Convert.IToGsaString(obj, obj.CustomData[AdapterId].ToString()));
        }

        /***************************************************/

        //Semi hacky method until one to many realationship is bottomed out in the BHoM_Adaptor Replace() method
        private bool CreateLinks(List<RigidLink> links)
        {
            
            bool success = true;
            foreach (RigidLink link in links)
            {
                success &= ComCall(Engine.GSA.Convert.ToGsaString(link, link.CustomData[AdapterId].ToString(), 0));
            }

            foreach (RigidLink link in links)
            {
                List<string> allIds = new List<string>();
                for (int i = 1; i < link.SlaveNodes.Count; i++)
                {
                    string id =  NextId(link.GetType(), i == 1).ToString();
                    success &= ComCall(Engine.GSA.Convert.ToGsaString(link, id, i));
                    allIds.Add(id);
                }
                if (link.SlaveNodes.Count > 1)
                {
                    allIds.Add(link.CustomData[AdapterId].ToString());
                    link.CustomData[AdapterId + "-AllIds"] = allIds;
                }
            }
            return success;
        }

        /***************************************************/

        private bool Create(LoadCombination loadComb)
        {
            bool success = true;
            int higestIndexComb = m_gsaCom.GwaCommand("HIGHEST, ANAL") + 1;
            string combNo = higestIndexComb.ToString();
            string desc = BH.Engine.GSA.Convert.GetCombinationString(loadComb);

            foreach (string gsaString in loadComb.ToGsaString(combNo, desc))
            {
                success &= ComCall(gsaString);
            }
            return success;
        }

        /***************************************************/

        private bool Create(Loadcase loadCase)
        {
            return ComCall(loadCase.ToGsaString());
        }

        /***************************************************/

        private bool Create(ILoad load)
        {
            bool success = true;
            double[] unitFactors = GetUnitFactors();

            foreach (string gsaString in load.IToGsaString(unitFactors))
            {
                success &= ComCall(gsaString);
            }
            return success;
        }

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
    }
}
