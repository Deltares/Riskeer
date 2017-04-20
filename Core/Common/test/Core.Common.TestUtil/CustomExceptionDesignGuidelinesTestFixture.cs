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
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Test fixture that asserts that a custom exception follows the design guidelines
    /// specified at https://msdn.microsoft.com/en-us/library/ms229064(v=vs.100).aspx.
    /// </summary>
    [TestFixture]
    public abstract class CustomExceptionDesignGuidelinesTestFixture<TCustomExceptionType, TBaseExceptionType> where TCustomExceptionType : Exception
                                                                                                               where TBaseExceptionType : Exception
    {
        [Test]
        [SetCulture("en-US")]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            TCustomExceptionType exception = CallDefaultConstructor();

            // Assert
            AssertDefaultConstructedInstance(exception);
        }

        [Test]
        public void MessageConstructor_ExpectedValues()
        {
            // Setup
            const string messageText = "<insert exception message>";

            // Call
            TCustomExceptionType exception = CallMessageConstructor(messageText);

            // Assert
            AssertMessageConstructedInstance(exception, messageText);
        }

        [Test]
        public void MessageAndInnerExceptionConstructor_ExpectedValues()
        {
            // Setup
            var innerException = new Exception();
            const string messageText = "<insert exception message>";

            // Call
            TCustomExceptionType exception = CallMessageAndInnerExceptionConstructor(messageText, innerException);

            // Assert
            AssertMessageAndInnerExceptionConstructedInstance(exception, messageText, innerException);
        }

        [Test]
        public void Constructor_SerializationRoundTrip_ExceptionProperlyInitialized()
        {
            // Setup
            TCustomExceptionType originalException = CreateFullyConfiguredException();

            // Precondition
            Assert.IsNotNull(originalException.InnerException);
            Assert.IsNull(originalException.InnerException.InnerException);

            // Call
            TCustomExceptionType persistedException = SerializationTestHelper.SerializeAndDeserializeException(originalException);

            // Assert
            AssertRoundTripResult(originalException, persistedException);
        }

        protected virtual void AssertDefaultConstructedInstance(TCustomExceptionType exception)
        {
            Assert.IsInstanceOf<TBaseExceptionType>(exception);
            string expectedMessage = $"Exception of type '{typeof(TCustomExceptionType)}' was thrown.";
            Assert.AreEqual(expectedMessage, exception.Message);
            CollectionAssert.IsEmpty(exception.Data);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);
        }

        protected virtual void AssertMessageConstructedInstance(TCustomExceptionType exception, string messageText,
                                                                bool assertData = true)
        {
            Assert.IsInstanceOf<TBaseExceptionType>(exception);
            Assert.AreEqual(messageText, exception.Message);
            Assert.IsNull(exception.HelpLink);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);

            if (assertData)
            {
                CollectionAssert.IsEmpty(exception.Data);
            }
        }

        protected virtual void AssertMessageAndInnerExceptionConstructedInstance(TCustomExceptionType exception, string messageText,
                                                                                 Exception innerException, bool assertData = true)
        {
            Assert.IsInstanceOf<TBaseExceptionType>(exception);
            Assert.AreEqual(messageText, exception.Message);
            Assert.IsNull(exception.HelpLink);
            Assert.AreEqual(innerException, exception.InnerException);
            Assert.IsNull(exception.Source);
            Assert.IsNull(exception.StackTrace);
            Assert.IsNull(exception.TargetSite);

            if (assertData)
            {
                CollectionAssert.IsEmpty(exception.Data);
            }
        }

        protected virtual void AssertRoundTripResult(TCustomExceptionType originalException, TCustomExceptionType persistedException)
        {
            Assert.AreEqual(originalException.Message, persistedException.Message);
            Assert.IsNotNull(persistedException.InnerException);
            Assert.AreEqual(originalException.InnerException.GetType(), persistedException.InnerException.GetType());
            Assert.AreEqual(originalException.InnerException.Message, persistedException.InnerException.Message);
            Assert.IsNull(persistedException.InnerException.InnerException);
        }

        protected virtual TCustomExceptionType CreateFullyConfiguredException()
        {
            var originalInnerException = new Exception("inner");
            return CallMessageAndInnerExceptionConstructor("outer", originalInnerException);
        }

        private static TCustomExceptionType CallDefaultConstructor()
        {
            return (TCustomExceptionType) Activator.CreateInstance(typeof(TCustomExceptionType));
        }

        private static TCustomExceptionType CallMessageConstructor(string message)
        {
            return (TCustomExceptionType) Activator.CreateInstance(typeof(TCustomExceptionType), message);
        }

        private static TCustomExceptionType CallMessageAndInnerExceptionConstructor(string message, Exception innerException)
        {
            return (TCustomExceptionType) Activator.CreateInstance(typeof(TCustomExceptionType), message, innerException);
        }
    }
}