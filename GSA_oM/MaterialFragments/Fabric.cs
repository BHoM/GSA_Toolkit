using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.MaterialFragments;

namespace BH.oM.Adapters.GSA.MaterialFragments
{
    public class Fabric : BHoMObject, IMaterialFragment
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("A unique Name is required for some structural packages to create and identify the object.")]
        public override string Name { get; set; }

        [Density]
        public virtual double Density { get; set; }

        [Ratio]
        [Description("Dynamic damping ratio, expressed as a ratio between actual damping and critical damping. For structures, typically taken as 0.02 (i.e. 2%).")]
        public virtual double DampingRatio { get; set; }

        [Ratio]
        [Description("Ratio between axial and transverse strain. Used together with YoungsModulus to derive the ShearModulus for isotropic materials.")]
        public virtual double PoissonsRatio { get; set; }

        [YoungsModulus]
        [Description("Modulus of elasticity of the material. Ratio between axial stress and axial strain.")]
        public virtual double WarpModulus { get; set; }

        [YoungsModulus]
        [Description("Modulus of elasticity of the material. Ratio between axial stress and axial strain.")]
        public virtual double WeftModulus { get; set; }

        [YoungsModulus]
        [Description("Modulus of elasticity of the material. Ratio between axial stress and axial strain.")]
        public virtual double ShearModulus { get; set; }


        /***************************************************/


    }
}
