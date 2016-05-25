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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class defines a view where <see cref="SimpleFailureMechanismSectionResult"/> are displayed in a grid
    /// and can be modified.
    /// </summary>
    public class SimpleFailureMechanismResultView : FailureMechanismResultView<SimpleFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SimpleFailureMechanismResultView"/>
        /// </summary>
        public SimpleFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(OnCellFormatting);
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
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

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            var dataSource = Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                                           .OfType<AssessmentLayerTwoAResult>()
                                           .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                                           .ToList();

            DataGridViewControl.AddComboBoxColumn(
                TypeUtils.GetMemberName<NumericFailureMechanismSectionResult>(sr => sr.AssessmentLayerTwoA), 
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                dataSource,
                edw => edw.Value,
                edw => edw.DisplayName
            );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<NumericFailureMechanismSectionResult>(sr => sr.AssessmentLayerTwoB), 
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_b
            );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<NumericFailureMechanismSectionResult>(sr => sr.AssessmentLayerThree), 
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three
            );
        }

        protected override object CreateFailureMechanismSectionResultRow(SimpleFailureMechanismSectionResult sectionResult)
        {
            return new SimpleFailureMechanismSectionResultRow(sectionResult);
        }
    }
}