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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Data;
using CoreCommonResources = Core.Common.Base.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public class PipingFailureMechanismResultView : FailureMechanismResultView<PipingFailureMechanismSectionResult>
    {
        private const int assessmentLayerTwoAIndex = 2;
        private const double tolerance = 1e-6;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultView"/>.
        /// </summary>
        public PipingFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(ShowAssessmentLayerTwoAErrors);
            DataGridViewControl.AddCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<PipingCalculationScenario>()
                                                   .Select(c => c.InputParameters)));
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<PipingCalculationScenario>()
                                                   .Select(c => c.Output)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateDataGridViewDataSource,
                c => c.Children);
        }

        public override IFailureMechanism FailureMechanism
        {
            set
            {
                base.FailureMechanism = value;

                var calculatableFailureMechanism = value as ICalculatableFailureMechanism;
                CalculationGroup observableGroup = calculatableFailureMechanism?.CalculationsGroup;

                calculationInputObserver.Observable = observableGroup;
                calculationOutputObserver.Observable = observableGroup;
                calculationGroupObserver.Observable = observableGroup;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.RemoveCellFormattingHandler(ShowAssessmentLayerTwoAErrors);
            DataGridViewControl.RemoveCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override object CreateFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult)
        {
            if (FailureMechanism == null)
            {
                return null;
            }
            return new PipingFailureMechanismSectionResultRow(sectionResult, FailureMechanism.Calculations.OfType<PipingCalculationScenario>());
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingFailureMechanismSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                true);
            DataGridViewControl.AddTextBoxColumn(
                nameof(PipingFailureMechanismSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }

        #region Event handling

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > AssessmentLayerOneColumnIndex)
            {
                if (HasPassedLevelOne(eventArgs.RowIndex))
                {
                    DataGridViewControl.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
        }

        private void ShowAssessmentLayerTwoAErrors(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != assessmentLayerTwoAIndex)
            {
                return;
            }

            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);

            var resultRow = (PipingFailureMechanismSectionResultRow) GetDataAtRow(e.RowIndex);
            PipingFailureMechanismSectionResult rowObject = resultRow.GetSectionResult;
            if (rowObject.AssessmentLayerOne == AssessmentLayerOneState.Sufficient)
            {
                currentDataGridViewCell.ErrorText = string.Empty;
                return;
            }

            PipingCalculationScenario[] relevantScenarios = rowObject.GetCalculationScenarios(FailureMechanism.Calculations.OfType<PipingCalculationScenario>()).ToArray();
            bool relevantScenarioAvailable = relevantScenarios.Length != 0;

            if (!relevantScenarioAvailable)
            {
                currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_Not_any_calculation_set;
                return;
            }

            if (Math.Abs(rowObject.GetTotalContribution(relevantScenarios) - 1.0) > tolerance)
            {
                currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_Scenario_contribution_for_this_section_not_100;
                return;
            }

            CalculationScenarioStatus calculationScenarioStatus = rowObject.GetCalculationScenarioStatus(relevantScenarios);
            if (calculationScenarioStatus == CalculationScenarioStatus.NotCalculated)
            {
                currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_Not_all_calculations_have_been_executed;
                return;
            }
            if (calculationScenarioStatus == CalculationScenarioStatus.Failed)
            {
                currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_All_calculations_must_have_valid_output;
                return;
            }
            currentDataGridViewCell.ErrorText = string.Empty;
        }

        #endregion
    }
}