// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Assessment section assembly kernel stub for testing purposes.
    /// </summary>
    public class AssessmentSectionAssemblyKernelStub : IAssessmentGradeAssembler
    {
        /// <summary>
        /// Gets a value indicating whether an assembly is partial.
        /// </summary>
        public bool? PartialAssembly { get; private set; }

        /// <summary>
        /// Gets or sets the failure mechanism category result
        /// </summary>
        public EFailureMechanismCategory FailureMechanismCategoryResult { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism assembly result.
        /// </summary>
        public FailureMechanismAssemblyResult FailureMechanismAssemblyResult { get; set; }

        /// <summary>
        /// Gets or sets the assessment section assembly result.
        /// </summary>
        public EAssessmentGrade AssessmentSectionAssemblyResult { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="FailureMechanismAssemblyResult"/> used as an input parameter for assembly methods.
        /// </summary>
        public IEnumerable<FailureMechanismAssemblyResult> FailureMechanismAssemblyResults { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFailureMechanismCategory"/> used as an input parameter for assembly methods.
        /// </summary>
        public EFailureMechanismCategory? AssemblyResultNoFailureProbability { get; private set; }

        /// <summary>
        /// Gets the <see cref="FailureMechanismAssemblyResult"/> used as an input parameter for assembly methods.
        /// </summary>
        public FailureMechanismAssemblyResult AssemblyResultWithFailureProbability { get; private set; }

        /// <summary>
        /// Gets the <see cref="CategoriesList{TCategory}"/> used as an input parameter for assembly methods.
        /// </summary>
        public CategoriesList<FailureMechanismCategory> FailureMechanismCategories { get; private set; }

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

        public EFailureMechanismCategory AssembleAssessmentSectionWbi2A1(IEnumerable<FailureMechanismAssemblyResult> failureMechanismAssemblyResults,
                                                                         bool partialAssembly)
        {
            ThrowException();

            PartialAssembly = partialAssembly;
            FailureMechanismAssemblyResults = failureMechanismAssemblyResults;

            Calculated = true;

            return FailureMechanismCategoryResult;
        }

        public FailureMechanismAssemblyResult AssembleAssessmentSectionWbi2B1(IEnumerable<FailureMechanismAssemblyResult> failureMechanismAssemblyResults,
                                                                              CategoriesList<FailureMechanismCategory> categories,
                                                                              bool partialAssembly)
        {
            ThrowException();

            PartialAssembly = partialAssembly;
            FailureMechanismCategories = categories;
            FailureMechanismAssemblyResults = failureMechanismAssemblyResults;

            Calculated = true;

            return FailureMechanismAssemblyResult;
        }

        public EAssessmentGrade AssembleAssessmentSectionWbi2C1(EFailureMechanismCategory assemblyResultNoFailureProbability,
                                                                FailureMechanismAssemblyResult assemblyResultWithFailureProbability)
        {
            ThrowException();

            AssemblyResultNoFailureProbability = assemblyResultNoFailureProbability;
            AssemblyResultWithFailureProbability = assemblyResultWithFailureProbability;

            Calculated = true;

            return AssessmentSectionAssemblyResult;
        }

        private void ThrowException()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            if (ThrowAssemblyExceptionOnCalculate)
            {
                throw new AssemblyException("entity", EAssemblyErrors.CategoryLowerLimitOutOfRange);
            }
        }
    }
}