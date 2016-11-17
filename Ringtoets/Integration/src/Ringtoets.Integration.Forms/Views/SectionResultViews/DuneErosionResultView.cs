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

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="DuneErosionFailureMechanismSectionResult"/>.
    /// </summary>
    public class DuneErosionResultView : FailureMechanismResultView<DuneErosionFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionResultView"/>.
        /// </summary>
        public DuneErosionResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(DisableIrrelevantFieldsFormatting);
        }

        protected override object CreateFailureMechanismSectionResultRow(DuneErosionFailureMechanismSectionResult sectionResult)
        {
            return new DuneErosionSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.RemoveCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            EnumDisplayWrapper<AssessmentLayerTwoAResult>[] twoAResultDataSource =
                Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                    .OfType<AssessmentLayerTwoAResult>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                TypeUtils.GetMemberName<DuneErosionSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                twoAResultDataSource,
                TypeUtils.GetMemberName<EnumDisplayWrapper<AssessmentLayerTwoAResult>>(edw => edw.Value),
                TypeUtils.GetMemberName<EnumDisplayWrapper<AssessmentLayerTwoAResult>>(edw => edw.DisplayName));
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<DuneErosionSectionResultRow>(sr => sr.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
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
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
        }
    }
}