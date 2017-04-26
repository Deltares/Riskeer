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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="ClosingStructuresFailureMechanismSectionResult"/>.
    /// </summary>
    public class ClosingStructuresFailureMechanismResultView : FailureMechanismResultView<ClosingStructuresFailureMechanismSectionResult>
    {
        private const int assessmentLayerTwoAIndex = 2;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismResultView"/>.
        /// </summary>
        public ClosingStructuresFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(ShowAssessmentLayerErrors);
            DataGridViewControl.AddCellFormattingHandler(OnCellFormatting);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<StructuresCalculation<ClosingStructuresInput>>()
                                                   .Select(c => c.InputParameters)));
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<StructuresCalculation<ClosingStructuresInput>>()
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

        protected override object CreateFailureMechanismSectionResultRow(ClosingStructuresFailureMechanismSectionResult sectionResult)
        {
            return new ClosingStructuresFailureMechanismSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.RemoveCellFormattingHandler(ShowAssessmentLayerErrors);
            DataGridViewControl.RemoveCellFormattingHandler(OnCellFormatting);

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<ClosingStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a);
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<ClosingStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
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

        private void ShowAssessmentLayerErrors(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != assessmentLayerTwoAIndex)
            {
                return;
            }

            var resultRow = (ClosingStructuresFailureMechanismSectionResultRow) GetDataAtRow(e.RowIndex);
            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);
            StructuresCalculation<ClosingStructuresInput> normativeCalculation = resultRow.GetSectionResultCalculation();

            FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(currentDataGridViewCell,
                                                                               resultRow.AssessmentLayerOne,
                                                                               resultRow.AssessmentLayerTwoA,
                                                                               normativeCalculation);
        }
    }
}