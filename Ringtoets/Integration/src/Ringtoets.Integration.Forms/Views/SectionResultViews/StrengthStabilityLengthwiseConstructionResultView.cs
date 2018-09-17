// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.Helpers;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult"/>.
    /// </summary>
    public class StrengthStabilityLengthwiseConstructionResultView
        : FailureMechanismResultView<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult,
            StrengthStabilityLengthwiseConstructionSectionResultRow,
            StrengthStabilityLengthwiseConstructionFailureMechanism,
            FailureMechanismAssemblyCategoryGroupControl>
    {
        private const int simpleAssessmentResultIndex = 1;
        private const int tailorMadeAssessmentResultIndex = 2;
        private const int simpleAssemblyCategoryGroupIndex = 3;
        private const int tailorMadeAssemblyCategoryGroupIndex = 4;
        private const int combinedAssemblyCategoryGroupIndex = 5;
        private const int manualAssemblyCategoryGroupIndex = 7;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="StrengthStabilityLengthwiseConstructionResultView"/>.
        /// </summary>
        public StrengthStabilityLengthwiseConstructionResultView(
            IObservableEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> failureMechanismSectionResults,
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override StrengthStabilityLengthwiseConstructionSectionResultRow CreateFailureMechanismSectionResultRow(
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult)
        {
            return new StrengthStabilityLengthwiseConstructionSectionResultRow(
                sectionResult,
                new StrengthStabilityLengthwiseConstructionSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = simpleAssessmentResultIndex,
                    TailorMadeAssessmentResultIndex = tailorMadeAssessmentResultIndex,
                    SimpleAssemblyCategoryGroupIndex = simpleAssemblyCategoryGroupIndex,
                    TailorMadeAssemblyCategoryGroupIndex = tailorMadeAssemblyCategoryGroupIndex,
                    CombinedAssemblyCategoryGroupIndex = combinedAssemblyCategoryGroupIndex,
                    ManualAssemblyCategoryGroupIndex = manualAssemblyCategoryGroupIndex
                });
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentResultColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.UseManualAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StrengthStabilityLengthwiseConstructionSectionResultRow.ManualAssemblyCategoryGroup));
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism));
        }

        protected override bool HasManualAssemblyResults()
        {
            return StrengthStabilityLengthwiseConstructionFailureMechanismHelper.HasManualAssemblyResults(FailureMechanism);
        }
    }
}