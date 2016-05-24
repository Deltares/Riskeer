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

using System.Collections.Generic;
using System.Windows.Forms;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class defines a view where <see cref="SimpleFailureMechanismSectionResult"/> are displayed in a grid
    /// and can be modified.
    /// </summary>
    public class CustomProbabilityFailureMechanismResultView : FailureMechanismResultView<CustomProbabilityFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="CustomFailureMechanismResultView"/>
        /// </summary>
        public CustomProbabilityFailureMechanismResultView()
        {
            AddCellFormattingHandler(OnCellFormatting);
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > 1)
            {
                if (HasPassedLevelZero(eventArgs.RowIndex))
                {
                    DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            } 
        }

        protected override IEnumerable<DataGridViewColumn> GetDataGridColumns()
        {
            foreach (var baseColumn in base.GetDataGridColumns())
            {
                yield return baseColumn;
            }

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoA",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                Name = "column_AssessmentLayerTwoA"
            };

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoB",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_b,
                Name = "column_AssessmentLayerTwoB"
            };

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerThree",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three,
                Name = "column_AssessmentLayerThree"
            };
        }

        protected override object CreateFailureMechanismSectionResultRow(CustomProbabilityFailureMechanismSectionResult sectionResult)
        {
            return new CustomProbabilityFailureMechanismSectionResultRow(sectionResult);
        }
    }
}