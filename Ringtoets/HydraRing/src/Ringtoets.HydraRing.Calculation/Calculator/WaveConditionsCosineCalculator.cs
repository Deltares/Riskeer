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

using System;
using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
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
        private readonly ReliabilityIndexCalculationParser reliabilityIndexCalculationParser;
        private readonly ConvergenceParser convergenceParser;

        /// <summary>
        /// Create a new instance of <see cref="WaveConditionsCosineCalculator"/>.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// general information for a Hydra-Ring calculation settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory" />
        /// equals <see cref="string.Empty"/>.</remarks>
        internal WaveConditionsCosineCalculator(HydraRingCalculationSettings calculationSettings) : base(calculationSettings)
        {
            waveConditionsCalculationParser = new WaveConditionsCalculationParser();
            reliabilityIndexCalculationParser = new ReliabilityIndexCalculationParser();
            convergenceParser = new ConvergenceParser();

            WaveHeight = double.NaN;
            WaveAngle = double.NaN;
            WavePeakPeriod = double.NaN;
            WaveDirection = double.NaN;
            ReliabilityIndex = double.NaN;
        }

        public double WaveHeight { get; private set; }

        public double WaveAngle { get; private set; }

        public double WavePeakPeriod { get; private set; }

        public double WaveDirection { get; private set; }

        public double ReliabilityIndex { get; private set; }

        public bool? Converged { get; private set; }

        public void Calculate(WaveConditionsCosineCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return waveConditionsCalculationParser;
            yield return reliabilityIndexCalculationParser;
            yield return convergenceParser;
        }

        protected override void SetOutputs()
        {
            if (waveConditionsCalculationParser.Output != null)
            {
                WaveHeight = waveConditionsCalculationParser.Output.WaveHeight;
                WaveAngle = waveConditionsCalculationParser.Output.WaveAngle;
                WaveDirection = waveConditionsCalculationParser.Output.WaveDirection;
                WavePeakPeriod = waveConditionsCalculationParser.Output.WavePeakPeriod;
            }

            if (reliabilityIndexCalculationParser.Output != null)
            {
                ReliabilityIndex = reliabilityIndexCalculationParser.Output.CalculatedReliabilityIndex;
            }

            Converged = convergenceParser.Output;
        }
    }
}