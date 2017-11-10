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
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Calculator for calculating probability of failure by overtopping or overflow and the
    /// associated wave height. This is used in a grass cover erosion inwards assessment.
    /// </summary>
    internal class OvertoppingCalculator : HydraRingCalculatorBase, IOvertoppingCalculator
    {
        private readonly ExceedanceProbabilityCalculationParser exceedanceProbabilityCalculationParser;
        private readonly OvertoppingCalculationWaveHeightParser waveHeightParser;

        /// <summary>
        /// Create a new instance of <see cref="OvertoppingCalculator"/>.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the hydraulic boundary database can be found.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/>
        /// or <paramref name="preprocessorDirectory"/> is <c>null</c>.</exception>
        internal OvertoppingCalculator(string hlcdDirectory, string preprocessorDirectory)
            : base(hlcdDirectory, preprocessorDirectory)
        {
            exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();
            waveHeightParser = new OvertoppingCalculationWaveHeightParser();

            ExceedanceProbabilityBeta = double.NaN;
            WaveHeight = double.NaN;
            IsOvertoppingDominant = false;
        }

        public double ExceedanceProbabilityBeta { get; private set; }

        public double WaveHeight { get; private set; }

        public bool IsOvertoppingDominant { get; private set; }

        public void Calculate(OvertoppingCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return exceedanceProbabilityCalculationParser;
            yield return waveHeightParser;
        }

        protected override void SetOutputs()
        {
            if (exceedanceProbabilityCalculationParser.Output.HasValue)
            {
                ExceedanceProbabilityBeta = exceedanceProbabilityCalculationParser.Output.Value;
            }
            if (waveHeightParser.Output != null)
            {
                WaveHeight = waveHeightParser.Output.WaveHeight;
            }
            IsOvertoppingDominant = waveHeightParser.Output != null && waveHeightParser.Output.IsOvertoppingDominant;
        }
    }
}