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

using System.Collections.Generic;
using System.Collections;
using BH.oM.Adapter;
using BH.oM.Reflection;
using BH.oM.Adapter.Commands;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public override Output<object, bool> Execute(IExecuteCommand command, ActionConfig actionConfig = null)
        {
            var output = new Output<object, bool>() { Item1 = null, Item2 = false };

            output.Item2 = RunCommand(command as dynamic);

            return output;
        }

        /***************************************************/

        public bool RunCommand(ClearResults command)
        {
            return m_gsaCom.Delete("RESULTS") == 0;
        }

        /***************************************************/

        public bool RunCommand(Close command)
        {
            return m_gsaCom.Close() == 0;
        }

        /***************************************************/

        public bool RunCommand(Save command)
        {
            return m_gsaCom.Save() == 0;

        }

        /***************************************************/

        public bool RunCommand(SaveAs command)
        {
            return m_gsaCom.SaveAs("@" + command.FileName) == 0;
        }

        /***************************************************/

        public bool RunCommand(AnalyseLoadCases command)
        {
            short res;

            var cases = command.LoadCases;

            if (cases == null)
            {
                res = m_gsaCom.Analyse();
            }
            else
            {
                res = 0;

                foreach (string c in cases)
                {
                    int num;

                    if (int.TryParse(c, out num))
                        res += m_gsaCom.Analyse(num);
                }
            }
            return res == 0;
        }

        /***************************************************/

        public bool RunCommand(CustomCommand command)
        {
            return ComCall(command.Command);
        }
    }
}

