// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="PipingFailureMechanismSectionResult"/>, which takes care of the
    /// representation of properties in a grid.
    /// </summary>
    internal class PipingFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<PipingFailureMechanismSectionResult>
    {
        private const double tolerance = 1e-6;
        private readonly IEnumerable<PipingCalculationScenario> calculations;
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="PipingFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult,
                                                      IEnumerable<PipingCalculationScenario> calculations,
                                                      PipingFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection)
            : base(sectionResult)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.calculations = calculations;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets or sets the value representing the simple assessment result.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentResult
        {
            get
            {
                return SectionResult.SimpleAssessmentResult;
            }
            set
            {
                SectionResult.SimpleAssessmentResult = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="value"/> is outside of the valid ranges.</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                SectionResult.AssessmentLayerThree = value;
            }
        }

        /// <summary>
        /// Gets the assessment layer two a of the <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double AssessmentLayerTwoA
        {
            get
            {
                PipingCalculationScenario[] relevantScenarios = SectionResult.GetCalculationScenarios(calculations).ToArray();
                bool relevantScenarioAvailable = relevantScenarios.Length != 0;

                if (relevantScenarioAvailable && Math.Abs(SectionResult.GetTotalContribution(relevantScenarios) - 1.0) > tolerance)
                {
                    return double.NaN;
                }

                if (!relevantScenarioAvailable || SectionResult.GetCalculationScenarioStatus(relevantScenarios) != CalculationScenarioStatus.Done)
                {
                    return double.NaN;
                }

                return SectionResult.GetAssessmentLayerTwoA(relevantScenarios, failureMechanism, assessmentSection);
            }
        }

        /// <summary>
        /// Gets the <see cref="PipingFailureMechanismSectionResult"/> that is the source of this row.
        /// </summary>
        public PipingFailureMechanismSectionResult GetSectionResult
        {
            get
            {
                return SectionResult;
            }
        }
    }
}