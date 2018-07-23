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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="FailureMechanismContribution"/>, from which the <see cref="FailureMechanismContribution.Norm"/>
    /// can be updated and the <see cref="FailureMechanismContributionItem.Contribution"/>
    /// and <see cref="FailureMechanismContributionItem.ProbabilitySpace"/> can be seen in a grid.
    /// </summary>
    public partial class FailureMechanismContributionView : UserControl, IView, IObserver
    {
        private const int isRelevantColumnIndex = 0;
        private const int probabilityPerYearColumnIndex = 4;

        /// <remarks>
        /// Actually only interested in the following changes:
        /// <list type="bullet">
        /// <item><see cref="IFailureMechanism.IsRelevant"/></item>
        /// <item><see cref="IFailureMechanism.Contribution"/></item>
        /// </list>
        /// </remarks>
        private readonly Observer failureMechanismObserver;

        private readonly IViewCommands viewCommands;
        private FailureMechanismContribution data;

        private IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionView"/>.
        /// </summary>
        /// <param name="viewCommands">Objects exposing high level <see cref="IView"/> related commands.</param>
        /// <exception cref="ArgumentNullException">When any input argument is null.</exception>
        public FailureMechanismContributionView(IViewCommands viewCommands)
        {
            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            InitializeComponent();
            InitializeGridColumns();
            SubscribeEvents();

            this.viewCommands = viewCommands;

            failureMechanismObserver = new Observer(probabilityDistributionGrid.RefreshDataGridView);
        }

        /// <summary>
        /// Gets or sets the assessment section this view belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                HandleNewAssessmentSectionSet(value);
            }
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                HandleNewDataSet((FailureMechanismContribution) value);
            }
        }

        public void UpdateObserver()
        {
            SetAssessmentSectionComposition();
            SetReturnPeriodText();
            SetGridDataSource();
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
                UnsubscribeEvents();
                DetachFromFailureMechanisms();
            }
            base.Dispose(disposing);
        }

        private void SubscribeEvents()
        {
            probabilityDistributionGrid.CellFormatting += ProbabilityDistributionGridOnCellFormatting;
            probabilityDistributionGrid.CellFormatting += DisableIrrelevantFieldsFormatting;
        }

        private void UnsubscribeEvents()
        {
            probabilityDistributionGrid.CellFormatting -= ProbabilityDistributionGridOnCellFormatting;
            probabilityDistributionGrid.CellFormatting -= DisableIrrelevantFieldsFormatting;
        }

        private void HandleNewDataSet(FailureMechanismContribution value)
        {
            DetachFromData();

            data = value;

            SetGridDataSource();
            SetReturnPeriodText();

            AttachToData();

            probabilityDistributionGrid.RefreshDataGridView();
        }

        private void HandleNewAssessmentSectionSet(IAssessmentSection value)
        {
            DetachFromFailureMechanisms();

            assessmentSection = value;

            AttachToFailureMechanisms();
            SetAssessmentSectionComposition();
        }

        private void DetachFromFailureMechanisms()
        {
            if (assessmentSection != null)
            {
                foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
                {
                    failureMechanism.Detach(failureMechanismObserver);

                    failureMechanismObserver.Dispose();
                }
            }
        }

        private void AttachToFailureMechanisms()
        {
            if (assessmentSection != null)
            {
                foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
                {
                    failureMechanism.Attach(failureMechanismObserver);
                }
            }
        }

        private void SetGridDataSource()
        {
            if (data != null)
            {
                probabilityDistributionGrid.SetDataSource(data.Distribution.Select(ci => new FailureMechanismContributionItemRow(ci, viewCommands)).ToArray());
                probabilityDistributionGrid.RefreshDataGridView();
            }
        }

        private void AttachToData()
        {
            data?.Attach(this);
        }

        private void DetachFromData()
        {
            data?.Detach(this);
        }

        private void SetReturnPeriodText()
        {
            if (data != null)
            {
                returnPeriodLabel.Text = string.Format(Resources.FailureMechanismContributionView_ReturnPeriodLabelText_Norm_is_one_over_ReturnPeriod_0_,
                                                       Convert.ToInt32(1.0 / data.Norm));
            }
        }

        private void SetAssessmentSectionComposition()
        {
            if (AssessmentSection != null)
            {
                string assessmentSectionComposition = new EnumDisplayWrapper<AssessmentSectionComposition>(AssessmentSection.Composition).DisplayName;
                assessmentSectionCompositionLabel.Text = string.Format(Resources.FailureMechanismContributionView_AssessmentSectionCompositionLabelText_AssessmentSectionComposition_0_,
                                                                       assessmentSectionComposition);
            }
        }

        private void InitializeGridColumns()
        {
            probabilityDistributionGrid.AddCheckBoxColumn(nameof(FailureMechanismContributionItemRow.IsRelevant),
                                                          Resources.FailureMechanismContributionView_GridColumn_RelevancyFilter);

            probabilityDistributionGrid.AddTextBoxColumn(nameof(FailureMechanismContributionItemRow.Assessment),
                                                         Resources.FailureMechanism_Name_DisplayName,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(nameof(FailureMechanismContributionItemRow.Code),
                                                         RingtoetsCommonFormsResources.FailureMechanism_Code_DisplayName,
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

        private void ProbabilityDistributionGridOnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (data == null)
            {
                return;
            }

            if (e.ColumnIndex == probabilityPerYearColumnIndex)
            {
                FailureMechanismContributionItem contributionItem = data.Distribution.ElementAt(e.RowIndex);
                if (Math.Abs(contributionItem.Contribution) < 1e-6)
                {
                    e.Value = Resources.FailureMechanismContributionView_ProbabilityPerYear_Not_applicable;
                    e.FormattingApplied = true;
                }
            }
        }

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (data == null)
            {
                return;
            }

            if (eventArgs.ColumnIndex != isRelevantColumnIndex)
            {
                if (!IsIrrelevantChecked(eventArgs.RowIndex))
                {
                    probabilityDistributionGrid.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    probabilityDistributionGrid.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
            else
            {
                probabilityDistributionGrid.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex, IsReadOnly(eventArgs.RowIndex));
            }
        }

        private bool IsIrrelevantChecked(int rowIndex)
        {
            return (bool) probabilityDistributionGrid.GetCell(rowIndex, isRelevantColumnIndex).Value;
        }

        private bool IsReadOnly(int rowIndex)
        {
            FailureMechanismContributionItem rowData = data.Distribution.ElementAt(rowIndex);
            return rowData.IsAlwaysRelevant;
        }

        #endregion
    }
}