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
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernel;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class TestMacroStabilityInwardsCalculatorFactoryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var factory = new TestMacroStabilityInwardsCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsCalculatorFactory>(factory);
            Assert.IsNotNull(factory.LastCreatedCalculator);
            Assert.IsNull(factory.LastCreatedCalculator.Input);
        }

        [Test]
        public void CreateCalculator_Always_ReturnStubWithInputSet()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsCalculatorFactory();
            var input = new MacroStabilityInwardsCalculatorInput(new MacroStabilityInwardsCalculatorInput.ConstructionProperties());

            // Call
            var calculator = (MacroStabilityInwardsCalculatorStub) factory.CreateCalculator(input, new TestMacroStabilityInwardsKernelFactory());

            // Assert
            Assert.AreSame(input, calculator.Input);
        }
    }
}