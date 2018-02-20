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
using Ringtoets.GrassCoverErosionOutwards.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismResultView
        : FailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanismSectionResult, GrassCoverErosionOutwardsFailureMechanism>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismResultView"/>.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanismResultView(
            IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> failureMechanismSectionResults,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            DataGridViewControl.CellFormatting += OnCellFormatting;
            UpdateDataGridViewDataSource();
        }

        protected override object CreateFailureMechanismSectionResultRow(GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult)
        {
            return new GrassCoverErosionOutwardsFailureMechanismSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= OnCellFormatting;

            base.Dispose(disposing);
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
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRow.SimpleAssessmentResult),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_ColumnHeader,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));

            EnumDisplayWrapper<AssessmentLayerTwoAResult>[] twoAResultDataSource =
                Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                    .OfType<AssessmentLayerTwoAResult>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_DetailedAssessment_ColumnHeader,
                twoAResultDataSource,
                nameof(EnumDisplayWrapper<AssessmentLayerTwoAResult>.Value),
                nameof(EnumDisplayWrapper<AssessmentLayerTwoAResult>.DisplayName));
            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_TailorMadeAssessment_ColumnHeader);
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > SimpleAssessmentColumnIndex)
            {
                var simpleAssessmentResult = (SimpleAssessmentResultType) DataGridViewControl.GetCell(eventArgs.RowIndex,
                                                                                                      SimpleAssessmentColumnIndex).Value;
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