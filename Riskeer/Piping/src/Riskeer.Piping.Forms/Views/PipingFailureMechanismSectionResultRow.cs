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
using System.Collections.Generic;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public class PipingFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<PipingFailureMechanismSectionResult>
    {
        private readonly IEnumerable<IPipingCalculationScenario<PipingInput>> calculations;
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
        internal PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult,
                                                        IEnumerable<IPipingCalculationScenario<PipingInput>> calculations,
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

        
        public override void Update()
        {
        }
    }
}