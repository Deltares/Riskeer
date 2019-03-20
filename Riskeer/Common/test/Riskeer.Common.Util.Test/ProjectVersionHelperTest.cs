// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class ProjectVersionHelperTest
    {
        [Test]
        public void GetCurrentDatabaseVersion_ReturnsExpectedValue()
        {
            // Call
            string currentVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();

            // Assert
            Assert.AreEqual("19.2", currentVersion);
        }

        [Test]
        [TestCase("9000.0")]
        [TestCase("19.3")]
        public void IsNewerThanCurrentString_NewerVersion_ReturnsTrue(string newerVersion)
        {
            // Call
            bool isNewer = ProjectVersionHelper.IsNewerThanCurrent(newerVersion);

            // Assert
            Assert.IsTrue(isNewer);
        }

        [Test]
        public void IsNewerThanCurrentString_SameVersion_ReturnsFalse()
        {
            // Setup
            string currentVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();

            // Call
            bool isNewer = ProjectVersionHelper.IsNewerThanCurrent(currentVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCase("5")]
        [TestCase("18.1")]
        public void IsNewerThanCurrentString_OlderVersion_ReturnsFalse(string olderVersion)
        {
            // Call
            bool isNewer = ProjectVersionHelper.IsNewerThanCurrent(olderVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(InvalidVersions))]
        public void IsNewerThanCurrentString_InvalidVersion_ReturnsFalse(string invalidVersion)
        {
            // Call
            bool isNewer = ProjectVersionHelper.IsNewerThanCurrent(invalidVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(ValidVersions))]
        public void IsValidVersion_ValidVersion_ReturnsTrue(string validVersion)
        {
            // Call
            bool isNewer = ProjectVersionHelper.IsValidVersion(validVersion);

            // Assert
            Assert.IsTrue(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(InvalidVersions))]
        public void IsValidVersion_InvalidVersion_ReturnsFalse(string invalidVersion)
        {
            // Call
            bool isNewer = ProjectVersionHelper.IsValidVersion(invalidVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCaseSource(nameof(ValidVersions))]
        public void ValidateVersion_ValidVersion_DoesNotThrowException(string validVersion)
        {
            // Call
            TestDelegate call = () => ProjectVersionHelper.ValidateVersion(validVersion);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource(nameof(InvalidVersions))]
        public void ValidateVersion_InvalidVersion_ThrowArgumentException(string validVersion)
        {
            // Call
            TestDelegate call = () => ProjectVersionHelper.ValidateVersion(validVersion);

            // Assert
            string expectedMessage = $@"'{validVersion}' is geen geldige Riskeer of Ringtoets projectbestand versie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        private static TestCaseData[] ValidVersions()
        {
            return new[]
            {
                new TestCaseData("5"),
                new TestCaseData("17.1"),
                new TestCaseData("17.2"),
                new TestCaseData("17.3"),
                new TestCaseData("18.1"),
                new TestCaseData("19.1"),
                new TestCaseData("19.2")
            };
        }

        private static TestCaseData[] InvalidVersions()
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