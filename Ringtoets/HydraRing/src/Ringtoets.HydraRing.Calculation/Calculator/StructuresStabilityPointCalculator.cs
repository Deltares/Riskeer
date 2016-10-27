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
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator calculating probability of failure by a structural failure structure. 
    /// This is used in a structural failure structures assessment.
    /// </summary>
    internal class StructuresStabilityPointCalculator : HydraRingCalculatorBase, IStructuresStabilityPointCalculator
    {
        private readonly ExceedanceProbabilityCalculationParser exceedanceProbabilityCalculationParser;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointCalculator"/>.
        /// </summary>
        /// <param name="hlcdDirectory">The directory in which the hydraulic boundary database can be found.</param>
        /// <param name="ringId">The id of the assessment section which is used in the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdDirectory"/> is <c>null</c>.</exception>
        public StructuresStabilityPointCalculator(string hlcdDirectory, string ringId)
            : base(hlcdDirectory, ringId)
        {
            exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();
            ExceedanceProbabilityBeta = double.NaN;
        }

        public double ExceedanceProbabilityBeta { get; private set; }

        public void Calculate(StructuresStabilityPointCalculationInput input)
        {
            Calculate(HydraRingUncertaintiesType.All, input);
        }

        protected override void SetOutputs()
        {
            if (exceedanceProbabilityCalculationParser.Output != null)
            {
                ExceedanceProbabilityBeta = exceedanceProbabilityCalculationParser.Output.Beta;
            }
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return exceedanceProbabilityCalculationParser;
        }
    }
}