﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Calculator which calculates the wave height associated to the result of iterating towards a
    /// probability of failure given a norm.
    /// </summary>
    internal class WaveHeightCalculator : HydraRingCalculatorBase, IWaveHeightCalculator
    {
        private readonly ReliabilityIndexCalculationParser targetProbabilityParser;
        private readonly ConvergenceParser convergenceParser;
        private readonly IllustrationPointsParser illustrationPointsParser;
        private bool includeIllustrationPoints;

        /// <summary>
        /// Create a new instance of <see cref="WaveHeightCalculator"/>.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the hydraulic boundary database can be found.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/> is <c>null</c>.</exception>
        internal WaveHeightCalculator(string hlcdDirectory)
            : base(hlcdDirectory)
        {
            targetProbabilityParser = new ReliabilityIndexCalculationParser();
            convergenceParser = new ConvergenceParser();
            illustrationPointsParser = new IllustrationPointsParser();

            WaveHeight = double.NaN;
            ReliabilityIndex = double.NaN;
        }

        public GeneralResult IllustrationPointsResult { get; private set; }

        public double WaveHeight { get; private set; }

        public double ReliabilityIndex { get; private set; }
        public bool? Converged { get; private set; }

        public void CalculateWithIllustrationPoints(WaveHeightCalculationInput input)
        {
            includeIllustrationPoints = true;
            try
            {
                Calculate(input);
            }
            finally
            {
                includeIllustrationPoints = false;
            }
        }

        public void Calculate(WaveHeightCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return targetProbabilityParser;
            yield return convergenceParser;
            if (includeIllustrationPoints)
            {
                yield return illustrationPointsParser;
            }
        }

        protected override void SetOutputs()
        {
            if (targetProbabilityParser.Output != null)
            {
                WaveHeight = targetProbabilityParser.Output.Result;
                ReliabilityIndex = targetProbabilityParser.Output.CalculatedReliabilityIndex;
            }
            Converged = convergenceParser.Output;

            if (includeIllustrationPoints)
            {
                IllustrationPointsResult = illustrationPointsParser.Output;
            }
        }
    }
}