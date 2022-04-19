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
using Assembly.Kernel.Model.FailureMechanismSections;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Combined failure mechanism section assembly kernel stub for testing purposes.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyKernelStub : ICommonFailureMechanismSectionAssembler
    {
        /// <summary>
        /// Gets a value indicating whether a calculation was called or not. 
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets the assessment section length used as an input parameter for assembly methods.
        /// </summary>
        public double AssessmentSectionLength { get; private set; }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSectionList"/> used as an input parameter for assembly methods.
        /// </summary>
        public IEnumerable<FailureMechanismSectionList> FailureMechanismSectionLists { get; private set; }

        /// <summary>
        /// Gets the partial assembly used as an input parameter for assembly methods.
        /// </summary>
        public bool PartialAssembly { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="AssemblyResult"/>.
        /// </summary>
        public GreatestCommonDenominatorAssemblyResult AssemblyResult { get; set; }

        public GreatestCommonDenominatorAssemblyResult AssembleCommonFailureMechanismSections(IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists,
                                                                                              double assessmentSectionLength, bool partialAssembly)
        {
            ThrowException();

            FailureMechanismSectionLists = failureMechanismSectionLists;
            AssessmentSectionLength = assessmentSectionLength;
            PartialAssembly = partialAssembly;

            Calculated = true;

            return AssemblyResult;
        }

        public FailureMechanismSectionList FindGreatestCommonDenominatorSectionsBoi3A1(IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists,
                                                                                       double assessmentSectionLength)
        {
            throw new NotImplementedException();
        }

        public FailureMechanismSectionList TranslateFailureMechanismResultsToCommonSectionsBoi3B1(FailureMechanismSectionList failureMechanismSectionList,
                                                                                                  FailureMechanismSectionList commonSections)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FailureMechanismSectionWithCategory> DetermineCombinedResultPerCommonSectionBoi3C1(IEnumerable<FailureMechanismSectionList> failureMechanismResults,
                                                                                                              bool partialAssembly)
        {
            throw new NotImplementedException();
        }

        private void ThrowException()
        {
            AssemblyKernelStubHelper.ThrowException(ThrowExceptionOnCalculate, ThrowAssemblyExceptionOnCalculate, EAssemblyErrors.EmptyResultsList);
        }
    }
}