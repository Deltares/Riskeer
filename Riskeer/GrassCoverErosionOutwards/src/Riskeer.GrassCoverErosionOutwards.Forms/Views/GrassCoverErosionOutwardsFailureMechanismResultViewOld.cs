﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Base;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResultOld"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismResultViewOld : FailureMechanismResultViewOld<GrassCoverErosionOutwardsFailureMechanismSectionResultOld,
        GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld,
        GrassCoverErosionOutwardsFailureMechanism,
        FailureMechanismAssemblyCategoryGroupControl>
    {
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultForFactorizedSignalingNormIndex = 2;
        private const int detailedAssessmentResultForSignalingNormIndex = 3;
        private const int detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = 4;
        private const int detailedAssessmentResultForLowerLimitNormIndex = 5;
        private const int detailedAssessmentResultForFactorizedLowerLimitNormIndex = 6;
        private const int tailorMadeAssessmentResultIndex = 7;
        private const int simpleAssemblyCategoryGroupIndex = 8;
        private const int detailedAssemblyCategoryGroupIndex = 9;
        private const int tailorMadeAssemblyCategoryGroupIndex = 10;
        private const int combinedAssemblyCategoryGroupIndex = 11;
        private const int manualAssemblyCategoryGroupIndex = 13;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsFailureMechanismResultViewOld"/>.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanismResultViewOld(
            IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResultOld> failureMechanismSectionResults,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld CreateFailureMechanismSectionResultRow(GrassCoverErosionOutwardsFailureMechanismSectionResultOld sectionResult)
        {
            return new GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld(
                sectionResult,
                new GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = simpleAssessmentResultIndex,
                    DetailedAssessmentResultForFactorizedSignalingNormIndex = detailedAssessmentResultForFactorizedSignalingNormIndex,
                    DetailedAssessmentResultForSignalingNormIndex = detailedAssessmentResultForSignalingNormIndex,
                    DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex,
                    DetailedAssessmentResultForLowerLimitNormIndex = detailedAssessmentResultForLowerLimitNormIndex,
                    DetailedAssessmentResultForFactorizedLowerLimitNormIndex = detailedAssessmentResultForFactorizedLowerLimitNormIndex,
                    TailorMadeAssessmentResultIndex = tailorMadeAssessmentResultIndex,
                    SimpleAssemblyCategoryGroupIndex = simpleAssemblyCategoryGroupIndex,
                    DetailedAssemblyCategoryGroupIndex = detailedAssemblyCategoryGroupIndex,
                    TailorMadeAssemblyCategoryGroupIndex = tailorMadeAssemblyCategoryGroupIndex,
                    CombinedAssemblyCategoryGroupIndex = combinedAssemblyCategoryGroupIndex,
                    ManualAssemblyCategoryGroupIndex = manualAssemblyCategoryGroupIndex
                });
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedSignalingNormColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.DetailedAssessmentResultForFactorizedSignalingNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForSignalingNormColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.DetailedAssessmentResultForSignalingNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForLowerLimitNormColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.DetailedAssessmentResultForLowerLimitNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.DetailedAssessmentResultForFactorizedLowerLimitNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.UseManualAssembly));

            FailureMechanismSectionResultViewColumnBuilder.AddSelectableAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRowOld.ManualAssemblyCategoryGroup));
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism, true));
        }
    }
}