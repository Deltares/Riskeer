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
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// View for configuring piping calculation scenarios.
    /// </summary>
    public partial class PipingScenariosView : UserControl, IView
    {
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private CalculationGroup calculationGroup;

        private Observer failureMechanismObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, SemiProbabilisticPipingCalculationScenario> calculationObserver;
        private RecursiveObserver<CalculationGroup, SemiProbabilisticPipingInput> calculationInputObserver;

        private IEnumerable<PipingScenarioRow> scenarioRows;

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

            InitializeListBox();
            InitializeDataGridView();

            UpdateSectionsListBox();
            UpdateDataGridViewDataSource();
        }

        public object Data
        {
            get => calculationGroup;
            set => calculationGroup = (CalculationGroup) value;
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
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
            selectConfigurationTypeComboBox.DataSource = enumDisplayWrappers;
            selectConfigurationTypeComboBox.ValueMember = nameof(EnumDisplayWrapper<PipingScenarioConfigurationType>.Value);
            selectConfigurationTypeComboBox.DisplayMember = nameof(EnumDisplayWrapper<PipingScenarioConfigurationType>.DisplayName);
            selectConfigurationTypeComboBox.SelectedValue = failureMechanism.ScenarioConfigurationType;
            selectConfigurationTypeComboBox.EndUpdate();
        }

        private void InitializeObservers()
        {
            failureMechanismObserver = new Observer(UpdateSectionsListBox)
            {
                Observable = failureMechanism
            };

            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };

            calculationObserver = new RecursiveObserver<CalculationGroup, SemiProbabilisticPipingCalculationScenario>(UpdateScenarioRows, pcg => pcg.Children)
            {
                Observable = calculationGroup
            };

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, SemiProbabilisticPipingInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(
                                                                                                                 pcg.Children.OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                                                    .Select(c => c.InputParameters)))
            {
                Observable = calculationGroup
            };
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(
                nameof(PipingScenarioRow.IsRelevant),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_In_final_rating
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.Contribution),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_Contribution
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.Name),
                RiskeerCommonFormsResources.ScenarioView_Name_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.FailureProbabilityUplift),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityUplift
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.FailureProbabilityHeave),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityHeave
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.FailureProbabilitySellmeijer),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilitySellmeijer
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.FailureProbability),
                RiskeerCommonFormsResources.ScenarioView_ProfileFailureProbability_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(PipingScenarioRow.SectionFailureProbability),
                RiskeerCommonFormsResources.ScenarioView_SectionFailureProbability_DisplayName
            );
        }

        private void UpdateDataGridViewDataSource()
        {
            if (!(listBox.SelectedItem is FailureMechanismSection failureMechanismSection))
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            scenarioRows = GetScenarioRows(failureMechanismSection);
            dataGridViewControl.SetDataSource(scenarioRows);
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (failureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(failureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = failureMechanism.Sections.First();
            }
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        private void UpdateScenarioRows()
        {
            scenarioRows.ForEachElementDo(row => row.Update());
            dataGridViewControl.RefreshDataGridView();
        }

        private IEnumerable<PipingScenarioRow> GetScenarioRows(FailureMechanismSection failureMechanismSection)
        {
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<SemiProbabilisticPipingCalculationScenario> pipingCalculations = calculationGroup
                                                                                         .GetCalculations()
                                                                                         .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                         .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            return pipingCalculations.Select(pc => new PipingScenarioRow(pc, failureMechanism, failureMechanismSection, assessmentSection)).ToList();
        }
    }
}