// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="ClosingStructuresFailureMechanismSectionResult"/> for closing structures.
    /// </summary>
    public class ClosingStructuresFailureMechanismResultView : FailureMechanismResultView<ClosingStructuresFailureMechanismSectionResult,
        ClosingStructuresFailureMechanismSectionResultRow, ClosingStructuresFailureMechanism>
    {
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int detailedAssessmentProbabilityIndex = 3;
        private const int tailorMadeAssessmentResultIndex = 4;
        private const int tailorMadeAssessmentProbabilityIndex = 5;
        private const int simpleAssemblyCategoryGroupIndex = 6;
        private const int detailedAssemblyCategoryGroupIndex = 7;
        private const int tailorMadeAssemblyCategoryGroupIndex = 8;
        private const int combinedAssemblyCategoryGroupIndex = 9;
        private const int combinedAssemblyProbabilityIndex = 10;
        private const int manualAssemblyProbabilityIndex = 12;

        private readonly IAssessmentSection assessmentSection;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        public ClosingStructuresFailureMechanismResultView(
            IObservableEnumerable<ClosingStructuresFailureMechanismSectionResult> failureMechanismSectionResults,
            ClosingStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;

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

            CalculationGroup observableGroup = failureMechanism.CalculationsGroup;
            calculationInputObserver.Observable = observableGroup;
            calculationOutputObserver.Observable = observableGroup;
            calculationGroupObserver.Observable = observableGroup;
        }

        protected override ClosingStructuresFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(ClosingStructuresFailureMechanismSectionResult sectionResult)
        {
            return new ClosingStructuresFailureMechanismSectionResultRow(
                sectionResult,
                FailureMechanism,
                assessmentSection,
                new ClosingStructuresFailureMechanismSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = simpleAssessmentResultIndex,
                    DetailedAssessmentResultIndex = detailedAssessmentResultIndex,
                    DetailedAssessmentProbabilityIndex = detailedAssessmentProbabilityIndex,
                    TailorMadeAssessmentResultIndex = tailorMadeAssessmentResultIndex,
                    TailorMadeAssessmentProbabilityIndex = tailorMadeAssessmentProbabilityIndex,
                    SimpleAssemblyCategoryGroupIndex = simpleAssemblyCategoryGroupIndex,
                    DetailedAssemblyCategoryGroupIndex = detailedAssemblyCategoryGroupIndex,
                    TailorMadeAssemblyCategoryGroupIndex = tailorMadeAssemblyCategoryGroupIndex,
                    CombinedAssemblyCategoryGroupIndex = combinedAssemblyCategoryGroupIndex,
                    CombinedAssemblyProbabilityIndex = combinedAssemblyProbabilityIndex,
                    ManualAssemblyProbabilityIndex = manualAssemblyProbabilityIndex
                });
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= ShowAssessmentLayerErrors;
            DataGridViewControl.CellFormatting -= OnCellFormatting;

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                nameof(ClosingStructuresFailureMechanismSectionResultRow.Name),
                RingtoetsCommonFormsResources.Section_DisplayName,
                true);

            EnumDisplayWrapper<SimpleAssessmentResultType>[] simpleAssessmentResultDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultType))
                    .OfType<SimpleAssessmentResultType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultType>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(ClosingStructuresFailureMechanismSectionResultRow.SimpleAssessmentResult),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_DisplayName,
                simpleAssessmentResultDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(ClosingStructuresFailureMechanismSectionResultRow.DetailedAssessmentProbability),
                RingtoetsCommonFormsResources.FailureMechanismResultView_DetailedAssessmentResult_DisplayName);
            DataGridViewControl.AddTextBoxColumn(
                nameof(ClosingStructuresFailureMechanismSectionResultRow.TailorMadeAssessmentProbability),
                RingtoetsCommonFormsResources.FailureMechanismResultView_TailorMadeAssessmentResult_DisplayName);
        }

        protected override void BindEvents()
        {
            base.BindEvents();

            DataGridViewControl.CellFormatting += ShowAssessmentLayerErrors;
            DataGridViewControl.CellFormatting += OnCellFormatting;
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > SimpleAssessmentColumnIndex)
            {
                SimpleAssessmentResultType simpleAssessmentResult = GetDataAtRow(eventArgs.RowIndex).SimpleAssessmentResult;
                if (FailureMechanismSectionResultRowHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult))
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
            if (e.ColumnIndex != detailedAssessmentResultIndex)
            {
                return;
            }

            ClosingStructuresFailureMechanismSectionResultRow resultRow = GetDataAtRow(e.RowIndex);
            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);
            StructuresCalculation<ClosingStructuresInput> normativeCalculation = resultRow.GetSectionResultCalculation();

            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(currentDataGridViewCell,
                                                                              resultRow.SimpleAssessmentResult,
                                                                              resultRow.DetailedAssessmentProbability,
                                                                              normativeCalculation);
        }
    }
}