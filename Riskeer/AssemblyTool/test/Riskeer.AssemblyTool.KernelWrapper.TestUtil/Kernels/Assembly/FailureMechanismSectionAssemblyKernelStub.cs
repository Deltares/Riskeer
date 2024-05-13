// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailureMechanismSections;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyKernelStub : IAssessmentResultsTranslator
    {
        /// <summary>
        /// Gets a value indicating whether a calculation was called or not. 
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets the initial probability of the section for the failure mechanism section.
        /// </summary>
        public Probability ProbabilityInitialMechanismSection { get; private set; }

        /// <summary>
        /// Gets the initial probability of the section for the failure mechanism section.
        /// </summary>
        public Probability RefinedProbabilitySection { get; private set; }

        /// <summary>
        /// Gets the collection of interpretation categories.
        /// </summary>
        public CategoriesList<InterpretationCategory> Categories { get; private set; }

        /// <summary>
        /// Gets the analysis state.
        /// </summary>
        public EAnalysisState AnalysisState { get; private set; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public EInterpretationCategory CategoryInput { get; private set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets the section probability.
        /// </summary>
        public Probability SectionProbability { private get; set; }

        /// <summary>
        /// Sets the category.
        /// </summary>
        public EInterpretationCategory CategoryOutput { private get; set; }

        /// <summary>
        /// Gets whether the refinement is necessary.
        /// </summary>
        public bool RefinementNecessary { get; private set; }

        /// <summary>
        /// Gets the section probability.
        /// </summary>
        public Probability SectionProbabilityInput { get; private set; }

        public FailureMechanismSectionAssemblyResult TranslateAssessmentResultAggregatedMethod(ESectionInitialMechanismProbabilitySpecification relevance,
                                                                                               Probability probabilityInitialMechanismSection,
                                                                                               ERefinementStatus refinementStatus,
                                                                                               Probability refinedProbabilitySection,
                                                                                               CategoriesList<InterpretationCategory> categories)
        {
            throw new NotImplementedException();
        }

        public Probability DetermineRepresentativeProbabilityBoi0A1(
            bool refinementNecessary, Probability probabilityInitialMechanismSection,
            Probability refinedProbabilitySection)
        {
            ThrowException();
            Calculated = true;

            RefinementNecessary = refinementNecessary;
            ProbabilityInitialMechanismSection = probabilityInitialMechanismSection;
            RefinedProbabilitySection = refinedProbabilitySection;

            return SectionProbability;
        }

        public ResultWithProfileAndSectionProbabilities DetermineRepresentativeProbabilitiesBoi0A2(
            bool refinementNecessary, Probability probabilityInitialMechanismProfile, Probability probabilityInitialMechanismSection,
            Probability refinedProbabilityProfile, Probability refinedProbabilitySection)
        {
            throw new NotImplementedException();
        }

        public EInterpretationCategory DetermineInterpretationCategoryFromFailureMechanismSectionProbabilityBoi0B1(
            Probability sectionProbability, CategoriesList<InterpretationCategory> categories)
        {
            ThrowException();
            Calculated = true;

            SectionProbabilityInput = sectionProbability;
            Categories = categories;

            return CategoryOutput;
        }

        public EInterpretationCategory DetermineInterpretationCategoryWithoutProbabilityEstimationBoi0C1(EAnalysisState analysisState)
        {
            ThrowException();
            Calculated = true;

            AnalysisState = analysisState;

            return CategoryOutput;
        }

        public Probability TranslateInterpretationCategoryToProbabilityBoi0C2(EInterpretationCategory category)
        {
            ThrowException();
            Calculated = true;

            CategoryInput = category;

            return SectionProbability;
        }

        public Probability CalculateProfileProbabilityToSectionProbabilityBoi0D1(Probability profileProbability, double lengthEffectFactor)
        {
            throw new NotImplementedException();
        }

        public Probability CalculateSectionProbabilityToProfileProbabilityBoi0D2(Probability sectionProbability, double lengthEffectFactor)
        {
            throw new NotImplementedException();
        }

        private void ThrowException()
        {
            AssemblyKernelStubHelper.ThrowException(ThrowExceptionOnCalculate, ThrowAssemblyExceptionOnCalculate,
                                                    EAssemblyErrors.EmptyResultsList);
        }
    }
}