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
using System.Collections.Generic;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FailureMechanismSections;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Failure mechanism assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismAssemblyKernelStub : IFailureMechanismResultAssembler
    {
        /// <summary>
        /// Gets a value indicating whether a calculation was called or not. 
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets the length effect factor.
        /// </summary>
        public double LenghtEffectFactor { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="ResultWithProfileAndSectionProbabilities"/>. 
        /// </summary>
        public IEnumerable<Probability> FailureMechanismSectionAssemblyResults { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="Probability"/>.
        /// </summary>
        public IEnumerable<Probability> FailureMechanismSectionProbabilities { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an assembly is partial.
        /// </summary>
        public bool PartialAssembly { get; private set; }

        /// <summary>
        /// Sets the resulting probability.
        /// </summary>
        public Probability ProbabilityResult { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        public Probability CalculateFailureMechanismFailureProbabilityBoi1A1(IEnumerable<Probability> failureMechanismSectionAssemblyResults, bool partialAssembly)
        {
            ThrowException();

            Calculated = true;
            FailureMechanismSectionProbabilities = failureMechanismSectionAssemblyResults;
            PartialAssembly = partialAssembly;

            return ProbabilityResult;
        }

        public Probability CalculateFailureMechanismFailureProbabilityBoi1A2(IEnumerable<Probability> failureMechanismSectionAssemblyResults, double lengthEffectFactor, bool partialAssembly)
        {
            ThrowException();

            Calculated = true;
            LenghtEffectFactor = lengthEffectFactor;
            FailureMechanismSectionAssemblyResults = failureMechanismSectionAssemblyResults;
            PartialAssembly = partialAssembly;

            return ProbabilityResult;
        }

        public BoundaryLimits CalculateFailureMechanismBoundariesBoi1B1(IEnumerable<Probability> failureMechanismSectionAssemblyResults, bool partialAssembly)
        {
            throw new NotImplementedException();
        }

        public BoundaryLimits CalculateFailureMechanismBoundariesBoi1B2(IEnumerable<ResultWithProfileAndSectionProbabilities> failureMechanismSectionAssemblyResults, bool partialAssembly)
        {
            throw new NotImplementedException();
        }

        private void ThrowException()
        {
            AssemblyKernelStubHelper.ThrowException(ThrowExceptionOnCalculate, ThrowAssemblyExceptionOnCalculate, EAssemblyErrors.InvalidCategoryLimits);
        }
    }
}