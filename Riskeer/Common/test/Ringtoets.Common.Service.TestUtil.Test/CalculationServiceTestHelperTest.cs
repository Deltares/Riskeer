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

using NUnit.Framework;

namespace Ringtoets.Common.Service.TestUtil.Test
{
    [TestFixture]
    public class CalculationServiceTestHelperTest
    {
        [Test]
        public void AssertValidationStartMessage_MessagesEqual_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertValidationStartMessage("Validatie is gestart.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("alidatie is gestart.")]
        [TestCase("Validatie gestart.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertValidationStartMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertValidationStartMessage(incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertValidationEndMessage_MessagesEqual_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertValidationEndMessage("Validatie is beëindigd.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("alidatie is beëindigd.")]
        [TestCase("Validatie beëindigd.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertValidationEndMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertValidationEndMessage(incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCalculationStartMessage_MessagesEqual_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertCalculationStartMessage("Berekening is gestart.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("erekening is gestart.")]
        [TestCase("Berekening gestart.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationStartMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertCalculationStartMessage(incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCalculationEndMessage_MessagesEqual_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertCalculationEndMessage("Berekening is beëindigd.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("erekening is beëindigd.")]
        [TestCase("Berekening beëindigd.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationEndMessage_MessagesNotEqual_ThrowsAssertionException(string incorrectMessage)
        {
            // Call
            TestDelegate test = () => CalculationServiceTestHelper.AssertCalculationEndMessage(incorrectMessage);

            // Assert
            Assert.Throws<AssertionException>(test);
        }
    }
}