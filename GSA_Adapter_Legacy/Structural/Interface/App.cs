/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using Interop.gsa_8_7;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace GSAToolkit
{
    /// <summary>
    /// 
    /// </summary>
    public class App
    {
        /// <summary>
        /// 
        /// </summary>
        public ComAuto GSAApp { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public App(string filePath)
        {
            GSAApp = new ComAuto();
            if (filePath != "")
                GSAApp.Open(filePath);
            else
                GSAApp.NewFile();

        }

        static public bool NewFile(ComAuto gsa)
        {
            short result = gsa.NewFile();
            //SetDefaultUnits(gsa);

            if (result == 0) return true;
            else return false;
        }

        public void Dispose()
        {
            //Maybe this should be in the Dispose(bool) method but I think that doing so will keep it alive indefinitely
            GC.KeepAlive(GSAApp);

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing">True if it is safe to free managed objects.
        /// True when called from Dispose(), false when called from Finalize()</param>
        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (GSAApp != null)
                {
                    GSAApp.Close();

                    Marshal.FinalReleaseComObject(GSAApp);
                }
            }
            //uniqueInstance = null;
            GC.Collect();
        }

        ~App()
        {
            Dispose(false);
        }
    }
}
