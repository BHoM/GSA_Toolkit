/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure;

namespace BH.oM.Adapters.GSA.SpacerProperties
{
    [Description("Property for spacer elements.")]
    public class SpacerProperty : BHoMObject, IProperty
    {
        [Description("A unique Name is required for some structural packages to create and identify the object.")]
        public override string Name { get; set; }

        [Description("The type of spacer.")]
        public virtual SpacerType Type { get; set; } = SpacerType.Geodesic;

        [Description("The spacer leg length type.")]
        public virtual SpacerLengthType LengthType { get; set; } = SpacerLengthType.Ratio;

        [Description("The spacer stiffness.")]
        public virtual double Stiffness { get; set; }

        [Description("The spacer leg ratio.")]
        public virtual double Ratio { get; set; } = 1;

    }
}


