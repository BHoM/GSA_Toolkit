/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Structure;

namespace BH.oM.Adapters.GSA.SpringProperties
{
    [Description("")]
    public class LinearSpringProperty : BHoMObject, IProperty
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("A unique Name is required for some structural packages to create and identify the object.")]
        public override string Name { get; set; }

        [ForcePerUnitLength]
        [Description("Translational stiffness in the X-direction as defined by element.")]
        public virtual double UX { get; set; }

        [ForcePerUnitLength]
        [Description("Translational stiffness in the Y-direction as defined by element.")]
        public virtual double UY { get; set; }

        [ForcePerUnitLength]
        [Description("Translational stiffness in the Z-direction as defined by element.")]
        public virtual double UZ { get; set; }

        [MomentPerUnitAngle]
        [Description("Rotational stiffness about the X-axis as defined by element.")]
        public virtual double RX { get; set; }

        [MomentPerUnitAngle]
        [Description("Rotational stiffness about the Y-axis as defined by element.")]
        public virtual double RY { get; set; }

        [MomentPerUnitAngle]
        [Description("Rotational stiffness about the Z-axis as defined by element.")]
        public virtual double RZ { get; set; }
        /***************************************************/

    }
}


