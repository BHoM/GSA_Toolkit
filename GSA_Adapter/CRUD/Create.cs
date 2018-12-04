using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
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

            if (typeof(RigidLink).IsAssignableFrom(typeof(T)))
                success = CreateLinks(objects as IEnumerable<RigidLink>);
            else
            {
                foreach (T obj in objects)
                {
                    if (typeof(FEMesh).IsAssignableFrom(typeof(T)))
                        success &= CreateFEMesh(obj as FEMesh);
                    else
                        success &= Create((obj as dynamic));
                }
            }

            UpdateViews();
            return success;
        }

        /***************************************************/

        private bool Create(BH.oM.Base.IBHoMObject obj)
        {
            return ComCall(Engine.GSA.Convert.IToGsaString(obj, obj.CustomData[AdapterId].ToString()));
        }

        /***************************************************/

        private bool Create(ISectionProperty prop)
        {
            //Try creating a catalogue section
            string catString = prop.CreateCatalogueString();
            if (catString != null)
            {
                bool success = ComCall(catString);
                if (success)
                    return true;
            }

            return ComCall(Engine.GSA.Convert.IToGsaString(prop, prop.CustomData[AdapterId].ToString()));
        }

        /***************************************************/

        //
        private bool CreateLinks(IEnumerable<RigidLink> links)
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

        private bool CreateFEMesh(FEMesh mesh)
        {
            bool success = true;
            string id = NextId(mesh.GetType(), true).ToString();
            List<string> allIds = new List<string>();

            for (int i = 0; i < mesh.MeshFaces.Count; i++)
            {
                success &= ComCall(Engine.GSA.Convert.ToGsaString(mesh,id,i));
                allIds.Add(id);
                id = (int.Parse(id) + 1).ToString();
                mesh.CustomData[AdapterId + "-AllIds"] = allIds;
            }

            return success;
        }

        /***************************************************/

        private bool Create(LoadCombination loadComb)
        {
            bool success = true;

            foreach (string gsaString in loadComb.ToGsaString())
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
