// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.TestUtil;
using Core.Common.Utils.Exceptions;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Exceptions
{
    [TestFixture]
    public class InvalidTypeParameterExceptionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var exception = new InvalidTypeParameterException();

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            var expectedMessage = string.Format("Exception of type '{0}' was thrown.", typeof(InvalidTypeParameterException).FullName);
            Assert.AreEqual(expectedMessage, exception.Message);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.TypeParamName);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        [Test]
        public void TypeParameterConstructor_ExpectedValues()
        {
            // Setup
            string typeParamName = "T";

            // Call
            var exception = new InvalidTypeParameterException(typeParamName);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            var expectedMessage = string.Format("Exception of type '{0}' was thrown.", exception.GetType());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(typeParamName, exception.TypeParamName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.TypeParamName, exception.Data["TypeParamName"]);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        [Test]
        public void TypeParameterAndMessageConstructor_ExpectedValues()
        {
            // Setup
            string typeParamName = "T";
            const string messageText = "<insert exception message>";

            // Call
            var exception = new InvalidTypeParameterException(typeParamName, messageText);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            Assert.AreEqual(typeParamName, exception.TypeParamName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.TypeParamName, exception.Data["TypeParamName"]);
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
            var exception = new InvalidTypeParameterException(messageText, innerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            Assert.IsNull(exception.TypeParamName);
            Assert.AreEqual(0, exception.Data.Count);
            Assert.AreEqual(exception.TypeParamName, exception.Data["TypeParamName"]);
            Assert.IsNull(exception.HelpLink);
            Assert.AreEqual(innerException, exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        [Test]
        public void TypeParameterAndMessageAndInnerExceptionConstructor_ExpectedValues()
        {
            // Setup
            var innerException = new Exception();
            string typeParamName = "T";
            const string messageText = "<insert exception message>";

            // Call
            var exception = new InvalidTypeParameterException(typeParamName, messageText, innerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(messageText, exception.Message);
            Assert.AreEqual(typeParamName, exception.TypeParamName);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual(exception.TypeParamName, exception.Data["TypeParamName"]);
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
            var originalException = new InvalidTypeParameterException("<parameter>", "<message>", originalInnerException);

            // Precondition
            Assert.IsNotNull(originalException.InnerException);
            Assert.IsNull(originalException.InnerException.InnerException);

            // Call
            InvalidTypeParameterException persistedException = SerializationTestHelper.SerializeAndDeserializeException(originalException);

            // Assert
            Assert.AreEqual(originalException.Message, persistedException.Message);
            Assert.AreEqual(originalException.TypeParamName, persistedException.TypeParamName);
            Assert.IsNotNull(persistedException.InnerException);
            Assert.AreEqual(originalException.InnerException.GetType(), persistedException.InnerException.GetType());
            Assert.AreEqual(originalException.InnerException.Message, persistedException.InnerException.Message);
            Assert.IsNull(persistedException.InnerException.InnerException);
        }
    }
}