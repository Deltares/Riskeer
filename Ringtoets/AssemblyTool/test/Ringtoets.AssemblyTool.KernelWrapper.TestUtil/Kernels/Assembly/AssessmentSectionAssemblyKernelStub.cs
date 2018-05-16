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
    /// Assessment section assembly kernel stub for testing purposes.
    /// </summary>
    public class AssessmentSectionAssemblyKernelStub : IAssessmentGradeAssembler
    {
        /// <summary>
        /// Gets the <see cref="AssessmentSection"/> used as an input parameter for assembly methods.
        /// </summary>
        public AssessmentSection AssessmentSectionInput { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an assembly is partial.
        /// </summary>
        public bool? PartialAssembly { get; private set; }

        /// <summary>
        /// Gets or sets the assessment section assembly result.
        /// </summary>
        public EAssessmentGrade AssessmentGradeResult { get; set; }

        /// <summary>
        /// Gets or sets the assessment section assembly result.
        /// </summary>
        public AssessmentSectionAssemblyResult AssessmentSectionAssemblyResult { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="FailureMechanismAssemblyResult"/> used as an input parameter for assembly methods.
        /// </summary>
        public IEnumerable<FailureMechanismAssemblyResult> FailureMechanismAssemblyResults { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssessmentSectionAssemblyResult"/> used as an input parameter for assembly methods.
        /// </summary>
        public AssessmentSectionAssemblyResult AssemblyResultNoFailureProbability { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssessmentSectionAssemblyResult"/> used as an input parameter for assembly methods.
        /// </summary>
        public AssessmentSectionAssemblyResult AssemblyResultWithFailureProbability { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a calculation was called or not. 
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowException { private get; set; }

        public EAssessmentGrade AssembleAssessmentSectionWbi2A1(IEnumerable<FailureMechanismAssemblyResult> failureMechanismAssemblyResults,
                                                                bool partialAssembly)
        {
            if (ThrowException)
            {
                throw new Exception("Message", new Exception());
            }

            PartialAssembly = partialAssembly;
            FailureMechanismAssemblyResults = failureMechanismAssemblyResults;

            Calculated = true;

            return AssessmentGradeResult;
        }

        public AssessmentSectionAssemblyResult AssembleAssessmentSectionWbi2B1(AssessmentSection section,
                                                                               IEnumerable<FailureMechanismAssemblyResult> failureMechanismAssemblyResults,
                                                                               bool partialAssembly)
        {
            if (ThrowException)
            {
                throw new Exception("Message", new Exception());
            }

            PartialAssembly = partialAssembly;
            AssessmentSectionInput = section;
            FailureMechanismAssemblyResults = failureMechanismAssemblyResults;

            Calculated = true;

            return AssessmentSectionAssemblyResult;
        }

        public AssessmentSectionAssemblyResult AssembleAssessmentSectionWbi2C1(AssessmentSectionAssemblyResult assemblyResultNoFailureProbability,
                                                                               AssessmentSectionAssemblyResult assemblyResultWithFailureProbability)
        {
            if (ThrowException)
            {
                throw new Exception("Message", new Exception());
            }

            AssemblyResultNoFailureProbability = assemblyResultNoFailureProbability;
            AssemblyResultWithFailureProbability = assemblyResultWithFailureProbability;

            Calculated = true;

            return AssessmentSectionAssemblyResult;
        }
    }
}