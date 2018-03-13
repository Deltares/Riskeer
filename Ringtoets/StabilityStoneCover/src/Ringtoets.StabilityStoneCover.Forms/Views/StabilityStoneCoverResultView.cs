﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Util;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityStoneCover.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>.
    /// </summary>
    public class StabilityStoneCoverResultView : FailureMechanismResultView<StabilityStoneCoverFailureMechanismSectionResult,
        StabilityStoneCoverSectionResultRow, StabilityStoneCoverFailureMechanism>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverResultView"/>.
        /// </summary>
        public StabilityStoneCoverResultView(IObservableEnumerable<StabilityStoneCoverFailureMechanismSectionResult> failureMechanismSectionResults,
                                             StabilityStoneCoverFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override StabilityStoneCoverSectionResultRow CreateFailureMechanismSectionResultRow(StabilityStoneCoverFailureMechanismSectionResult sectionResult)
        {
            return new StabilityStoneCoverSectionResultRow(sectionResult);
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(StabilityStoneCoverSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(StabilityStoneCoverSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedSignalingNormColumn(
                DataGridViewControl,
                nameof(StabilityStoneCoverSectionResultRow.DetailedAssessmentResultForFactorizedSignalingNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForSignalingNormColumn(
                DataGridViewControl,
                nameof(StabilityStoneCoverSectionResultRow.DetailedAssessmentResultForSignalingNorm));

            DataGridViewControl.AddTextBoxColumn(
                nameof(StabilityStoneCoverSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_TailorMadeAssessmentResult_DisplayName);
        }
    }
}