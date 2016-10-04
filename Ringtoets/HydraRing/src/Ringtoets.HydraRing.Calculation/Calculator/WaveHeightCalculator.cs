﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Calculator which calculates the wave height associated to the result of iterating towards a
    /// probability of failure given a norm.
    /// </summary>
    public class WaveHeightCalculator : HydraRingCalculatorBase, IWaveHeightCalculator
    {
        private readonly string hlcdDirectory;
        private readonly string ringId;
        private readonly ReliabilityIndexCalculationParser targetProbabilityParser;

        /// <summary>
        /// Create a new instance of <see cref="WaveHeightCalculator"/>.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the Hydraulic Boundary Database can be found.</param>
        /// <param name="ringId">The id of the traject which is used in the calculation.</param>
        internal WaveHeightCalculator(string hlcdDirectory, string ringId)
        {
            this.hlcdDirectory = hlcdDirectory;
            this.ringId = ringId;
            targetProbabilityParser = new ReliabilityIndexCalculationParser();

            WaveHeight = double.NaN;
            ReliabilityIndex = double.NaN;
        }

        public double WaveHeight { get; private set; }
        public double ReliabilityIndex { get; private set; }

        public void Calculate(WaveHeightCalculationInput input)
        {
            Calculate(hlcdDirectory, ringId, HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return targetProbabilityParser;
        }

        protected override void SetOutputs()
        {
            if (targetProbabilityParser.Output != null)
            {
                WaveHeight = targetProbabilityParser.Output.Result;
                ReliabilityIndex = targetProbabilityParser.Output.CalculatedReliabilityIndex;
            }
        }
    }
}