﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for a <see cref="FailureMechanismContribution"/> to show 
    /// a collection of <see cref="AssessmentSectionAssemblyCategory"/>.
    /// </summary>
    public partial class AssessmentSectionAssemblyCategoriesView : UserControl, IView
    {
        private readonly Observer failureMechanismContributionObserver;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCategoriesView"/>.
        /// </summary>
        /// <param name="failureMechanismContribution">The failure mechanism contribution belonging to the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismContribution"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionAssemblyCategoriesView(FailureMechanismContribution failureMechanismContribution)
        {
            if (failureMechanismContribution == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismContribution));
            }

            InitializeComponent();

            failureMechanismContributionObserver = new Observer(UpdateTableData)
            {
                Observable = failureMechanismContribution
            };

            FailureMechanismContribution = failureMechanismContribution;

            UpdateTableData();
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismContribution"/> the view belongs to.
        /// </summary>
        public FailureMechanismContribution FailureMechanismContribution { get; }

        public object Data { get; set; }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed; otherwise, <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            failureMechanismContributionObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateTableData()
        {
            assessmentSectionAssemblyCategoriesTable.SetData(AssemblyToolCategoriesFactory.CreateAssessmentSectionAssemblyCategories(
                                                                 FailureMechanismContribution.SignalingNorm,
                                                                 FailureMechanismContribution.LowerLimitNorm));
        }
    }
}