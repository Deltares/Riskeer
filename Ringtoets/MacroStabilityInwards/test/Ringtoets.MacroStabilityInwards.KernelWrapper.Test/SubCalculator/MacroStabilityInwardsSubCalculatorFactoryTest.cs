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
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.SubCalculator
{
    [TestFixture]
    public class MacroStabilityInwardsSubCalculatorFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IMacroStabilityInwardsSubCalculatorFactory factory = MacroStabilityInwardsSubCalculatorFactory.Instance;

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSubCalculatorFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsANewInstance()
        {
            IMacroStabilityInwardsSubCalculatorFactory firstFactory = MacroStabilityInwardsSubCalculatorFactory.Instance;
            MacroStabilityInwardsSubCalculatorFactory.Instance = null;

            // Call
            IMacroStabilityInwardsSubCalculatorFactory secondFactory = MacroStabilityInwardsSubCalculatorFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestMacroStabilityInwardsSubCalculatorFactory();
            MacroStabilityInwardsSubCalculatorFactory.Instance = firstFactory;

            // Call
            IMacroStabilityInwardsSubCalculatorFactory secondFactory = MacroStabilityInwardsSubCalculatorFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }
    }
}