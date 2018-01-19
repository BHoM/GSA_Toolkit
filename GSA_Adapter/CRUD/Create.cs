using System;
using System.Collections.Generic;
using BH.oM.Structural.Loads;
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

            if (typeof(BH.oM.Base.BHoMObject).IsAssignableFrom(typeof(T)))
            {
                foreach (T obj in objects)
                {
                    success &= Create(obj as dynamic);
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

            foreach (string gsaString in load.IToGsaString())
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
