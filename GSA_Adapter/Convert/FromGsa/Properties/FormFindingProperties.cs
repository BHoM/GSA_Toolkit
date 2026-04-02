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


using BH.Engine.Adapter;
using BH.oM.Adapters.GSA;
using BH.oM.Structure.Constraints;
using System.Linq;
using BH.oM.Adapters.GSA.FormFindingProperties;

namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SoapStress1D FromGsaSoapStress1D(string gsaProp, double unitFactor)
        {

            string[] props = gsaProp.Split(',');
            string name = props[2];
            string stress = props[4];
            string id = props[1];

            SoapStress1D soapStress1D = new SoapStress1D();
            soapStress1D.Name = name;
            soapStress1D.SetAdapterId(typeof(GSAId), int.Parse(id));
            soapStress1D.Stress = double.Parse(stress) / unitFactor;

            return soapStress1D;
        }

        public static SoapStress2D FromGsaSoapStress2D(string gsaProp, double unitFactor)
        {

            string[] props = gsaProp.Split(',');
            string name = props[2];
            string stressX = props[3];
            string stressY = props[4];
            string id = props[1];

            SoapStress2D soapStress2D = new SoapStress2D();
            soapStress2D.Name = name;
            soapStress2D.SetAdapterId(typeof(GSAId), int.Parse(id));
            soapStress2D.StressX = double.Parse(stressX) / unitFactor;
            soapStress2D.StressY = double.Parse(stressY) / unitFactor;

            return soapStress2D;
        }

        /***************************************************/
    }
}




