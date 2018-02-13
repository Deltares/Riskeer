﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismResultView
        : FailureMechanismResultView<MacroStabilityInwardsFailureMechanismSectionResult,
            MacroStabilityInwardsFailureMechanism>
    {
        private const int assessmentLayerTwoAIndex = 2;
        private const double tolerance = 1e-6;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly Observer failureMechanismObserver;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section that the failure mechanism belongs to.</param>
        /// <inheritdoc />
        public MacroStabilityInwardsFailureMechanismResultView(
            IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> failureMechanismSectionResults,
            MacroStabilityInwardsFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            failureMechanismObserver = new Observer(UpdateDataGridViewDataSource)
            {
                Observable = failureMechanism
            };

            this.assessmentSection = assessmentSection;

            DataGridViewControl.CellFormatting += ShowAssessmentLayerTwoAErrors;
            DataGridViewControl.CellFormatting += DisableIrrelevantFieldsFormatting;

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
                                                   .Select(c => c.InputParameters)));
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
                                                   .Select(c => c.Output)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateDataGridViewDataSource,
                c => c.Children);

            CalculationGroup observableGroup = failureMechanism.CalculationsGroup;

            calculationInputObserver.Observable = observableGroup;
            calculationOutputObserver.Observable = observableGroup;
            calculationGroupObserver.Observable = observableGroup;

            UpdateDataGridViewDataSource();
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= ShowAssessmentLayerTwoAErrors;
            DataGridViewControl.CellFormatting -= DisableIrrelevantFieldsFormatting;

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();
            failureMechanismObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override object CreateFailureMechanismSectionResultRow(MacroStabilityInwardsFailureMechanismSectionResult sectionResult)
        {
            return new MacroStabilityInwardsFailureMechanismSectionResultRow(sectionResult,
                                                                             FailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>(),
                                                                             FailureMechanism,
                                                                             assessmentSection);
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            EnumDisplayWrapper<SimpleAssessmentResultType>[] simpleAssessmentDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultType))
                    .OfType<SimpleAssessmentResultType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultType>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.SimpleAssessmentInput),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                true);
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }

        /// <summary>
        /// Finds out whether the assessment section which is represented by the row at index 
        /// <paramref name="rowIndex"/> has passed the simple assessment.
        /// </summary>
        /// <param name="rowIndex">The index of the row which has a section attached.</param>
        /// <returns><c>false</c> if the simple assessment has passed, <c>true</c> otherwise.</returns>
        private bool HasPassedSimpleAssessment(int rowIndex)
        {
            var simpleAssessmentType = (SimpleAssessmentResultType) DataGridViewControl.GetCell(rowIndex, AssessmentLayerOneColumnIndex).Value;
            return simpleAssessmentType == SimpleAssessmentResultType.ProbabilityNegligible
                   || simpleAssessmentType == SimpleAssessmentResultType.NotApplicable;
        }

        #region Event handling

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > AssessmentLayerOneColumnIndex)
            {
                if (HasPassedSimpleAssessment(eventArgs.RowIndex))
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

            var resultRow = (MacroStabilityInwardsFailureMechanismSectionResultRow) GetDataAtRow(e.RowIndex);
            MacroStabilityInwardsFailureMechanismSectionResult rowObject = resultRow.GetSectionResult;
            if (rowObject.SimpleAssessmentInput == SimpleAssessmentResultType.ProbabilityNegligible ||
                rowObject.SimpleAssessmentInput == SimpleAssessmentResultType.NotApplicable)
            {
                currentDataGridViewCell.ErrorText = string.Empty;
                return;
            }

            MacroStabilityInwardsCalculationScenario[] relevantScenarios = rowObject.GetCalculationScenarios(FailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()).ToArray();
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

            if (double.IsNaN(resultRow.AssessmentLayerTwoA))
            {
                currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_All_calculations_must_have_valid_output;
                return;
            }

            currentDataGridViewCell.ErrorText = string.Empty;
        }

        #endregion
    }
}