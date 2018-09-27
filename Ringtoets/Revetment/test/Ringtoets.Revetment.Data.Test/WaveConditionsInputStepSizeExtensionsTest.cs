// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputStepSizeExtensionsTest
    {
        [Test]
        [TestCase(WaveConditionsInputStepSize.Half, 0.5)]
        [TestCase(WaveConditionsInputStepSize.One, 1.0)]
        [TestCase(WaveConditionsInputStepSize.Two, 2.0)]
        public void AsValue_Half_ReturnHalfValue(WaveConditionsInputStepSize stepSize, double expectedValue)
        {
            // Call
            double result = stepSize.AsValue();

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void AsValue_InvalidValue_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 4;

            // Call
            TestDelegate call = () => ((WaveConditionsInputStepSize) invalidValue).AsValue();

            // Assert
            string expectedMessage = $"The value of argument 'stepSize' ({invalidValue}) is invalid for Enum type '{nameof(WaveConditionsInputStepSize)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("stepSize", parameterName);
        }
    }
}