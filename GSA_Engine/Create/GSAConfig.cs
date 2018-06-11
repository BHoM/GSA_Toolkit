using BH.oM.GSA;

namespace BH.Engine.GSA
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public methods                            ****/
        /***************************************************/

        public static GSAConfig GSAConfig(  SteelDesignSpecification steelDesign = SteelDesignSpecification.Eurocode1993,
                                            ConcreteDesignSpecification concreteDesign = ConcreteDesignSpecification.Eurocode1992,
                                            Country country = Country.Undefined)
        {
            return new GSAConfig
            {
                SteelDesign = steelDesign,
                ConcreteDesign = concreteDesign,
                Country = country
            };
        }


        /***************************************************/

    }
}
