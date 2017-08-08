using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structural.Elements;
using BH.oM.Materials;
using BH.oM.Structural.Properties;

using Interop.gsa_8_7;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {


        /***************************************/
        private List<string> FromGsa(out List<Material> materials, List<string> ids = null, bool includeStandard = false)
        {
            string allProps = m_gsa.GwaCommand("GET_ALL, MAT").ToString();
            string[] matArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');
            if (ids == null)
                materials = matArr.Select(x => Convert.GetMaterialFromGsaString(x)).ToList();
            else
                materials = matArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.GetMaterialFromGsaString(x)).ToList();

            if (includeStandard)
                materials.AddRange(Convert.GetStandardGsaMaterials());

            return materials.Select(x => x.CustomData[GSAAdapter.ID].ToString()).ToList();
        }

        /***************************************/

        private List<string> FromGsa(out List<Bar> bars, List<string> ids = null)
        {

            int[] potentialBeamRefs = GeneratePotentialElementIndecies(ids);

            GsaElement[] gsaElements = new GsaElement[potentialBeamRefs.Length];
            m_gsa.Elements(potentialBeamRefs, out gsaElements);

            List<SectionProperty> secPropList;
            List<Node> nodeList;

            FromGsa(out nodeList);
            FromGsa(out secPropList);

            Dictionary<string, SectionProperty> secProps = secPropList.ToDictionary(x => x.CustomData[GSAAdapter.ID].ToString());
            Dictionary<string, Node> nodes = nodeList.ToDictionary(x => x.CustomData[GSAAdapter.ID].ToString());

            bars = Convert.GetBHoMBars(gsaElements, secProps, nodes);
            return bars.Select(x => x.CustomData[GSAAdapter.ID].ToString()).ToList();
        }

        /***************************************/

        private List<string> FromGsa(out List<Node> nodes, List<string> ids = null)
        {

            GsaNode[] gsaNodes;
            m_gsa.Nodes(GeneratePotentialNodeIdIndecies(ids), out gsaNodes);

            nodes = gsaNodes.Select(x => Convert.ToBHoMNode(x)).ToList();
            return nodes.Select(x => x.CustomData[GSAAdapter.ID].ToString()).ToList();

        }

        /***************************************/

        private List<string> FromGsa(out List<SectionProperty> sectionProperties, List<string> ids = null)
        {
            List<Material> matList;
            FromGsa(out matList, null, true);
            Dictionary<string, Material> materials = matList.ToDictionary(x => x.CustomData[GSAAdapter.ID].ToString());

            string allProps = m_gsa.GwaCommand("GET_ALL, PROP_SEC").ToString();
            string[] proArr = string.IsNullOrWhiteSpace(allProps) ? new string[0] : allProps.Split('\n');

            if (ids == null)
                sectionProperties = proArr.Select(x => Convert.GetSectionFromGsaString(x, materials)).ToList();
            else
                sectionProperties = proArr.Where(x => ids.Contains(x.Split(',')[1])).Select(x => Convert.GetSectionFromGsaString(x, materials)).ToList();
            return sectionProperties.Select(x => x.CustomData[GSAAdapter.ID].ToString()).ToList();


        }

    }
}
