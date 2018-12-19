﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Calculator which calculates the dunes boundary conditions associated to the result of iterating towards a
    /// probability of failure given a norm.
    /// </summary>
    internal class DunesBoundaryConditionsCalculator : HydraRingCalculatorBase, IDunesBoundaryConditionsCalculator
    {
        private readonly DunesBoundaryConditionsCalculationParser dunesBoundaryParser;
        private readonly ReliabilityIndexCalculationParser targetProbabilityParser;
        private readonly ConvergenceParser convergenceParser;

        /// <summary>
        /// Create a new instance of <see cref="DesignWaterLevelCalculator"/>.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// general information for a Hydra-Ring calculation settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        internal DunesBoundaryConditionsCalculator(HydraRingCalculationSettings calculationSettings) : base(calculationSettings)
        {
            dunesBoundaryParser = new DunesBoundaryConditionsCalculationParser();
            targetProbabilityParser = new ReliabilityIndexCalculationParser();
            convergenceParser = new ConvergenceParser();

            WaterLevel = double.NaN;
            WaveHeight = double.NaN;
            WavePeriod = double.NaN;
            ReliabilityIndex = double.NaN;
        }

        public double WaterLevel { get; private set; }

        public double WaveHeight { get; private set; }

        public double WavePeriod { get; private set; }

        public double ReliabilityIndex { get; private set; }

        public bool? Converged { get; private set; }

        public void Calculate(DunesBoundaryConditionsCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return dunesBoundaryParser;
            yield return targetProbabilityParser;
            yield return convergenceParser;
        }

        protected override void SetOutputs()
        {
            if (dunesBoundaryParser.Output != null)
            {
                WaterLevel = dunesBoundaryParser.Output.WaterLevel;
                WaveHeight = dunesBoundaryParser.Output.WaveHeight;
                WavePeriod = dunesBoundaryParser.Output.WavePeriod;
            }

            if (targetProbabilityParser.Output != null)
            {
                ReliabilityIndex = targetProbabilityParser.Output.CalculatedReliabilityIndex;
            }

            Converged = convergenceParser.Output;
        }
    }
}