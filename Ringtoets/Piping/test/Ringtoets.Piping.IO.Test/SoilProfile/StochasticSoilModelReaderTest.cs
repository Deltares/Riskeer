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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "StochasticSoilModelDatabaseReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(testFile)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            var expectedMessage = $"Fout bij het lezen van bestand '{fileName}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(fileName)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build(string.Format(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column, dbName));

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(dbFile)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_InvalidSchemaThatPassesVersionValidation_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "withoutSoilModelTables.soil");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Kan geen ondergrondmodellen lezen. Mogelijk bestaat de 'StochasticSoilModel' tabel niet.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(dbFile)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_NonUniqueSoilModelNames_ThrowsCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO.Path, "SoilDatabaseConstraintsReader"));
            string dbFile = Path.Combine(path, "nonUniqueSoilModelNames.soil");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Namen van ondergrondmodellen zijn niet uniek.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(dbFile)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ParameteredConstructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Call
            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, stochasticSoilModelDatabaseReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(stochasticSoilModelDatabaseReader);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
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

            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(dbFile)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedVersionMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                bool isPrepared = stochasticSoilModelDatabaseReader.HasNext;

                // Assert
                Assert.IsFalse(isPrepared);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_CompleteDatabase_ReturnsTrue()
        {
            // Setup
            string dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                bool hasNext = stochasticSoilModelDatabaseReader.HasNext;

                // Assert
                Assert.IsTrue(hasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_InvalidSegmentPoint_ThrowsStochasticSoilModelReadException()
        {
            // Setup
            string dbName = "invalidSegmentPoint.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                .Build(Resources.StochasticSoilProfileDatabaseReader_StochasticSoilProfile_has_invalid_value);

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => stochasticSoilModelDatabaseReader.ReadStochasticSoilModel();

                // Assert
                CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_EmptyDatabase_ReturnsNull()
        {
            // Setup
            string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            const int expectedNrOfModels = 0;

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                int nrOfModels = stochasticSoilModelDatabaseReader.PipingStochasticSoilModelCount;

                // Call
                StochasticSoilModel stochasticSoilModel = stochasticSoilModelDatabaseReader.ReadStochasticSoilModel();

                // Assert
                Assert.IsNull(stochasticSoilModel);
                Assert.AreEqual(expectedNrOfModels, nrOfModels);
                Assert.IsFalse(stochasticSoilModelDatabaseReader.HasNext);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_ModelWithoutProfile_ThreeModelsWithSecondWithoutProfiles()
        {
            // Setup
            string dbName = "modelWithoutProfile.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            const int expectedNrOfModels = 3;

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                int nrOfModels = stochasticSoilModelDatabaseReader.PipingStochasticSoilModelCount;
                var readModels = new List<StochasticSoilModel>();
                while (stochasticSoilModelDatabaseReader.HasNext)
                {
                    // Call
                    readModels.Add(stochasticSoilModelDatabaseReader.ReadStochasticSoilModel());
                }

                // Assert
                var expectedSegmentAndModelNames = new[]
                {
                    "36005_Piping",
                    "36006_Piping",
                    "36007_Piping"
                };
                var expectedModelIds = new[]
                {
                    2,
                    4,
                    6
                };
                var expectedSegmentPointCount = new[]
                {
                    1797,
                    144,
                    606
                };
                var expectedProfileCount = new[]
                {
                    10,
                    0,
                    8
                };
                Assert.AreEqual(expectedNrOfModels, nrOfModels);

                CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.SegmentName));
                CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.Name));
                CollectionAssert.AreEqual(expectedModelIds, readModels.Select(m => m.Id));
                CollectionAssert.AreEqual(expectedSegmentPointCount, readModels.Select(m => m.Geometry.Count));
                CollectionAssert.AreEqual(expectedProfileCount, readModels.Select(m => m.StochasticSoilProfiles.Count));

                Assert.IsFalse(stochasticSoilModelDatabaseReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_CompleteDatabase_ThreeModelsWithProfiles()
        {
            // Setup
            string dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            const int expectedNrOfModels = 3;

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                int nrOfModels = stochasticSoilModelDatabaseReader.PipingStochasticSoilModelCount;
                var readModels = new List<StochasticSoilModel>();
                while (stochasticSoilModelDatabaseReader.HasNext)
                {
                    // Call
                    readModels.Add(stochasticSoilModelDatabaseReader.ReadStochasticSoilModel());
                }

                // Assert
                var expectedSegmentAndModelNames = new[]
                {
                    "36005_Piping",
                    "36006_Piping",
                    "36007_Piping"
                };
                var expectedModelIds = new[]
                {
                    2,
                    4,
                    6
                };
                var expectedSegmentPointCount = new[]
                {
                    1797,
                    144,
                    606
                };
                var expectedProfileCount = new[]
                {
                    10,
                    6,
                    8
                };
                Assert.AreEqual(expectedNrOfModels, nrOfModels);

                CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.SegmentName));
                CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.Name));
                CollectionAssert.AreEqual(expectedModelIds, readModels.Select(m => m.Id));
                CollectionAssert.AreEqual(expectedSegmentPointCount, readModels.Select(m => m.Geometry.Count));
                CollectionAssert.AreEqual(expectedProfileCount, readModels.Select(m => m.StochasticSoilProfiles.Count));

                Assert.IsFalse(stochasticSoilModelDatabaseReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Count_ThreeModelsOneModelWithoutSegmentPoints_ReturnsTwo()
        {
            // Setup
            string dbName = "modelWithoutSegmentPoints.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                int nrOfModels = stochasticSoilModelDatabaseReader.PipingStochasticSoilModelCount;

                Assert.AreEqual(2, nrOfModels);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_ThreeModelsOneModelWithoutSegmentPointsUsingCount_ReturnsTwoModels()
        {
            // Setup
            string dbName = "modelWithoutSegmentPoints.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                var readModels = new List<StochasticSoilModel>();
                for (int i = 0; i < stochasticSoilModelDatabaseReader.PipingStochasticSoilModelCount; i++)
                {
                    // Call
                    readModels.Add(stochasticSoilModelDatabaseReader.ReadStochasticSoilModel());
                }

                // Assert
                CheckModelWithoutSegmentPoints(readModels);
                Assert.IsFalse(stochasticSoilModelDatabaseReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_ThreeModelsOneModelWithoutSegmentPointsUsingHasNext_ReturnsTwoModels()
        {
            // Setup
            string dbName = "modelWithoutSegmentPoints.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                var readModels = new List<StochasticSoilModel>();
                while (stochasticSoilModelDatabaseReader.HasNext)
                {
                    // Call
                    readModels.Add(stochasticSoilModelDatabaseReader.ReadStochasticSoilModel());
                }

                // Assert
                CheckModelWithoutSegmentPoints(readModels);
                Assert.IsFalse(stochasticSoilModelDatabaseReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        private static void CheckModelWithoutSegmentPoints(List<StochasticSoilModel> readModels)
        {
            var expectedSegmentAndModelNames = new[]
            {
                "36005_Piping",
                "36007_Piping"
            };
            var expectedModelIds = new[]
            {
                2,
                6
            };
            var expectedSegmentPointCount = new[]
            {
                1797,
                606
            };
            var expectedProfileCount = new[]
            {
                10,
                8
            };

            Assert.AreEqual(2, readModels.Count);
            CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.SegmentName));
            CollectionAssert.AreEqual(expectedSegmentAndModelNames, readModels.Select(m => m.Name));
            CollectionAssert.AreEqual(expectedModelIds, readModels.Select(m => m.Id));
            CollectionAssert.AreEqual(expectedSegmentPointCount, readModels.Select(m => m.Geometry.Count));
            CollectionAssert.AreEqual(expectedProfileCount, readModels.Select(m => m.StochasticSoilProfiles.Count));
        }
    }
}