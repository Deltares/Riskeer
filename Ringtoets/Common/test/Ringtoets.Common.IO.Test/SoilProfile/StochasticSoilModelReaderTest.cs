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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(StochasticSoilModelReader));
        private readonly string constraintsReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SoilDatabaseConstraintsReader));
        private readonly string versionReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SoilDatabaseVersionReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilModelReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilModelReader(fileName)) {}
            };

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Validate_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Validate();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Validate_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(versionReaderTestDataPath, "incorrectVersion.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Validate();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                const string version = "15.0.6.0";
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "De database heeft niet de vereiste versie informatie. " +
                    $"Vereiste versie is '{version}'.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Validate_InvalidSchemaThatPassesVersionValidation_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(constraintsReaderTestDataPath, "missingStochasticSoilModelTable.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Validate();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kan geen ondergrondmodellen lezen. Mogelijk bestaat de 'StochasticSoilModel' tabel niet.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Validate_NonUniqueSoilModelNames_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(constraintsReaderTestDataPath, "nonUniqueSoilModelNames.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Validate();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Namen van ondergrondmodellen zijn niet uniek.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                bool hasNext = reader.HasNext;

                // Assert
                Assert.IsFalse(hasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_ValidDatabase_ReturnsTrue()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                bool hasNext = reader.HasNext;

                // Assert
                Assert.IsTrue(hasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_EmptyDatabase_ReturnsNull()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.IsNull(model);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_InvalidSegmentPoint_ThrowsStochasticSoilModelException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "invalidSegmentPoint.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                TestDelegate test = () => reader.ReadStochasticSoilModel();

                // Assert
                var exception = Assert.Throws<StochasticSoilModelException>(test);

                const string expectedMessage = "Het stochastische ondergrondmodel 'StochasticSoilModelName' moet een geometrie bevatten.";
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_SoilModelWithoutStochasticSoilProfile_ThrowsStochasticSoilModelExceptionAndHasNext()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWithoutProfile.soil");

            StochasticSoilModelException expectedException = null;
            var readModels = new List<StochasticSoilModel>();
            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                while (reader.HasNext)
                {
                    try
                    {
                        // Call
                        readModels.Add(reader.ReadStochasticSoilModel());
                    }
                    catch (StochasticSoilModelException e)
                    {
                        expectedException = e;
                        reader.MoveNext();
                    }
                }

                Assert.IsFalse(reader.HasNext);
            }

            // Assert
            var expectedSegmentAndModelNames = new[]
            {
                "36005_Stability",
                "36005_Piping",
                "36006_Stability",
                "36007_Stability",
                "36007_Piping"
            };
            var expectedSegmentPointCount = new[]
            {
                1797,
                1797,
                144,
                606,
                606
            };
            var expectedProfileCount = new[]
            {
                10,
                10,
                6,
                8,
                8
            };

            CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.Name));
            CollectionAssert.AreEqual(expectedSegmentPointCount, readModels.Select(m => m.Geometry.Count));
            CollectionAssert.AreEqual(expectedProfileCount, readModels.Select(m => m.StochasticSoilProfiles.Count));

            Assert.IsInstanceOf<StochasticSoilModelException>(expectedException);
            Assert.AreEqual("Er zijn geen ondergrondschematisaties gevonden in het stochastische ondergrondmodel '36006_Piping'",
                            expectedException.Message);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_CompleteDatabase_SixModelsWithProfiles()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.soil");

            var readModels = new List<StochasticSoilModel>();
            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                while (reader.HasNext)
                {
                    // Call
                    readModels.Add(reader.ReadStochasticSoilModel());
                }

                // Assert
                Assert.IsFalse(reader.HasNext);
            }

            var expectedSegmentAndModelNames = new[]
            {
                "36005_Stability",
                "36005_Piping",
                "36006_Stability",
                "36006_Piping",
                "36007_Stability",
                "36007_Piping"
            };
            var expectedSegmentPointCount = new[]
            {
                1797,
                1797,
                144,
                144,
                606,
                606
            };

            var expected1DProfileCount = new[]
            {
                0,
                10,
                6,
                6,
                8,
                8
            };

            var expected2DProfileCount = new[]
            {
                10,
                0,
                0,
                0,
                0,
                0
            };

            Assert.AreEqual(expectedSegmentAndModelNames.Length, readModels.Count);
            CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.Name));
            CollectionAssert.AreEqual(expectedSegmentPointCount, readModels.Select(m => m.Geometry.Count));
            CollectionAssert.AreEqual(expected1DProfileCount, readModels.Select(m => m.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile).OfType<SoilProfile1D>().Count()));
            CollectionAssert.AreEqual(expected2DProfileCount, readModels.Select(m => m.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile).OfType<SoilProfile2D>().Count()));

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}