// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;

namespace Riskeer.HydraRing.Calculation.TestUtil.Calculator
{
    /// <summary>
    /// Implementation of <see cref="IWaveConditionsCosineCalculator"/> for testing purposes.
    /// </summary>
    public class TestWaveConditionsCosineCalculator : TestHydraRingCalculator<WaveConditionsCosineCalculationInput>, IWaveConditionsCosineCalculator
    {
        public double WaveDirection { get; }
        public double ReliabilityIndex { get; }
        public string OutputDirectory { get; }
        public string LastErrorFileContent { get; set; }
        public double WaveHeight { get; }
        public double WaveAngle { get; }
        public double WavePeakPeriod { get; }
        public bool? Converged { get; set; }
    }
}