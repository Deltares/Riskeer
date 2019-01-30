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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(StochasticSoilModelReader));
        private readonly string constraintsReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilDatabaseConstraintsReader));
        private readonly string versionReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilDatabaseVersionReader));
        private readonly string soilProfile1DReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilProfile1DReader));
        private readonly string soilProfile2DReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilProfile2DReader));

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
        public void Constructor_InvalidPath_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () => new StochasticSoilModelReader(fileName);

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

                const string version = "17.2.0.0";
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
        public void Validate_InvalidSoilProfile1d_ThrowsStochasticSoilModelException()
        {
            // Setup
            string dbFile = Path.Combine(soilProfile1DReaderTestDataPath, "1dprofileWithInvalidBottom.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate call = () => reader.Validate();

                // Assert
                var exception = Assert.Throws<StochasticSoilModelException>(call);
                Assert.IsInstanceOf<SoilProfileReadException>(exception.InnerException);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Validate_InvalidSoilProfile2d_ThrowsStochasticSoilModelException()
        {
            // Setup
            string dbFile = Path.Combine(soilProfile2DReaderTestDataPath, "2dProfileWithXInvalid.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate call = () => reader.Validate();

                // Assert
                var exception = Assert.Throws<StochasticSoilModelException>(call);
                Assert.IsInstanceOf<SoilProfileReadException>(exception.InnerException);
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
                int nrOfModels = reader.StochasticSoilModelCount;

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.IsNull(model);
                Assert.AreEqual(0, nrOfModels);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_EmptySoilModelAndNoGeometry_ReturnsSoilModelWithoutGeometry()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWithoutGeometry.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.AreEqual("SoilModel", model.Name);
                Assert.AreEqual(FailureMechanismType.Piping, model.FailureMechanismType);
                CollectionAssert.IsEmpty(model.Geometry);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenDatabaseWithStochasticSoilModelWithAndWithoutGeometry_WhenReading_ThenReturnsAllModels()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelsWithAndWithoutGeometry.soil");

            var readModels = new List<StochasticSoilModel>();
            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                while (reader.HasNext)
                {
                    readModels.Add(reader.ReadStochasticSoilModel());
                }

                // Assert
                Assert.IsFalse(reader.HasNext);

                CollectionAssert.AreEqual(new[]
                {
                    "36005_Stability",
                    "36005_Piping",
                    "36006_Stability",
                    "36006_Piping",
                    "36007_Stability",
                    "36007_Piping"
                }, readModels.Select(m => m.Name));
                CollectionAssert.AreEqual(new[]
                {
                    1797,
                    0,
                    144,
                    0,
                    606,
                    0
                }, readModels.Select(m => m.Geometry.Count));
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
        public void ReadStochasticSoilModel_SoilModelWithoutStochasticSoilProfiles_ReturnsSoilModelWithoutLayers()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWithoutStochasticSoilProfiles.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.AreEqual("SoilModel", model.Name);
                Assert.AreEqual(FailureMechanismType.Piping, model.FailureMechanismType);
                CollectionAssert.IsEmpty(model.StochasticSoilProfiles);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenDatabaseWithStochasticSoilModelWithAndWithoutSoilProfiles_WhenReading_ReturnsAllModels()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelsWithAndWithoutStochasticSoilProfiles.soil");

            var readModels = new List<StochasticSoilModel>();
            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                while (reader.HasNext)
                {
                    readModels.Add(reader.ReadStochasticSoilModel());
                }

                // Assert
                Assert.IsFalse(reader.HasNext);

                CollectionAssert.AreEqual(new[]
                {
                    "36005_Stability",
                    "36005_Piping",
                    "36006_Stability",
                    "36006_Piping",
                    "36007_Stability",
                    "36007_Piping"
                }, readModels.Select(m => m.Name));
                CollectionAssert.AreEqual(new[]
                {
                    10,
                    0,
                    6,
                    0,
                    8,
                    0
                }, readModels.Select(m => m.StochasticSoilProfiles.Count));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_ModelWithStochasticSoilProfilesWith1DProfiles_ReturnsModelWithStochasticProfiles()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWithStochasticSoilProfile1D.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.AreEqual("SoilModel", model.Name);
                Assert.AreEqual(FailureMechanismType.Stability, model.FailureMechanismType);
                CollectionAssert.AreEqual(new[]
                {
                    0.1,
                    0.35,
                    0.05,
                    0.1,
                    0.35,
                    0.05
                }.OrderBy(p => p), model.StochasticSoilProfiles.Select(profile => profile.Probability));
                CollectionAssert.AllItemsAreInstancesOfType(model.StochasticSoilProfiles.Select(profile => profile.SoilProfile), typeof(SoilProfile1D));
            }
        }

        [Test]
        public void ReadStochasticSoilModel_ModelWithStochasticProfilesWith1DProfilesLastProfileEmpty_ReturnsModelWithStochasticProfiles()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWith1dProfilesLastProfileEmpty.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.AreEqual("43003_Piping", model.Name);
                Assert.AreEqual(FailureMechanismType.Piping, model.FailureMechanismType);
                Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
                var emptyProfile = (SoilProfile1D) model.StochasticSoilProfiles.First().SoilProfile;
                var profileWithLayers = (SoilProfile1D) model.StochasticSoilProfiles.Last().SoilProfile;
                CollectionAssert.IsEmpty(emptyProfile.Layers);
                CollectionAssert.IsNotEmpty(profileWithLayers.Layers);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_ModelWithStochasticProfilesWith2DProfiles_ReturnsModelWithStochasticProfiles()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWithStochasticSoilProfile2D.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.AreEqual("SoilModel", model.Name);
                Assert.AreEqual(FailureMechanismType.Stability, model.FailureMechanismType);
                CollectionAssert.AreEqual(new[]
                {
                    0.15,
                    0.175,
                    0.075,
                    0.05,
                    0.05,
                    0.15,
                    0.175,
                    0.075,
                    0.05,
                    0.05
                }.OrderBy(p => p), model.StochasticSoilProfiles.Select(profile => profile.Probability));
                CollectionAssert.AllItemsAreInstancesOfType(model.StochasticSoilProfiles.Select(profile => profile.SoilProfile), typeof(SoilProfile2D));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_ModelWithStochasticProfilesWith2DProfilesLastProfileEmpty_ReturnsModelWithStochasticProfiles()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "modelWith2dProfilesLastProfileEmpty.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                StochasticSoilModel model = reader.ReadStochasticSoilModel();

                // Assert
                Assert.AreEqual("43003_Stability", model.Name);
                Assert.AreEqual(FailureMechanismType.Stability, model.FailureMechanismType);
                Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
                var emptyProfile = (SoilProfile2D) model.StochasticSoilProfiles.First().SoilProfile;
                var profileWithLayers = (SoilProfile2D) model.StochasticSoilProfiles.Last().SoilProfile;
                CollectionAssert.IsEmpty(emptyProfile.Layers);
                CollectionAssert.IsNotEmpty(profileWithLayers.Layers);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilModel_OtherFailureMechanism_ThrowsStochasticSoilModelException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "otherFailureMechanism.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                reader.Validate();

                // Call
                TestDelegate test = () => reader.ReadStochasticSoilModel();

                // Assert
                var exception = Assert.Throws<StochasticSoilModelException>(test);

                const string expectedMessage = "Het faalmechanisme 'UNKNOWN' wordt niet ondersteund.";
                Assert.AreEqual(expectedMessage, exception.Message);
            }

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
                int nrOfModels = reader.StochasticSoilModelCount;

                while (reader.HasNext)
                {
                    // Call
                    readModels.Add(reader.ReadStochasticSoilModel());
                }

                // Assert
                Assert.IsFalse(reader.HasNext);
                Assert.AreEqual(6, nrOfModels);
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