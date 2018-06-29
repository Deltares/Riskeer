// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Application.Ringtoets.Migration.Core.Test
{
    [TestFixture]
    public class MigrationLogMessageTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_FromVersionNullOrEmpty_ThrowsArgumentException(string fromVersion)
        {
            // Setup
            const string toVersion = "toVersion";
            const string message = "message";

            // Call
            TestDelegate test = () => new MigrationLogMessage(fromVersion, toVersion, message);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             test,
                                             @"Parameter 'fromVersion' must contain a value")
                                         .ParamName;
            Assert.AreEqual("fromVersion", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_ToVersionNullOrEmpty_ThrowsArgumentException(string toVersion)
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string message = "message";

            // Call
            TestDelegate test = () => new MigrationLogMessage(fromVersion, toVersion, message);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             test,
                                             @"Parameter 'toVersion' must contain a value")
                                         .ParamName;
            Assert.AreEqual("toVersion", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_MessageNullOrEmpty_ThrowsArgumentException(string message)
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";

            // Call
            TestDelegate test = () => new MigrationLogMessage(fromVersion, toVersion, null);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                                             test,
                                             @"Parameter 'message' must contain a value")
                                         .ParamName;
            Assert.AreEqual("message", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedPropertiesSet()
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";
            const string message = "message";

            // Call
            var migrationLogMessage = new MigrationLogMessage(fromVersion, toVersion, message);

            // Assert
            Assert.AreEqual(fromVersion, migrationLogMessage.FromVersion);
            Assert.AreEqual(toVersion, migrationLogMessage.ToVersion);
            Assert.AreEqual(message, migrationLogMessage.Message);
        }
    }
}