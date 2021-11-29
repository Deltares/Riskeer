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

using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators
{
    [TestFixture]
    public class AssemblyToolCalculatorFactoryOldTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IAssemblyToolCalculatorFactoryOld factory = AssemblyToolCalculatorFactoryOld.Instance;

            // Assert
            Assert.IsInstanceOf<AssemblyToolCalculatorFactoryOld>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IAssemblyToolCalculatorFactoryOld firstFactory = AssemblyToolCalculatorFactoryOld.Instance;
            AssemblyToolCalculatorFactoryOld.Instance = null;

            // Call
            IAssemblyToolCalculatorFactoryOld secondFactory = AssemblyToolCalculatorFactoryOld.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestAssemblyToolCalculatorFactoryOld();
            AssemblyToolCalculatorFactoryOld.Instance = firstFactory;

            // Call
            IAssemblyToolCalculatorFactoryOld secondFactory = AssemblyToolCalculatorFactoryOld.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateAssemblyCategoriesCalculator_WithKernelFactory_ReturnsAssemblyCategoriesCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactoryOld factory = AssemblyToolCalculatorFactoryOld.Instance;

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                // Call
                IAssemblyCategoriesCalculator calculator = factory.CreateAssemblyCategoriesCalculator(
                    AssemblyToolKernelFactoryOld.Instance);

                // Assert
                Assert.IsInstanceOf<AssemblyCategoriesCalculator>(calculator);
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCalculator_WithKernelFactory_ReturnsFailureMechanismSectionAssemblyCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactoryOld factory = AssemblyToolCalculatorFactoryOld.Instance;

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                // Call
                IFailureMechanismSectionAssemblyCalculatorOld calculator =
                    factory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorOld>(calculator);
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCalculator_WithKernelFactory_ReturnsFailureMechanismAssemblyCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactoryOld factory = AssemblyToolCalculatorFactoryOld.Instance;

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                // Call
                IFailureMechanismAssemblyCalculator calculator =
                    factory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                // Assert
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculator>(calculator);
            }
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCalculator_WithKernelFactory_ReturnsAssessmentSectionAssemblyCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactoryOld factory = AssemblyToolCalculatorFactoryOld.Instance;

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                // Call
                IAssessmentSectionAssemblyCalculator calculator =
                    factory.CreateAssessmentSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                // Assert
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculator>(calculator);
            }
        }
    }
}