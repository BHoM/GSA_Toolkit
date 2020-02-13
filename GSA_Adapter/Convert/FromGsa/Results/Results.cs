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

        public static NodeDisplacement FromGsaNodeDisplacement(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {
            NodeDisplacement disp = new NodeDisplacement
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static NodeReaction FromGsaReaction(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {

            NodeReaction reac = new NodeReaction
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                FX = results.dynaResults[0],
                FY = results.dynaResults[1],
                FZ = results.dynaResults[2],
                MX = results.dynaResults[4],
                MY = results.dynaResults[5],
                MZ = results.dynaResults[6]
            };

            return reac;
        }

        /***************************************************/

        public static NodeVelocity FromGsaNodeVelocity(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {
            //TODO: Needs testing
            NodeVelocity disp = new NodeVelocity
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static NodeAcceleration FromGsaNodeAcceleration(GsaResults results, string id, string loadCase, int divisions = 0, double timeStep = 0)
        {
            //TODO: Needs testing
            NodeAcceleration disp = new NodeAcceleration
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static BarForce FromGsaBarForce(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarForce force = new BarForce
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                FX = results.dynaResults[0],
                FY = results.dynaResults[1],
                FZ = results.dynaResults[2],
                MX = results.dynaResults[4],
                MY = results.dynaResults[5],
                MZ = -results.dynaResults[6]
            };
            return force;
        }

        /***************************************************/

        public static BarStress FromGsaBarStress(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarStress force = new BarStress
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                Axial = results.dynaResults[0],
                ShearY = results.dynaResults[1],
                ShearZ = results.dynaResults[2],
                BendingY_Bot = results.dynaResults[3],
                BendingY_Top = results.dynaResults[4],
                BendingZ_Bot = results.dynaResults[5],
                BendingZ_Top = results.dynaResults[6],
                CombAxialBendingPos = results.dynaResults[7],
                CombAxialBendingNeg = results.dynaResults[8]
            };
            return force;
        }

        /***************************************************/

        public static BarDisplacement FromGsaBarDisplacement(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarDisplacement disp = new BarDisplacement
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                UX = results.dynaResults[0],
                UY = results.dynaResults[1],
                UZ = results.dynaResults[2],
                RX = results.dynaResults[4],
                RY = results.dynaResults[5],
                RZ = results.dynaResults[6]
            };
            return disp;
        }

        /***************************************************/

        public static BarStrain FromGsaBarStrain(GsaResults results, string id, string loadCase, int divisions, double timeStep = 0)
        {
            BarStrain strain = new BarStrain
            {
                ObjectId = id,
                ResultCase = loadCase,
                TimeStep = timeStep,
                Divisions = divisions,
                Position = results.Pos,
                Axial = results.dynaResults[0]
            };
            return strain;
        }

        /***************************************************/

        public static GlobalReactions FromGsaGlobalReactions(string id, string force, string moment)
        {
            string[] fArr = force.Split(',');
            string[] mArr = moment.Split(',');

            return new GlobalReactions()
            {
                ResultCase = "A" + fArr[1],
                FX = double.Parse(fArr[3]),
                FY = double.Parse(fArr[4]),
                FZ = double.Parse(fArr[5]),
                MX = double.Parse(mArr[3]),
                MY = double.Parse(mArr[4]),
                MZ = double.Parse(mArr[5])
            };
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


            return new GlobalReactions()
            {
                ResultCase = lCase,
                FX = fx,
                FY = fy,
                FZ = fz,
                MX = mx,
                MY = my,
                MZ = mz,
            };
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
            return new ModalDynamics()
            {
                ObjectId = id,
                ResultCase = "A" + modeArr[1],
                ModeNumber = int.Parse(modeArr[2]),
                Frequency = double.Parse(frArr[2]),
                ModalMass = totMass,
                ModalStiffness = double.Parse(stiArr[2]),
                MassRatioX = double.Parse(tranArr[3]) / totMass,
                MassRatioY = double.Parse(tranArr[4]) / totMass,
                MassRatioZ = double.Parse(tranArr[5]) / totMass,
                InertiaRatioX = double.Parse(rotArr[3]) / totMass,
                InertiaRatioY = double.Parse(rotArr[4]) / totMass,
                InertiaRatioZ = double.Parse(rotArr[5]) / totMass,
                ModalDamping = damp
            };
        }

        /***************************************************/
    }
}
