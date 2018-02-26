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
    /// The view for a collection of <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityOutwardsResultView : FailureMechanismResultView<MacroStabilityOutwardsFailureMechanismSectionResult,
        MacroStabilityOutwardsSectionResultRow, MacroStabilityOutwardsFailureMechanism>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsResultView"/>.
        /// </summary>
        public MacroStabilityOutwardsResultView(IObservableEnumerable<MacroStabilityOutwardsFailureMechanismSectionResult> failureMechanismSectionResults,
                                                MacroStabilityOutwardsFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override MacroStabilityOutwardsSectionResultRow CreateFailureMechanismSectionResultRow(MacroStabilityOutwardsFailureMechanismSectionResult sectionResult)
        {
            return new MacroStabilityOutwardsSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= OnCellFormatting;

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityOutwardsSectionResultRow.Name),
                RingtoetsCommonFormsResources.Section_DisplayName,
                true);

            EnumDisplayWrapper<SimpleAssessmentResultType>[] simpleAssessmentDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultType))
                    .OfType<SimpleAssessmentResultType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultType>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(MacroStabilityOutwardsSectionResultRow.SimpleAssessmentResult),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_DisplayName,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityOutwardsSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_DetailedAssessment_DisplayName);
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityOutwardsSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_TailorMadeAssessment_DisplayName);
        }

        protected override void BindEvents()
        {
            base.BindEvents();

            DataGridViewControl.CellFormatting += OnCellFormatting;
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > SimpleAssessmentColumnIndex)
            {
                SimpleAssessmentResultType simpleAssessmentResult = GetDataAtRow(eventArgs.RowIndex).SimpleAssessmentResult;
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