﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Assembly.Kernel.Implementations;
using Assembly.Kernel.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Kernels
{
    [TestFixture]
    public class AssemblyToolKernelFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IAssemblyToolKernelFactory factory = AssemblyToolKernelFactory.Instance;

            // Assert
            Assert.IsInstanceOf<AssemblyToolKernelFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IAssemblyToolKernelFactory firstFactory = AssemblyToolKernelFactory.Instance;
            AssemblyToolKernelFactory.Instance = null;

            // Call
            IAssemblyToolKernelFactory secondFactory = AssemblyToolKernelFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var mocks = new MockRepository();
            var firstFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            AssemblyToolKernelFactory.Instance = firstFactory;

            // Call
            IAssemblyToolKernelFactory secondFactory = AssemblyToolKernelFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssemblyGroupsKernel_Always_ReturnsKernelCategoryLimitsCalculator()
        {
            // Setup
            IAssemblyToolKernelFactory factory = AssemblyToolKernelFactory.Instance;

            // Call
            ICategoryLimitsCalculator assemblyGroupsKernel = factory.CreateAssemblyGroupsKernel();

            // Assert
            Assert.IsInstanceOf<CategoryLimitsCalculator>(assemblyGroupsKernel);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyKernel_Always_ReturnsKernelAssessmentResultsTranslator()
        {
            // Setup
            IAssemblyToolKernelFactory factory = AssemblyToolKernelFactory.Instance;

            // Call
            IAssessmentResultsTranslator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<AssessmentResultsTranslator>(kernel);
        }

        [Test]
        public void CreateFailureMechanismAssemblyKernel_Always_ReturnsKernelFailureMechanismResultAssembler()
        {
            // Setup
            IAssemblyToolKernelFactory factory = AssemblyToolKernelFactory.Instance;

            // Call
            IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<FailureMechanismResultAssembler>(kernel);
        }

        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyKernel_Always_ReturnsKernelCommonFailureMechanismSectionAssembler()
        {
            // Setup
            IAssemblyToolKernelFactory factory = AssemblyToolKernelFactory.Instance;

            // Call
            ICommonFailureMechanismSectionAssembler kernel = factory.CreateCombinedFailureMechanismSectionAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<CommonFailureMechanismSectionAssembler>(kernel);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyKernel_Always_ReturnsKernelAssessmentGradeAssembler()
        {
            // Setup
            IAssemblyToolKernelFactory factory = AssemblyToolKernelFactory.Instance;

            // Call
            IAssessmentGradeAssembler kernel = factory.CreateAssessmentSectionAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<AssessmentGradeAssembler>(kernel);
        }
    }
}