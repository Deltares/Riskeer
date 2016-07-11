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
using Ringtoets.Piping.IO.Exceptions;

namespace Ringtoets.Piping.IO.Test.Exceptions
{
    [TestFixture]
    public class PipingSoilProfileReadExceptionTest
    {
        [Test]
        public void Constructor_WithProfileName_InnerExceptionNullMessageDefaultAndProfileNameSet()
        {
            // Setup
            var profileName = "name";
            var expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(PipingSoilProfileReadException).FullName);

            // Call
            var exception = new PipingSoilProfileReadException(profileName);

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            var profileName = "name";
            var expectedMessage = "Some exception message";

            // Call
            var exception = new PipingSoilProfileReadException(profileName, expectedMessage);

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessageAndInnerException_InnerExceptionSetAndMessageSetToCustom()
        {
            // Setup
            var profileName = "name";
            var expectedMessage = "Some exception message";
            var expectedInnerException = new Exception();

            // Call
            var exception = new PipingSoilProfileReadException(profileName, expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}