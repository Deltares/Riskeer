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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class PreconsolidationStressReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                          nameof(PreconsolidationStressReader));

        [Test]
        public void Constructor_NonExistingFilePath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new PreconsolidationStressReader(testFile)) {}
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
            TestDelegate test = () => new PreconsolidationStressReader(fileName);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
                Assert.IsInstanceOf<IRowBasedDatabaseReader>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_IncorrectFormatFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "text.txt");

            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kon geen grensspanningen verkrijgen uit de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
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
        public void HasNext_DatabaseWithPreconsolidationStresses_ReturnsTrue()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStresses.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
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
        public void ReadSoilProfileId_EmptyDatabase_ThrowsInvalidOperationException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfileId();

                // Assert
                var exception = Assert.Throws<InvalidOperationException>(test);
                Assert.AreEqual("The reader does not have a row to read.", exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfileId_DatabaseWithPreconsolidationStresses_ReturnsCurrentId()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStresses.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                reader.Initialize();

                // Call
                long id = reader.ReadSoilProfileId();

                // Assert
                Assert.AreEqual(1, id);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadPreconsolidationStresses_EmptyFile_ReturnsEmptyPreconsolidationStressCollection()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                reader.Initialize();

                // Call 
                PreconsolidationStress[] preconsolidationStresses = reader.ReadPreconsolidationStresses().ToArray();

                // Assert
                CollectionAssert.IsEmpty(preconsolidationStresses);
            }
        }

        [Test]
        public void ReadPreconsolidationStresses_FirstSoilProfileInCompleteDatabase_ReturnsExpectedPreconsolidationStresses()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStresses.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                reader.Initialize();

                // Call
                PreconsolidationStress[] preconsolidationStresses = reader.ReadPreconsolidationStresses().ToArray();

                // Assert
                Assert.AreEqual(4, preconsolidationStresses.Length);

                CollectionAssert.AreEqual(new[]
                {
                    1,
                    2,
                    3,
                    4
                }, preconsolidationStresses.Select(stress => stress.XCoordinate));
                CollectionAssert.AreEqual(new[]
                {
                    5,
                    6,
                    7,
                    8
                }, preconsolidationStresses.Select(stress => stress.ZCoordinate));

                CollectionAssert.AreEqual(new[]
                {
                    3,
                    3,
                    3,
                    3
                }, preconsolidationStresses.Select(stress => stress.StressDistributionType));
                CollectionAssert.AreEqual(new[]
                {
                    1337,
                    3371,
                    8.5,
                    9.3
                }, preconsolidationStresses.Select(stress => stress.StressMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.0074794315632011965,
                    0.0029664787896766538,
                    1.8823529411764706,
                    0.8064516129032258
                }, preconsolidationStresses.Select(stress => stress.StressCoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    11,
                    12,
                    0,
                    0
                }, preconsolidationStresses.Select(stress => stress.StressShift));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadPreconsolidationStresses_FirstSoilProfileWithNullValuesInCompleteDatabase_ReturnsDefaultPreconsolidationStressValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStressesNullValues.soil");

            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                reader.Initialize();

                // Call
                PreconsolidationStress[] preconsolidationStresses = reader.ReadPreconsolidationStresses().ToArray();

                // Assert
                Assert.AreEqual(1, preconsolidationStresses.Length);
                PreconsolidationStress actualPreconsolidationStress = preconsolidationStresses[0];

                Assert.IsNaN(actualPreconsolidationStress.XCoordinate);
                Assert.IsNaN(actualPreconsolidationStress.ZCoordinate);
                Assert.IsNull(actualPreconsolidationStress.StressDistributionType);
                Assert.IsNaN(actualPreconsolidationStress.StressMean);
                Assert.IsNaN(actualPreconsolidationStress.StressCoefficientOfVariation);
                Assert.IsNaN(actualPreconsolidationStress.StressShift);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenDatabaseWithThreeProfilesWithPreconsolidationStresses_WhenReadingPreconsolidationStressesFails_ThenThrowsSoilProfileReadExceptionAndCanContinueReading()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "2dprofileWithPreconsolidationStressesAndUnparsableValues.soil");

            SoilProfileReadException actualException = null;
            var readStresses = new List<PreconsolidationStress>();
            using (var reader = new PreconsolidationStressReader(dbFile))
            {
                reader.Initialize();

                while (reader.HasNext)
                {
                    try
                    {
                        // When
                        readStresses.AddRange(reader.ReadPreconsolidationStresses());
                    }
                    catch (SoilProfileReadException e)
                    {
                        actualException = e;
                    }
                }

                // Then
                Assert.IsFalse(reader.HasNext);
            }

            Assert.IsNotNull(actualException);
            Assert.AreEqual("Profile_1", actualException.ProfileName);

            CollectionAssert.AreEqual(new[]
            {
                2,
                3
            }, readStresses.Select(stress => stress.XCoordinate));
            CollectionAssert.AreEqual(new[]
            {
                2,
                3
            }, readStresses.Select(stress => stress.ZCoordinate));

            CollectionAssert.AreEqual(new[]
            {
                3,
                3
            }, readStresses.Select(stress => stress.StressDistributionType));
            CollectionAssert.AreEqual(new[]
            {
                2,
                3
            }, readStresses.Select(stress => stress.StressMean));
            CollectionAssert.AreEqual(new[]
            {
                1,
                1
            }, readStresses.Select(stress => stress.StressCoefficientOfVariation));
            CollectionAssert.AreEqual(new[]
            {
                0,
                0
            }, readStresses.Select(stress => stress.StressShift));

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}