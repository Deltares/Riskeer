// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Core.Common.Base;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Views;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverFailureMechanismResultView : FailureMechanismResultView<WaveImpactAsphaltCoverFailureMechanismSectionResult,
        WaveImpactAsphaltCoverFailureMechanismSectionResultRow,
        WaveImpactAsphaltCoverFailureMechanism,
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
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverFailureMechanismResultView"/>.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanismResultView(IObservableEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult> failureMechanismSectionResults,
                                                                WaveImpactAsphaltCoverFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override WaveImpactAsphaltCoverFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(
            WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult)
        {
            return new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(
                sectionResult,
                new WaveImpactAsphaltCoverFailureMechanismSectionResultRow.ConstructionProperties
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
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedSignalingNormColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.DetailedAssessmentResultForFactorizedSignalingNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForSignalingNormColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.DetailedAssessmentResultForSignalingNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForLowerLimitNormColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.DetailedAssessmentResultForLowerLimitNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.DetailedAssessmentResultForFactorizedLowerLimitNorm));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.UseManualAssembly));

            FailureMechanismSectionResultViewColumnBuilder.AddSelectableAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaveImpactAsphaltCoverFailureMechanismSectionResultRow.ManualAssemblyCategoryGroup));
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism, true));
        }
    }
}