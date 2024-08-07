﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Assembly.Kernel.Interfaces;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Groups;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels
{
    /// <summary>
    /// Factory that creates assembly tool kernel stubs for testing purposes.
    /// </summary>
    public class TestAssemblyToolKernelFactory : IAssemblyToolKernelFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestAssemblyToolKernelFactory"/>.
        /// </summary>
        public TestAssemblyToolKernelFactory()
        {
            LastCreatedAssemblyCategoryLimitsKernel = new AssemblyCategoryLimitsKernelStub();
            LastCreatedFailureMechanismSectionAssemblyKernel = new FailureMechanismSectionAssemblyKernelStub();
            LastCreatedFailureMechanismAssemblyKernel = new FailureMechanismAssemblyKernelStub();
            LastCreatedAssessmentSectionAssemblyKernel = new AssessmentSectionAssemblyKernelStub();
        }

        /// <summary>
        /// Gets the last created category limits kernel.
        /// </summary>
        public AssemblyCategoryLimitsKernelStub LastCreatedAssemblyCategoryLimitsKernel { get; }

        /// <summary>
        /// Gets the last created failure mechanism section assembly kernel.
        /// </summary>
        public FailureMechanismSectionAssemblyKernelStub LastCreatedFailureMechanismSectionAssemblyKernel { get; }

        /// <summary>
        /// Gets the last created failure mechanism assembly kernel.
        /// </summary>
        public FailureMechanismAssemblyKernelStub LastCreatedFailureMechanismAssemblyKernel { get; }

        public AssessmentSectionAssemblyKernelStub LastCreatedAssessmentSectionAssemblyKernel { get; }

        public ICategoryLimitsCalculator CreateAssemblyGroupsKernel()
        {
            return LastCreatedAssemblyCategoryLimitsKernel;
        }

        public IAssessmentResultsTranslator CreateFailureMechanismSectionAssemblyKernel()
        {
            return LastCreatedFailureMechanismSectionAssemblyKernel;
        }

        public IFailureMechanismResultAssembler CreateFailureMechanismAssemblyKernel()
        {
            return LastCreatedFailureMechanismAssemblyKernel;
        }

        public IAssessmentGradeAssembler CreateAssessmentSectionAssemblyKernel()
        {
            return LastCreatedAssessmentSectionAssemblyKernel;
        }
    }
}