/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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


using BH.oM.Geometry;
using System.Collections.Generic;
using System;

namespace BH.Engine.GSA
{
    public class BasisComparer : IEqualityComparer<Basis>
    {

        public BasisComparer()
        {
            //TODO: Grab tolerance from global tolerance settings
            m_multiplier = 1000000;
        }

        /***************************************************/

        public BasisComparer(int decimals)
        {
            m_multiplier = Math.Pow(10, decimals);
        }

        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public bool Equals(Basis basis1, Basis basis2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(basis1, basis2))
                return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(basis1, null) || Object.ReferenceEquals(basis2, null))
                return false;


            if ((int)Math.Round(basis1.X.X * m_multiplier) != (int)Math.Round(basis2.X.X * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.X.Y * m_multiplier) != (int)Math.Round(basis2.X.Y * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.X.Z * m_multiplier) != (int)Math.Round(basis2.X.Z * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.Y.X * m_multiplier) != (int)Math.Round(basis2.Y.X * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.Y.Y * m_multiplier) != (int)Math.Round(basis2.Y.Y * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.Y.Z * m_multiplier) != (int)Math.Round(basis2.Y.Z * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.Z.X * m_multiplier) != (int)Math.Round(basis2.Z.X * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.Z.Y * m_multiplier) != (int)Math.Round(basis2.Z.Y * m_multiplier))
                return false;

            if ((int)Math.Round(basis1.Z.Z * m_multiplier) != (int)Math.Round(basis2.Z.Z * m_multiplier))
                return false;

            return true;
        }

        /***************************************************/

        public int GetHashCode(Basis basis)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(basis, null)) return 0;

            int xx = ((int)Math.Round(basis.X.X * m_multiplier)).GetHashCode();
            int xy = ((int)Math.Round(basis.X.Y * m_multiplier)).GetHashCode();
            int xz = ((int)Math.Round(basis.X.Z * m_multiplier)).GetHashCode();

            int yx = ((int)Math.Round(basis.Y.X * m_multiplier)).GetHashCode();
            int yy = ((int)Math.Round(basis.Y.Y * m_multiplier)).GetHashCode();
            int yz = ((int)Math.Round(basis.Y.Z * m_multiplier)).GetHashCode();

            int zx = ((int)Math.Round(basis.Z.X * m_multiplier)).GetHashCode();
            int zy = ((int)Math.Round(basis.Z.Y * m_multiplier)).GetHashCode();
            int zz = ((int)Math.Round(basis.Z.Z * m_multiplier)).GetHashCode();

            return xx ^ xy ^ xz ^ yx ^ yy ^ yz ^ zx ^ zy ^ zz;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private double m_multiplier;

        /***************************************************/
    }
}

