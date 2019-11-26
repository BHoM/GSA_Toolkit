/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

namespace BH.Engine.GSA
{

    /***************************************/

    public enum Out_0D_Results
    {
        REF_DISP = 12001000,
        REF_DISP_DX,
        REF_DISP_DY,
        REF_DISP_DZ,
        REF_DISP_TRANS,
        REF_DISP_RXX,
        REF_DISP_RYY,
        REF_DISP_RZZ,
        REF_DISP_ROT,
        REF_DISP_DXY,

        REF_REAC = 12004000,
        REF_REAC_FX,
        REF_REAC_FY,
        REF_REAC_FZ,
        REF_REAC_FRC,
        REF_REAC_MXX,
        REF_REAC_MYY,
        REF_REAC_MZZ,
        REF_REAC_MOM,
    }

    /***************************************/

    /// <summary>
    /// 
    /// </summary>
    public enum Out_1D_Results
    {
        REF_DISP_EL1D = 14001000,
        REF_DISP_EL1D_DX,
        REF_DISP_EL1D_DY,
        REF_DISP_EL1D_DZ,
        REF_DISP_EL1D_TRANS,
        REF_DISP_EL1D_RXX,
        REF_DISP_EL1D_RYY,
        REF_DISP_EL1D_RZZ,
        REF_DISP_EL1D_ROT,

        REF_FORCE_EL1D = 14002000,
        REF_FORCE_EL1D_FX,
        REF_FORCE_EL1D_FY,
        REF_FORCE_EL1D_FZ,
        REF_FORCE_EL1D_FRC,
        REF_FORCE_EL1D_MXX,
        REF_FORCE_EL1D_MYY,
        REF_FORCE_EL1D_MZZ,
        REF_FORCE_EL1D_MOM,

        REF_STRESS_EL1D = 14003000,
        REF_STRESS_EL1D_A,
        REF_STRESS_EL1D_SY,
        REF_STRESS_EL1D_SZ,
        REF_STRESS_EL1D_BY_POSZ,
        REF_STRESS_EL1D_BY_NEGZ,
        REF_STRESS_EL1D_BZ_POSY,
        REF_STRESS_EL1D_BZ_NEGY,
        REF_STRESS_EL1D_C1,
        REF_STRESS_EL1D_C2,
        REF_STRESS_EL1D_CY,
        REF_STRESS_EL1D_CZ,

        REF_STRESS_EL1D_DRV = 14003200,
        REF_STRESS_EL1D_DRV_SY,
        REF_STRESS_EL1D_DRV_SZ,
        REF_STRESS_EL1D_DRV_ST,
        REF_STRESS_EL1D_DRV_V_MISES,

        REF_STRAIN_EL1D = 14003500,
        REF_STRAIN_EL1D_A,

        REF_SED_EL1D = 14004000,
        REF_SED_EL1D_WEB,
        REF_SED_EL1D_FLG,
        REF_SED_EL1D_TOT,

        REF_SED_AVG_EL1D = 14005000,
        REF_SED_AVG_EL1D_WEB,
        REF_SED_AVG_EL1D_FLG,
        REF_SED_AVG_EL1D_TOT,

        REF_STL_UTIL = 14006000,
        REF_STL_UTIL_OVER_ALL,
        REF_STL_UTIL_LCL_COMB,
        REF_STL_UTIL_BCK_COMB,
        REF_STL_UTIL_LCL_A,
        REF_STL_UTIL_LCL_SU,
        REF_STL_UTIL_LCL_SV,
        REF_STL_UTIL_LCL_MXX,
        REF_STL_UTIL_LCL_MUU,
        REF_STL_UTIL_LCL_MVV,
        REF_STL_UTIL_BCK_UU,
        REF_STL_UTIL_BCK_VV,
        REF_STL_UTIL_BCK_LT,
        REF_STL_UTIL_BCK_TOR,
        REF_STL_UTIL_BCK_FT,
    }

    /***************************************/

    public enum Out_1D_Properties
    {
        REF_ELEM = 1011000,
        REF_ELEM_NAME,
        REF_ELEM_TYPE,
        REF_ELEM_PROP,
        REF_ELEM_GROUP,
        REF_ELEM_NODE,
        REF_ELEM_ANGLE,
        REF_ELEM_VERT,
        REF_ELEM_LEN,
        REF_ELEM_TOPO,
    }

    /***************************************/

    public enum Output_Init_Flags
    {
        //These are hexadecimals. "0x" represents the hexadecimal representation of a subsequent number.
        OP_INIT_2D_BOTTOM = 0x1,     // output 2D stresses at bottom layer
        OP_INIT_2D_MIDDLE = 0x2,     // output 2D stresses at middle layer
        OP_INIT_2D_TOP = 0x4,        // output 2D stresses at top layer
        OP_INIT_2D_BENDING = 0x8,    // output 2D stresses at bending layer
        OP_INIT_2D_AVGE = 0x10,      // average 2D element stresses at nodes
        OP_INIT_1D_AUTO_PTS = 0x20,  // calculate 1D results at interesting points
        OP_INIT_INFINITY = 0x40      //infinity and NaN values as such, else as zero
    }

    /***************************************/

    //In lack of an "Estring" function we have created a static class which returns
    //the three different options of outputaxis. It's used when extracting info in SIGSA.
    public static class Output_Axis
    {
        static public string Default() { return "default"; }
        static public string Global() { return "global"; }
        static public string Local() { return "local"; }
    }

    /***************************************/

    public enum UnitType
    {
        FORCE = 0,
        LENGTH,
        DISP,
        SECTION,
        MASS,
        TIME,
        TEMP,
        STRESS,
        ACCEL,
        ENERGY
    }

    /***************************************/

    public enum LoadType
    {
        DEAD,
        IMPOSED,
        WIND,
        SNOW,
        NOTIONAL,
        SEISMIC,
        UNDEF
    }

    /***************************************/

    public enum MaterialType
    {
        MT_STEEL,
        MT_CONCRETE,
        MT_ALUMINIUM,
        MT_GLASS,
        MT_TIMBER,
        MT_UNDEF,
        MT_REBAR
    }

    /***************************************/

}
