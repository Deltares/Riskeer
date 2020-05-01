// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Riskeer.Piping.KernelWrapper.SubCalculator;

namespace Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real Sellmeijer sub calculator of piping.
    /// </summary>
    public class SellmeijerCalculatorStub : ISellmeijerCalculator
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Validate"/> was called or not.
        /// </summary>
        public bool Validated { get; private set; }

        public double BeddingAngle { get; private set; }
        public double D70 { get; private set; }
        public double D70Mean { get; private set; }
        public double DAquifer { get; private set; }
        public double DarcyPermeability { get; private set; }
        public double DTotal { get; private set; }
        public double GammaSubParticles { get; private set; }
        public double Gravity { get; private set; }
        public double HExit { get; private set; }
        public double HRiver { get; private set; }
        public double KinematicViscosityWater { get; private set; }
        public double ModelFactorPiping { get; private set; }
        public double Rc { get; private set; }
        public double SeepageLength { get; private set; }
        public double VolumetricWeightOfWater { get; private set; }
        public double WhitesDragCoefficient { get; private set; }
        public double BottomLevelAquitardAboveExitPointZ { get; private set; }
        public double CreepCoefficient { get; private set; }
        public double CriticalFall { get; private set; }
        public double ReducedFall { get; private set; }
        public double Zp { get; private set; }
        public double FoSp { get; private set; }

        public void SetBeddingAngle(double beddingAngle)
        {
            BeddingAngle = beddingAngle;
        }

        public void SetD70(double d70)
        {
            D70 = d70;
        }

        public void SetD70Mean(double d70Mean)
        {
            D70Mean = d70Mean;
        }

        public void SetDAquifer(double dAquifer)
        {
            DAquifer = dAquifer;
        }

        public void SetDarcyPermeability(double darcyPermeability)
        {
            DarcyPermeability = darcyPermeability;
        }

        public void SetDTotal(double dTotal)
        {
            DTotal = dTotal;
        }

        public void SetGammaSubParticles(double gammaSubParticles)
        {
            GammaSubParticles = gammaSubParticles;
        }

        public void SetGravity(double gravity)
        {
            Gravity = gravity;
        }

        public void SetHExit(double hExit)
        {
            HExit = hExit;
        }

        public void SetHRiver(double hRiver)
        {
            HRiver = hRiver;
        }

        public void SetKinematicViscosityWater(double kinematicViscosityWater)
        {
            KinematicViscosityWater = kinematicViscosityWater;
        }

        public void SetModelFactorPiping(double modelFactorPiping)
        {
            ModelFactorPiping = modelFactorPiping;
        }

        public void SetRc(double rc)
        {
            Rc = rc;
        }

        public void SetSeepageLength(double seepageLength)
        {
            SeepageLength = seepageLength;
        }

        public void SetVolumetricWeightOfWater(double volumetricWeightOfWater)
        {
            VolumetricWeightOfWater = volumetricWeightOfWater;
        }

        public void SetWhitesDragCoefficient(double whitesDragCoefficient)
        {
            WhitesDragCoefficient = whitesDragCoefficient;
        }

        public void SetBottomLevelAquitardAboveExitPointZ(double bottomLevelAquitardAboveExitPointZ)
        {
            BottomLevelAquitardAboveExitPointZ = bottomLevelAquitardAboveExitPointZ;
        }

        public void Calculate()
        {
            Calculated = true;
        }

        public List<string> Validate()
        {
            Validated = true;
            return new List<string>();
        }
    }
}