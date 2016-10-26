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
using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator which calculates values for a wave at a water level.
    /// These are used in different failure mechanisms as input.
    /// </summary>
    internal class WaveConditionsCosineCalculator : HydraRingCalculatorBase, IWaveConditionsCosineCalculator
    {
        private readonly WaveConditionsCalculationParser waveConditionsCalculationParser;

        /// <summary>
        /// Create a new instance of <see cref="WaveConditionsCosineCalculator"/>.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the Hydraulic Boundary Database can be found.</param>
        /// <param name="ringId">The id of the assessment section which is used in the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/> is <c>null</c>.</exception>
        internal WaveConditionsCosineCalculator(string hlcdDirectory, string ringId)
            : base(hlcdDirectory, ringId)
        {
            waveConditionsCalculationParser = new WaveConditionsCalculationParser();

            WaveHeight = double.NaN;
            WaveAngle = double.NaN;
            WavePeakPeriod = double.NaN;
        }

        public double WaveHeight { get; private set; }
        public double WaveAngle { get; private set; }
        public double WavePeakPeriod { get; private set; }

        public void Calculate(WaveConditionsCosineCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return waveConditionsCalculationParser;
        }

        protected override void SetOutputs()
        {
            if (waveConditionsCalculationParser.Output != null)
            {
                WaveHeight = waveConditionsCalculationParser.Output.WaveHeight;
                WaveAngle = waveConditionsCalculationParser.Output.WaveAngle;
                WavePeakPeriod = waveConditionsCalculationParser.Output.WavePeakPeriod;
            }
        }
    }
}