// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.ComponentModel;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingScenariosView"/>.
    /// </summary>
    public class PipingScenarioRow : ScenarioRow<PipingCalculationScenario>
    {
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private DerivedPipingOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <param name="failureMechanism">The failure mechanism that the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal PipingScenarioRow(PipingCalculationScenario calculationScenario, PipingFailureMechanism failureMechanism,
                                   IAssessmentSection assessmentSection) : base(calculationScenario)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            CreateDerivedOutput();
        }

        public override double FailureProbability => derivedOutput?.PipingProbability ?? double.NaN;

        /// <summary>
        /// Gets the failure probability of uplift sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double FailureProbabilityUplift => derivedOutput?.UpliftProbability ?? double.NaN;

        /// <summary>
        /// Gets the failure probability of heave sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double FailureProbabilityHeave => derivedOutput?.HeaveProbability ?? double.NaN;

        /// <summary>
        /// Gets the failure probability of sellmeijer sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double FailureProbabilitySellmeijer => derivedOutput?.SellmeijerProbability ?? double.NaN;

        public override void Update()
        {
            CreateDerivedOutput();
        }

        private void CreateDerivedOutput()
        {
            derivedOutput = CalculationScenario.HasOutput
                                ? DerivedPipingOutputFactory.Create(CalculationScenario.Output, failureMechanism, assessmentSection)
                                : null;
        }
    }
}