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
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>
    /// for a piping assessment.
    /// </summary>
    public class PipingFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public PipingFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            CalculationScenarios = new List<ICalculationScenario>();
        }

        /// <summary>
        /// Gets or sets the state of the assessment layer one.
        /// </summary>
        public bool AssessmentLayerOne { get; set; }

        /// <summary>
        /// Gets the value of assessment layer two a.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoA
        {
            get
            {
                return CalculationScenarios.Where(cs => cs.IsRelevant && cs.CalculationScenarioStatus == CalculationScenarioStatus.Done)
                                           .Aggregate((RoundedDouble) 0.0, (current, scenario) => (current + scenario.Contribution * scenario.Probability));
            }
        }

        /// <summary>
        /// Gets or sets the value of assessment layer two b.
        /// </summary>
        public RoundedDouble AssessmentLayerTwoB { get; set; }

        /// <summary>
        /// Gets or sets the value of assessment layer three.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }

        /// <summary>
        /// Gets the contribution of all relevant <see cref="CalculationScenarios"/> together.
        /// </summary>
        public RoundedDouble TotalContribution
        {
            get
            {
                return (RoundedDouble) CalculationScenarios.Where(cs => cs.IsRelevant)
                                                           .Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="ICalculationScenario"/>.
        /// </summary>
        public List<ICalculationScenario> CalculationScenarios { get; private set; }

        /// <summary>
        /// Gets the status of the section result depending on the calculation scenarios.
        /// </summary>
        public CalculationScenarioStatus CalculationScenarioStatus
        {
            get
            {
                return GetCalculationStatus();
            }
        }

        private CalculationScenarioStatus GetCalculationStatus()
        {
            bool failed = false;
            bool notCalculated = false;
            foreach (var calculationScenario in CalculationScenarios.Where(cs => cs.IsRelevant))
            {
                switch (calculationScenario.CalculationScenarioStatus) 
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
    }
}