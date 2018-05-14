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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Combined failure mechanism section assembly kernel stub for testing purposes.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyKernelStub : ICommonFailureMechanismSectionAssembler
    {
        /// <summary>
        /// Indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowException { private get; set; }

        /// <summary>
        /// Gets a value indicating whether a calculation was called or not. 
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an assembly is partial.
        /// </summary>
        public bool? PartialAssembly { get; private set; }

        /// <summary>
        /// Gets the assessment section length  used as an input parameter for assembly method.
        /// </summary>
        public double? AssessmentSectionLengthInput { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="FailureMechanismSectionList"/> used as an input parameter for assembly method.
        /// </summary>
        public IEnumerable<FailureMechanismSectionList> FailureMechanismSectionListsInput { get; private set; }

        /// <summary>
        /// Gets or sets the assembly result.
        /// </summary>
        public AssemblyResult AssemblyResult { get; set; }

        public AssemblyResult AssembleCommonFailureMechanismSections(IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists,
                                                                     double assessmentSectionLength, bool partialAssembly)
        {
            if (ThrowException)
            {
                throw new Exception("Message", new Exception());
            }

            FailureMechanismSectionListsInput = failureMechanismSectionLists;
            AssessmentSectionLengthInput = assessmentSectionLength;
            PartialAssembly = partialAssembly;

            Calculated = true;

            return AssemblyResult;
        }
    }
}