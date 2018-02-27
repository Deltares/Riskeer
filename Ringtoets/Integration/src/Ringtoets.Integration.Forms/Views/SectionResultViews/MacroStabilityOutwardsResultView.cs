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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityOutwardsResultView : FailureMechanismResultView<MacroStabilityOutwardsFailureMechanismSectionResult,
        MacroStabilityOutwardsSectionResultRow, MacroStabilityOutwardsFailureMechanism>
    {
        private const int detailedAssessmentResultIndex = 2;
        private const int detailedAssessmentProbabilityIndex = 3;
        private const int tailorMadeAssessmentResultIndex = 4;
        private const int tailorMadeAssessmentProbabilityIndex = 5;

        private readonly IAssessmentSection assessmentSection;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belongs to.</param>
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

            FormattingRules = CreateFormattingRules();
        }

        protected override IEnumerable<DataGridViewColumnFormattingRule<MacroStabilityOutwardsSectionResultRow>> FormattingRules { get; }

        protected override MacroStabilityOutwardsSectionResultRow CreateFailureMechanismSectionResultRow(MacroStabilityOutwardsFailureMechanismSectionResult sectionResult)
        {
            return new MacroStabilityOutwardsSectionResultRow(sectionResult, FailureMechanism, assessmentSection);
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.Name));

            FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssessmentResult));

            FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssessmentProbability));

            FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssessmentProbability));

            FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentAssemblyColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentAssemblyColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentAssemblyColumn(
                DataGridViewControl,
                nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssemblyCategoryGroup));
        }

        private static IEnumerable<DataGridViewColumnFormattingRule<MacroStabilityOutwardsSectionResultRow>> CreateFormattingRules()
        {
            return new[]
            {
                new DataGridViewColumnFormattingRule<MacroStabilityOutwardsSectionResultRow>(
                    new[]
                    {
                        detailedAssessmentResultIndex,
                        detailedAssessmentProbabilityIndex,
                        tailorMadeAssessmentResultIndex,
                        tailorMadeAssessmentProbabilityIndex
                    },
                    row => FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(row.SimpleAssessmentResult)),
                new DataGridViewColumnFormattingRule<MacroStabilityOutwardsSectionResultRow>(
                    new[]
                    {
                        detailedAssessmentProbabilityIndex
                    },
                    row => row.DetailedAssessmentResult != DetailedAssessmentResultType.Probability),
                new DataGridViewColumnFormattingRule<MacroStabilityOutwardsSectionResultRow>(
                    new[]
                    {
                        tailorMadeAssessmentProbabilityIndex
                    },
                    row => row.TailorMadeAssessmentResult != TailorMadeAssessmentResultType.Probability)
            };
        }
    }
}