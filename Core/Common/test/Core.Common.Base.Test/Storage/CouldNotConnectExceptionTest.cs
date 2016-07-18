﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Storage;
using NUnit.Framework;

namespace Core.Common.Base.Test.Storage
{
    [TestFixture]
    public class CouldNotConnectExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            string expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(CouldNotConnectException).FullName);

            // Call
            CouldNotConnectException exception = new CouldNotConnectException();

            // Assert
            Assert.IsInstanceOf<StorageException>(exception);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            const string expectedMessage = "Some exception message";

            // Call
            CouldNotConnectException exception = new CouldNotConnectException(expectedMessage);

            // Assert
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
            CouldNotConnectException exception = new CouldNotConnectException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}