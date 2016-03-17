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
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
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
    public class StochasticSoilModelDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "StochasticSoilModelDatabaseReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => new StochasticSoilModelDatabaseReader(testFile).Dispose();

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
            TestDelegate test = () => new StochasticSoilModelDatabaseReader(fileName).Dispose();

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
                Build(String.Format(Resources.StochasticSoilModelDatabaseReader_failed_to_read_database, dbName));

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => new StochasticSoilModelDatabaseReader(dbFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
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
            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelDatabaseReader(dbFile))
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
            TestDelegate test = () => { using (var s = new StochasticSoilModelDatabaseReader(dbFile)) {} };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedVersionMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetStochasticSoilModelSegmentOfPiping_EmptyFileThatPassesVersionCheck_ReturnsEmptyList()
        {
            // Setup
            var dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            IEnumerable<StochasticSoilModelSegment> stochasticSoilModelSegment;

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelDatabaseReader(dbFile))
            {
                // Call
                stochasticSoilModelSegment = stochasticSoilModelDatabaseReader.GetStochasticSoilModelSegmentOfPiping();
            }

            // Assert
            Assert.IsNull(stochasticSoilModelSegment);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetStochasticSoilModelSegmentOfPiping_CompleteScenario_ReturnsExpectedValues()
        {
            // Setup
            List<StochasticSoilModelSegment> segmentSoilModels;
            const string dbName = "complete.soil";
            var dbFile = Path.Combine(testDataPath, dbName);
            const int expectedSegmentSoilModels = 3;

            const string expectedSegmentName1 = "36005_Piping";
            const string expectedSegmentSoilModelName1 = "36005_Piping";
            const long expectedSegmentSoilModelId1 = 2;
            const long expectedSegmentSoilModelPoints1 = 1797;
            const long expectedSegmentSoilModelProbabilities1 = 10;

            const string expectedSegmentName2 = "36006_Piping";
            const string expectedSegmentSoilModelName2 = "36006_Piping";
            const long expectedSegmentSoilModelId2 = 4;
            const long expectedSegmentSoilModelPoints2 = 144;
            const long expectedSegmentSoilModelProbabilities2 = 6;

            const string expectedSegmentName3 = "36007_Piping";
            const string expectedSegmentSoilModelName3 = "36007_Piping";
            const long expectedSegmentSoilModelId3 = 6;
            const long expectedSegmentSoilModelPoints3 = 606;
            const long expectedSegmentSoilModelProbabilities3 = 8;

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelDatabaseReader(dbFile))
            {
                // Call
                segmentSoilModels = stochasticSoilModelDatabaseReader.GetStochasticSoilModelSegmentOfPiping().ToList();
            }

            // Assert
            Assert.IsInstanceOf<IEnumerable<StochasticSoilModelSegment>>(segmentSoilModels);
            CollectionAssert.AllItemsAreInstancesOfType(segmentSoilModels, typeof(StochasticSoilModelSegment));
            Assert.AreEqual(expectedSegmentSoilModels, segmentSoilModels.Count);

            StochasticSoilModelSegment stochasticSoilModelSegmentSoilModel1 = segmentSoilModels[0];
            Assert.AreEqual(expectedSegmentName1, stochasticSoilModelSegmentSoilModel1.SegmentName);
            Assert.AreEqual(expectedSegmentSoilModelName1, stochasticSoilModelSegmentSoilModel1.SegmentSoilModelName);
            Assert.AreEqual(expectedSegmentSoilModelId1, stochasticSoilModelSegmentSoilModel1.SegmentSoilModelId);
            Assert.AreEqual(expectedSegmentSoilModelPoints1, stochasticSoilModelSegmentSoilModel1.SegmentPoints.Count);
            CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModelSegmentSoilModel1.SegmentPoints, typeof(Point2D));
            Assert.AreEqual(expectedSegmentSoilModelProbabilities1, stochasticSoilModelSegmentSoilModel1.StochasticSoilProfileProbabilities.Count);
            CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModelSegmentSoilModel1.StochasticSoilProfileProbabilities, typeof(StochasticSoilProfileProbability));

            StochasticSoilModelSegment stochasticSoilModelSegmentSoilModel2 = segmentSoilModels[1];
            Assert.AreEqual(expectedSegmentName2, stochasticSoilModelSegmentSoilModel2.SegmentName);
            Assert.AreEqual(expectedSegmentSoilModelName2, stochasticSoilModelSegmentSoilModel2.SegmentSoilModelName);
            Assert.AreEqual(expectedSegmentSoilModelId2, stochasticSoilModelSegmentSoilModel2.SegmentSoilModelId);
            Assert.AreEqual(expectedSegmentSoilModelPoints2, stochasticSoilModelSegmentSoilModel2.SegmentPoints.Count);
            CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModelSegmentSoilModel2.SegmentPoints, typeof(Point2D));
            Assert.AreEqual(expectedSegmentSoilModelProbabilities2, stochasticSoilModelSegmentSoilModel2.StochasticSoilProfileProbabilities.Count);
            CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModelSegmentSoilModel2.StochasticSoilProfileProbabilities, typeof(StochasticSoilProfileProbability));

            StochasticSoilModelSegment stochasticSoilModelSegmentSoilModel3 = segmentSoilModels[2];
            Assert.AreEqual(expectedSegmentName3, stochasticSoilModelSegmentSoilModel3.SegmentName);
            Assert.AreEqual(expectedSegmentSoilModelName3, stochasticSoilModelSegmentSoilModel3.SegmentSoilModelName);
            Assert.AreEqual(expectedSegmentSoilModelId3, stochasticSoilModelSegmentSoilModel3.SegmentSoilModelId);
            Assert.AreEqual(expectedSegmentSoilModelPoints3, stochasticSoilModelSegmentSoilModel3.SegmentPoints.Count);
            CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModelSegmentSoilModel3.SegmentPoints, typeof(Point2D));
            Assert.AreEqual(expectedSegmentSoilModelProbabilities3, stochasticSoilModelSegmentSoilModel3.StochasticSoilProfileProbabilities.Count);
            CollectionAssert.AllItemsAreInstancesOfType(stochasticSoilModelSegmentSoilModel3.StochasticSoilProfileProbabilities, typeof(StochasticSoilProfileProbability));
        }

        [Test]
        public void GetStochasticSoilModelSegmentOfPiping_SchemaWithoutSoilModelTables_ThrowsCriticalFileReadException()
        {
            // Setup
            var dbName = "withoutSoilModelTables.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(Resources.StochasticSoilModelDatabaseReader_failed_to_read_database);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => stochasticSoilModelDatabaseReader.GetStochasticSoilModelSegmentOfPiping();

                // Assert
                CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetStochasticSoilModelSegmentOfPiping_InvalidSegmentPoint_SkipsSegmentPoint()
        {
            // Setup
            var dbName = "invalidSegmentPoint.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            List<StochasticSoilModelSegment> stochasticSoilModelSegmentList = null;
            const string expectedLogMessage = "De coördinaten van het stochastisch ondergrondsmodel bevatten geen geldige waarde.";

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelDatabaseReader(dbFile))
            {
                // Call
                Action action = () => { stochasticSoilModelSegmentList = stochasticSoilModelDatabaseReader.GetStochasticSoilModelSegmentOfPiping().ToList(); };
                TestHelper.AssertLogMessageIsGenerated(action, expectedLogMessage, 1);
            }

            // Assert
            Assert.IsInstanceOf<IEnumerable<StochasticSoilModelSegment>>(stochasticSoilModelSegmentList);
            Assert.AreEqual(2, stochasticSoilModelSegmentList.Count);

            StochasticSoilModelSegment stochasticSoilModelSegment1 = stochasticSoilModelSegmentList[0];
            Assert.AreEqual(1, stochasticSoilModelSegment1.StochasticSoilProfileProbabilities.Count);

            StochasticSoilModelSegment stochasticSoilModelSegment2 = stochasticSoilModelSegmentList[1];
            Assert.AreEqual(1, stochasticSoilModelSegment2.StochasticSoilProfileProbabilities.Count);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetStochasticSoilModelSegmentOfPiping_InvalidSoilProfile_SkipsModel()
        {
            // Setup
            var dbName = "invalidStochasticSoilProfile.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            List<StochasticSoilModelSegment> stochasticSoilModelSegmentList = null;
            const string expectedLogMessage = "Het uitlezen van een stochastisch ondergrondsmodel misgelukt, deze zal worden overgeslagen.";

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (var stochasticSoilModelDatabaseReader = new StochasticSoilModelDatabaseReader(dbFile))
            {
                // Call
                Action action = () => { stochasticSoilModelSegmentList = stochasticSoilModelDatabaseReader.GetStochasticSoilModelSegmentOfPiping().ToList(); };
                TestHelper.AssertLogMessageIsGenerated(action, expectedLogMessage, 1);
            }

            // Assert
            Assert.IsInstanceOf<IEnumerable<StochasticSoilModelSegment>>(stochasticSoilModelSegmentList);
            Assert.AreEqual(0, stochasticSoilModelSegmentList.Count);

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}