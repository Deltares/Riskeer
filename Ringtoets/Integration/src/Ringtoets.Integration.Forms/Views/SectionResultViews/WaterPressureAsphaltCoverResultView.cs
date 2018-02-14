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
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="WaterPressureAsphaltCoverFailureMechanismSectionResult"/>.
    /// </summary>
    public class WaterPressureAsphaltCoverResultView
        : FailureMechanismResultView<WaterPressureAsphaltCoverFailureMechanismSectionResult,
            WaterPressureAsphaltCoverFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaterPressureAsphaltCoverResultView"/>.
        /// </summary>
        /// <inheritdoc />
        public WaterPressureAsphaltCoverResultView(
            IObservableEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> failureMechanismSectionResults,
            WaterPressureAsphaltCoverFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            DataGridViewControl.CellFormatting += OnCellFormatting;
            UpdateDataGridViewDataSource();
        }

        protected override object CreateFailureMechanismSectionResultRow(WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult)
        {
            return new WaterPressureAsphaltCoverSectionResultRow(sectionResult);
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
                nameof(WaterPressureAsphaltCoverSectionResultRow.SimpleAssessmentInput),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_ColumnHeader,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(WaterPressureAsphaltCoverSectionResultRow.AssessmentLayerThree),
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