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
using Rhino.Mocks;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators
{
    [TestFixture]
    public class AssemblyToolCalculatorFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IAssemblyToolCalculatorFactory factory = AssemblyToolCalculatorFactory.Instance;

            // Assert
            Assert.IsInstanceOf<AssemblyToolCalculatorFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IAssemblyToolCalculatorFactory firstFactory = AssemblyToolCalculatorFactory.Instance;
            AssemblyToolCalculatorFactory.Instance = null;

            // Call
            IAssemblyToolCalculatorFactory secondFactory = AssemblyToolCalculatorFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolCalculatorFactory>();
            mocks.ReplayAll();

            AssemblyToolCalculatorFactory.Instance = factory;

            // Call
            IAssemblyToolCalculatorFactory secondFactory = AssemblyToolCalculatorFactory.Instance;

            // Assert
            Assert.AreSame(factory, secondFactory);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssemblyGroupLimitsCalculatorCalculator_WithKernelFactory_ReturnsAssemblyCategoriesCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactory factory = AssemblyToolCalculatorFactory.Instance;

            using (new AssemblyToolKernelFactoryConfig())
            {
                // Call
                IAssemblyGroupLimitsCalculator calculator = factory.CreateAssemblyGroupLimitsCalculatorCalculator(
                    AssemblyToolKernelFactory.Instance);

                // Assert
                Assert.IsInstanceOf<AssemblyGroupLimitsCalculator>(calculator);
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCalculator_WithKernelFactory_ReturnsFailureMechanismSectionAssemblyCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactory factory = AssemblyToolCalculatorFactory.Instance;

            using (new AssemblyToolKernelFactoryConfig())
            {
                // Call
                IFailureMechanismSectionAssemblyCalculator calculator =
                    factory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculator>(calculator);
            }
        }

        [Test]
        public void CreateFailurePathAssemblyCalculator_WithKernelFactory_ReturnsFailureMechanismAssemblyCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactory factory = AssemblyToolCalculatorFactory.Instance;

            using (new AssemblyToolKernelFactoryConfig())
            {
                // Call
                IFailurePathAssemblyCalculator calculator =
                    factory.CreateFailurePathAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                // Assert
                Assert.IsInstanceOf<FailurePathAssemblyCalculator>(calculator);
            }
        }
    }
}