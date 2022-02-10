﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators
{
    /// <summary>
    /// Factory which creates assembly tool calculator stubs for testing purposes.
    /// </summary>
    public class TestAssemblyToolCalculatorFactoryOld : IAssemblyToolCalculatorFactoryOld
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestAssemblyToolCalculatorFactoryOld"/>.
        /// </summary>
        public TestAssemblyToolCalculatorFactoryOld()
        {
            LastCreatedAssemblyCategoriesCalculator = new AssemblyCategoriesCalculatorStub();
            LastCreatedFailureMechanismSectionAssemblyCalculator = new FailureMechanismSectionAssemblyCalculatorOldStub();
            LastCreatedFailureMechanismAssemblyCalculator = new FailureMechanismAssemblyCalculatorOldStub();
            LastCreatedAssessmentSectionAssemblyCalculator = new AssessmentSectionAssemblyCalculatorStub();
        }

        /// <summary>
        /// Gets the last created <see cref="AssemblyCategoriesCalculatorStub"/>.
        /// </summary>
        public AssemblyCategoriesCalculatorStub LastCreatedAssemblyCategoriesCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="FailureMechanismSectionAssemblyCalculatorOldStub"/>.
        /// </summary>
        public FailureMechanismSectionAssemblyCalculatorOldStub LastCreatedFailureMechanismSectionAssemblyCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="FailureMechanismAssemblyCalculatorOldStub"/>.
        /// </summary>
        public FailureMechanismAssemblyCalculatorOldStub LastCreatedFailureMechanismAssemblyCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="AssessmentSectionAssemblyCalculatorStub"/>.
        /// </summary>
        public AssessmentSectionAssemblyCalculatorStub LastCreatedAssessmentSectionAssemblyCalculator { get; }

        public IAssemblyCategoriesCalculator CreateAssemblyCategoriesCalculator(IAssemblyToolKernelFactoryOld factory)
        {
            return LastCreatedAssemblyCategoriesCalculator;
        }

        public IFailureMechanismSectionAssemblyCalculatorOld CreateFailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactoryOld factory)
        {
            return LastCreatedFailureMechanismSectionAssemblyCalculator;
        }

        public IFailureMechanismAssemblyCalculatorOld CreateFailureMechanismAssemblyCalculator(IAssemblyToolKernelFactoryOld factory)
        {
            return LastCreatedFailureMechanismAssemblyCalculator;
        }

        public IAssessmentSectionAssemblyCalculatorOld CreateAssessmentSectionAssemblyCalculator(IAssemblyToolKernelFactoryOld factory)
        {
            return LastCreatedAssessmentSectionAssemblyCalculator;
        }
    }
}