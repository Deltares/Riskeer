// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Structures;

namespace Ringtoets.Common.IO.Test.Structures
{
    [TestFixture]
    public class ValidationResultTest
    {
        [Test]
        public void Constructor_ErrorMessagesNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new ValidationResult(true, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("errorMessages", paramName);
        }
        

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_ErrorMessagesContainsNullOrWhiteSpace_ThrowsArgumentException(string errormessage)
        {
            // Setup
            List<string> errorMessages = TestMessages();
            errorMessages.Add(errormessage);

            // Call
            TestDelegate call = () => new ValidationResult(true, errorMessages);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Invalid error message string.");
        }

        [Test]
        public void Constructor_CriticalValidationErrorTrueAndErrorMessagesEmpty_ThrowArgumentException()
        {
            // Setup
            var errorMessages = new List<string>();

            // Call
            TestDelegate test = () => new ValidationResult(true, errorMessages);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "No messages supplied with critical error.");
        }

        [Test]
        public void Constructor_CriticalErrorTrueAndErrorMessages_ValidationWarningFalse()
        {
            // Setup
            List<string> errorMessages = TestMessages();

            // Call
            var result = new ValidationResult(true, errorMessages);

            // Assert
            CollectionAssert.AreEqual(errorMessages, result.ErrorMessages);
            Assert.IsTrue(result.CriticalValidationError);
            Assert.IsFalse(result.ValidationWarning);
        }

        [Test]
        public void Constructor_CriticalerrorFalseAndErrorMessages_ValidationWarningTrue()
        {
            // Setup
            List<string> errorMessages = TestMessages();

            // Call
            var result = new ValidationResult(false, errorMessages);

            // Assert
            CollectionAssert.AreEqual(errorMessages, result.ErrorMessages);
            Assert.IsFalse(result.CriticalValidationError);
            Assert.IsTrue(result.ValidationWarning);
        }

        private List<string> TestMessages()
        {
            return new List<string>
            {
                "Some text."
            };
        }
    }
}