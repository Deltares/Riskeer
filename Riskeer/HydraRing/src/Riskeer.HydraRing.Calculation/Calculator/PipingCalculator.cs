﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System;
using System.Collections.Generic;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.Parsers;

namespace Riskeer.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Calculator for calculating probability of failure by piping. This is used in a piping assessment.
    /// </summary>
    internal class PipingCalculator : HydraRingCalculatorBase, IPipingCalculator
    {
        private readonly ExceedanceProbabilityCalculationParser exceedanceProbabilityCalculationParser;

        /// <summary>
        /// Create a new instance of <see cref="PipingCalculator"/>.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        internal PipingCalculator(HydraRingCalculationSettings calculationSettings) : base(calculationSettings)
        {
            exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();

            ExceedanceProbabilityBeta = double.NaN;
        }

        public double ExceedanceProbabilityBeta { get; private set; }

        public void Calculate(PipingCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return exceedanceProbabilityCalculationParser;
        }

        protected override void SetOutputs()
        {
            if (exceedanceProbabilityCalculationParser.Output.HasValue)
            {
                ExceedanceProbabilityBeta = exceedanceProbabilityCalculationParser.Output.Value;
            }
        }
    }
}