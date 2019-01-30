// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels
{
    [TestFixture]
    public class TestAssemblyToolKernelFactoryTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var factory = new TestAssemblyToolKernelFactory();

            // Assert
            Assert.IsInstanceOf<IAssemblyToolKernelFactory>(factory);
            Assert.IsNotNull(factory.LastCreatedAssemblyCategoriesKernel);
            Assert.IsNotNull(factory.LastCreatedFailureMechanismSectionAssemblyKernel);
            Assert.IsNotNull(factory.LastCreatedFailureMechanismAssemblyKernel);
            Assert.IsNotNull(factory.LastCreatedAssessmentSectionAssemblyKernel);
            Assert.IsNotNull(factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel);
        }

        [Test]
        public void CreateAssemblyCategoriesKernel_Always_ReturnLastCreatedAssemblyCategoriesKernel()
        {
            // Setup
            var factory = new TestAssemblyToolKernelFactory();

            // Call
            ICategoryLimitsCalculator kernel = factory.CreateAssemblyCategoriesKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedAssemblyCategoriesKernel, kernel);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyKernel_Always_ReturnLastCreatedFailureMechanismSectionAssemblyKernel()
        {
            // Setup
            var factory = new TestAssemblyToolKernelFactory();

            // Call
            IAssessmentResultsTranslator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedFailureMechanismSectionAssemblyKernel, kernel);
        }

        [Test]
        public void CreateFailureMechanismAssemblyKernel_Always_ReturnLastCreatedFailureMechanismAssemblyKernel()
        {
            // Setup
            var factory = new TestAssemblyToolKernelFactory();

            // Call
            IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedFailureMechanismAssemblyKernel, kernel);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyKernel_Always_ReturnLastCreatedAssessmentSectionAssemblyKernel()
        {
            // Setup
            var factory = new TestAssemblyToolKernelFactory();

            // Call
            IAssessmentGradeAssembler kernel = factory.CreateAssessmentSectionAssemblyKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedAssessmentSectionAssemblyKernel, kernel);
        }

        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyKernel_Always_ReturnLastCreatedCombinedFailureMechanismSectionAssemblyKernel()
        {
            // Setup
            var factory = new TestAssemblyToolKernelFactory();

            // Call
            ICommonFailureMechanismSectionAssembler kernel = factory.CreateCombinedFailureMechanismSectionAssemblyKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel, kernel);
        }
    }
}