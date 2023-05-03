// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Assessment section assembly kernel stub for testing purposes.
    /// </summary>
    public class AssessmentSectionAssemblyKernelStub : IAssessmentGradeAssembler
    {
        /// <summary>
        /// Gets the collection of <see cref="Probability"/> used as an input parameter for assembly methods.
        /// </summary>
        public IEnumerable<Probability> FailureMechanismProbabilities { get; private set; }

        /// <summary>
        /// Gets the collection of assessment section categories.
        /// </summary>
        public CategoriesList<AssessmentSectionCategory> Categories { get; private set; }

        /// <summary>
        /// Gets the assembly probability.
        /// </summary>
        public Probability AssemblyProbabilityInput { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an assembly is partial.
        /// </summary>
        public bool? PartialAssembly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a probability calculation was called or not. 
        /// </summary>
        public bool ProbabilityCalculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an assembly group calculation was called or not. 
        /// </summary>
        public bool AssemblyGroupCalculated { get; private set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets the assembly probability of an assessment section.
        /// </summary>
        public Probability AssemblyProbability { private get; set; }

        /// <summary>
        /// Sets the assembly group of an assessment section.
        /// </summary>
        public EAssessmentGrade AssemblyGroup { private get; set; }

        public Probability CalculateAssessmentSectionFailureProbabilityBoi2A1(IEnumerable<Probability> failureMechanismProbabilities, bool partialAssembly)
        {
            ThrowException();

            ProbabilityCalculated = true;
            FailureMechanismProbabilities = failureMechanismProbabilities;
            PartialAssembly = partialAssembly;

            return AssemblyProbability;
        }

        public Probability CalculateAssessmentSectionFailureProbabilityBoi2A2(
            IEnumerable<Probability> correlatedFailureMechanismProbabilities,
            IEnumerable<Probability> uncorrelatedFailureMechanismProbabilities,
            bool partialAssembly)
        {
            ThrowException();

            ProbabilityCalculated = true;
            FailureMechanismProbabilities = correlatedFailureMechanismProbabilities;
            PartialAssembly = partialAssembly;

            return AssemblyProbability;
        }

        public EAssessmentGrade DetermineAssessmentGradeBoi2B1(Probability failureProbability, CategoriesList<AssessmentSectionCategory> categories)
        {
            ThrowException();

            AssemblyGroupCalculated = true;
            AssemblyProbabilityInput = failureProbability;
            Categories = categories;

            return AssemblyGroup;
        }

        private void ThrowException()
        {
            AssemblyKernelStubHelper.ThrowException(ThrowExceptionOnCalculate, ThrowAssemblyExceptionOnCalculate, EAssemblyErrors.InvalidCategoryLimits);
        }
    }
}