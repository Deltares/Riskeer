// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.Piping.KernelWrapper.SubCalculator;

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

        public double BeddingAngle { get; set; }
        public double D70 { get; set; }
        public double D70Mean { get; set; }
        public double DAquifer { get; set; }
        public double DarcyPermeability { get; set; }
        public double DTotal { get; set; }
        public double GammaSubParticles { get; set; }
        public double Gravity { get; set; }
        public double HExit { get; set; }
        public double HRiver { get; set; }
        public double KinematicViscosityWater { get; set; }
        public double ModelFactorPiping { get; set; }
        public double Rc { get; set; }
        public double SeepageLength { get; set; }
        public double VolumetricWeightOfWater { get; set; }
        public double WhitesDragCoefficient { get; set; }
        public double BottomLevelAquitardAboveExitPointZ { get; set; }
        public double CreepCoefficient { get; private set; }
        public double CriticalFall { get; private set; }
        public double ReducedFall { get; private set; }
        public double Zp { get; private set; }
        public double FoSp { get; private set; }

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