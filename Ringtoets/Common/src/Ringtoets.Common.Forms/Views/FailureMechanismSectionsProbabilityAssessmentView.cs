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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// View for a collection of <see cref="FailureMechanismSection"/> that have a section specific N.
    /// </summary>
    public class FailureMechanismSectionsProbabilityAssessmentView : FailureMechanismSectionsView
    {
        private readonly ProbabilityAssessmentInput probabilityAssessmentInput;

        private readonly Observer failureMechanismObserver;
        private double currentA;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsProbabilityAssessmentView"/>.
        /// </summary>
        /// <param name="sections">The sections to be displayed in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the view belongs to.</param>
        /// <param name="probabilityAssessmentInput">The probability assessment input belonging to the
        /// failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public FailureMechanismSectionsProbabilityAssessmentView(IEnumerable<FailureMechanismSection> sections,
                                                                 IFailureMechanism failureMechanism,
                                                                 ProbabilityAssessmentInput probabilityAssessmentInput)
            : base(sections, failureMechanism)
        {
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentInput));
            }

            this.probabilityAssessmentInput = probabilityAssessmentInput;

            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionProbabilityAssessmentRow.N),
                                                                         Resources.FailureMechanismSectionProbabilityAssessment_N_Rounded_DisplayName,
                                                                         true);

            failureMechanismObserver = new Observer(HandleProbabilityAssessmentInputChange)
            {
                Observable = failureMechanism
            };
        }

        protected override void SetDataGridViewControlData()
        {
            currentA = probabilityAssessmentInput.A;

            var rows = new List<FailureMechanismSectionProbabilityAssessmentRow>();
            double startDistance = 0;
            foreach (FailureMechanismSection section in sections)
            {
                double endDistance = startDistance + section.Length;

                rows.Add(new FailureMechanismSectionProbabilityAssessmentRow(section, startDistance, endDistance, probabilityAssessmentInput));

                startDistance = endDistance;
            }

            failureMechanismSectionsDataGridViewControl.SetDataSource(rows);
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();

            base.Dispose(disposing);
        }

        private void HandleProbabilityAssessmentInputChange()
        {
            if (currentA.Equals(probabilityAssessmentInput.A))
            {
                return;
            }

            currentA = probabilityAssessmentInput.A;

            failureMechanismSectionsDataGridViewControl.RefreshDataGridView();
        }
    }
}