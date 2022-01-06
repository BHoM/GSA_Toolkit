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
using System.ComponentModel;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.GSA.SpacerProperties;
using BH.oM.Analytical.Elements;

namespace BH.oM.Adapters.GSA.Elements
{
    [Description("A spacer defining an edge of a fabric panel in GSA.")]
    public class Spacer : BHoMObject, ILink<Node>
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Defines the start position of the element. Note that Nodes can contain Supports which should not be confused with Releases.")]
        public virtual Node StartNode { get; set; }

        [Description("Defines the end position of the element. Note that Nodes can contain Supports which should not be confused with Releases.")]
        public virtual Node EndNode { get; set; }

        [Description("Spacer property of the spacer, defining the spacer type.")]
        public virtual SpacerProperty SpacerProperty { get; set; } = null;


    }
}

