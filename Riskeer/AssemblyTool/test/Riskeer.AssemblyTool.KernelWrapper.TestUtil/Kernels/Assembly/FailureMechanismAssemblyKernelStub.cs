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
using Assembly.Kernel.Model.FmSectionTypes;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Failure mechanism assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismAssemblyKernelStub : IFailureMechanismResultAssembler
    {
        /// <summary>
        /// Gets the collection of <see cref="FmSectionAssemblyDirectResult"/> used as input parameter for assembly methods.
        /// </summary>
        public IEnumerable<FmSectionAssemblyDirectResult> FmSectionAssemblyResultsInput { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="FmSectionAssemblyDirectResult"/> used as input parameter for assembly methods.
        /// </summary>
        public IEnumerable<FmSectionAssemblyDirectResultWithProbability> FmSectionAssemblyResultsWithProbabilityInput { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an assembly is partial.
        /// </summary>
        public bool? PartialAssembly { get; private set; }

        /// <summary>
        /// Gets the <see cref="FailureMechanism"/> used as input parameter for assembly methods.
        /// </summary>
        public FailureMechanism FailureMechanismInput { get; private set; }

        /// <summary>
        /// Gets the <see cref="CategoriesList{TCategory}"/> with <see cref="FailureMechanismCategory"/>
        /// used as input parameter for assembly methods.
        /// </summary>
        public CategoriesList<FailureMechanismCategory> Categories { get; private set; }

        /// <summary>
        /// Gets or sets the failure mechanism category result.
        /// </summary>
        public EFailureMechanismCategory FailureMechanismCategoryResult { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism assembly result.
        /// </summary>
        public FailureMechanismAssemblyResult FailureMechanismAssemblyResult { get; set; }

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

        public EFailureMechanismCategory AssembleFailureMechanismWbi1A1(IEnumerable<FmSectionAssemblyDirectResult> fmSectionAssemblyResults,
                                                                        bool partialAssembly)
        {
            ThrowException();

            FmSectionAssemblyResultsInput = fmSectionAssemblyResults;
            PartialAssembly = partialAssembly;

            Calculated = true;
            return FailureMechanismCategoryResult;
        }

        public EIndirectAssessmentResult AssembleFailureMechanismWbi1A2(IEnumerable<FmSectionAssemblyIndirectResult> fmSectionAssemblyResults,
                                                                        bool partialAssembly)
        {
            throw new NotImplementedException();
        }

        public FailureMechanismAssemblyResult AssembleFailureMechanismWbi1B1(FailureMechanism failureMechanism,
                                                                             IEnumerable<FmSectionAssemblyDirectResultWithProbability> fmSectionAssemblyResults,
                                                                             CategoriesList<FailureMechanismCategory> categoryLimits,
                                                                             bool partialAssembly)
        {
            ThrowException();

            Categories = categoryLimits;
            FailureMechanismInput = failureMechanism;
            FmSectionAssemblyResultsWithProbabilityInput = fmSectionAssemblyResults;
            PartialAssembly = partialAssembly;

            Calculated = true;
            return FailureMechanismAssemblyResult;
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