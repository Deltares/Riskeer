﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;
using CommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsGrassCoverErosionOutwardsFormsResources = Ringtoets.GrassCoverErosionOutwards.Forms.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

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

        /// <summary>
        /// This observer is listening for changes to:
        /// <list type="bullet">
        /// <item><see cref="IFailureMechanism.IsRelevant"/></item>
        /// <item><see cref="IFailureMechanism.Contribution"/></item>
        /// </list>
        /// </summary>
        private readonly Observer failureMechanismObserver;

        private readonly IFailureMechanismContributionNormChangeHandler normChangeHandler;
        private readonly IAssessmentSectionCompositionChangeHandler compositionChangeHandler;
        private readonly IViewCommands viewCommands;
        private FailureMechanismContribution data;

        private IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionView"/>.
        /// </summary>
        /// <param name="normChangeHandler">The object responsible for handling the change
        /// in the <see cref="FailureMechanismContribution.Norm"/>.</param>
        /// <param name="compositionChangeHandler">The object responsible for handling the
        /// change in the <see cref="IAssessmentSection.Composition"/>.</param>
        /// <param name="viewCommands">Objects exposing high level <see cref="IView"/> related commands.</param>
        /// <exception cref="ArgumentNullException">When any input argument is null.</exception>
        public FailureMechanismContributionView(IFailureMechanismContributionNormChangeHandler normChangeHandler,
                                                IAssessmentSectionCompositionChangeHandler compositionChangeHandler,
            IViewCommands viewCommands)
        {
            if (normChangeHandler == null)
            {
                throw new ArgumentNullException("normChangeHandler");
            }
            if (compositionChangeHandler == null)
            {
                throw new ArgumentNullException("compositionChangeHandler");
            }
            if (viewCommands == null)
            {
                throw new ArgumentNullException("viewCommands");
            }

            InitializeComponent();
            InitializeGridColumns();
            InitializeAssessmentSectionCompositionComboBox();
            BindNormChange();
            BindNormInputLeave();
            SubscribeEvents();

            this.normChangeHandler = normChangeHandler;
            this.compositionChangeHandler = compositionChangeHandler;
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
            SetNormText();
            probabilityDistributionGrid.RefreshDataGridView();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed; otherwise, <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                UnsubscribeEvents();
                UnbindAssessmentSectionCompositionChange();
                DetachFromFailureMechanisms();
            }
            base.Dispose(disposing);
        }

        private void InitializeAssessmentSectionCompositionComboBox()
        {
            assessmentSectionCompositionComboBox.DataSource = new[]
            {
                Tuple.Create(AssessmentSectionComposition.Dike, RingtoetsIntegrationFormsResources.FailureMechanismContributionView_InitializeAssessmentSectionCompositionComboBox_Dike),
                Tuple.Create(AssessmentSectionComposition.Dune, RingtoetsIntegrationFormsResources.FailureMechanismContributionView_InitializeAssessmentSectionCompositionComboBox_Dune),
                Tuple.Create(AssessmentSectionComposition.DikeAndDune, RingtoetsIntegrationFormsResources.FailureMechanismContributionView_InitializeAssessmentSectionCompositionComboBox_DikeAndDune)
            };
            assessmentSectionCompositionComboBox.ValueMember = TypeUtils.GetMemberName<Tuple<AssessmentSectionComposition, string>>(t => t.Item1);
            assessmentSectionCompositionComboBox.DisplayMember = TypeUtils.GetMemberName<Tuple<AssessmentSectionComposition, string>>(t => t.Item2);
        }

        private void SubscribeEvents()
        {
            probabilityDistributionGrid.AddCellFormattingHandler(ProbabilityDistributionGridOnCellFormatting);
            probabilityDistributionGrid.AddCellFormattingHandler(DisableIrrelevantFieldsFormatting);
        }

        private void UnsubscribeEvents()
        {
            probabilityDistributionGrid.RemoveCellFormattingHandler(ProbabilityDistributionGridOnCellFormatting);
            probabilityDistributionGrid.RemoveCellFormattingHandler(DisableIrrelevantFieldsFormatting);
        }

        private void HandleNewDataSet(FailureMechanismContribution value)
        {
            UnbindNormChange();
            DetachFromData();

            data = value;

            SetGridDataSource();
            SetNormText();

            AttachToData();
            BindNormChange();

            probabilityDistributionGrid.RefreshDataGridView();
        }

        private void HandleNewAssessmentSectionSet(IAssessmentSection value)
        {
            UnbindAssessmentSectionCompositionChange();
            DetachFromFailureMechanisms();

            assessmentSection = value;

            AttachToFailureMechanisms();
            SetAssessmentSectionComposition();
            BindAssessmentSectionCompositionChange();
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
            if (data != null)
            {
                data.Attach(this);
            }
        }

        private void DetachFromData()
        {
            if (data != null)
            {
                data.Detach(this);
            }
        }

        private void BindAssessmentSectionCompositionChange()
        {
            assessmentSectionCompositionComboBox.SelectionChangeCommitted += AssessmentSectionCompositionComboBoxSelectionChangeComitted;
        }

        private void UnbindAssessmentSectionCompositionChange()
        {
            assessmentSectionCompositionComboBox.SelectionChangeCommitted -= AssessmentSectionCompositionComboBoxSelectionChangeComitted;
        }

        private void BindNormChange()
        {
            // Attaching to inner TextBox instead of 'normInput' control to capture all
            // key presses. (This prevents some unexpected unresponsive behavior):
            var innerTextBox = normInput.Controls.OfType<TextBox>().First();
            innerTextBox.KeyDown += NormNumericUpDownInnerTextBox_KeyDown;
        }

        private void UnbindNormChange()
        {
            var innerTextBox = normInput.Controls.OfType<TextBox>().First();
            innerTextBox.KeyDown -= NormNumericUpDownInnerTextBox_KeyDown;
        }

        private void BindNormInputLeave()
        {
            normInput.Leave += NormInputLeave;
        }

        private void SetNormText()
        {
            if (data != null)
            {
                // Note: Set the Text instead of value to ensure Value property is correct when handling Validating events.
                normInput.Text = Convert.ToInt32(1.0/data.Norm).ToString(CultureInfo.CurrentCulture);
            }
        }

        private void SetAssessmentSectionComposition()
        {
            if (AssessmentSection != null)
            {
                assessmentSectionCompositionComboBox.SelectedValue = AssessmentSection.Composition;
            }
        }

        private void InitializeGridColumns()
        {
            probabilityDistributionGrid.AddCheckBoxColumn(TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.IsRelevant),
                                                          CommonGuiResources.FailureMechanismContributionView_GridColumn_RelevancyFilter);

            probabilityDistributionGrid.AddTextBoxColumn(TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.Assessment),
                                                         CommonGuiResources.FailureMechanismContributionView_GridColumn_Assessment,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.Code),
                                                         CommonGuiResources.FailureMechanismContributionView_GridColumn_AssessmentCode,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.Contribution),
                                                         CommonGuiResources.FailureMechanismContributionView_GridColumn_Contribution,
                                                         true);

            probabilityDistributionGrid.AddTextBoxColumn(TypeUtils.GetMemberName<FailureMechanismContributionItemRow>(fmci => fmci.ProbabilitySpace),
                                                         CommonGuiResources.FailureMechanismContributionView_GridColumn_ProbabilitySpace,
                                                         true,
                                                         DataGridViewAutoSizeColumnMode.Fill,
                                                         100,
                                                         "1/#,#");
        }

        #region Event handling

        private void NormInputLeave(object sender, EventArgs e)
        {
            ResetTextIfEmpty();
        }

        private void ResetTextIfEmpty()
        {
            if (string.IsNullOrEmpty(normInput.Text))
            {
                normInput.Text = normInput.Value.ToString(CultureInfo.CurrentCulture);
            }
        }

        private void ProbabilityDistributionGridOnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (data == null)
            {
                return;
            }

            if (e.ColumnIndex == probabilityPerYearColumnIndex)
            {
                var contributionItem = data.Distribution.ElementAt(e.RowIndex);
                if (Math.Abs(contributionItem.Contribution) < 1e-6)
                {
                    e.Value = RingtoetsIntegrationFormsResources.FailureMechanismContributionView_ProbabilityPerYear_Not_applicable;
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

        private void AssessmentSectionCompositionComboBoxSelectionChangeComitted(object sender, EventArgs e)
        {
            var newComposition = (AssessmentSectionComposition)assessmentSectionCompositionComboBox.SelectedValue;
            if (assessmentSection.Composition != newComposition && compositionChangeHandler.ConfirmCompositionChange())
            {
                IEnumerable<IObservable> changedObjects = compositionChangeHandler.ChangeComposition(assessmentSection, newComposition);
                foreach (IObservable changedObject in changedObjects)
                {
                    changedObject.NotifyObservers();
                }
            }
            else
            {
                assessmentSectionCompositionComboBox.SelectedValue = assessmentSection.Composition;
            }
        }

        private void NormNumericUpDown_Validating(object sender, CancelEventArgs e)
        {
            int returnPeriod = Convert.ToInt32(normInput.Value);
            if (returnPeriod != 0 && assessmentSection.FailureMechanismContribution.Norm.CompareTo(1.0/returnPeriod) != 0)
            {
                if (!normChangeHandler.ConfirmNormChange())
                {
                    e.Cancel = true;
                    RevertNormInputValue();
                }
            }
        }

        private void NormNumericUpDown_Validated(object sender, EventArgs e)
        {
            double newNormValue = 1.0/Convert.ToInt32(normInput.Value);
            IEnumerable<IObservable> changedObjects = normChangeHandler.ChangeNorm(assessmentSection, newNormValue);
            foreach (IObservable changedObject in changedObjects)
            {
                changedObject.NotifyObservers();
            }
        }

        private void NormNumericUpDownInnerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RevertNormInputValue();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ActiveControl = null;

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void RevertNormInputValue()
        {
            SetNormText();
            ActiveControl = null;
        }

        #endregion
    }
}