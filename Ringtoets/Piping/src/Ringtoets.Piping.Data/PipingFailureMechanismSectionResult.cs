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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// for a piping assessment.
    /// </summary>
    public class PipingFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        private readonly CalculationGroup calculationsGroup;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <param name="calculationsGroup"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public PipingFailureMechanismSectionResult(FailureMechanismSection section, CalculationGroup calculationsGroup = null) : base(section)
        {
            this.calculationsGroup = calculationsGroup;
        }

        /// <summary>
        /// Gets the value of assessment layer two a.
        /// </summary>
        /// <param name="calculations"></param>
        public RoundedDouble GetAssessmentLayerTwoA(IEnumerable<PipingCalculationScenario> calculations)
        {
            var calculationScenarios = GetCalculationScenarios(calculations).Where(cs => cs.Status == CalculationScenarioStatus.Done).ToList();

            return calculationScenarios.Any()
                       ? (RoundedDouble) (1.0/calculationScenarios.Sum(scenario => (scenario.Probability.Value)*scenario.Contribution.Value))
                       : (RoundedDouble) 0.0;
        }

        /// <summary>
        /// Gets or sets the value of assessment layer three.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }

        /// <summary>
        /// Gets the contribution of all relevant <see cref="GetCalculationScenarios"/> together.
        /// </summary>
        /// <param name="calculations"></param>
        public RoundedDouble GetTotalContribution(IEnumerable<PipingCalculationScenario> calculations)
        {
            return (RoundedDouble) GetCalculationScenarios(calculations).Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
        }

        /// <summary>
        /// Gets a list of <see cref="ICalculationScenario"/>.
        /// </summary>
        /// <param name="calculations"></param>
        public IEnumerable<PipingCalculationScenario> GetCalculationScenarios(IEnumerable<PipingCalculationScenario> calculations)
        {
            var lineSegments = Math2D.ConvertLinePointsToLineSegments(Section.Points);
            return calculations
                .Where(pc => pc.IsRelevant && pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        /// <summary>
        /// Gets the status of the section result depending on the calculation scenarios.
        /// </summary>
        /// <param name="calculations"></param>
        public CalculationScenarioStatus GetCalculationScenarioStatus(IEnumerable<PipingCalculationScenario> calculations)
        {
            bool failed = false;
            bool notCalculated = false;
            foreach (var calculationScenario in GetCalculationScenarios(calculations).Where(cs => cs.IsRelevant))
            {
                switch (calculationScenario.Status)
                {
                    case CalculationScenarioStatus.Failed:
                        failed = true;
                        break;
                    case CalculationScenarioStatus.NotCalculated:
                        notCalculated = true;
                        break;
                    case CalculationScenarioStatus.Done:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (failed)
            {
                return CalculationScenarioStatus.Failed;
            }

            if (notCalculated)
            {
                return CalculationScenarioStatus.NotCalculated;
            }

            return CalculationScenarioStatus.Done;
        }

        /// <summary>
        /// Gets or sets the state of the assessment layer one.
        /// </summary>
        public bool AssessmentLayerOne { get; set; }
    }
}