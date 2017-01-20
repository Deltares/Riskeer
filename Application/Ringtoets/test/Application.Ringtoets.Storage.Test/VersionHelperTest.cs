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

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class VersionHelperTest
    {
        [Test]
        public void GetCurrentDatabaseversion_ReturnsExpectedValue()
        {
            // Call
            string currentVersion = VersionHelper.GetCurrentDatabaseVersion();

            // Assert
            Assert.AreEqual("17.1", currentVersion);
        }

        [Test]
        [TestCase("9000.0")]
        [TestCase("17.2")]
        public void IsNewerThanCurrentString_NewerVersion_ReturnsTrue(string newerVersion)
        {
            // Call
            bool isNewer = VersionHelper.IsNewerThanCurrent(newerVersion);

            // Assert
            Assert.IsTrue(isNewer);
        }

        [Test]
        public void IsNewerThanCurrentString_SameVersion_ReturnsFalse()
        {
            // Setup
            string currentVersion = VersionHelper.GetCurrentDatabaseVersion();

            // Call
            bool isNewer = VersionHelper.IsNewerThanCurrent(currentVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCase("4")]
        [TestCase("17.0")]
        public void IsNewerThanCurrentString_OlderVersion_ReturnsFalse(string olderVersion)
        {
            // Call
            bool isNewer = VersionHelper.IsNewerThanCurrent(olderVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCase("4a.0")]
        [TestCase("17..")]
        public void IsNewerThanCurrentString_InvalidVersion_ReturnsFalse(string invalidVersion)
        {
            // Call
            bool isNewer = VersionHelper.IsNewerThanCurrent(invalidVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }

        [Test]
        [TestCase("4")]
        [TestCase("17")]
        public void IsValidVersion_ValidVersion_ReturnsTrue(string validVersion)
        {
            bool isNewer = VersionHelper.IsValidVersion(validVersion);

            // Assert
            Assert.IsTrue(isNewer);
        }

        [Test]
        [TestCase("3")]
        [TestCase("..")]
        public void IsValidVersion_InvalidVersion_ReturnsFalse(string invalidVersion)
        {
            bool isNewer = VersionHelper.IsValidVersion(invalidVersion);

            // Assert
            Assert.IsFalse(isNewer);
        }
    }
}