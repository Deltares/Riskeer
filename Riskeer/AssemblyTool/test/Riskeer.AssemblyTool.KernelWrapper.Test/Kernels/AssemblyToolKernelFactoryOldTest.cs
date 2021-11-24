// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Assembly.Kernel.Old.Implementations;
using Assembly.Kernel.Old.Interfaces;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Kernels
{
    [TestFixture]
    public class AssemblyToolKernelFactoryOldTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IAssemblyToolKernelFactoryOld factory = AssemblyToolKernelFactoryOld.Instance;

            // Assert
            Assert.IsInstanceOf<AssemblyToolKernelFactoryOld>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IAssemblyToolKernelFactoryOld firstFactory = AssemblyToolKernelFactoryOld.Instance;
            AssemblyToolKernelFactoryOld.Instance = null;

            // Call
            IAssemblyToolKernelFactoryOld secondFactory = AssemblyToolKernelFactoryOld.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestAssemblyToolKernelFactory();
            AssemblyToolKernelFactoryOld.Instance = firstFactory;

            // Call
            IAssemblyToolKernelFactoryOld secondFactory = AssemblyToolKernelFactoryOld.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateAssemblyCategoriesKernel_Always_ReturnsKernelCategoryLimitsCalculator()
        {
            // Setup
            IAssemblyToolKernelFactoryOld factory = AssemblyToolKernelFactoryOld.Instance;

            // Call
            ICategoryLimitsCalculator assemblyCategoriesKernel = factory.CreateAssemblyCategoriesKernel();

            // Assert
            Assert.IsInstanceOf<CategoryLimitsCalculator>(assemblyCategoriesKernel);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyKernel_Always_ReturnsKernelAssessmentResultsTranslator()
        {
            // Setup
            IAssemblyToolKernelFactoryOld factory = AssemblyToolKernelFactoryOld.Instance;

            // Call
            IAssessmentResultsTranslator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<AssessmentResultsTranslator>(kernel);
        }

        [Test]
        public void CreateFailureMechanismAssemblyKernel_Always_ReturnsKernelFailureMechanismResultAssembler()
        {
            // Setup
            IAssemblyToolKernelFactoryOld factory = AssemblyToolKernelFactoryOld.Instance;

            // Call
            IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<FailureMechanismResultAssembler>(kernel);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyKernel_Always_ReturnsKernelAssessmentGradeAssembler()
        {
            // Setup
            IAssemblyToolKernelFactoryOld factory = AssemblyToolKernelFactoryOld.Instance;

            // Call
            IAssessmentGradeAssembler kernel = factory.CreateAssessmentSectionAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<AssessmentGradeAssembler>(kernel);
        }

        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyKernel_Always_ReturnsKernelCommonFailureMechanismSectionAssembler()
        {
            // Setup
            IAssemblyToolKernelFactoryOld factory = AssemblyToolKernelFactoryOld.Instance;

            // Call
            ICommonFailureMechanismSectionAssembler kernel = factory.CreateCombinedFailureMechanismSectionAssemblyKernel();

            // Assert
            Assert.IsInstanceOf<CommonFailureMechanismSectionAssembler>(kernel);
        }
    }
}