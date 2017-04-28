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
using Ringtoets.Piping.IO.SoilProfile;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilDatabaseConstraintsReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SoilDatabaseConstraintsReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseConstraintsReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            string expectedMessage = $"Fout bij het lezen van bestand '{fileName}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseConstraintsReader(fileName)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void VerifyConstraints_NonUniqueSoilModelNames_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "nonUniqueSoilModelNames.soil");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Namen van ondergrondmodellen zijn niet uniek.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_MissingStochasticSoilModelTable_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilModelTable.soil");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Kan geen ondergrondmodellen lezen. Mogelijk bestaat de 'StochasticSoilModel' tabel niet.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_MissingStochasticSoilProfileTable_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilProfileTable.soil");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Kan geen ondergrondschematisaties lezen. Mogelijk bestaat de 'StochasticSoilProfile' tabel niet.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_MissingStochasticSoilProfileProbability_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilProfileProbability.soil");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Er zijn stochastische ondergrondschematisaties zonder geldige kans van voorkomen.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}