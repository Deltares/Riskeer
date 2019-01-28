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
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.Views.SectionResultRows;

namespace Riskeer.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="MicrostabilityFailureMechanismSectionResult"/>.
    /// </summary>
    public class MicrostabilityResultView : FailureMechanismResultView<MicrostabilityFailureMechanismSectionResult,
        MicrostabilitySectionResultRow,
        MicrostabilityFailureMechanism,
        FailureMechanismAssemblyCategoryGroupControl>
    {
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int tailorMadeAssessmentResultIndex = 3;
        private const int simpleAssemblyCategoryGroupIndex = 4;
        private const int detailedAssemblyCategoryGroupIndex = 5;
        private const int tailorMadeAssemblyCategoryGroupIndex = 6;
        private const int combinedAssemblyCategoryGroupIndex = 7;
        private const int manualAssemblyCategoryGroupIndex = 9;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="MicrostabilityResultView"/>.
        /// </summary>
        public MicrostabilityResultView(IObservableEnumerable<MicrostabilityFailureMechanismSectionResult> failureMechanismSectionResults,
                                        MicrostabilityFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override MicrostabilitySectionResultRow CreateFailureMechanismSectionResultRow(MicrostabilityFailureMechanismSectionResult sectionResult)
        {
            return new MicrostabilitySectionResultRow(
                sectionResult,
                new MicrostabilitySectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = simpleAssessmentResultIndex,
                    DetailedAssessmentResultIndex = detailedAssessmentResultIndex,
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
                nameof(MicrostabilitySectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.DetailedAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentResultColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.UseManualAssembly));

            FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MicrostabilitySectionResultRow.ManualAssemblyCategoryGroup));
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism, true));
        }
    }
}