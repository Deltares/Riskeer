﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Gui.Commands;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="IAssessmentSection.FailureMechanismContribution"/>.
    /// </summary>
    public partial class FailureMechanismContributionView : UserControl, IView
    {
        private const int isRelevantColumnIndex = 0;
        private const int nameColumnIndex = 1;
        private const int codeColumnIndex = 2;
        private const int contributionColumnIndex = 3;
        private const int probabilitySpaceColumnIndex = 4;

        /// <remarks>
        /// Actually only interested in the following changes:
        /// <list type="bullet">
        /// <item><see cref="IFailureMechanism.IsRelevant"/></item>
        /// <item><see cref="IFailureMechanism.Contribution"/></item>
        /// </list>
        /// </remarks>
        private readonly List<Observer> failureMechanismObservers;

        private readonly Observer failureMechanismContributionObserver;
        private readonly Observer assessmentSectionObserver;

        private readonly IViewCommands viewCommands;

        private bool rowUpdating;
        private FailureMechanismContributionItemRow[] failureMechanismContributionItemRows;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the data from.</param>
        /// <param name="viewCommands">Objects exposing high level <see cref="IView"/> related commands.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismContributionView(IAssessmentSection assessmentSection,
                                                IViewCommands viewCommands)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            InitializeComponent();

            this.viewCommands = viewCommands;

            failureMechanismObservers = new List<Observer>();
            failureMechanismContributionObserver = new Observer(() =>
            {
                probabilityDistributionGrid.RefreshDataGridView();
                SetReturnPeriodText();
            })
            {
                Observable = assessmentSection.FailureMechanismContribution
            };
            assessmentSectionObserver = new Observer(() =>
            {
                DetachFromFailureMechanisms();
                UpdateView();
                AttachToFailureMechanisms();
            })
            {
                Observable = assessmentSection
            };

            AssessmentSection = assessmentSection;
            AttachToFailureMechanisms();
        }

        /// <summary>
        /// Gets the assessment section this view belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeGridColumns();

            probabilityDistributionGrid.CellFormatting += HandleCellStyling;
            probabilityDistributionGrid.CellFormatting += ProbabilityDistributionGridOnCellFormatting;

            UpdateView();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed; otherwise, <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                DetachFromFailureMechanisms();
                failureMechanismContributionObserver.Dispose();
                assessmentSectionObserver.Dispose();

                RemoveRowEvents();
            }

            base.Dispose(disposing);
        }

        private void UpdateView()
        {
            SetAssessmentSectionComposition();
            SetReturnPeriodText();
            SetGridDataSource();
        }

        private Observer CreateFailureMechanismObserver(IFailureMechanism failureMechanism)
        {
            return new Observer(() =>
            {
                if (rowUpdating)
                {
                    return;
                }

                failureMechanismContributionItemRows.ForEachElementDo(row => row.Update());
                probabilityDistributionGrid.RefreshDataGridView();
            })
            {
                Observable = failureMechanism
            };
        }

        private void DetachFromFailureMechanisms()
        {
            failureMechanismObservers.ForEachElementDo(o => o.Dispose());
            failureMechanismObservers.Clear();
        }

        private void AttachToFailureMechanisms()
        {
            failureMechanismObservers.AddRange(AssessmentSection.GetContributingFailureMechanisms()
                                                                .Select(CreateFailureMechanismObserver)
                                                                .ToArray());
        }

        private void SetGridDataSource()
        {
            RemoveRowEvents();
            failureMechanismContributionItemRows = AssessmentSection.GetContributingFailureMechanisms()
                                                                    .Select(fm => new FailureMechanismContributionItemRow(
                                                                                fm, AssessmentSection.FailureMechanismContribution,
                                                                                viewCommands, new FailureMechanismContributionItemRow.ConstructionProperties
                                                                                {
                                                                                    IsRelevantColumnIndex = isRelevantColumnIndex,
                                                                                    NameColumnIndex = nameColumnIndex,
                                                                                    CodeColumnIndex = codeColumnIndex,
                                                                                    ContributionColumnIndex = contributionColumnIndex,
                                                                                    ProbabilitySpaceColumnIndex = probabilitySpaceColumnIndex
                                                                                })).ToArray();

            probabilityDistributionGrid.SetDataSource(failureMechanismContributionItemRows);

            failureMechanismContributionItemRows.ForEachElementDo(row =>
            {
                row.RowUpdated += RowUpdated;
                row.RowUpdateDone += RowUpdateDone;
            });
        }

        private void RemoveRowEvents()
        {
            failureMechanismContributionItemRows?.ForEachElementDo(row =>
            {
                row.RowUpdated -= RowUpdated;
                row.RowUpdateDone -= RowUpdateDone;
            });
        }

        private void RowUpdateDone(object sender, EventArgs eventArgs)
        {
            rowUpdating = false;
        }

        private void RowUpdated(object sender, EventArgs eventArgs)
        {
            rowUpdating = true;
            probabilityDistributionGrid.RefreshDataGridView();
        }

        private void SetReturnPeriodText()
        {
            returnPeriodLabel.Text = string.Format(Resources.FailureMechanismContributionView_ReturnPeriodLabelText_Norm_is_one_over_ReturnPeriod_0_,
                                                   Convert.ToInt32(1.0 / AssessmentSection.FailureMechanismContribution.Norm));
        }

        private void SetAssessmentSectionComposition()
        {
            string assessmentSectionComposition = new EnumDisplayWrapper<AssessmentSectionComposition>(AssessmentSection.Composition).DisplayName;
            assessmentSectionCompositionLabel.Text = string.Format(Resources.FailureMechanismContributionView_AssessmentSectionCompositionLabelText_AssessmentSectionComposition_0_,
                                                                   assessmentSectionComposition);
        }

        private void InitializeGridColumns()
        {
            probabilityDistributionGrid.AddCheckBoxColumn(nameof(FailureMechanismContributionItemRow.IsRelevant),
                                                          Resources.FailureMechanism_IsRelevant_DisplayName);

            probabilityDistributionGrid.AddTextBoxColumn(nameof(FailureMechanismContributionItemRow.Name),
                                                         Resources.FailureMechanism_Name_DisplayName,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(nameof(FailureMechanismContributionItemRow.Code),
                                                         RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(nameof(FailureMechanismContributionItemRow.Contribution),
                                                         Resources.FailureMechanismContributionView_GridColumn_Contribution,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(nameof(FailureMechanismContributionItemRow.ProbabilitySpace),
                                                         Resources.FailureMechanismContributionView_GridColumn_ProbabilitySpace,
                                                         true,
                                                         DataGridViewAutoSizeColumnMode.Fill,
                                                         100,
                                                         "1/#,#");
        }

        #region Event handling

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            probabilityDistributionGrid.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        private void ProbabilityDistributionGridOnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == probabilitySpaceColumnIndex)
            {
                var row = (FailureMechanismContributionItemRow) probabilityDistributionGrid.Rows[e.RowIndex].DataBoundItem;
                if (Math.Abs(row.Contribution) < 1e-6)
                {
                    e.Value = Resources.FailureMechanismContributionView_ProbabilityPerYear_Not_applicable;
                    e.FormattingApplied = true;
                }
            }
        }

        #endregion
    }
}