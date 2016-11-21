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

using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.WaveImpactAsphaltCover.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverFailureMechanismResultView : FailureMechanismResultView<WaveImpactAsphaltCoverFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverFailureMechanismResultView"/>.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(OnCellFormatting);
        }

        protected override object CreateFailureMechanismSectionResultRow(WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult)
        {
            return new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(sectionResult);
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.RemoveCellFormattingHandler(OnCellFormatting);

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<WaveImpactAsphaltCoverFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a);
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<WaveImpactAsphaltCoverFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerThree),
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
    }
}