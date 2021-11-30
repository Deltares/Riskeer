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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.Properties;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// View for configuring piping calculation scenarios.
    /// </summary>
    public partial class PipingScenariosView : UserControl, IView
    {
        private const int failureProbabilityUpliftColumnIndex = 3;
        private const int failureProbabilityHeaveColumnIndex = 4;
        private const int failureProbabilitySellmeijerColumnIndex = 5;

        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private CalculationGroup calculationGroup;

        private Observer failureMechanismObserver;
        private RecursiveObserver<IObservableEnumerable<PipingScenarioConfigurationPerFailureMechanismSection>, PipingScenarioConfigurationPerFailureMechanismSection> scenarioConfigurationsPerFailureMechanismSectionObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, IPipingCalculationScenario<PipingInput>> calculationObserver;
        private RecursiveObserver<CalculationGroup, PipingInput> calculationInputObserver;

        private IEnumerable<IPipingScenarioRow> scenarioRows;
        private bool selectConfigurationTypeComboBoxUpdating;
        private PipingScenariosViewFailureMechanismSectionViewModel selectedFailureMechanismSection;

        private RadioButton checkedRadioButton;

        /// <summary>
        /// Creates a new instance of <see cref="PipingScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/>
        /// to get the sections from.</param>
        /// <param name="assessmentSection">The assessment section the scenarios belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public PipingScenariosView(CalculationGroup calculationGroup, PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.calculationGroup = calculationGroup;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            InitializeObservers();

            InitializeComponent();

            InitializeCombobox();
            InitializeWarningIcon();

            checkedRadioButton = radioButtonSemiProbabilistic;

            InitializeListBox();
            InitializeDataGridView();

            UpdateSectionsListBox();
            UpdateDataGridViewDataSource();

            UpdateVisibility();
        }

        public object Data
        {
            get => calculationGroup;
            set => calculationGroup = (CalculationGroup) value;
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            scenarioConfigurationsPerFailureMechanismSectionObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            calculationInputObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeCombobox()
        {
            EnumDisplayWrapper<PipingScenarioConfigurationType>[] enumDisplayWrappers = Enum.GetValues(typeof(PipingScenarioConfigurationType))
                                                                                            .OfType<PipingScenarioConfigurationType>()
                                                                                            .Select(ev => new EnumDisplayWrapper<PipingScenarioConfigurationType>(ev))
                                                                                            .ToArray();

            selectConfigurationTypeComboBox.BeginUpdate();
            selectConfigurationTypeComboBoxUpdating = true;
            selectConfigurationTypeComboBox.DataSource = enumDisplayWrappers;
            selectConfigurationTypeComboBox.ValueMember = nameof(EnumDisplayWrapper<PipingScenarioConfigurationType>.Value);
            selectConfigurationTypeComboBox.DisplayMember = nameof(EnumDisplayWrapper<PipingScenarioConfigurationType>.DisplayName);
            selectConfigurationTypeComboBox.SelectedValue = failureMechanism.ScenarioConfigurationType;
            selectConfigurationTypeComboBoxUpdating = false;
            selectConfigurationTypeComboBox.EndUpdate();
        }

        private void InitializeWarningIcon()
        {
            warningIcon.BackgroundImage = CoreGuiResources.warning.ToBitmap();
            toolTip.SetToolTip(warningIcon, Resources.PipingScenariosView_InitializeInfoIcon_ScenarioConfigurationType_PerSection_ToolTip);
        }

        private void InitializeObservers()
        {
            failureMechanismObserver = new Observer(UpdateSectionsListBox)
            {
                Observable = failureMechanism
            };

            scenarioConfigurationsPerFailureMechanismSectionObserver = new RecursiveObserver<IObservableEnumerable<PipingScenarioConfigurationPerFailureMechanismSection>, PipingScenarioConfigurationPerFailureMechanismSection>(
                UpdateSectionsListBox, section => section)
            {
                Observable = failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
            };

            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };

            calculationObserver = new RecursiveObserver<CalculationGroup, IPipingCalculationScenario<PipingInput>>(UpdateScenarioRows, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, PipingInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(
                                                                                                pcg.Children.OfType<IPipingCalculationScenario<PipingInput>>()
                                                                                                   .Select(c => c.InputParameters)))
            {
                Observable = calculationGroup
            };
        }

        private void InitializeListBox()
        {
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(
                nameof(PipingScenarioRow<IPipingCalculationScenario<PipingInput>>.IsRelevant),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_In_final_rating
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow<IPipingCalculationScenario<PipingInput>>.Contribution),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_Contribution
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow<IPipingCalculationScenario<PipingInput>>.Name),
                RiskeerCommonFormsResources.ScenarioView_Name_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(SemiProbabilisticPipingScenarioRow.FailureProbabilityUplift),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityUplift
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(SemiProbabilisticPipingScenarioRow.FailureProbabilityHeave),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityHeave
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(SemiProbabilisticPipingScenarioRow.FailureProbabilitySellmeijer),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilitySellmeijer
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow<IPipingCalculationScenario<PipingInput>>.FailureProbability),
                RiskeerCommonFormsResources.ScenarioView_ProfileFailureProbability_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(IPipingScenarioRow.SectionFailureProbability),
                RiskeerCommonFormsResources.ScenarioView_SectionFailureProbability_DisplayName
            );
        }

        private void UpdateVisibility()
        {
            bool perFailureMechanismSemiProbabilistic = selectedFailureMechanismSection != null
                                                            ? selectedFailureMechanismSection.ScenarioConfigurationPerSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
                                                            : radioButtonSemiProbabilistic.Checked;

            bool perFailureMechanismSection = failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.PerFailureMechanismSection;
            bool semiProbabilisticColumnsVisible = failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                                                   || perFailureMechanismSection && perFailureMechanismSemiProbabilistic;

            radioButtonsPanel.Visible = perFailureMechanismSection;
            warningIcon.Visible = perFailureMechanismSection;
            dataGridViewControl.GetColumnFromIndex(failureProbabilityUpliftColumnIndex).Visible = semiProbabilisticColumnsVisible;
            dataGridViewControl.GetColumnFromIndex(failureProbabilityHeaveColumnIndex).Visible = semiProbabilisticColumnsVisible;
            dataGridViewControl.GetColumnFromIndex(failureProbabilitySellmeijerColumnIndex).Visible = semiProbabilisticColumnsVisible;
        }

        private void UpdateDataGridViewDataSource()
        {
            if (selectedFailureMechanismSection == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            scenarioRows = GetScenarioRows();
            dataGridViewControl.SetDataSource(scenarioRows);
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (failureMechanism.Sections.Any())
            {
                PipingScenariosViewFailureMechanismSectionViewModel[] failureMechanismSectionViewModels = failureMechanism.Sections.Select(
                    section => new PipingScenariosViewFailureMechanismSectionViewModel(
                        section, failureMechanism,
                        failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
                                        .First(sc => sc.Section == section))).ToArray();

                listBox.Items.AddRange(failureMechanismSectionViewModels.Cast<object>().ToArray());
                listBox.SelectedItem = selectedFailureMechanismSection != null
                                           ? failureMechanismSectionViewModels.FirstOrDefault(vm => vm.Section == selectedFailureMechanismSection.Section)
                                             ?? failureMechanismSectionViewModels.First()
                                           : failureMechanismSectionViewModels.First();
            }
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            selectedFailureMechanismSection = listBox.SelectedItem as PipingScenariosViewFailureMechanismSectionViewModel;
            UpdateRadioButtons();
            UpdateDataGridViewDataSource();
        }

        private void UpdateRadioButtons()
        {
            bool semiProbabilisticChecked = selectedFailureMechanismSection.ScenarioConfigurationPerSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
            radioButtonSemiProbabilistic.Checked = semiProbabilisticChecked;
            radioButtonProbabilistic.Checked = !semiProbabilisticChecked;
        }

        private void UpdateScenarioRows()
        {
            scenarioRows.ForEachElementDo(row => row.Update());
            dataGridViewControl.RefreshDataGridView();
        }

        private IEnumerable<IPipingScenarioRow> GetScenarioRows()
        {
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(selectedFailureMechanismSection.Section.Points);

            return failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                   || failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.PerFailureMechanismSection
                   && selectedFailureMechanismSection.ScenarioConfigurationPerSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
                       ? GetSemiProbabilisticPipingScenarioRows(lineSegments)
                       : GetProbabilisticPipingScenarioRows(lineSegments);
        }

        private IEnumerable<IPipingScenarioRow> GetSemiProbabilisticPipingScenarioRows(IEnumerable<Segment2D> lineSegments)
        {
            return GetScenarios<SemiProbabilisticPipingCalculationScenario>(lineSegments)
                   .Select(pc => new SemiProbabilisticPipingScenarioRow(pc, failureMechanism, selectedFailureMechanismSection.Section, assessmentSection))
                   .ToList();
        }

        private IEnumerable<IPipingScenarioRow> GetProbabilisticPipingScenarioRows(IEnumerable<Segment2D> lineSegments)
        {
            return GetScenarios<ProbabilisticPipingCalculationScenario>(lineSegments)
                   .Select(pc => new ProbabilisticPipingScenarioRow(pc))
                   .ToList();
        }

        private IEnumerable<T> GetScenarios<T>(IEnumerable<Segment2D> lineSegments)
            where T : IPipingCalculationScenario<PipingInput>
        {
            return calculationGroup.GetCalculations()
                                   .OfType<T>()
                                   .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        private void SelectConfigurationTypeComboBox_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectConfigurationTypeComboBoxUpdating || selectConfigurationTypeComboBox.SelectedIndex == -1)
            {
                return;
            }

            failureMechanism.ScenarioConfigurationType = (PipingScenarioConfigurationType) selectConfigurationTypeComboBox.SelectedValue;
            failureMechanism.NotifyObservers();

            UpdateVisibility();
            UpdateDataGridViewDataSource();
        }

        private void RadioButton_OnCheckedChanged(object sender, EventArgs e)
        {
            var newCheckedRadioButton = (RadioButton) sender;
            if (checkedRadioButton == newCheckedRadioButton)
            {
                return;
            }

            if (selectedFailureMechanismSection?.ScenarioConfigurationPerSection != null)
            {
                selectedFailureMechanismSection.ScenarioConfigurationPerSection.ScenarioConfigurationType = newCheckedRadioButton == radioButtonSemiProbabilistic
                                                                                                                ? PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
                                                                                                                : PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic;
                selectedFailureMechanismSection.ScenarioConfigurationPerSection.NotifyObservers();
            }

            UpdateVisibility();
            UpdateDataGridViewDataSource();
            checkedRadioButton = newCheckedRadioButton;
        }
    }
}