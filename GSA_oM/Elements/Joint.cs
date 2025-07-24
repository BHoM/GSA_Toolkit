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
    [Description("A joint describing linked degrees of freedom between nodes. Relates the displacement or force at the constrained degree of freedom to the primary degree of freedom without considering eccentricities.")]
    public class Joint : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Defines the primary node of the joint.")]
        public virtual Node PrimaryNode { get; set; }

        [Description("Defines the constrained node of the joint.")]
        public virtual Node ConstrainedNode { get; set; }

        [Description("X direction.")]
        public virtual bool X { get; set; } = true;

        [Description("Y direction.")]
        public virtual bool Y { get; set; } = true;

        [Description("Z direction.")]
        public virtual bool Z { get; set; } = true;

        [Description("XX direction.")]
        public virtual bool XX { get; set; } = true;

        [Description("YY direction.")]
        public virtual bool YY { get; set; } = true;

        [Description("ZZ direction.")]
        public virtual bool ZZ { get; set; } = true;

        [Description("Stage")]
        public virtual string Stage { get; set; } = "all";
    }
}




