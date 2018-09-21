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

using System;
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityOutwardsResultView : FailureMechanismResultView<MacroStabilityOutwardsFailureMechanismSectionResult,
        MacroStabilityOutwardsSectionResultRow,
        MacroStabilityOutwardsFailureMechanism,
        FailureMechanismAssemblyCategoryGroupControl>
    {
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int detailedAssessmentProbabilityIndex = 3;
        private const int tailorMadeAssessmentResultIndex = 4;
        private const int tailorMadeAssessmentProbabilityIndex = 5;
        private const int simpleAssemblyCategoryGroupIndex = 6;
        private const int detailedAssemblyCategoryGroupIndex = 7;
        private const int tailorMadeAssemblyCategoryGroupIndex = 8;
        private const int combinedAssemblyCategoryGroupIndex = 9;
        private const int manualAssemblyCategoryGroupIndex = 11;

        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityOutwardsResultView(IObservableEnumerable<MacroStabilityOutwardsFailureMechanismSectionResult> failureMechanismSectionResults,
                                                MacroStabilityOutwardsFailureMechanism failureMechanism,
                                                IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        protected override MacroStabilityOutwardsSectionResultRow CreateFailureMechanismSectionResultRow(MacroStabilityOutwardsFailureMechanismSectionResult sectionResult)
        {
            return new MacroStabilityOutwardsSectionResultRow(
                sectionResult, FailureMechanism, assessmentSection,
                new MacroStabilityOutwardsSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = simpleAssessmentResultIndex,
                    DetailedAssessmentResultIndex = detailedAssessmentResultIndex,
                    DetailedAssessmentProbabilityIndex = detailedAssessmentProbabilityIndex,
                    TailorMadeAssessmentResultIndex = tailorMadeAssessmentResultIndex,
                    TailorMadeAssessmentProbabilityIndex = tailorMadeAssessmentProbabilityIndex,
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
                nameof(MacroStabilityOutwardsSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityOnlyResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssessmentProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssessmentProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.UseManualAssembly));

            FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.ManualAssemblyCategoryGroup));
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism,
                                                                                                                                                   assessmentSection,
                                                                                                                                                   true));
        }

        protected override bool HasManualAssemblyResults()
        {
            return HasSectionResultsHelper.HasManualAssemblyResults(FailureMechanism);
        }
    }
}