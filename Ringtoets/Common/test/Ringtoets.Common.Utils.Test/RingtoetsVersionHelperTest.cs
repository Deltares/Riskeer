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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Common.Utils.Test
{
    [TestFixture]
    public class RingtoetsVersionHelperTest
    {
        [Test]
        public void GetCurrentDatabaseversion_ReturnsExpectedValue()
        {
            // Call
            string currentVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Assert
            Assert.AreEqual("17.1", currentVersion);
        }

        [Test]
        [TestCase("9000.0")]
        [TestCase("17.2")]
        public void IsNewerThanCurrentString_NewerVersion_ReturnsTrue(string newerVersion)
        {
            // Call
            bool isNewer = RingtoetsVersionHelper.IsNewerThanCurrent(newerVersion);

            // Assert
            Assert.IsTrue(isNewer);
        }

        [Test]
        public void IsNewerThanCurrentString_SameVersion_ReturnsFalse()
        {
            // Setup
            string currentVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Call
            bool isNewer = RingtoetsVersionHelper.IsNewerThanCurrent(currentVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCase("5")]
        [TestCase("17.0")]
        public void IsNewerThanCurrentString_OlderVersion_ReturnsFalse(string olderVersion)
        {
            // Call
            bool isNewer = RingtoetsVersionHelper.IsNewerThanCurrent(olderVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(InValidVersions))]
        public void IsNewerThanCurrentString_InvalidVersion_ReturnsFalse(string invalidVersion)
        {
            // Call
            bool isNewer = RingtoetsVersionHelper.IsNewerThanCurrent(invalidVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(ValidVersions))]
        public void IsValidVersion_ValidVersion_ReturnsTrue(string validVersion)
        {
            // Call
            bool isNewer = RingtoetsVersionHelper.IsValidVersion(validVersion);

            // Assert
            Assert.IsTrue(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(InValidVersions))]
        public void IsValidVersion_InvalidVersion_ReturnsFalse(string invalidVersion)
        {
            // Call
            bool isNewer = RingtoetsVersionHelper.IsValidVersion(invalidVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(ValidVersions))]
        public void ValidateVersion_ValidVersion_DoesNotThrowException(string validVersion)
        {
            // Call
            TestDelegate call = () => RingtoetsVersionHelper.ValidateVersion(validVersion);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource(nameof(InValidVersions))]
        public void ValidateVersion_InValidVersion_ThrowArgumentException(string validVersion)
        {
            // Call
            TestDelegate call = () => RingtoetsVersionHelper.ValidateVersion(validVersion);

            // Assert
            string expectedMessage = $@"'{validVersion}' is geen geldige Ringtoets versie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        private static TestCaseData[] ValidVersions()
        {
            return new[]
            {
                new TestCaseData("5"),
                new TestCaseData("17.1")
            };
        }

        private static TestCaseData[] InValidVersions()
        {
            return new[]
            {
                new TestCaseData("4"),
                new TestCaseData("5a"),
                new TestCaseData("..")
            };
        }
    }
}