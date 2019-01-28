// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SegmentPointReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SegmentPointReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new SegmentPointReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_InvalidPath_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () => new SegmentPointReader(fileName);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new SegmentPointReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_IncorrectFormatFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "text.txt");

            using (var reader = new SegmentPointReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kon geen stochastische ondergrondmodellen verkrijgen uit de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                bool hasNext = reader.HasNext;

                // Assert
                Assert.IsFalse(hasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_DatabaseWithStochasticSoilModels_ReturnsTrue()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                bool hasNext = reader.HasNext;

                // Assert
                Assert.IsTrue(hasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModelId_EmptyDatabase_ThrowsInvalidOperationException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadStochasticSoilModelId();

                // Assert
                var exception = Assert.Throws<InvalidOperationException>(test);
                Assert.AreEqual("The reader does not have a row to read.", exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModelId_DatabaseWithStochasticSoilModels_ReturnsCurrentId()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                long id = reader.ReadStochasticSoilModelId();

                // Assert
                Assert.AreEqual(1, id);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenReadSegmentPoints_WhenReadSegmentPoints_ThenReturnsNextSegmentPoints()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilModel.soil");
            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                Point2D[] firstGeometry = reader.ReadSegmentPoints().ToArray();

                // Precondition
                Assert.AreEqual(new[]
                {
                    new Point2D(1, 2)
                }, firstGeometry);

                // When
                Point2D[] nextGeometry = reader.ReadSegmentPoints().ToArray();

                // Then
                Assert.AreEqual(new[]
                {
                    new Point2D(5, 6)
                }, nextGeometry);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_SegmentPointCoordinatesNull_ThrowsStochasticSoilModelException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "invalidSegmentPoint.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSegmentPoints().ToArray();

                // Assert
                var exception = Assert.Throws<StochasticSoilModelException>(test);
                Assert.AreEqual("Het stochastische ondergrondmodel 'StochasticSoilModelName' moet een geometrie bevatten.",
                                exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenReadStochasticSoilModelThrowsException_WhenReadSegmentPoints_ReturnsNextStochasticSoilModel()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "invalidSegmentPoint.soil");
            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                StochasticSoilModelException throwException = null;
                try
                {
                    reader.ReadSegmentPoints().ToArray();
                }
                catch (StochasticSoilModelException e)
                {
                    throwException = e;
                }

                // When
                Point2D[] nextGeometry = reader.ReadSegmentPoints().ToArray();

                // Then
                Assert.AreEqual(new[]
                {
                    new Point2D(1, 2)
                }, nextGeometry);

                Assert.IsInstanceOf<StochasticSoilModelException>(throwException);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_EmptyDatabase_ReturnsEmptyCollection()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                Point2D[] segmentPoints = reader.ReadSegmentPoints().ToArray();

                // Assert
                CollectionAssert.IsEmpty(segmentPoints);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSegmentPoints_FirstModelInCompleteDatabase_ReturnsExpectedSegmentPoints()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.soil");

            using (var reader = new SegmentPointReader(dbFile))
            {
                reader.Initialize();

                // Call
                Point2D[] segmentPoints = reader.ReadSegmentPoints().ToArray();

                // Assert
                Assert.AreEqual(1797, segmentPoints.Length);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}