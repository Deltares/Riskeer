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
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismResultView : FailureMechanismResultView<MacroStabilityInwardsFailureMechanismSectionResult,
        MacroStabilityInwardsFailureMechanismSectionResultRow, MacroStabilityInwardsFailureMechanism>
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of
        /// <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/> to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismResultView(IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> failureMechanismSectionResults,
                                                               MacroStabilityInwardsFailureMechanism failureMechanism,
                                                               IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        protected override MacroStabilityInwardsFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(MacroStabilityInwardsFailureMechanismSectionResult sectionResult)
        {
            throw new NotImplementedException();
        }

        protected override double GetN()
        {
            throw new NotImplementedException();
        }

        protected override void AddDataGridColumns()
        {
            throw new NotImplementedException();
        }
    }
}