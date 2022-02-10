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
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
    public class NonAdoptableWithProfileProbabilityFailureMechanismResultView<TFailureMechanism> : FailureMechanismResultView<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow, TFailureMechanism>
        where TFailureMechanism : IHasSectionResults<FailureMechanismSectionResultOld, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultProfileProbabilityIndex = 3;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 4;
        private const int furtherAnalysisTypeIndex = 5;
        private const int refinedProfileProbabilityIndex = 6;
        private const int refinedSectionProbabilityIndex = 7;
        private const int profileProbabilityIndex = 8;
        private const int sectionProbabilityIndex = 9;
        private const int sectionNIndex = 10;
        private const int assemblyGroupIndex = 11;

        private readonly IAssessmentSection assessmentSection;
        private readonly Func<TFailureMechanism, double> getNFunc;
        private readonly Func<TFailureMechanism, bool> getUseLengthEffectFunc;

        /// <summary>
        /// Creates a new instance of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismResultView{TFailureMechanism}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <param name="getNFunc">The <see cref="Func{T1,TResult}"/> to get the N.</param>
        /// <param name="getUseLengthEffectFunc">The <see cref="Func{T1,TResult}"/> to get whether to use the length effect.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public NonAdoptableWithProfileProbabilityFailureMechanismResultView(IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> failureMechanismSectionResults,
                                                                            TFailureMechanism failureMechanism,
                                                                            IAssessmentSection assessmentSection,
                                                                            Func<TFailureMechanism, double> getNFunc,
                                                                            Func<TFailureMechanism, bool> getUseLengthEffectFunc)
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

            if (getUseLengthEffectFunc == null)
            {
                throw new ArgumentNullException(nameof(getUseLengthEffectFunc));
            }

            this.assessmentSection = assessmentSection;
            this.getNFunc = getNFunc;
            this.getUseLengthEffectFunc = getUseLengthEffectFunc;
        }

        protected override void UpdateInternalViewData()
        {
            UpdateVisibility();
            base.UpdateInternalViewData();
        }

        protected override NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            return new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(
                sectionResult,
                CreateErrorProvider(),
                CreateLengthEffectProvider(),
                assessmentSection,
                new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultTypeIndex = initialFailureMechanismResultTypeIndex,
                    InitialFailureMechanismResultProfileProbabilityIndex = initialFailureMechanismResultProfileProbabilityIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisTypeIndex = furtherAnalysisTypeIndex,
                    RefinedProfileProbabilityIndex = refinedProfileProbabilityIndex,
                    RefinedSectionProbabilityIndex = refinedSectionProbabilityIndex,
                    ProfileProbabilityIndex = profileProbabilityIndex,
                    SectionProbabilityIndex = sectionProbabilityIndex,
                    SectionNIndex = sectionNIndex,
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
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.IsRelevant));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<NonAdoptableInitialFailureMechanismResultType>(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultType));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultProfileProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.FurtherAnalysisType));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedProfileProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyProfileProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionNColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionN));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(
                DataGridViewControl,
                nameof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow.AssemblyGroup));
        }

        private IFailureMechanismSectionResultRowErrorProvider CreateErrorProvider()
        {
            return new FailureMechanismSectionResultRowErrorProvider();
        }
        
        private ILengthEffectProvider CreateLengthEffectProvider()
        {
            return new LengthEffectProvider(() => getUseLengthEffectFunc(FailureMechanism), () => 1.0);
        }

        private void UpdateVisibility()
        {
            bool useLengthEffect = getUseLengthEffectFunc(FailureMechanism);

            DataGridViewControl.GetColumnFromIndex(initialFailureMechanismResultProfileProbabilityIndex).Visible = useLengthEffect;
            DataGridViewControl.GetColumnFromIndex(refinedProfileProbabilityIndex).Visible = useLengthEffect;
            DataGridViewControl.GetColumnFromIndex(profileProbabilityIndex).Visible = useLengthEffect;
            DataGridViewControl.GetColumnFromIndex(sectionNIndex).Visible = useLengthEffect;
        }
    }
}