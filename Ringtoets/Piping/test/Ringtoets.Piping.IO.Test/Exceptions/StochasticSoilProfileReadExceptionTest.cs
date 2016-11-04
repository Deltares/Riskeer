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
using Ringtoets.Piping.IO.Exceptions;

namespace Ringtoets.Piping.IO.Test.Exceptions
{
    [TestFixture]
    public class StochasticSoilProfileReadExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            string expectedMessage = string.Format("Exception of type '{0}' was thrown.", typeof(StochasticSoilProfileReadException).FullName);

            // Call
            StochasticSoilProfileReadException exception = new StochasticSoilProfileReadException();

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            const string expectedMessage = "Some exception message";

            // Call
            StochasticSoilProfileReadException exception = new StochasticSoilProfileReadException(expectedMessage);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessageAndInnerException_InnerExceptionSetAndMessageSetToCustom()
        {
            // Setup
            const string expectedMessage = "Some exception message";
            Exception expectedInnerException = new Exception();

            // Call
            StochasticSoilProfileReadException exception = new StochasticSoilProfileReadException(expectedMessage, expectedInnerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_SerializationRoundTrip_ExceptionProperlyInitialized()
        {
            // Setup
            var originalInnerException = new Exception("inner");
            var originalException = new StochasticSoilProfileReadException("outer", originalInnerException);

            // Precondition
            Assert.IsNotNull(originalException.InnerException);
            Assert.IsNull(originalException.InnerException.InnerException);

            // Call
            StochasticSoilProfileReadException persistedException = SerializationTestHelper.SerializeAndDeserializeException(originalException);

            // Assert
            Assert.AreEqual(originalException.Message, persistedException.Message);
            Assert.IsNotNull(persistedException.InnerException);
            Assert.AreEqual(originalException.InnerException.GetType(), persistedException.InnerException.GetType());
            Assert.AreEqual(originalException.InnerException.Message, persistedException.InnerException.Message);
            Assert.IsNull(persistedException.InnerException.InnerException);
        }
    }
}