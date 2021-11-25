// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.Views.SectionResultRows;

namespace Riskeer.Integration.Forms.Views.SectionResultViews
{
    /// <summary>
    /// The view for a collection of <see cref="WaterPressureAsphaltCoverFailureMechanismSectionResultOld"/>.
    /// </summary>
    public class WaterPressureAsphaltCoverResultViewOld : FailureMechanismResultViewOld<WaterPressureAsphaltCoverFailureMechanismSectionResultOld,
        WaterPressureAsphaltCoverSectionResultRowOld,
        WaterPressureAsphaltCoverFailureMechanism,
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
        /// Creates a new instance of <see cref="WaterPressureAsphaltCoverResultViewOld"/>.
        /// </summary>
        public WaterPressureAsphaltCoverResultViewOld(
            IObservableEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResultOld> failureMechanismSectionResults,
            WaterPressureAsphaltCoverFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        protected override WaterPressureAsphaltCoverSectionResultRowOld CreateFailureMechanismSectionResultRow(WaterPressureAsphaltCoverFailureMechanismSectionResultOld sectionResult)
        {
            return new WaterPressureAsphaltCoverSectionResultRowOld(
                sectionResult,
                new WaterPressureAsphaltCoverSectionResultRowOld.ConstructionProperties
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
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentResultColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.UseManualAssembly));

            FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(WaterPressureAsphaltCoverSectionResultRowOld.ManualAssemblyCategoryGroup));
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(WaterPressureAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism, true));
        }
    }
}