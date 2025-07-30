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

        [Description("True defines a rigid connectivity between primary and constrained node for translations along the X-axis, i.e. true prohibits relative translation along the X-axis between primary and constrained node.")]
        public virtual bool X { get; set; } = true;

        [Description("True defines a rigid connectivity between primary and constrained node for translations along the Y-axis, i.e. true prohibits relative translation along the Y-axis between primary and constrained node.")]
        public virtual bool Y { get; set; } = true;

        [Description("True defines a rigid connectivity between primary and constrained node for translations along the Z-axis, i.e. true prohibits relative translation along the Z-axis between primary and constrained node.")]
        public virtual bool Z { get; set; } = true;

        [Description("True defines a rigid connectivity between primary and constrained node for rotations about the X-axis, i.e. true prohibits relative rotation about the X-axis between primary and constrained node.")]
        public virtual bool XX { get; set; } = true;

        [Description("True defines a rigid connectivity between primary and constrained node for rotations about the Y-axis, i.e. true prohibits relative rotation about the Y-axis between primary and constrained node.")]
        public virtual bool YY { get; set; } = true;

        [Description("True defines a rigid connectivity between primary and constrained node for rotations about the Z-axis, i.e. true prohibits relative rotation about the Z-axis between primary and constrained node.")]
        public virtual bool ZZ { get; set; } = true;

        [Description("List of analysis stages where the joint should be active as a list of integers. If none specified 'all' will be applied.")]
        public virtual List<int> StageList { get; set; } = new List<int> ();
    }
}




