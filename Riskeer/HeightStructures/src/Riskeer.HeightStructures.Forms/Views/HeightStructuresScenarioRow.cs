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
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Views;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Forms.Views
{
    /// <summary>
    /// Representation of a <see cref="StructuresCalculationScenario{T}"/>
    /// which takes care of the representation of properties in a grid.
    /// </summary>
    public class HeightStructuresScenarioRow : ScenarioRow<StructuresCalculationScenario<HeightStructuresInput>>
    {
        private ProbabilityAssessmentOutput probabilityAssessmentOutput;

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresScenarioRow"/>.
        /// <param name="calculationScenario">The <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationScenario"/> is <c>null</c>.</exception>
        /// </summary>
        public HeightStructuresScenarioRow(StructuresCalculationScenario<HeightStructuresInput> calculationScenario)
            : base(calculationScenario)
        {
            CreateProbabilityAssessmentOutput();
        }

        public override double FailureProbability => probabilityAssessmentOutput?.Probability ?? double.NaN;

        public override void Update()
        {
            CreateProbabilityAssessmentOutput();
        }

        private void CreateProbabilityAssessmentOutput()
        {
            probabilityAssessmentOutput = CalculationScenario.HasOutput
                                              ? ProbabilityAssessmentOutputFactory.Create(CalculationScenario.Output.Reliability)
                                              : null;
        }
    }
}