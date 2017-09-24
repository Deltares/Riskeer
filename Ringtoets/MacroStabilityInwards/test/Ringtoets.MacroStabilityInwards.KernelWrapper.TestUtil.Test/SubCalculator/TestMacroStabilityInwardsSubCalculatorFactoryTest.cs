﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class TestMacroStabilityInwardsSubCalculatorFactoryTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var factory = new TestMacroStabilityInwardsSubCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsKernelFactory>(factory);
            Assert.IsNotNull(factory.LastCreatedUpliftVanCalculator);
        }

        [Test]
        public void CreateUpliftVanCalculator_Always_ReturnLastCreatedUpliftVanCalculator()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsSubCalculatorFactory();

            // Call
            IUpliftVanKernel subCalculator = factory.CreateUpliftVanKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedUpliftVanCalculator, subCalculator);
        }
    }
}