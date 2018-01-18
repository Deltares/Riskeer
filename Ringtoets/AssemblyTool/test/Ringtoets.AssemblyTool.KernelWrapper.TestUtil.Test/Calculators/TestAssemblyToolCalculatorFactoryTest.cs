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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators
{
    [TestFixture]
    public class TestAssemblyToolCalculatorFactoryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var factory = new TestAssemblyToolCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IAssemblyToolCalculatorFactory>(factory);
            Assert.IsNotNull(factory.LastCreatedAssemblyCategoriesCalculator);
            Assert.IsNull(factory.LastCreatedAssemblyCategoriesCalculator.Input);
        }

        [Test]
        public void CreateAssemblyCategoriesCalculator_Always_ReturnStub()
        {
            // Setup
            var factory = new TestAssemblyToolCalculatorFactory();

            // Call
            IAssemblyCategoriesCalculator calculator = factory.CreateAssemblyCategoriesCalculator(null);

            // Assert
            Assert.IsInstanceOf<AssemblyCategoriesCalculatorStub>(calculator);
            Assert.AreSame(factory.LastCreatedAssemblyCategoriesCalculator, calculator);
        }
    }
}