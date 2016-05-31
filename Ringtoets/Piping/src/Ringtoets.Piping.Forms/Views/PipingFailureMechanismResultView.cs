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
        private const double tolerance = 1e-6;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        private readonly int assessmentLayerTwoAIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultView"/>.
        /// </summary>
        public PipingFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(ShowAssementLayerTwoAErrors);
            DataGridViewControl.AddCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(UpdataDataGridViewDataSource, cg => cg.Children.Concat<object>(cg.Children.OfType<ICalculationScenario>().Select(c => c.GetObservableInput())));
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(UpdataDataGridViewDataSource, cg => cg.Children.Concat<object>(cg.Children.OfType<ICalculationScenario>().Select(c => c.GetObservableOutput())));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(UpdataDataGridViewDataSource, c => c.Children);

            AddDataGridColumns();
        }

        public override IFailureMechanism FailureMechanism
        {
            set
            {
                base.FailureMechanism = value;

                var calculatableFailureMechanism = value as ICalculatableFailureMechanism;
                CalculationGroup observableGroup = calculatableFailureMechanism != null ? calculatableFailureMechanism.CalculationsGroup : null;

                calculationInputObserver.Observable = observableGroup;
                calculationOutputObserver.Observable = observableGroup;
                calculationGroupObserver.Observable = observableGroup;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.RemoveCellFormattingHandler(ShowAssementLayerTwoAErrors);
            DataGridViewControl.RemoveCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingFailureMechanismSectionResultRow>(sr => sr.Name),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true
                );
            DataGridViewControl.AddCheckBoxColumn(
                TypeUtils.GetMemberName<PipingFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerOne),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                true
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<PipingFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three
                );
        }

        protected override object CreateFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult)
        {
            return new PipingFailureMechanismSectionResultRow(sectionResult);
        }

        #region Event handling

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > 1)
            {
                if (HasPassedLevelOne(eventArgs.RowIndex))
                {
                    DataGridViewControl.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex, eventArgs.ColumnIndex == assessmentLayerTwoAIndex);
                }
            }
        }

        private void ShowAssementLayerTwoAErrors(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 0)
            {
                return;
            }
            
            var currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);
            
            PipingFailureMechanismSectionResultRow resultRow = (PipingFailureMechanismSectionResultRow) GetDataAtRow(e.RowIndex);

            if (resultRow != null && e.ColumnIndex == assessmentLayerTwoAIndex)
            {
                PipingFailureMechanismSectionResult rowObject = resultRow.SectionResult;

                var relevantScenarios = rowObject.CalculationScenarios.Where(cs => cs.IsRelevant).ToArray();
                bool relevantScenarioAvailable = relevantScenarios.Length != 0;

                if (rowObject.AssessmentLayerOne || !relevantScenarioAvailable)
                {
                    currentDataGridViewCell.ErrorText = string.Empty;
                    return;
                }

                if (Math.Abs(rowObject.TotalContribution - 1.0) > tolerance)
                {
                    currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_Scenario_contribution_for_this_section_not_100;
                    return;
                }

                var calculationScenarioStatus = rowObject.CalculationScenarioStatus;

                if (calculationScenarioStatus == CalculationScenarioStatus.NotCalculated)
                {
                    currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_Not_all_calculations_are_executed;
                    return;
                }

                if (calculationScenarioStatus == CalculationScenarioStatus.Failed)
                {
                    currentDataGridViewCell.ErrorText = RingtoetsCommonFormsResources.FailureMechanismResultView_DataGridViewCellFormatting_Not_all_calculations_have_valid_output;
                    return;
                }

                currentDataGridViewCell.ErrorText = string.Empty;
            }
        }

        #endregion
    }
}