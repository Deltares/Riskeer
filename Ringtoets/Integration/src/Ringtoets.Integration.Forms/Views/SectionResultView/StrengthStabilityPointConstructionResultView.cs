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

using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class StrengthStabilityPointConstructionResultView : FailureMechanismResultView<StrengthStabilityPointConstructionFailureMechanismSectionResult>
    {
        public StrengthStabilityPointConstructionResultView()
        {
            AddDataGridColumns();
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.Name),
                Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.AssessmentLayerThree),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three
                );
        }

        protected override object CreateFailureMechanismSectionResultRow(StrengthStabilityPointConstructionFailureMechanismSectionResult sectionResult)
        {
            return new StrengthStabilityPointConstructionSectionResultRow(sectionResult);
        }
    }
}