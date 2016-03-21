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

using System;
using System.IO;
using Core.Common.Base.Geometry;
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
            var testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(testFile)) {} };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                fileName, UtilsResources.Error_Path_must_be_specified);
            // Call
            TestDelegate test = () => { using (new StochasticSoilModelReader(fileName)) {} };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsPipingCriticalFileReadException(string dbName)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, dbName);
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build(String.Format(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column, dbName));

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
        [TestCase("withoutSoilModelTables.soil")]
        public void Constructor_InvalidSchemaThatPassesValidation_ThrowsPipingCriticalFileReadException(string dbName)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, dbName);
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build(String.Format(Resources.StochasticSoilModelDatabaseReader_Failed_to_read_database, dbName));

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
            var dbName = "emptyschema.soil";
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
            const string version = "15.0.5.0";
            string expectedVersionMessage = String.Format(Resources.PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_, version);
            const string dbName = "incorrectversion.soil";
            var dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => { using (var s = new StochasticSoilModelReader(dbFile)) {} };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedVersionMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            var dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            bool isPrepared = true;

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                isPrepared = stochasticSoilModelDatabaseReader.HasNext;
            }

            // Assert
            Assert.IsFalse(isPrepared);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_CompleteDatabase_ReturnsTrue()
        {
            // Setup
            var dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            bool hasNext = false;

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                hasNext = stochasticSoilModelDatabaseReader.HasNext;
            }

            // Assert
            Assert.IsTrue(hasNext);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilProfile_InvalidSegmentPoint_ThrowsStochasticSoilModelReadException()
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
        public void ReadStochasticSoilProfile_CompleteDatabase_ReturnsExpectedValues()
        {
            // Setup
            string dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            const string expectedSegmentName = "36005_Piping";
            const string expectedSegmentSoilModelName = "36005_Piping";
            const long expectedSegmentSoilModelId = 2;
            const long expectedSegmentSoilModelPoints = 1797;
            const int expectedNrOfModels = 3;

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelReader(dbFile))
            {
                int nrOfModels = stochasticSoilModelDatabaseReader.PipingStochasticSoilModelCount;

                // Call
                StochasticSoilModel stochasticSoilModel = stochasticSoilModelDatabaseReader.ReadStochasticSoilModel();

                // Assert
                Assert.IsNotNull(stochasticSoilModel);
                Assert.AreEqual(expectedNrOfModels, nrOfModels);
                Assert.AreEqual(expectedSegmentName, stochasticSoilModel.SegmentName);
                Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModel.Name);
                Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModel.Id);
                Assert.AreEqual(expectedSegmentSoilModelPoints, stochasticSoilModel.Geometry.Count);
                CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModel.Geometry, typeof(Point2D));
                CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModel.StochasticSoilProfiles, typeof(StochasticSoilProfile));
                Assert.IsTrue(stochasticSoilModelDatabaseReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilProfile_EmptyDatabase_ReturnsNull()
        {
            // Setup
            var dbName = "emptyschema.soil";
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
    }
}