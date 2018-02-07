﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assessments;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels
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
            LastCreatedAssemblyCategoriesKernel = new AssemblyCategoriesKernelStub();
            LastCreatedFailureMechanismSectionAssessmentAssemblyKernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();
        }

        /// <summary>
        /// Gets the last created assembly categories kernel.
        /// </summary>
        public AssemblyCategoriesKernelStub LastCreatedAssemblyCategoriesKernel { get; }

        /// <summary>
        /// Gets the last created failure mechanism section assessment assembly kernel.
        /// </summary>
        public FailureMechanismSectionAssessmentAssemblyKernelStub LastCreatedFailureMechanismSectionAssessmentAssemblyKernel { get; }

        public ICategoriesCalculator CreateAssemblyCategoriesKernel()
        {
            return LastCreatedAssemblyCategoriesKernel;
        }

        public IFailureMechanismSectionAssemblyCalculator CreateFailureMechanismSectionAssemblyKernel()
        {
            return LastCreatedFailureMechanismSectionAssessmentAssemblyKernel;
        }
    }
}