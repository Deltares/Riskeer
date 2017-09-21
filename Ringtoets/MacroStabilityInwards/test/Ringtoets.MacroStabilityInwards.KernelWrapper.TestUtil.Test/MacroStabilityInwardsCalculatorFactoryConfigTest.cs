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

using System;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorFactoryConfigTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculatorFactoryConfig(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("newFactory", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFactory_InstanceTemporarilyChanged()
        {
            // Setup
            IMacroStabilityInwardsCalculatorFactory originalInstance = MacroStabilityInwardsCalculatorFactory.Instance;
            var factory = new TestMacroStabilityInwardsCalculatorFactory();

            // Call
            using (var config = new MacroStabilityInwardsCalculatorFactoryConfig(factory))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(config);
                Assert.AreSame(factory, MacroStabilityInwardsCalculatorFactory.Instance);
            }
            Assert.AreSame(originalInstance, MacroStabilityInwardsCalculatorFactory.Instance);
        }
    }
}