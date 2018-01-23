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

            if (typeof(BH.oM.Base.IObject).IsAssignableFrom(typeof(T)))
            {
                foreach (T obj in objects)
                {
                    success &= ComCall((obj as dynamic));

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

            foreach (string gsaString in load.IToGsaString(unitFactors[1], unitFactors[0]))
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

        //public static string CreateIdListOrGroupName<T>(ComAuto gsa, BHB.Group<T> group) where T : BHB.IBase
        //{
        //    if (!string.IsNullOrWhiteSpace(group.Name))
        //        return "\"" + group.Name + "\"";

        //    List<string> ids;

        //    //if (group is BHB.Group<BHE.Node>)
        //    //    Elements.NodeIO.GetOrCreateNodes(gsa, group as List<BHE.Node>, out ids);
        //    //else
        //    //{
        //    List<BHB.IBase> idItems;

        //    bool isMesh = group is BHB.Group<BHE.FEMesh>;
        //    bool isIareaElement = group is BHB.Group<BHE.IAreaElement>;

        //    if (isMesh)
        //    {
        //        idItems = new List<BHB.IBase>();
        //        foreach (BHE.FEMesh mesh in group as BHB.Group<BHE.FEMesh>)
        //        {
        //            foreach (BHE.FEFace face in mesh.Faces)
        //            {
        //                if (face.CustomData.ContainsKey(Utils.ID))
        //                    idItems.Add(face);
        //                else
        //                    return null;
        //            }
        //        }
        //    }
        //    else if (isIareaElement)
        //    {
        //        idItems = new List<BHB.IBase>();
        //        foreach (BHE.IAreaElement elem in group as BHB.Group<BHE.IAreaElement>)
        //        {
        //            if (!(elem is BHE.FEMesh))
        //            {
        //                Utility.Utils.SendErrorMessage("Mesh is only IAreaElement implemented in GSA");
        //            }
        //            BHE.FEMesh mesh = elem as BHE.FEMesh;

        //            foreach (BHE.FEFace face in mesh.Faces)
        //            {
        //                if (face.CustomData.ContainsKey(Utils.ID))
        //                    idItems.Add(face);
        //                else
        //                    return null;
        //            }
        //        }
        //    }
        //    else
        //    {

        //        List<BHB.IBase> nonIdItems = group.Where(x => !x.CustomData.ContainsKey(Utils.ID)).Select(x => (BHB.IBase)x).ToList();

        //        if (nonIdItems.Count > 0)
        //            return null;

        //        idItems = group.Where(x => x.CustomData.ContainsKey(Utils.ID)).Select(x => (BHB.IBase)x).ToList();
        //    }

        //    ids = idItems.Select(x => x.CustomData[Utils.ID].ToString()).ToList();

        //    IEnumerable<int> intIds = ids.Select(x => int.Parse(x));

        //    return Utils.GeterateIdString(intIds);
        //}
    }
}
