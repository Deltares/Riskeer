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
using Ringtoets.StabilityStoneCover.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>.
    /// </summary>
    public class StabilityStoneCoverResultView
        : FailureMechanismResultView<StabilityStoneCoverFailureMechanismSectionResult, StabilityStoneCoverFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverResultView"/>.
        /// </summary>
        /// <inheritdoc />
        public StabilityStoneCoverResultView(IObservableEnumerable<StabilityStoneCoverFailureMechanismSectionResult> failureMechanismSectionResults,
                                             StabilityStoneCoverFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            DataGridViewControl.CellFormatting += DisableIrrelevantFieldsFormatting;
            UpdateDataGridViewDataSource();
        }

        protected override object CreateFailureMechanismSectionResultRow(StabilityStoneCoverFailureMechanismSectionResult sectionResult)
        {
            return new StabilityStoneCoverSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= DisableIrrelevantFieldsFormatting;

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>[] simpleAssessmentDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultValidityOnlyType))
                    .OfType<SimpleAssessmentResultValidityOnlyType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(StabilityStoneCoverSectionResultRow.SimpleAssessmentInput),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_ColumnHeader,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.DisplayName));

            EnumDisplayWrapper<AssessmentLayerTwoAResult>[] twoAResultDataSource =
                Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                    .OfType<AssessmentLayerTwoAResult>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(StabilityStoneCoverSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                twoAResultDataSource,
                nameof(EnumDisplayWrapper<AssessmentLayerTwoAResult>.Value),
                nameof(EnumDisplayWrapper<AssessmentLayerTwoAResult>.DisplayName));
            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityStoneCoverSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > 1)
            {
                var simpleAssessmentResult = (SimpleAssessmentResultValidityOnlyType) DataGridViewControl.GetCell(eventArgs.RowIndex,
                                                                                                                  AssessmentLayerOneColumnIndex).Value;
                if (FailureMechanismResultViewHelper.HasPassedSimpleAssessment(simpleAssessmentResult))
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