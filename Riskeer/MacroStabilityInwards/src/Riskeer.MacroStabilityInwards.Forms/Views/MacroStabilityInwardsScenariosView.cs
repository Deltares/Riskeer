// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Controls;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// View for configuring macrostability inwards calculation scenarios.
    /// </summary>
    public partial class MacroStabilityInwardsScenariosView : UserControl, IView
    {
        private const int totalScenarioContributionNrOfDecimals = 2;
        private readonly MacroStabilityInwardsFailureMechanism failureMechanism;
        private CalculationGroup calculationGroup;

        private Observer failureMechanismObserver;

        private RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration>
            failureMechanismSectionConfigurationsObserver;

        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario> calculationObserver;
        private RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput> calculationInputObserver;

        private IEnumerable<MacroStabilityInwardsScenarioRow> scenarioRows;
        private MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel selectedFailureMechanismSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// to get the sections from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsScenariosView(CalculationGroup calculationGroup, MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.calculationGroup = calculationGroup;
            this.failureMechanism = failureMechanism;

            InitializeObservers();

            failureMechanismSectionConfigurationControl =
                new FailureMechanismSectionConfigurationControl(failureMechanism.GeneralInput.B);
            InitializeComponent();

            InitializeListBox();
            InitializeDataGridView();

            UpdateSectionsListBox();
            UpdateScenarioControls();
            UpdateFailureMechanismSectionConfigurationControl();
        }

        public object Data
        {
            get => calculationGroup;
            set => calculationGroup = (CalculationGroup) value;
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            failureMechanismSectionConfigurationsObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            calculationInputObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeObservers()
        {
            failureMechanismObserver = new Observer(() =>
            {
                UpdateSectionsListBox();
                UpdateFailureMechanismSectionConfigurationControl();
            })
            {
                Observable = failureMechanism
            };

            failureMechanismSectionConfigurationsObserver = new RecursiveObserver<IObservableEnumerable<
                FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration>(
                () =>
                {
                    UpdateSectionsListBox();
                    UpdateScenarioRows();
                }, section => section)
            {
                Observable = failureMechanism.SectionConfigurations
            };

            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateScenarioControls, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };

            calculationObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario>(() =>
            {
                UpdateScenarioRows();
                UpdateTotalScenarioContributionLabel();
            }, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput>(
                UpdateScenarioControls, pcg => pcg.Children.Concat<object>(pcg.Children
                                                                              .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                              .Select(c => c.InputParameters)))
            {
                Observable = calculationGroup
            };
        }

        private void InitializeListBox()
        {
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (failureMechanism.Sections.Any())
            {
                MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel[] failureMechanismSectionViewModels = failureMechanism.Sections.Select(
                    section => new MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel(failureMechanism.SectionConfigurations
                                                                                                                      .First(sc => sc.Section == section))).ToArray();

                listBox.Items.AddRange(failureMechanismSectionViewModels.Cast<object>().ToArray());
                listBox.SelectedItem =
                    selectedFailureMechanismSection != null
                        ? Array.Find(failureMechanismSectionViewModels,
                                     vm => vm.SectionConfiguration.Section == selectedFailureMechanismSection.SectionConfiguration.Section)
                          ?? failureMechanismSectionViewModels[0]
                        : failureMechanismSectionViewModels[0];
            }
            else
            {
                selectedFailureMechanismSection = null;
            }
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            selectedFailureMechanismSection = listBox.SelectedItem as MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel;
            UpdateScenarioControls();
            UpdateFailureMechanismSectionConfigurationControl();
        }

        private void UpdateScenarioRows()
        {
            scenarioRows.ForEachElementDo(row => row.Update());
            dataGridViewControl.RefreshDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.IsRelevant),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_In_final_rating
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.Contribution),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_Contribution
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.Name),
                RiskeerCommonFormsResources.ScenarioView_Name_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.FailureProbability),
                RiskeerCommonFormsResources.ScenarioView_ProfileFailureProbability_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.SectionFailureProbability),
                RiskeerCommonFormsResources.ScenarioView_SectionFailureProbability_DisplayName
            );
        }

        private void UpdateScenarioControls()
        {
            UpdateDataGridViewDataSource();
            UpdateTotalScenarioContributionLabel();
        }

        private void UpdateDataGridViewDataSource()
        {
            if (selectedFailureMechanismSection == null)
            {
                scenarioRows = null;
                dataGridViewControl.SetDataSource(null);
                return;
            }

            scenarioRows = GetScenarioRows();
            dataGridViewControl.SetDataSource(scenarioRows);
        }

        private void UpdateTotalScenarioContributionLabel()
        {
            ClearErrorMessage();

            IEnumerable<MacroStabilityInwardsScenarioRow> contributingScenarios = scenarioRows?.Where(r => r.IsRelevant);
            if (contributingScenarios == null || !contributingScenarios.Any())
            {
                labelTotalScenarioContribution.Visible = false;
                return;
            }

            labelTotalScenarioContribution.Visible = true;

            double totalScenarioContribution = contributingScenarios.Sum(r => r.Contribution);
            var roundedTotalScenarioContribution = new RoundedDouble(totalScenarioContributionNrOfDecimals, totalScenarioContribution);
            if (Math.Abs(totalScenarioContribution - 100) >= 1e-6)
            {
                SetErrorMessage(RiskeerCommonFormsResources.CalculationScenarios_Scenario_contribution_for_this_section_not_100);
            }

            labelTotalScenarioContribution.Text = string.Format(RiskeerCommonFormsResources.ScenariosView_Total_contribution_of_relevant_scenarios_for_this_section_is_equal_to_total_scenario_contribution_0_,
                                                                roundedTotalScenarioContribution);
        }

        private void SetErrorMessage(string errorMessage)
        {
            errorProvider.SetError(labelTotalScenarioContribution, errorMessage);
        }

        private void ClearErrorMessage()
        {
            errorProvider.SetError(labelTotalScenarioContribution, string.Empty);
        }

        private IEnumerable<MacroStabilityInwardsScenarioRow> GetScenarioRows()
        {
            FailureMechanismSection section = selectedFailureMechanismSection.SectionConfiguration.Section;
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(section.Points);
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = calculationGroup
                                                                                 .GetCalculations()
                                                                                 .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                 .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            return calculations.Select(pc => new MacroStabilityInwardsScenarioRow(pc, failureMechanism, selectedFailureMechanismSection.SectionConfiguration)).ToList();
        }

        private void UpdateFailureMechanismSectionConfigurationControl()
        {
            failureMechanismSectionConfigurationControl.ClearData();

            if (selectedFailureMechanismSection != null)
            {
                failureMechanismSectionConfigurationControl.SetData(selectedFailureMechanismSection.SectionConfiguration);
            }
        }
    }
}