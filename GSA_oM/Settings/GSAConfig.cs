using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.oM.GSA
{
    public class GSAConfig : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public SteelDesignSpecification SteelDesign { get; set; } = SteelDesignSpecification.Eurocode1993;

        public ConcreteDesignSpecification ConcreteDesign { get; set; } = ConcreteDesignSpecification.Eurocode1992;

        public Country Country { get; set; } = Country.UK;

        /***************************************************/
    }
}
