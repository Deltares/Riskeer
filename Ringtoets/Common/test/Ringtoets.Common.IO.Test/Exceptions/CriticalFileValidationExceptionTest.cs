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
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;

namespace Ringtoets.Common.IO.Test.Exceptions
{
    [TestFixture]
    public class CriticalFileValidationExceptionTest
    {
        [Test]
        [SetCulture("en-US")]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var exception = new CriticalFileValidationException();

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            var expectedMessage = string.Format("Exception of type '{0}' was thrown.", exception.GetType());
            Assert.AreEqual(expectedMessage, exception.Message);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        [Test]
        public void MessageConstructor_ExpectedValues()
        {
            // Setup
            const string messageText = "<insert exception message>";

            // Call
            var exception = new CriticalFileValidationException(messageText);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        [Test]
        public void MessageAndInnerExceptionConstructor_ExpectedValues()
        {
            // Setup
            var innerException = new Exception();
            const string messageText = "<insert exception message>";

            // Call
            var exception = new CriticalFileValidationException(messageText, innerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.HelpLink);
            Assert.AreSame(innerException, exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }
    }
}