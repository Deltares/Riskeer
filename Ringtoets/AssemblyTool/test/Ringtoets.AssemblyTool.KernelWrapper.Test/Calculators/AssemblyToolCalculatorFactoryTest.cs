// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.CategoryBoundaries;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators
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
            var firstFactory = new TestAssemblyToolCalculatorFactory();
            AssemblyToolCalculatorFactory.Instance = firstFactory;

            // Call
            IAssemblyToolCalculatorFactory secondFactory = AssemblyToolCalculatorFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateAssemblyCategoryBoundariesCalculator_WithCalculatorInput_ReturnsAssemblyCategoryBoundariesCalculator()
        {
            // Setup
            IAssemblyToolCalculatorFactory factory = AssemblyToolCalculatorFactory.Instance;

            // Call
            IAssemblyCategoryBoundariesCalculator calculator = factory.CreateAssemblyCategoryBoundariesCalculator(
                new AssemblyCategoryBoundariesCalculatorInput(0, 0),
                new TestAssemblyToolKernelFactory());

            // Assert
            Assert.IsInstanceOf<AssemblyCategoryBoundariesCalculator>(calculator);
        }
    }
}