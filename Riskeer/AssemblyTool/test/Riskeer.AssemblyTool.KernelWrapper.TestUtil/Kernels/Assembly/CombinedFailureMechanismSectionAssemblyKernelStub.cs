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
using System.Collections.Generic;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model.AssessmentSection;
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
        /// Gets the common sections used as an input parameter for assembly methods.
        /// </summary>
        public FailureMechanismSectionList CommonSections { get; private set; }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSectionList"/> used as an input parameter for assembly methods.
        /// </summary>
        public FailureMechanismSectionList FailureMechanismSectionListInput { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="AssemblyResult"/>.
        /// </summary>
        public AssemblyResult AssemblyResult { get; set; }

        /// <summary>
        /// Gets ors sets the <see cref="FailureMechanismSectionList"/> result.
        /// </summary>
        public FailureMechanismSectionList FailureMechanismSectionListResult { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism sections with category result.
        /// </summary>
        public IEnumerable<FailureMechanismSectionWithCategory> FailureMechanismSectionsWithCategory { get; set; }

        public AssemblyResult AssembleCommonFailureMechanismSections(IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists,
                                                                     double assessmentSectionLength, bool partialAssembly)
        {
            ThrowException();

            FailureMechanismSectionLists = failureMechanismSectionLists;
            AssessmentSectionLength = assessmentSectionLength;
            PartialAssembly = partialAssembly;

            Calculated = true;

            return AssemblyResult;
        }

        public FailureMechanismSectionList FindGreatestCommonDenominatorSectionsWbi3A1(IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists,
                                                                                       double assessmentSectionLength)
        {
            ThrowException();

            FailureMechanismSectionLists = failureMechanismSectionLists;
            AssessmentSectionLength = assessmentSectionLength;

            Calculated = true;

            return FailureMechanismSectionListResult;
        }

        public FailureMechanismSectionList TranslateFailureMechanismResultsToCommonSectionsWbi3B1(FailureMechanismSectionList failureMechanismSectionList,
                                                                                                  FailureMechanismSectionList commonSections)
        {
            ThrowException();

            FailureMechanismSectionListInput = failureMechanismSectionList;
            CommonSections = commonSections;

            Calculated = true;

            return FailureMechanismSectionListResult;
        }

        public IEnumerable<FailureMechanismSectionWithCategory> DetermineCombinedResultPerCommonSectionWbi3C1(IEnumerable<FailureMechanismSectionList> failureMechanismResults,
                                                                                                              bool partialAssembly)
        {
            ThrowException();

            FailureMechanismSectionLists = failureMechanismResults;
            PartialAssembly = partialAssembly;

            Calculated = true;

            return FailureMechanismSectionsWithCategory;
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