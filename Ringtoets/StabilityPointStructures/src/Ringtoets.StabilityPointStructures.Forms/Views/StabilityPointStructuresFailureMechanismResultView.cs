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

using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="StabilityPointStructuresFailureMechanismSectionResult"/>.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismResultView : FailureMechanismResultView<StabilityPointStructuresFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresFailureMechanismResultView"/>.
        /// </summary>
        public StabilityPointStructuresFailureMechanismResultView()
        {
            AddDataGridColumns();
        }

        protected override object CreateFailureMechanismSectionResultRow(StabilityPointStructuresFailureMechanismSectionResult sectionResult)
        {
            return new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult);
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StabilityPointStructuresFailureMechanismSectionResultRow>(sr => sr.Name),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true);
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StabilityPointStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a);
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StabilityPointStructuresFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }
    }
}