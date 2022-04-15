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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Data;
using Riskeer.Integration.Util;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View to show a collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>.
    /// </summary>
    public partial class FailureMechanismSectionAssemblyGroupsView : UserControl, IView
    {
        private readonly Observer failureMechanismContributionObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyGroupsView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="assessmentSection"/> is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyGroupsView(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;

            InitializeComponent();

            failureMechanismContributionObserver = new Observer(UpdateTableData)
            {
                Observable = assessmentSection.FailureMechanismContribution
            };

            UpdateTableData();
        }

        /// <summary>
        /// Gets the <see cref="AssessmentSection"/>.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismContributionObserver.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateTableData()
        {
            Tuple<AssemblyGroupBoundaries, Color, FailureMechanismSectionAssemblyGroup>[] dataToSet =
                FailureMechanismSectionAssemblyGroupsHelper.GetFailureMechanismSectionAssemblyGroupBoundaries(AssessmentSection).Select(
                    assemblyGroupBoundaries => new Tuple<AssemblyGroupBoundaries, Color, FailureMechanismSectionAssemblyGroup>(
                        assemblyGroupBoundaries,
                        FailureMechanismSectionAssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyGroupColor(assemblyGroupBoundaries.FailureMechanismSectionAssemblyGroup),
                        assemblyGroupBoundaries.FailureMechanismSectionAssemblyGroup)).ToArray();

            assemblyGroupsTable.SetData(dataToSet);
        }
    }
}