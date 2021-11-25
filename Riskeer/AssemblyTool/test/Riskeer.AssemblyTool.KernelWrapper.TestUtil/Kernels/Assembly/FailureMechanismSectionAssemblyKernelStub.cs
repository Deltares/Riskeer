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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailurePathSections;

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
        /// Gets an indicator whether the section was relevant or not.
        /// </summary>
        public bool IsRelevant { get; private set; }

        /// <summary>
        /// Gets the initial probability of the profile for the failure path section.
        /// </summary>
        public Probability InitialProbabilityProfile { get; private set; }

        /// <summary>
        /// Gets the initial probability of the section for the failure path section.
        /// </summary>
        public Probability InitialProbabilitySection { get; private set; }

        /// <summary>
        /// Gets an indicator whether the section needs refinement or not.
        /// </summary>
        public bool NeedsRefinement { get; private set; }

        /// <summary>
        /// Gets the initial probability of the profile for the failure path section.
        /// </summary>
        public Probability RefinedProbabilityProfile { get; private set; }

        /// <summary>
        /// Gets the initial probability of the section for the failure path section.
        /// </summary>
        public Probability RefinedProbabilitySection { get; private set; }

        /// <summary>
        /// Gets the collection of categories.
        /// </summary>
        public CategoriesList<InterpretationCategory> Categories { get; private set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets the assembly result of a failure path section.
        /// </summary>
        public FailurePathSectionAssemblyResult FailurePathSectionAssemblyResult { private get; set; }

        public FailurePathSectionAssemblyResult TranslateAssessmentResultWbi0A2(bool isRelevant,
                                                                                Probability probabilityInitialMechanismProfile,
                                                                                Probability probabilityInitialMechanismSection,
                                                                                bool needsRefinement,
                                                                                Probability refinedProbabilityProfile,
                                                                                Probability refinedProbabilitySection,
                                                                                CategoriesList<InterpretationCategory> categories)
        {
            ThrowException();
            Calculated = true;

            IsRelevant = isRelevant;
            InitialProbabilityProfile = probabilityInitialMechanismProfile;
            InitialProbabilitySection = probabilityInitialMechanismSection;

            NeedsRefinement = needsRefinement;
            RefinedProbabilityProfile = refinedProbabilityProfile;
            RefinedProbabilitySection = refinedProbabilitySection;

            Categories = categories;

            return FailurePathSectionAssemblyResult;
        }

        private void ThrowException()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            if (ThrowAssemblyExceptionOnCalculate)
            {
                throw new AssemblyException("entity", EAssemblyErrors.EmptyResultsList);
            }
        }
    }
}