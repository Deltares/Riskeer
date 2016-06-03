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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="HeightStructuresFailureMechanismSectionResult"/>.
    /// </summary>
    public class HeightStructuresFailureMechanismResultView : FailureMechanismResultView<HeightStructuresFailureMechanismSectionResult>
    {
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        private readonly int assessmentLayerTwoAIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresFailureMechanismResultView"/>.
        /// </summary>
        public HeightStructuresFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdataDataGridViewDataSource, 
                cg => cg.Children.Concat<object>(
                    cg.Children
                        .OfType<ICalculation>()
                        .Select(c => c.GetObservableInput())
                )
            );
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdataDataGridViewDataSource, 
                cg => cg.Children.Concat<object>(
                    cg.Children
                        .OfType<ICalculation>()
                        .Select(c => c.GetObservableOutput())
                )
            );
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdataDataGridViewDataSource, 
                c => c.Children
            );

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
            DataGridViewControl.RemoveCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<HeightStructuresFailureMechanismSectionResultRow>(sr => sr.Name),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true
                );
            DataGridViewControl.AddCheckBoxColumn(
                TypeUtils.GetMemberName<HeightStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerOne),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<HeightStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a, 
                true
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<HeightStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three
                );
        }

        protected override object CreateFailureMechanismSectionResultRow(HeightStructuresFailureMechanismSectionResult sectionResult)
        {
            return new HeightStructuresFailureMechanismSectionResultRow(sectionResult);
        }

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
    }
}