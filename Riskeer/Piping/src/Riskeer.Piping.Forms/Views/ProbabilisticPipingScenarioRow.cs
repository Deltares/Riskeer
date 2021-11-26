// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="ProbabilisticPipingCalculationScenario"/> in the <see cref="PipingScenariosView"/>.
    /// </summary>
    public class ProbabilisticPipingScenarioRow : PipingScenarioRow<ProbabilisticPipingCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingScenarioRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="ProbabilisticPipingCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationScenario"/> is <c>null</c>.</exception>
        internal ProbabilisticPipingScenarioRow(ProbabilisticPipingCalculationScenario calculationScenario)
            : base(calculationScenario) {}

        public override double FailureProbability
        {
            get
            {
                return CalculationScenario.HasOutput
                           ? CalculationScenario.Output.ProfileSpecificOutput.Reliability
                           : double.NaN;
            }
        }

        public override double SectionFailureProbability
        {
            get
            {
                return CalculationScenario.HasOutput
                           ? CalculationScenario.Output.SectionSpecificOutput.Reliability
                           : double.NaN;
            }
        }

        public override void Update() {}
    }
}