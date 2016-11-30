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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Utils.Reflection;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(FailureMechanismContributionView));

        private readonly Observer isFailureMechanismRelevantObserver;
        private readonly Observer closeViewsForIrrelevantFailureMechanismObserver;
        private readonly IFailureMechanismContributionNormChangeHandler changeHandler;
        private FailureMechanismContribution data;

        private IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionView"/>.
        /// </summary>
        public FailureMechanismContributionView(IFailureMechanismContributionNormChangeHandler changeHandler)
        {
            InitializeComponent();
            InitializeGridColumns();
            InitializeAssessmentSectionCompositionComboBox();
            BindNormChange();
            BindNormInputLeave();
            SubscribeEvents();

            this.changeHandler = changeHandler;

            isFailureMechanismRelevantObserver = new Observer(probabilityDistributionGrid.RefreshDataGridView);
            closeViewsForIrrelevantFailureMechanismObserver = new Observer(CloseViewsForIrrelevantFailureMechanism);
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

        public IViewCommands ViewCommands { private get; set; }

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
                DetachFromFailureMechanisms();
                ViewCommands = null;
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
                    failureMechanism.Detach(isFailureMechanismRelevantObserver);
                    failureMechanism.Detach(closeViewsForIrrelevantFailureMechanismObserver);

                    isFailureMechanismRelevantObserver.Dispose();
                    closeViewsForIrrelevantFailureMechanismObserver.Dispose();
                }
            }
        }

        private void AttachToFailureMechanisms()
        {
            if (assessmentSection != null)
            {
                foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
                {
                    failureMechanism.Attach(isFailureMechanismRelevantObserver);
                    failureMechanism.Attach(closeViewsForIrrelevantFailureMechanismObserver);
                }
            }
        }

        private void CloseViewsForIrrelevantFailureMechanism()
        {
            if (ViewCommands != null)
            {
                var irrelevantFailureMechanisms = assessmentSection.GetFailureMechanisms().Where(failureMechanism => !failureMechanism.IsRelevant);

                foreach (var failureMechanism in irrelevantFailureMechanisms)
                {
                    ViewCommands.RemoveAllViewsForItem(failureMechanism);
                }
            }
        }

        private void SetGridDataSource()
        {
            if (data != null)
            {
                probabilityDistributionGrid.SetDataSource(data.Distribution.Select(ci => new FailureMechanismContributionItemRow(ci)).ToArray());
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
            assessmentSectionCompositionComboBox.SelectedIndexChanged += AssessmentSectionCompositionComboBoxSelectedIndexChanged;
        }

        private void UnbindAssessmentSectionCompositionChange()
        {
            assessmentSectionCompositionComboBox.SelectedIndexChanged -= AssessmentSectionCompositionComboBoxSelectedIndexChanged;
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
                normInput.Text = data.Norm.ToString(CultureInfo.CurrentCulture);
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

        private void AssessmentSectionCompositionComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            assessmentSection.ChangeComposition((AssessmentSectionComposition) assessmentSectionCompositionComboBox.SelectedValue);
            SetGridDataSource();

            if (assessmentSection.HydraulicBoundaryDatabase != null)
            {
                ClearGrassCoverErosionOutwardsHydraulicBoundaryLocations();
            }

            assessmentSection.NotifyObservers();
        }

        private void ClearGrassCoverErosionOutwardsHydraulicBoundaryLocations()
        {
            var grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                             .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                             .First();
            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations;
            bool locationsAffected = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(hydraulicBoundaryLocations)
                                                                              .Any();
            if (locationsAffected)
            {
                hydraulicBoundaryLocations.NotifyObservers();
                log.Info(RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwards_NormValueChanged_Waveheight_and_design_water_level_results_cleared);
            }
        }

        private void NormNumericUpDown_Validating(object sender, CancelEventArgs e)
        {
            if (Convert.ToInt32(normInput.Value) != assessmentSection.FailureMechanismContribution.Norm)
            {
                if (!changeHandler.ConfirmNormChange())
                {
                    e.Cancel = true;
                    RevertNormInputValue();
                }
            }
        }

        private void NormNumericUpDown_Validated(object sender, EventArgs e)
        {
            int newNormValue = Convert.ToInt32(normInput.Value);
            IEnumerable<IObservable> changedObjects = changeHandler.ChangeNorm(assessmentSection, newNormValue);
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