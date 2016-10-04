﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class ValidationResultTest
    {
        List<string> TestMessages()
        {
            return new List<string>
            {
                "Some text."
            };
        }

        [Test]
        public void Constructor_ErrorMessagesNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new ValidationResult(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "errorMessages");
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
            TestDelegate call = () => new ValidationResult(errorMessages);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "errorMessages");
        }

        [Test]
        public void Constructor_ErrorMessagesEmpty_ExpectedValues()
        {
            // Setup
            List<string> errorMessages = new List<string>();

            // Call
            var validationResult = new ValidationResult(errorMessages);

            // Assert
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
        }

        [Test]
        public void Constructor_ErrorMessages_ExpectedValues()
        {
            // Setup
            List<string> errorMessages = TestMessages();

            // Call
            var validationResult = new ValidationResult(errorMessages);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            CollectionAssert.AreEqual(errorMessages, validationResult.ErrorMessages);
        }
    }
}