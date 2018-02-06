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

using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assessments;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assessments;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators
{
    /// <summary>
    /// Factory which creates assembly tool calculator stubs for testing purposes.
    /// </summary>
    public class TestAssemblyToolCalculatorFactory : IAssemblyToolCalculatorFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestAssemblyToolCalculatorFactory"/>.
        /// </summary>
        public TestAssemblyToolCalculatorFactory()
        {
            LastCreatedAssemblyCategoriesCalculator = new AssemblyCategoriesCalculatorStub();
            LastCreatedFailureMechanismSectionAssessmentAssemblyCalculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub();
        }

        /// <summary>
        /// Gets the last created <see cref="AssemblyCategoriesCalculatorStub"/>.
        /// </summary>
        public AssemblyCategoriesCalculatorStub LastCreatedAssemblyCategoriesCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="FailureMechanismSectionAssessmentAssemblyCalculatorStub"/>.
        /// </summary>
        public FailureMechanismSectionAssessmentAssemblyCalculatorStub LastCreatedFailureMechanismSectionAssessmentAssemblyCalculator { get; }

        public IAssemblyCategoriesCalculator CreateAssemblyCategoriesCalculator(IAssemblyToolKernelFactory factory)
        {
            return LastCreatedAssemblyCategoriesCalculator;
        }

        public IFailureMechanismSectionAssessmentAssemblyCalculator CreateFailureMechanismSectionAssessmentAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            return LastCreatedFailureMechanismSectionAssessmentAssemblyCalculator;
        }
    }
}