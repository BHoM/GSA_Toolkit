/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

namespace BH.Adapter.GSA
{
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
}




