using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using Interop.gsa_8_7;
using BH.oM.Geometry;
using BH.Engine.Structure;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        private static string GetRestraintString(Node node)
        {
            if (node.Constraint != null)
            {
                string rest = "REST";


                bool[] fixities = node.Constraint.GetFixities();
                for (int i = 0; i < fixities.Length; i++)
                {
                    rest += "," + (fixities[i] ? 1 : 0);
                }

                rest += ",STIFF";

                double[] stiffnesses = node.Constraint.GetElasticValues();
                for (int i = 0; i < stiffnesses.Length; i++)
                {
                    rest += "," + ((stiffnesses[i] > 0) ? stiffnesses[i] : 0);
                }


                return rest;
            }
            else
                return "NO_REST,NO_STIFF";


        }

        /***************************************/

        private static Constraint6DOF GetConstraint(int gsaConst, double[] stiffnesses)
        {
            //Construct the constraint
            BitArray arr = new BitArray(new int[] { gsaConst });
            bool[] fixities = new bool[6];

            for (int i = 0; i < 6; i++)
            {
                fixities[i] = arr[i];
            }

            return BH.Engine.Structure.Create.Constraint6DOF("", fixities, stiffnesses);

        }
    }
}
