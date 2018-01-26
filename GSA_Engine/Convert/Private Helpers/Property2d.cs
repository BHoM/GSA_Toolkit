using BH.oM.Geometry;
using BH.oM.Structural.Properties;
using System;
using System.Collections.Generic;

namespace BH.Engine.GSA
{
    public static partial class Convert
    {
        private static LoadPanelSupportConditions GetLoadingConditionFromString(string str)
        {
            switch (str)
            {
                case "SUP_ALL":
                    return LoadPanelSupportConditions.AllSides;
                case "SUP_THREE":
                    return LoadPanelSupportConditions.ThreeSides;
                case "SUP_TWO":
                    return LoadPanelSupportConditions.TwoSides;
                case "SUP_TWO_ADJ":
                    return LoadPanelSupportConditions.TwoAdjacentSides;
                case "SUP_ONE":
                    return LoadPanelSupportConditions.OneSide;
                case "SUP_ONE_MOM":
                    return LoadPanelSupportConditions.Cantilever;
                default:
                    return LoadPanelSupportConditions.AllSides;
            }
        }

        private static string GetLoadPanelSupportConditions(LoadPanelSupportConditions cond)
        {
            switch (cond)
            {
                case LoadPanelSupportConditions.AllSides:
                    return "SUP_ALL";
                case LoadPanelSupportConditions.ThreeSides:
                    return "SUP_THREE";
                case LoadPanelSupportConditions.TwoSides:
                    return "SUP_TWO";
                case LoadPanelSupportConditions.TwoAdjacentSides:
                    return "SUP_TWO_ADJ";
                case LoadPanelSupportConditions.OneSide:
                    return "SUP_ONE";
                case LoadPanelSupportConditions.Cantilever:
                    return "SUP_ONE_MOM";
                default:
                    return "SUP_AUTO";
            }
        }

    }
}
