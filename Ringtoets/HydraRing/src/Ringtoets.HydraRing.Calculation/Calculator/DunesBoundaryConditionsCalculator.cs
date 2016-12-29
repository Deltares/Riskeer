// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Calculator which calculates the dunes boundary conditions associated to the result of iterating towards a
    /// probability of failure given a norm.
    /// </summary>
    internal class DunesBoundaryConditionsCalculator : HydraRingCalculatorBase, IDunesBoundaryConditionsCalculator
    {
        /// <summary>
        /// Create a new instance of <see cref="DesignWaterLevelCalculator"/>.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the Hydraulic Boundary Database can be found.</param>
        /// <param name="ringId">The id of the assessment section which is used in the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/> is <c>null</c>.</exception>
        internal DunesBoundaryConditionsCalculator(string hlcdDirectory, string ringId)
            : base(hlcdDirectory, ringId)
        {
            WaterLevel = double.NaN;
            WaveHeight = double.NaN;
            WavePeriod = double.NaN;
        }

        public double WaterLevel { get; private set; }
        public double WaveHeight { get; private set; }
        public double WavePeriod { get; private set; }

        public void Calculate(DunesBoundaryConditionsCalculationInput input) {}

        protected override void SetOutputs() {}
    }
}