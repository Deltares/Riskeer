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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using CoreCommonControlsResources = Core.Common.Controls.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view for configuring piping calculations.
    /// </summary>
    public partial class PipingScenariosView : UserControl, IView
    {
        private readonly RecursiveObserver<CalculationGroup, PipingInput> pipingInputObserver;
        private readonly RecursiveObserver<CalculationGroup, CalculationGroup> pipingCalculationGroupObserver;
        private readonly RecursiveObserver<CalculationGroup, PipingCalculationScenario> pipingCalculationObserver;
        private readonly Observer pipingFailureMechanismObserver;
        private CalculationGroup calculationGroup;
        private PipingFailureMechanism pipingFailureMechanism;

        /// <summary>
        /// Creates a new instance of the <see cref="PipingScenariosView"/> class.
        /// </summary>
        public PipingScenariosView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();

            pipingFailureMechanismObserver = new Observer(OnPipingFailureMechanismUpdate);

            // The concat is needed to observe the input of calculations in child groups.
            pipingInputObserver = new RecursiveObserver<CalculationGroup, PipingInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(pcg.Children.OfType<PipingCalculationScenario>().Select(pc => pc.InputParameters)));
            pipingCalculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children);
            pipingCalculationObserver = new RecursiveObserver<CalculationGroup, PipingCalculationScenario>(dataGridViewControl.RefreshDataGridView, pcg => pcg.Children);
        }

        /// <summary>
        /// Gets or sets the piping failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism
        {
            get
            {
                return pipingFailureMechanism;
            }
            set
            {
                pipingFailureMechanism = value;
                pipingFailureMechanismObserver.Observable = pipingFailureMechanism;

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
                    pipingInputObserver.Observable = calculationGroup;
                    pipingCalculationObserver.Observable = calculationGroup;
                    pipingCalculationGroupObserver.Observable = calculationGroup;
                }
                else
                {
                    dataGridViewControl.SetDataSource(null);
                    pipingInputObserver.Observable = null;
                    pipingCalculationObserver.Observable = null;
                    pipingCalculationGroupObserver.Observable = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            pipingFailureMechanismObserver.Dispose();
            pipingInputObserver.Dispose();
            pipingCalculationObserver.Dispose();
            pipingCalculationGroupObserver.Dispose();

            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.IsRelevant),
                Resources.PipingCalculationsView_InitializeDataGridView_In_final_rating
            );
            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.Contribution),
                Resources.PipingCalculationsView_InitializeDataGridView_Contribution
            );
            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.Name),
                Resources.PipingCalculation_Name_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.FailureProbabilityPiping),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityPiping
            );
            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.FailureProbabilityUplift),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityUplift
            );
            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.FailureProbabilityHeave),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilityHeave
            );
            dataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingScenarioRow>(row => row.FailureProbabilitySellmeijer),
                Resources.PipingScenarioView_PipingScenarioRow_FailureProbabilitySellmeijer
            );
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = TypeUtils.GetMemberName<FailureMechanismSection>(s => s.Name);
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

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertLinePointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<PipingCalculationScenario> pipingCalculations = calculationGroup
                .GetCalculations()
                .OfType<PipingCalculationScenario>()
                .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            List<PipingScenarioRow> dataSource = pipingCalculations.Select(pc => new PipingScenarioRow(pc)).ToList();
            dataGridViewControl.SetDataSource(dataSource);
        }

        #region Event handling

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        private void OnPipingFailureMechanismUpdate()
        {
            UpdateSectionsListBox();
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (pipingFailureMechanism != null && pipingFailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(pipingFailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = pipingFailureMechanism.Sections.First();
            }
        }

        #endregion
    }
}