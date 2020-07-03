// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators
{
    [TestFixture]
    public class MacroStabilityInwardsKernelMessageTest
    {
        [Test]
        public void Constructor_MessageNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsKernelMessage(MacroStabilityInwardsKernelMessageType.Error, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("message", exception.ParamName);
        }

        [Test]
        public void Constructor_InvalidType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const MacroStabilityInwardsKernelMessageType type = (MacroStabilityInwardsKernelMessageType) 99;

            // Call
            void Call() => new MacroStabilityInwardsKernelMessage(type, "test");

            // Assert
            string expectedMessage = $"The value of argument 'type' ({type}) is invalid for Enum type '{nameof(MacroStabilityInwardsKernelMessageType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("type", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var type = random.NextEnumValue<MacroStabilityInwardsKernelMessageType>();
            const string message = "Error in validation";

            // Call
            var kernelMessage = new MacroStabilityInwardsKernelMessage(type, message);

            // Assert
            Assert.AreEqual(message, kernelMessage.Message);
            Assert.AreEqual(type, kernelMessage.Type);
        }
    }
}