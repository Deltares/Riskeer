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

using System;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="NonAdoptableFailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
    public class NonAdoptableFailureMechanismResultView<TFailureMechanism> : FailureMechanismResultView<NonAdoptableFailureMechanismSectionResult, NonAdoptableFailureMechanismSectionResultRow, TFailureMechanism>
        where TFailureMechanism : IFailurePath<NonAdoptableFailureMechanismSectionResult>
    {
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 3;
        private const int furtherAnalysisTypeIndex = 4;
        private const int refinedSectionProbabilityIndex = 5;
        private const int sectionProbabilityIndex = 6;
        private const int assemblyGroupIndex = 7;

        private readonly IAssessmentSection assessmentSection;
        private readonly Func<TFailureMechanism, double> getNFunc;

        /// <summary>
        /// Creates a new instance of <see cref="NonAdoptableFailureMechanismResultView{TFailureMechanism}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="NonAdoptableFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <param name="getNFunc">The <see cref="Func{T1,TResult}"/> to get the N.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public NonAdoptableFailureMechanismResultView(IObservableEnumerable<NonAdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                                                      TFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection,
                                                      Func<TFailureMechanism, double> getNFunc)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNFunc == null)
            {
                throw new ArgumentNullException(nameof(getNFunc));
            }

            this.assessmentSection = assessmentSection;
            this.getNFunc = getNFunc;
        }

        protected override NonAdoptableFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(NonAdoptableFailureMechanismSectionResult sectionResult)
        {
            return new NonAdoptableFailureMechanismSectionResultRow(
                sectionResult,
                CreateErrorProvider(),
                assessmentSection,
                new NonAdoptableFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultTypeIndex = initialFailureMechanismResultTypeIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisTypeIndex = furtherAnalysisTypeIndex,
                    RefinedSectionProbabilityIndex = refinedSectionProbabilityIndex,
                    SectionProbabilityIndex = sectionProbabilityIndex,
                    AssemblyGroupIndex = assemblyGroupIndex
                });
        }

        protected override double GetN()
        {
            return getNFunc(FailureMechanism);
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.IsRelevant));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<NonAdoptableInitialFailureMechanismResultType>(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultType));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.FurtherAnalysisType));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.RefinedSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.SectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(
                DataGridViewControl,
                nameof(NonAdoptableFailureMechanismSectionResultRow.AssemblyGroup));
        }

        private static IFailureMechanismSectionResultRowErrorProvider CreateErrorProvider()
        {
            return new FailureMechanismSectionResultRowErrorProvider();
        }
    }
}