using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using System.Collections;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        /***************************************/

        private static string GetRestraintString(Node node)
        {
            if (node.Constraint != null)
            {
                string rest = "REST";


                bool[] fixities = node.Constraint.Fixities();
                for (int i = 0; i < fixities.Length; i++)
                {
                    rest += "," + (fixities[i] ? 1 : 0);
                }

                rest += ",STIFF";

                double[] stiffnesses = node.Constraint.ElasticValues();
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

        //private static Constraint6DOF GetConstraint(int gsaConst, double[] stiffnesses)
        //{
        //    //Construct the constraint
        //    BitArray arr = new BitArray(new int[] { gsaConst });
        //    bool[] fixities = new bool[6];

        //    for (int i = 0; i < 6; i++)
        //    {
        //        fixities[i] = arr[i];
        //    }

        //    return Create.Constraint6DOF("", fixities, stiffnesses);

        //}

        /***************************************/
    }
}
