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
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.Primitives;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class PipingSoilProfileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => { using (new PipingSoilProfileReader(testFile)) {} };

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
            // Call
            TestDelegate test = () => { using (new PipingSoilProfileReader(fileName)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   fileName, "bestandspad mag niet leeg of ongedefinieerd zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsPipingCriticalFileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                .Build(string.Format(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column, dbName));

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => { using (new PipingSoilProfileReader(dbFile)) {} };

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
            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, pipingSoilProfileReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(pipingSoilProfileReader);
            }
        }

        [Test]
        public void Constructor_EmptyDatabase_HasNextFalse()
        {
            // Setup
            string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Call
            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                // Assert
                Assert.IsFalse(pipingSoilProfileReader.HasNext);
                Assert.AreEqual(0, pipingSoilProfileReader.Count);
            }
        }

        [Test]
        public void Constructor_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            var version = "15.0.6.0";
            var dbName = "incorrectversion.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => { using (new PipingSoilProfileReader(dbFile)) {} };

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(string.Format(Resources.PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_, version), exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadProfile_AfterDatabaseHasBeenRead_ReturnsNull()
        {
            // Setup
            var testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                while (pipingSoilProfileReader.HasNext)
                {
                    pipingSoilProfileReader.ReadProfile();
                }

                // Call & Assert
                Assert.IsNull(pipingSoilProfileReader.ReadProfile());
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DAnd2DProfilesWithSameName_ReturnTwoProfilesWithSameName()
        {
            // Setup
            var testFile = "combined1d2d.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile))
            {
                var result = new Collection<PipingSoilProfile>();

                // Call
                while (pipingSoilProfilesReader.HasNext)
                {
                    result.Add(pipingSoilProfilesReader.ReadProfile());
                }

                // Assert
                const int expectedNumberOfProfiles = 2;
                Assert.AreEqual(expectedNumberOfProfiles, pipingSoilProfilesReader.Count);
                Assert.AreEqual(expectedNumberOfProfiles, result.Count);
                Assert.AreEqual(SoilProfileType.SoilProfile2D, result[0].SoilProfileType);
                Assert.AreEqual(SoilProfileType.SoilProfile1D, result[1].SoilProfileType);
                Assert.AreEqual(result[0].Name, result[1].Name);
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DAnd1DSoilProfileWithoutSoilLayers_ReturnOneProfile()
        {
            // Setup
            var testFile = "1dprofileWithEmpty1d.soil";
            string dbFile = Path.Combine(testDataPath, testFile);
            var expextedProfileName = "Profile";

            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile))
            {
                var result = new Collection<PipingSoilProfile>();

                // Call
                while (pipingSoilProfilesReader.HasNext)
                {
                    result.Add(pipingSoilProfilesReader.ReadProfile());
                }

                // Assert
                Assert.AreEqual(1, pipingSoilProfilesReader.Count);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(expextedProfileName, result[0].Name);
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithInvalid2dLayerGeometry_SkipsTheProfile()
        {
            // Setup
            var testFile = "invalid2dGeometry.soil";
            string databaseFilePath = Path.Combine(testDataPath, testFile);
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(databaseFilePath))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(profile);
                string expectedMessage = new FileReaderErrorMessageBuilder(databaseFilePath)
                    .WithSubject("ondergrondschematisatie 'Profile'")
                    .Build(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml);
                Assert.AreEqual(expectedMessage, exception.Message);

                // Call
                PipingSoilProfile pipingSoilProfile = pipingSoilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile2", pipingSoilProfile.Name);
                Assert.AreEqual(3, pipingSoilProfile.Layers.Count());

                Assert.IsTrue(TestHelper.CanOpenFileForWrite(testFile));
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithVerticalSegmentAtX_SkipsTheProfile()
        {
            // Setup
            var testFile = "vertical2dGeometry.soil";
            string databaseFilePath = Path.Combine(testDataPath, testFile);
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(databaseFilePath))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(profile);
                string message = new FileReaderErrorMessageBuilder(databaseFilePath)
                    .WithSubject("ondergrondschematisatie 'Profile'")
                    .Build(string.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_, 85.2));
                Assert.AreEqual(message, exception.Message);

                // Call
                PipingSoilProfile pipingSoilProfile = pipingSoilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile2", pipingSoilProfile.Name);
                Assert.AreEqual(3, pipingSoilProfile.Layers.Count());

                Assert.IsTrue(TestHelper.CanOpenFileForWrite(testFile));
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithoutValuesForLayerProperties_ReturnsProfileWithAllLayers()
        {
            // Setup
            var testFile = "1dprofileNoValues.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                PipingSoilProfile profile = pipingSoilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile", profile.Name);
                const int expectedNumberOfLayers = 3;
                Assert.AreEqual(expectedNumberOfLayers, profile.Layers.Count());
                CollectionAssert.AreEqual(Enumerable.Repeat(false, expectedNumberOfLayers), profile.Layers.Select(l => l.IsAquifer));
                IEnumerable<double> nanValues = Enumerable.Repeat(double.NaN, expectedNumberOfLayers).ToArray();
                CollectionAssert.AreEqual(nanValues, profile.Layers.Select(l => l.BelowPhreaticLevelMean));
            }
        }

        [Test]
        [TestCase("incorrect2dStochastDistributionProperty.soil", "Parameter \'Verzadigd gewicht\' is niet verschoven lognormaal verdeeld.")]
        [TestCase("incorrect2dStochastShiftProperty.soil", "Parameter \'Korrelgrootte\' is niet lognormaal verdeeld.")]
        [TestCase("incorrectValue2dStochastProperty.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'BelowPhreaticLevelMean\'.")]
        [TestCase("incorrectValue2dProperty.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'Color\'.")]
        [TestCase("invalidTop1dProfile.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'Top\'.")]
        [TestCase("invalidBottom1dProfile.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'Bottom\'.")]
        public void ReadProfile_DatabaseProfileWithLayerWithIncorrectDistributionForBelowPhreaticLevelProperty_ThrowsPipingSoilProfileReadException(
            string testFile,
            string expectedMessage)
        {
            // Setup
            string databaseFilePath = Path.Combine(testDataPath, testFile);
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(databaseFilePath))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                string exceptionMessage = Assert.Throws<PipingSoilProfileReadException>(profile).Message;
                string message = new FileReaderErrorMessageBuilder(databaseFilePath)
                    .WithSubject("ondergrondschematisatie 'Profile'")
                    .Build(expectedMessage);
                Assert.AreEqual(message, exceptionMessage);
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DProfile3Layers_ReturnsProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);
            using (var reader = new PipingSoilProfileReader(dbFile))
            {
                // Call
                PipingSoilProfile profile = reader.ReadProfile();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    false,
                    false,
                    true
                }, profile.Layers.Select(l => l.IsAquifer));
                CollectionAssert.AreEqual(new[]
                {
                    Color.FromArgb(128, 255, 128),
                    Color.FromArgb(255, 0, 0),
                    Color.FromArgb(70, 130, 180)
                }, profile.Layers.Select(l => l.Color));
                CollectionAssert.AreEqual(new[]
                {
                    3.88,
                    0.71,
                    0.21
                }, profile.Layers.Select(l => l.BelowPhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.08,
                    0.02,
                    0.001
                }, profile.Layers.Select(l => l.BelowPhreaticLevelDeviation));
                CollectionAssert.AreEqual(new[]
                {
                    0.4,
                    0.32,
                    0.3
                }, profile.Layers.Select(l => l.BelowPhreaticLevelShift));
                CollectionAssert.AreEqual(new[]
                {
                    11.3,
                    0.01,
                    0.51
                }, profile.Layers.Select(l => l.DiameterD70Mean));
                CollectionAssert.AreEqual(new[]
                {
                    0.2,
                    0.001,
                    0.015
                }, profile.Layers.Select(l => l.DiameterD70Deviation));
                CollectionAssert.AreEqual(new[]
                {
                    5.21,
                    9.99,
                    1.01
                }, profile.Layers.Select(l => l.PermeabilityMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.3,
                    0.1,
                    0.025
                }, profile.Layers.Select(l => l.PermeabilityDeviation));
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith2DProfile3Layers_ReturnsProfile()
        {
            // Setup
            var testFile = "2dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);
            using (var reader = new PipingSoilProfileReader(dbFile))
            {
                // Call
                PipingSoilProfile profile = reader.ReadProfile();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    true,
                    false,
                    false
                }, profile.Layers.Select(l => l.IsAquifer));
                CollectionAssert.AreEqual(new[]
                {
                    Color.FromArgb(70, 130, 180),
                    Color.FromArgb(255, 0, 0),
                    Color.FromArgb(128, 255, 128)
                }, profile.Layers.Select(l => l.Color));
                CollectionAssert.AreEqual(new[]
                {
                    0.21,
                    0.71,
                    3.88
                }, profile.Layers.Select(l => l.BelowPhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.001,
                    0.02,
                    0.08
                }, profile.Layers.Select(l => l.BelowPhreaticLevelDeviation));
                CollectionAssert.AreEqual(new[]
                {
                    0.3,
                    0.32,
                    0.4
                }, profile.Layers.Select(l => l.BelowPhreaticLevelShift));
                CollectionAssert.AreEqual(new[]
                {
                    0.51,
                    0.01,
                    11.3
                }, profile.Layers.Select(l => l.DiameterD70Mean));
                CollectionAssert.AreEqual(new[]
                {
                    0.015,
                    0.001,
                    0.2
                }, profile.Layers.Select(l => l.DiameterD70Deviation));
                CollectionAssert.AreEqual(new[]
                {
                    1.01,
                    9.99,
                    5.21
                }, profile.Layers.Select(l => l.PermeabilityMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.025,
                    0.1,
                    0.3
                }, profile.Layers.Select(l => l.PermeabilityDeviation));
            }
        }

        [Test]
        public void Dispose_AfterConstruction_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            // Call
            new PipingSoilProfileReader(dbFile).Dispose();

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenReadProfile_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            PipingSoilProfileReader pipingSoilProfilesReader = null;
            PipingSoilProfile profile;
            try
            {
                pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile);
                profile = pipingSoilProfilesReader.ReadProfile();
            }
            finally
            {
                // Call
                if (pipingSoilProfilesReader != null)
                {
                    pipingSoilProfilesReader.Dispose();
                }
            }

            // Assert
            Assert.NotNull(profile);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenDatabaseWith1DProfileAndDutchLocale_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile()
        {
            GivenDatabaseWith1DProfile_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile();
        }

        [Test]
        [SetCulture("en-US")]
        public void GivenDatabaseWith1DProfileAndEnglishLocale_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile()
        {
            GivenDatabaseWith1DProfile_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenACompleteDatabaseAndDutchLocale_WhenReadingTheCompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            GivenACompleteDatabase_WhenReadingTheCompleteDatabase_ReturnsProfilesWithLayersAndGeometries();
        }

        [Test]
        [SetCulture("en-US")]
        public void GivenACompleteDatabaseAndEnglishLocale_WhenReadingTheCompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            GivenACompleteDatabase_WhenReadingTheCompleteDatabase_ReturnsProfilesWithLayersAndGeometries();
        }

        private void GivenDatabaseWith1DProfile_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile()
        {
            // Given
            var testFile = "1dprofile.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // When
                var result = new Collection<PipingSoilProfile>();
                var skipped = 0;

                while (pipingSoilProfilesReader.HasNext)
                {
                    try
                    {
                        result.Add(pipingSoilProfilesReader.ReadProfile());
                    }
                    catch
                    {
                        skipped++;
                    }
                }

                // Then
                Assert.AreEqual(0, skipped);
                Assert.AreEqual(1, pipingSoilProfilesReader.Count);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(-2.1, result[0].Bottom);
                CollectionAssert.AreEqual(new[]
                {
                    3.3,
                    2.2,
                    1.1
                }, result[0].Layers.Select(l => l.Top));
            }
        }

        private void GivenACompleteDatabase_WhenReadingTheCompleteDatabase_ReturnsProfilesWithLayersAndGeometries()
        {
            // Given
            var testFile = "complete.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // When
                ICollection<PipingSoilProfile> result = new List<PipingSoilProfile>();
                int skipped = 0;

                while (pipingSoilProfilesReader.HasNext)
                {
                    try
                    {
                        result.Add(pipingSoilProfilesReader.ReadProfile());
                    }
                    catch
                    {
                        skipped++;
                    }
                }

                // Then
                Assert.AreEqual(0, skipped);
                Assert.AreEqual(26, pipingSoilProfilesReader.Count);
                Assert.AreEqual(26, result.Count);
                CollectionAssert.AreEqual(new[]
                {
                    "AD640M00_Segment_36005_1D1",
                    "AD640M00_Segment_36005_1D2",
                    "Segment_36005_1D1",
                    "Segment_36005_1D2",
                    "Segment_36005_1D3",
                    "Segment_36005_1D4",
                    "Segment_36005_1D5",
                    "Segment_36005_1D6",
                    "Segment_36005_1D7",
                    "Segment_36005_1D8",
                    "Segment_36005_1D9",
                    "Segment_36005_1D10",
                    "Segment_36006_1D1",
                    "Segment_36006_1D2",
                    "Segment_36006_1D3",
                    "Segment_36006_1D4",
                    "Segment_36006_1D5",
                    "Segment_36006_1D6",
                    "Segment_36007_1D1",
                    "Segment_36007_1D2",
                    "Segment_36007_1D3",
                    "Segment_36007_1D4",
                    "Segment_36007_1D5",
                    "Segment_36007_1D6",
                    "Segment_36007_1D7",
                    "Segment_36007_1D8"
                }, result.Select(p => p.Name).ToArray());

                CollectionAssert.AreEqual(new[]
                {
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -45,
                    -52,
                    -52,
                    -52,
                    -52,
                    -24,
                    -21.25,
                    -21,
                    -21
                }, result.Select(p => p.Bottom).ToArray());

                CollectionAssert.AreEqual(new[]
                {
                    9,
                    7,
                    8,
                    6,
                    6,
                    5,
                    5,
                    6,
                    4,
                    4,
                    3,
                    3,
                    7,
                    7,
                    7,
                    5,
                    5,
                    5,
                    6,
                    6,
                    4,
                    5,
                    4,
                    4,
                    2,
                    3
                }, result.Select(p => p.Layers.Count()).ToArray());

                var firstProfile = result.FirstOrDefault(l => l.Name == "Segment_36005_1D1");
                Assert.NotNull(firstProfile);
                var expectedFirstProfileLayersTops = new[]
                {
                    3.25,
                    2.75,
                    1.25,
                    1.0,
                    -2.5,
                    -13.0,
                    -17.0,
                    -25.0
                };
                var layerTops = firstProfile.Layers.Select(l => l.Top).ToArray();
                CollectionAssert.AllItemsAreUnique(layerTops);
                CollectionAssert.AreEqual(expectedFirstProfileLayersTops, layerTops, new DoubleWithToleranceComparer(1e-6));

                var secondProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D1");
                Assert.NotNull(secondProfile);
                var expectedSecondProfileLayersTops = new[]
                {
                    5.9075,
                    3.25,
                    2.75,
                    1.25,
                    1,
                    -2.5,
                    -13,
                    -17,
                    -25,
                };
                var layer2Tops = secondProfile.Layers.Select(l => l.Top).ToArray();
                CollectionAssert.AllItemsAreUnique(layer2Tops);
                CollectionAssert.AreEqual(expectedSecondProfileLayersTops, layer2Tops, new DoubleWithToleranceComparer(1e-6));
            }
        }
    }
}