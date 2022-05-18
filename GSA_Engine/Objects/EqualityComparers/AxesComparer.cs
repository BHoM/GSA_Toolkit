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

using System;
using System.Collections.Generic;
using BH.oM.Adapters.GSA.Fragments;
using BH.oM.Geometry;

namespace BH.Engine.Adapters.GSA
{
    public class AxesComparer : IEqualityComparer<Axes>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public AxesComparer()
        {
            //TODO: Grab tolerance from global tolerance settings
            m_multiplier = 1000;
        }

        /***************************************************/

        public AxesComparer(int decimals)
        {
            m_multiplier = Math.Pow(10, decimals);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Axes nodeAxes1, Axes nodeAxes2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(nodeAxes1, nodeAxes2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(nodeAxes1, null) || Object.ReferenceEquals(nodeAxes2, null))
                return false;

            //Check whether any of the compared objects nodes are null.
            if (Object.ReferenceEquals(nodeAxes1.Orientation, null) || Object.ReferenceEquals(nodeAxes2.Orientation, null))
                return false;

            if (!VectorEquals(nodeAxes1.Orientation.X, nodeAxes2.Orientation.X))
                return false;

            if (!VectorEquals(nodeAxes1.Orientation.Y, nodeAxes2.Orientation.Y))
                return false;

            if (!VectorEquals(nodeAxes1.Orientation.Z, nodeAxes2.Orientation.Z))
                return false;

            return true;
        }

        /***************************************************/

        private bool VectorEquals(Vector v1, Vector v2)
        {
            //Check whether any of the vectors are null.
            if (object.ReferenceEquals(v1, null) || object.ReferenceEquals(v2, null))
                return false;

            if ((int)Math.Round(v1.X * m_multiplier) != (int)Math.Round(v2.X * m_multiplier))
                return false;
            if ((int)Math.Round(v1.Y * m_multiplier) != (int)Math.Round(v2.Y * m_multiplier))
                return false;
            if ((int)Math.Round(v1.Z * m_multiplier) != (int)Math.Round(v2.Z * m_multiplier))
                return false;
            return true;
        }

        /***************************************************/

        public int GetHashCode(Axes nodeAxes)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(nodeAxes, null)) return 0;

            //Check whether the position is null
            if (Object.ReferenceEquals(nodeAxes.Orientation, null)) return 0;

            int x = GetVectorHashCode(nodeAxes.Orientation.X);
            int y = GetVectorHashCode(nodeAxes.Orientation.Y);
            int z = GetVectorHashCode(nodeAxes.Orientation.Z);
            return x ^ y ^ z;

        }

        public int GetVectorHashCode(Vector vector)
        {
            //Check whether the vector is null
            if (Object.ReferenceEquals(vector, null)) return 0;

            int x = ((int)Math.Round(vector.X * m_multiplier)).GetHashCode();
            int y = ((int)Math.Round(vector.Y * m_multiplier)).GetHashCode();
            int z = ((int)Math.Round(vector.Z * m_multiplier)).GetHashCode();
            return x ^ y ^ z;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private double m_multiplier;


        /***************************************************/
    }
}



