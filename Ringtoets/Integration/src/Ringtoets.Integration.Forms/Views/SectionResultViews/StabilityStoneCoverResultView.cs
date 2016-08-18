﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>.
    /// </summary>
    public class StabilityStoneCoverResultView : FailureMechanismResultView<StabilityStoneCoverFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverResultView"/>.
        /// </summary>
        public StabilityStoneCoverResultView()
        {
            AddDataGridColumns();
        }

        protected override object CreateFailureMechanismSectionResultRow(StabilityStoneCoverFailureMechanismSectionResult sectionResult)
        {
            return new StabilityStoneCoverSectionResultRow(sectionResult);
        }

        private void AddDataGridColumns()
        {
            EnumDisplayWrapper<AssessmentLayerTwoAResult>[] twoAResultDataSource =
                Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                    .OfType<AssessmentLayerTwoAResult>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                    .ToArray();

            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StabilityStoneCoverSectionResultRow>(sr => sr.Name),
                Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true);
            DataGridViewControl.AddComboBoxColumn(
                TypeUtils.GetMemberName<StabilityStoneCoverSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                twoAResultDataSource,
                TypeUtils.GetMemberName<EnumDisplayWrapper<AssessmentLayerTwoAResult>>(edw => edw.Value),
                TypeUtils.GetMemberName<EnumDisplayWrapper<AssessmentLayerTwoAResult>>(edw => edw.DisplayName));
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StabilityStoneCoverSectionResultRow>(sr => sr.AssessmentLayerThree),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }
    }
}