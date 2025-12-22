/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System;
using System.ComponentModel;
using System.Collections.Generic;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.GSA.SpacerProperties;
using BH.oM.Analytical.Elements;

namespace BH.oM.Adapters.GSA.Elements
{
    [Description("A rigid constraint defining linkage between nodes.")]
    public class RigidConstraint : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Defines the primary node of the rigid constraint.")]
        public virtual Node PrimaryNode { get; set; }

        [Description("Defines the constrained nodes of the rigid constraint. Can be a list of nodes.")]
        public virtual List<Node> ConstrainedNodes { get; set; }

        [Description("Type of rigid constraint.")]
        public virtual RigidConstraintLinkType Type { get; set; } = RigidConstraintLinkType.ALL;


    }
}





