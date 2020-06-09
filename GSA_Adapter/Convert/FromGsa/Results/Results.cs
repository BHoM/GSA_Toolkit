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

using Interop.gsa_8_7;
using System;
using System.Collections.Generic;
using BH.oM.Structure.Results;


namespace BH.Adapter.GSA
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static NodeDisplacement FromGsaNodeDisplacement(GsaResults results, int id, string loadCase, int divisions = 0, double timeStep = 0, int mode = -1)
        {
            return new NodeDisplacement(
                        id,
                        loadCase,
                        mode,
                        timeStep,
                        oM.Geometry.Basis.XY,
                        results.dynaResults[0],
                        results.dynaResults[1],
                        results.dynaResults[2],
                        results.dynaResults[4],
                        results.dynaResults[5],
                        results.dynaResults[6]
                        );
        }

        /***************************************************/

        public static NodeReaction FromGsaReaction(GsaResults results, int id, string loadCase, int divisions = 0, double timeStep = 0, int mode = -1)
        {
            return new NodeReaction(
                        id,
                        loadCase,
                        mode,
                        timeStep,
                        oM.Geometry.Basis.XY,
                        results.dynaResults[0],
                        results.dynaResults[1],
                        results.dynaResults[2],
                        results.dynaResults[4],
                        results.dynaResults[5],
                        results.dynaResults[6]
                        );

        }

        /***************************************************/

        public static NodeVelocity FromGsaNodeVelocity(GsaResults results, int id, string loadCase, int divisions = 0, double timeStep = 0, int mode = -1)
        {
            //TODO: Needs testing
            return new NodeVelocity(
                        id,
                        loadCase,
                        mode,
                        timeStep,
                        oM.Geometry.Basis.XY,
                        results.dynaResults[0],
                        results.dynaResults[1],
                        results.dynaResults[2],
                        results.dynaResults[4],
                        results.dynaResults[5],
                        results.dynaResults[6]
                        );
        }

        /***************************************************/

        public static NodeAcceleration FromGsaNodeAcceleration(GsaResults results, int id, string loadCase, int divisions = 0, double timeStep = 0, int mode = -1)
        {
            //TODO: Needs testing
            return new NodeAcceleration(
                        id,
                        loadCase,
                        mode,
                        timeStep,
                        oM.Geometry.Basis.XY,
                        results.dynaResults[0],
                        results.dynaResults[1],
                        results.dynaResults[2],
                        results.dynaResults[4],
                        results.dynaResults[5],
                        results.dynaResults[6]
            );
        }

        /***************************************************/

        public static BarForce FromGsaBarForce(GsaResults results, int id, string loadCase, int divisions, double timeStep = 0, int mode = -1)
        {
            return new BarForce(
                id,
                loadCase,
                mode,
                timeStep,
                results.Pos,
                divisions,
                results.dynaResults[0],
                results.dynaResults[1],
                results.dynaResults[2],
                results.dynaResults[4],
                results.dynaResults[5],
                -results.dynaResults[6]
                );
        }

        /***************************************************/

        public static BarStress FromGsaBarStress(GsaResults results, int id, string loadCase, int divisions, double timeStep = 0, int mode = -1)
        {
            return new BarStress(
                id,
                loadCase,
                mode,
                timeStep,
                results.Pos,
                divisions,
                results.dynaResults[0],
                results.dynaResults[1],
                results.dynaResults[2],
                results.dynaResults[4],
                results.dynaResults[3],
                results.dynaResults[6],
                results.dynaResults[5],
                results.dynaResults[7],
                results.dynaResults[8]
                );
        }

        /***************************************************/

        public static BarDisplacement FromGsaBarDisplacement(GsaResults results, int id, string loadCase, int divisions, double timeStep = 0, int mode = -1)
        {
            return new BarDisplacement(
                id,
                loadCase,
                mode,
                timeStep,
                results.Pos,
                divisions,
                results.dynaResults[0],
                results.dynaResults[1],
                results.dynaResults[2],
                results.dynaResults[4],
                results.dynaResults[5],
                results.dynaResults[6]
                );
        }

        /***************************************************/

        public static BarStrain FromGsaBarStrain(GsaResults results, int id, string loadCase, int divisions, double timeStep = 0, int mode = -1)
        {
            return new BarStrain(id, loadCase, mode, timeStep, results.Pos, divisions, results.dynaResults[0], 0, 0, 0, 0, 0, 0, 0, 0);
        }

        /***************************************************/

        public static GlobalReactions FromGsaGlobalReactions(string id, string force, string moment)
        {
            string[] fArr = force.Split(',');
            string[] mArr = moment.Split(',');


            return new GlobalReactions(id, "A" + fArr[1], -1, 0,
                        double.Parse(fArr[3]),
                        double.Parse(fArr[4]),
                        double.Parse(fArr[5]),
                        double.Parse(mArr[3]),
                        double.Parse(mArr[4]),
                        double.Parse(mArr[5])
                );
        }

        /***************************************************/

        public static GlobalReactions FromGsaGlobalReactions(string id, List<string> force, List<string> moment)
        {
            double fx = 0;
            double fy = 0;
            double fz = 0;
            double mx = 0;
            double my = 0;
            double mz = 0;
            string lCase = "";

            foreach (string str in force)
            {
                if (string.IsNullOrWhiteSpace(str))
                    continue;

                string[] arr = str.Split(',');

                if (arr.Length < 6)
                    continue;

                lCase = "A" + arr[1];

                fx += double.Parse(arr[3]);
                fy += double.Parse(arr[4]);
                fz += double.Parse(arr[5]);
            }

            foreach (string str in moment)
            {
                if (string.IsNullOrWhiteSpace(str))
                    continue;

                string[] arr = str.Split(',');

                if (arr.Length < 6)
                    continue;

                mx += double.Parse(arr[3]);
                my += double.Parse(arr[4]);
                mz += double.Parse(arr[5]);
            }

            return new GlobalReactions("", lCase, -1, 0, fx, fy, fz, mx, my, mz);

        }

        /***************************************************/

        public static ModalDynamics FromGsaModalDynamics(string id, string mode, string frequency, string mass, string stiffness, string damping, string effMassTran, string effMassRot)
        {
            string[] modeArr = mode.Split(',');
            string[] frArr = frequency.Split(',');
            string[] massArr = mass.Split(',');
            string[] stiArr = stiffness.Split(',');
            string[] tranArr = effMassTran.Split(',');
            string[] rotArr = effMassRot.Split(',');
            double damp;
            if (String.IsNullOrWhiteSpace(damping))
                damp = 0;
            else
                damp = double.Parse(damping.Split(',')[2]);

            double totMass = double.Parse(massArr[2]);
            //TODO: Modal damping

            return new ModalDynamics(
                id,
                "A" + modeArr[1],
                int.Parse(modeArr[2]),
                0,
                double.Parse(frArr[2]),
                totMass,
                double.Parse(stiArr[2]),
                damp,
                double.Parse(tranArr[3]) / totMass,
                double.Parse(tranArr[4]) / totMass,
                double.Parse(tranArr[5]) / totMass,
                double.Parse(rotArr[3]) / totMass,
                double.Parse(rotArr[4]) / totMass,
                double.Parse(rotArr[5]) / totMass
                );
        }

        /***************************************************/
    }
}
