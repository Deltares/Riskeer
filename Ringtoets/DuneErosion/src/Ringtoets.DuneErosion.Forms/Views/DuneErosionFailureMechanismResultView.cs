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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="DuneErosionFailureMechanismSectionResult"/>.
    /// </summary>
    public class DuneErosionFailureMechanismResultView : FailureMechanismResultView
        <DuneErosionFailureMechanismSectionResult, DuneErosionSectionResultRow, DuneErosionFailureMechanism>
    {
        /// <inheritdoc/>
        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionFailureMechanismResultView"/>.
        /// </summary>
        public DuneErosionFailureMechanismResultView(
            IObservableEnumerable<DuneErosionFailureMechanismSectionResult> failureMechanismSectionResults,
            DuneErosionFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            DataGridViewControl.CellFormatting += DisableIrrelevantFieldsFormatting;
            UpdateDataGridViewDataSource();
        }

        protected override DuneErosionSectionResultRow CreateFailureMechanismSectionResultRow(DuneErosionFailureMechanismSectionResult sectionResult)
        {
            return new DuneErosionSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= DisableIrrelevantFieldsFormatting;

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                nameof(DuneErosionSectionResultRow.Name),
                RingtoetsCommonFormsResources.Section_DisplayName,
                true);

            EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>[] simpleAssessmentDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultValidityOnlyType))
                    .OfType<SimpleAssessmentResultValidityOnlyType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(DuneErosionSectionResultRow.SimpleAssessmentResult),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_DisplayName,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.DisplayName));

            EnumDisplayWrapper<AssessmentLayerTwoAResult>[] twoAResultDataSource =
                Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                    .OfType<AssessmentLayerTwoAResult>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(DuneErosionSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_DetailedAssessment_DisplayName,
                twoAResultDataSource,
                nameof(EnumDisplayWrapper<AssessmentLayerTwoAResult>.Value),
                nameof(EnumDisplayWrapper<AssessmentLayerTwoAResult>.DisplayName));
            DataGridViewControl.AddTextBoxColumn(
                nameof(DuneErosionSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_TailorMadeAssessment_DisplayName);
        }

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > SimpleAssessmentColumnIndex)
            {
                SimpleAssessmentResultValidityOnlyType simpleAssessmentResult =
                    ((DuneErosionSectionResultRow) GetDataAtRow(eventArgs.RowIndex)).SimpleAssessmentResult;
                if (FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult))
                {
                    DataGridViewControl.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
        }
    }
}