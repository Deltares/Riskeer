// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring macro stability inwards calculations.
    /// </summary>
    public partial class MacroStabilityInwardsScenariosView : UserControl, IView
    {
        private readonly IAssessmentSection assessmentSection;
        private readonly RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput> inputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario> calculationObserver;
        private readonly Observer failureMechanismObserver;
        private CalculationGroup calculationGroup;
        private MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism;
        private List<MacroStabilityInwardsScenarioRow> scenarioRows;

        /// <summary>
        /// Creates a new instance of the <see cref="MacroStabilityInwardsScenariosView"/> class.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the scenarios belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsScenariosView(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            this.assessmentSection = assessmentSection;

            failureMechanismObserver = new Observer(OnFailureMechanismUpdate);

            // The concat is needed to observe the input of calculations in child groups.
            inputObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsInput>(UpdateDataGridViewDataSource, cg => cg.Children
                                                                                                                                      .Concat<object>(cg.Children
                                                                                                                                                        .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                                                                                        .Select(pc => pc.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, cg => cg.Children);
            calculationObserver = new RecursiveObserver<CalculationGroup, MacroStabilityInwardsCalculationScenario>(UpdateScenarioRows, cg => cg.Children);
        }

        /// <summary>
        /// Gets or sets the macro stability inwards failure mechanism.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism MacroStabilityInwardsFailureMechanism
        {
            get
            {
                return macroStabilityInwardsFailureMechanism;
            }
            set
            {
                macroStabilityInwardsFailureMechanism = value;
                failureMechanismObserver.Observable = macroStabilityInwardsFailureMechanism;

                UpdateSectionsListBox();
            }
        }

        public object Data
        {
            get
            {
                return calculationGroup;
            }
            set
            {
                calculationGroup = value as CalculationGroup;

                if (calculationGroup != null)
                {
                    UpdateDataGridViewDataSource();
                    inputObserver.Observable = calculationGroup;
                    calculationObserver.Observable = calculationGroup;
                    calculationGroupObserver.Observable = calculationGroup;
                }
                else
                {
                    dataGridViewControl.SetDataSource(null);
                    inputObserver.Observable = null;
                    calculationObserver.Observable = null;
                    calculationGroupObserver.Observable = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            inputObserver.Dispose();
            calculationObserver.Dispose();
            calculationGroupObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.IsRelevant),
                Resources.MacroStabilityInwardsCalculationsView_InitializeDataGridView_In_final_rating
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.Contribution),
                Resources.MacroStabilityInwardsCalculationsView_InitializeDataGridView_Contribution
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.Name),
                Resources.MacroStabilityInwardsCalculation_Name_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.FailureProbabilityMacroStabilityInwards),
                Resources.MacroStabilityInwardsScenarioView_MacroStabilityInwardsScenarioRow_MacroStabilityInwardsProbability
            );
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateDataGridViewDataSource()
        {
            var failureMechanismSection = listBox.SelectedItem as FailureMechanismSection;
            if (failureMechanismSection == null || calculationGroup == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = calculationGroup
                                                                                 .GetCalculations()
                                                                                 .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                 .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            scenarioRows = calculations.Select(pc => new MacroStabilityInwardsScenarioRow(
                                                   pc,
                                                   MacroStabilityInwardsFailureMechanism,
                                                   assessmentSection)).ToList();
            dataGridViewControl.SetDataSource(scenarioRows);
        }

        private void UpdateScenarioRows()
        {
            scenarioRows.ForEachElementDo(row => row.Update());
            dataGridViewControl.RefreshDataGridView();
        }

        #region Event handling

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        private void OnFailureMechanismUpdate()
        {
            UpdateSectionsListBox();
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (macroStabilityInwardsFailureMechanism != null && macroStabilityInwardsFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(macroStabilityInwardsFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = macroStabilityInwardsFailureMechanism.Sections.First();
            }
        }

        #endregion
    }
}