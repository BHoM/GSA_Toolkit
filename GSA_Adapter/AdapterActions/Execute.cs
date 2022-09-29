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

using System.Collections.Generic;
using System.Collections;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Adapter.Commands;

namespace BH.Adapter.GSA
{
#if GSA_10_1
    public partial class GSA101Adapter
#else
    public partial class GSA87Adapter
#endif
    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public override Output<List<object>, bool> Execute(IExecuteCommand command, ActionConfig actionConfig = null)
        {
            var output = new Output<List<object>, bool>() { Item1 = null, Item2 = false };

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
            if (command.SaveBeforeClose)
            {
                if (m_gsaCom.Save() != 0)
                {
                    Engine.Base.Compute.RecordError($"File not closed. File does not have a name. Please manually save the file or use the {nameof(SaveAs)} command before trying to Close the file. If you want to close the file anyway, please toggle {nameof(Close.SaveBeforeClose)} to false.");
                    return false;
                }
            }

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
            string path = System.IO.Path.GetFullPath(command.FileName);

            string dir = System.IO.Path.GetDirectoryName(path);

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            return m_gsaCom.SaveAs(path) == 0;
        }

        /***************************************************/

        public bool RunCommand(Analyse command)
        {
            return m_gsaCom.Analyse() == 0;
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

        public bool RunCommand(NewModel command)
        {
            return m_gsaCom.NewFile() == 0;
        }

        /***************************************************/

        public bool RunCommand(Open command)
        {
            return m_gsaCom.Open(command.FileName) == 0;
        }

        /***************************************************/

        public bool RunCommand(Exit command)
        {
            if (command.SaveBeforeClose)
            {
                if (m_gsaCom.Save() != 0)
                {
                    Engine.Base.Compute.RecordError($"Application not exited. File does not have a name. Please manually save the file or use the {nameof(SaveAs)} command before trying to Exit the application. If you want to close the application anyway, please toggle {nameof(Exit.SaveBeforeClose)} to false.");
                    return false;
                }
            }

            using (System.Diagnostics.Process gsaProcess = System.Diagnostics.Process.GetProcessById(m_gsaCom.ProcessID()))
            {
                if (gsaProcess == null)
                {
                    Engine.Base.Compute.RecordError("Could not find GSA process. Unable to exit application,");
                    return false;
                }
                string name = gsaProcess.ProcessName;
                if (!name.ToUpper().Contains("GSA"))
                {
                    Engine.Base.Compute.RecordError("Could not find the running GSA process.");
                    return false;
                }
                gsaProcess.Kill();
            }

            m_gsaCom = null;
            return true;
        }

        /***************************************************/

        public bool RunCommand(CustomCommand command)
        {
            return ComCall(command.Command);
        }

        /***************************************************/

        public bool RunCommand(IExecuteCommand command)
        {
            Engine.Base.Compute.RecordWarning($"The command {command.GetType().Name} is not supported by this Adapter.");
            return false;
        }
    }
}




