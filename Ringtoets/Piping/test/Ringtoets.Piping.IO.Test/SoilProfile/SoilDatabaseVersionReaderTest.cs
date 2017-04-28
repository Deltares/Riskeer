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

using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilDatabaseVersionReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SoilDatabaseVersionReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseVersionReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   fileName, "bestandspad mag niet leeg of ongedefinieerd zijn.");
            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseVersionReader(fileName)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            const string version = "15.0.6.0";
            string expectedVersionMessage = string.Format(Resources.PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_, version);
            const string dbName = "incorrectversion.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (var versionReader = new SoilDatabaseVersionReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyVersion();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedVersionMessage, exception.Message);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}