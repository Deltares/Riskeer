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

namespace Ringtoets.Common.Service.TestUtil.Test
{
    [TestFixture]
    public class CalculationServiceTestHelperTest
    {
        [Test]
        [TestCase("aab")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertValidationStartMessage_MessagesEqual_DoesNotThrow(string calculationName)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertValidationStartMessage(calculationName, $"Validatie van '{calculationName}' is gestart.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("alidatie van '' is gestart.")]
        [TestCase("Validatie an '' is gestart.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertValidationStartMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertValidationStartMessage(string.Empty, incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("aab")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertValidationEndMessage_MessagesEqual_DoesNotThrow(string calculationName)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertValidationEndMessage(calculationName, $"Validatie van '{calculationName}' is beëindigd.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("alidatie van '' is beëindigd.")]
        [TestCase("Validatie an '' is beëindigd.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertValidationEndMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertValidationEndMessage(string.Empty, incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }
        [Test]
        [TestCase("aab")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationStartMessage_MessagesEqual_DoesNotThrow(string calculationName)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertCalculationStartMessage(calculationName, $"Berekening van '{calculationName}' is gestart.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("erekening van '' is gestart.")]
        [TestCase("Berekening an '' is gestart.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationStartMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertCalculationStartMessage(string.Empty, incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }
        [Test]
        [TestCase("aab")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationEndMessage_MessagesEqual_DoesNotThrow(string calculationName)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertCalculationEndMessage(calculationName, $"Berekening van '{calculationName}' is beëindigd.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("erekening van '' is beëindigd.")]
        [TestCase("Berekening an '' is beëindigd.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationEndMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => 
                CalculationServiceTestHelper.AssertCalculationEndMessage(string.Empty, incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }
    }
}