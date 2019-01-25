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
    public class TestAssemblyToolCalculatorFactory : IAssemblyToolCalculatorFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestAssemblyToolCalculatorFactory"/>.
        /// </summary>
        public TestAssemblyToolCalculatorFactory()
        {
            LastCreatedAssemblyCategoriesCalculator = new AssemblyCategoriesCalculatorStub();
            LastCreatedFailureMechanismSectionAssemblyCalculator = new FailureMechanismSectionAssemblyCalculatorStub();
            LastCreatedFailureMechanismAssemblyCalculator = new FailureMechanismAssemblyCalculatorStub();
            LastCreatedAssessmentSectionAssemblyCalculator = new AssessmentSectionAssemblyCalculatorStub();
        }

        /// <summary>
        /// Gets the last created <see cref="AssemblyCategoriesCalculatorStub"/>.
        /// </summary>
        public AssemblyCategoriesCalculatorStub LastCreatedAssemblyCategoriesCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="FailureMechanismSectionAssemblyCalculatorStub"/>.
        /// </summary>
        public FailureMechanismSectionAssemblyCalculatorStub LastCreatedFailureMechanismSectionAssemblyCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="FailureMechanismAssemblyCalculatorStub"/>.
        /// </summary>
        public FailureMechanismAssemblyCalculatorStub LastCreatedFailureMechanismAssemblyCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="AssessmentSectionAssemblyCalculatorStub"/>.
        /// </summary>
        public AssessmentSectionAssemblyCalculatorStub LastCreatedAssessmentSectionAssemblyCalculator { get; }

        public IAssemblyCategoriesCalculator CreateAssemblyCategoriesCalculator(IAssemblyToolKernelFactory factory)
        {
            return LastCreatedAssemblyCategoriesCalculator;
        }

        public IFailureMechanismSectionAssemblyCalculator CreateFailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            return LastCreatedFailureMechanismSectionAssemblyCalculator;
        }

        public IFailureMechanismAssemblyCalculator CreateFailureMechanismAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            return LastCreatedFailureMechanismAssemblyCalculator;
        }

        public IAssessmentSectionAssemblyCalculator CreateAssessmentSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            return LastCreatedAssessmentSectionAssemblyCalculator;
        }
    }
}