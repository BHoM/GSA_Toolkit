﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structural.Results;
using Interop.gsa_8_7;

namespace BH.Engine.GSA
{
    public static class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ResHeader ResultHeader(this Type type)
        {
            if (typeof(NodeReaction).IsAssignableFrom(type))
                return ResHeader.REF_REAC;
            else if (typeof(NodeDisplacement).IsAssignableFrom(type))
                return ResHeader.REF_DISP;
            else if (typeof(NodeAcceleration).IsAssignableFrom(type))
                return ResHeader.REF_ACC;
            else if (typeof(NodeVelocity).IsAssignableFrom(type))
                return ResHeader.REF_VEL;
            else if (typeof(BarForce).IsAssignableFrom(type))
                return ResHeader.REF_FORCE_EL1D;
            else if (typeof(BarDeformation).IsAssignableFrom(type))
                return ResHeader.REF_DISP_EL1D;
            else if (typeof(BarStress).IsAssignableFrom(type))
                return ResHeader.REF_STRESS_EL1D;
            else if (typeof(BarStress).IsAssignableFrom(type))
                return ResHeader.REF_STRAIN_EL1D;

            return ResHeader.REF_DISP;
        }

        /***************************************************/

        public static ResHeader ResultHeader(this NodeReaction result)
        {
            return ResHeader.REF_REAC;
        }

        /***************************************************/

        public static ResHeader ResultHeader(this NodeDisplacement result)
        {
            return ResHeader.REF_DISP;
        }

        /***************************************************/

        public static ResHeader ResultHeader(this NodeAcceleration result)
        {
            return ResHeader.REF_ACC;
        }

        /***************************************************/
        public static ResHeader ResultHeader(this NodeVelocity result)
        {
            return ResHeader.REF_VEL;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/
        public static ResHeader IResultHeader(this IResult result)
        {
            return ResultHeader(result as dynamic);
        }
    }
}