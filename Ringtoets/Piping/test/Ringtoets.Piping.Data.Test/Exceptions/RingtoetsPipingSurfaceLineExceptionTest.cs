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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives.Exceptions;

namespace Ringtoets.Piping.Data.Test.Exceptions
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineExceptionExceptionTest
    {
        [Test]
        [SetCulture("en-US")]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var exception = new RingtoetsPipingSurfaceLineException();

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
            var exception = new RingtoetsPipingSurfaceLineException(messageText);

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
            var exception = new RingtoetsPipingSurfaceLineException(messageText, innerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.HelpLink);
            Assert.AreEqual(innerException, exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        [Test]
        public void Constructor_SerializationRoundTrip_ExceptionProperlyInitialized()
        {
            // Setup
            var originalInnerException = new Exception("inner");
            var originalException = new RingtoetsPipingSurfaceLineException("outer", originalInnerException);

            // Precondition
            Assert.IsNotNull(originalException.InnerException);
            Assert.IsNull(originalException.InnerException.InnerException);

            // Call
            RingtoetsPipingSurfaceLineException persistedException = SerializationTestHelper.SerializeAndDeserializeException(originalException);

            // Assert
            Assert.AreEqual(originalException.Message, persistedException.Message);
            Assert.IsNotNull(persistedException.InnerException);
            Assert.AreEqual(originalException.InnerException.GetType(), persistedException.InnerException.GetType());
            Assert.AreEqual(originalException.InnerException.Message, persistedException.InnerException.Message);
            Assert.IsNull(persistedException.InnerException.InnerException);
        }
    }
}