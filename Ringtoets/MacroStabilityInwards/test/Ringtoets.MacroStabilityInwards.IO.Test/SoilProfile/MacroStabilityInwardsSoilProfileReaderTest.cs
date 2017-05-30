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
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO, "MacroStabilityInwardsSoilProfilesReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () =>
            {
                using (new MacroStabilityInwardsSoilProfileReader(testFile)) {}
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
            // Call
            TestDelegate test = () =>
            {
                using (new MacroStabilityInwardsSoilProfileReader(fileName)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = $"Fout bij het lezen van bestand '{fileName}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                .Build(string.Format("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.", dbName));

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () =>
            {
                using (new MacroStabilityInwardsSoilProfileReader(dbFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ParameteredConstructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            const string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Call
            using (var soilProfileReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, soilProfileReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(soilProfileReader);
            }
        }

        [Test]
        public void Constructor_EmptyDatabase_HasNextFalse()
        {
            // Setup
            const string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Call
            using (var soilProfileReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                // Assert
                Assert.IsFalse(soilProfileReader.HasNext);
                Assert.AreEqual(0, soilProfileReader.Count);
            }
        }

        [Test]
        public void Constructor_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            const string version = "15.0.6.0";
            const string dbName = "incorrectversion.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () =>
            {
                using (new MacroStabilityInwardsSoilProfileReader(dbFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual($"De database heeft niet de vereiste versie informatie. Vereiste versie is '{version}'.", exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadProfile_AfterDatabaseHasBeenRead_ReturnsNull()
        {
            // Setup
            const string testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var soilProfileReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                while (soilProfileReader.HasNext)
                {
                    soilProfileReader.ReadProfile();
                }

                // Call & Assert
                Assert.IsNull(soilProfileReader.ReadProfile());
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DAnd2DProfilesWithSameName_ReturnTwoProfilesWithSameName()
        {
            // Setup
            const string testFile = "combined1d2d.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                var result = new Collection<MacroStabilityInwardsSoilProfile>();

                // Call
                while (soilProfilesReader.HasNext)
                {
                    result.Add(soilProfilesReader.ReadProfile());
                }

                // Assert
                const int expectedNumberOfProfiles = 2;
                Assert.AreEqual(expectedNumberOfProfiles, soilProfilesReader.Count);
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
            const string testFile = "1dprofileWithEmpty1d.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                var result = new Collection<MacroStabilityInwardsSoilProfile>();

                // Call
                while (soilProfilesReader.HasNext)
                {
                    result.Add(soilProfilesReader.ReadProfile());
                }

                // Assert
                Assert.AreEqual(1, soilProfilesReader.Count);
                Assert.AreEqual(1, result.Count);
                const string expextedProfileName = "Profile";
                Assert.AreEqual(expextedProfileName, result[0].Name);
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithInvalid2dLayerGeometry_SkipsTheProfile()
        {
            // Setup
            const string testFile = "invalid2dGeometry.soil";
            string databaseFilePath = Path.Combine(testDataPath, testFile);
            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(databaseFilePath))
            {
                // Call
                TestDelegate profile = () => soilProfilesReader.ReadProfile();

                // Assert
                var exception = Assert.Throws<MacroStabilityInwardsSoilProfileReadException>(profile);
                string expectedMessage = new FileReaderErrorMessageBuilder(databaseFilePath)
                    .WithSubject("ondergrondschematisatie 'Profile'")
                    .Build("Het XML-document dat de geometrie beschrijft voor de laag is niet geldig.");
                Assert.AreEqual(expectedMessage, exception.Message);

                // Call
                MacroStabilityInwardsSoilProfile macroStabilityInwardsSoilProfile = soilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile2", macroStabilityInwardsSoilProfile.Name);
                Assert.AreEqual(3, macroStabilityInwardsSoilProfile.Layers.Count());

                Assert.IsTrue(TestHelper.CanOpenFileForWrite(testFile));
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithVerticalSegmentAtX_SkipsTheProfile()
        {
            // Setup
            const string testFile = "vertical2dGeometry.soil";
            string databaseFilePath = Path.Combine(testDataPath, testFile);
            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(databaseFilePath))
            {
                // Call
                TestDelegate profile = () => soilProfilesReader.ReadProfile();

                // Assert
                var exception = Assert.Throws<MacroStabilityInwardsSoilProfileReadException>(profile);
                string message = new FileReaderErrorMessageBuilder(databaseFilePath)
                    .WithSubject("ondergrondschematisatie 'Profile'")
                    .Build($"Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {85.2}.");
                Assert.AreEqual(message, exception.Message);

                // Call
                MacroStabilityInwardsSoilProfile macroStabilityInwardsSoilProfile = soilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile2", macroStabilityInwardsSoilProfile.Name);
                Assert.AreEqual(3, macroStabilityInwardsSoilProfile.Layers.Count());

                Assert.IsTrue(TestHelper.CanOpenFileForWrite(testFile));
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithoutValuesForLayerProperties_ReturnsProfileWithAllLayers()
        {
            // Setup
            const string testFile = "1dprofileNoValues.soil";
            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                MacroStabilityInwardsSoilProfile profile = soilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile", profile.Name);
                const int expectedNumberOfLayers = 3;
                Assert.AreEqual(expectedNumberOfLayers, profile.Layers.Count());
                CollectionAssert.AreEqual(Enumerable.Repeat(false, expectedNumberOfLayers), profile.Layers.Select(l => l.IsAquifer));
            }
        }

        [Test]
        [TestCase("incorrectValue2dStochastProperty.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'BelowPhreaticLevelMean\'.")]
        [TestCase("incorrectValue2dProperty.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'Color\'.")]
        [TestCase("invalidTop1dProfile.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'Top\'.")]
        [TestCase("invalidBottom1dProfile.soil", "Ondergrondschematisatie bevat geen geldige waarde in kolom \'Bottom\'.")]
        public void ReadProfile_DatabaseProfileWithLayerWithIncorrectDistributionForBelowPhreaticLevelProperty_ThrowsMacroStabilityInwardsSoilProfileReadException(
            string testFile,
            string expectedMessage)
        {
            // Setup
            string databaseFilePath = Path.Combine(testDataPath, testFile);
            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(databaseFilePath))
            {
                // Call
                TestDelegate profile = () => soilProfilesReader.ReadProfile();

                // Assert
                string exceptionMessage = Assert.Throws<MacroStabilityInwardsSoilProfileReadException>(profile).Message;
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
            const string testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);
            using (var reader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                // Call
                MacroStabilityInwardsSoilProfile profile = reader.ReadProfile();

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
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith2DProfile3Layers_ReturnsProfile()
        {
            // Setup
            const string testFile = "2dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);
            using (var reader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                // Call
                MacroStabilityInwardsSoilProfile profile = reader.ReadProfile();

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
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DProfileInTwoSegments_ReturnsOne1DProfile()
        {
            // Setup
            const string testFile = "1dProfileUsedInTwoSegments.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                var result = new Collection<MacroStabilityInwardsSoilProfile>();

                // Call
                while (soilProfilesReader.HasNext)
                {
                    result.Add(soilProfilesReader.ReadProfile());
                }

                // Assert
                Assert.AreEqual(1, soilProfilesReader.Count);
                Assert.AreEqual(1, result.Count);
                MacroStabilityInwardsSoilProfile macroStabilityInwardsSoilProfile = result[0];
                Assert.AreEqual("SoilProfile1D", macroStabilityInwardsSoilProfile.Name);
                Assert.AreEqual(SoilProfileType.SoilProfile1D, macroStabilityInwardsSoilProfile.SoilProfileType);
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith2DProfileInTwoSegments_ReturnsOne2DProfile()
        {
            // Setup
            const string testFile = "2dProfileUsedInTwoSegments.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(dbFile))
            {
                var result = new Collection<MacroStabilityInwardsSoilProfile>();

                // Call
                while (soilProfilesReader.HasNext)
                {
                    result.Add(soilProfilesReader.ReadProfile());
                }

                // Assert
                Assert.AreEqual(1, soilProfilesReader.Count);
                Assert.AreEqual(1, result.Count);
                MacroStabilityInwardsSoilProfile macroStabilityInwardsSoilProfile = result[0];
                Assert.AreEqual("SoilProfile2D", macroStabilityInwardsSoilProfile.Name);
                Assert.AreEqual(SoilProfileType.SoilProfile2D, macroStabilityInwardsSoilProfile.SoilProfileType);
            }
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            const string testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            // Call
            TestDelegate call = () =>
            {
                using (var reader = new MacroStabilityInwardsSoilProfileReader(dbFile))
                {
                    reader.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Dispose_AfterConstruction_CorrectlyReleasesFile()
        {
            // Setup
            const string testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            // Call
            new MacroStabilityInwardsSoilProfileReader(dbFile).Dispose();

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenReadProfile_CorrectlyReleasesFile()
        {
            // Setup
            const string testFile = "1dprofile.soil";
            string dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            MacroStabilityInwardsSoilProfileReader soilProfilesReader = null;
            MacroStabilityInwardsSoilProfile profile;
            try
            {
                soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(dbFile);
                profile = soilProfilesReader.ReadProfile();
            }
            finally
            {
                // Call
                soilProfilesReader?.Dispose();
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
            const string testFile = "1dprofile.soil";
            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // When
                var result = new Collection<MacroStabilityInwardsSoilProfile>();
                var skipped = 0;

                while (soilProfilesReader.HasNext)
                {
                    try
                    {
                        result.Add(soilProfilesReader.ReadProfile());
                    }
                    catch
                    {
                        skipped++;
                    }
                }

                // Then
                Assert.AreEqual(0, skipped);
                Assert.AreEqual(1, soilProfilesReader.Count);
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
            const string testFile = "complete.soil";
            using (var soilProfilesReader = new MacroStabilityInwardsSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // When
                ICollection<MacroStabilityInwardsSoilProfile> result = new List<MacroStabilityInwardsSoilProfile>();
                var skipped = 0;

                while (soilProfilesReader.HasNext)
                {
                    try
                    {
                        result.Add(soilProfilesReader.ReadProfile());
                    }
                    catch
                    {
                        skipped++;
                    }
                }

                // Then
                Assert.AreEqual(0, skipped);
                Assert.AreEqual(26, soilProfilesReader.Count);
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

                MacroStabilityInwardsSoilProfile firstProfile = result.FirstOrDefault(l => l.Name == "Segment_36005_1D1");
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
                double[] layerTops = firstProfile.Layers.Select(l => l.Top).ToArray();
                CollectionAssert.AllItemsAreUnique(layerTops);
                CollectionAssert.AreEqual(expectedFirstProfileLayersTops, layerTops, new DoubleWithToleranceComparer(1e-6));

                MacroStabilityInwardsSoilProfile secondProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D1");
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
                    -25
                };
                double[] layer2Tops = secondProfile.Layers.Select(l => l.Top).ToArray();
                CollectionAssert.AllItemsAreUnique(layer2Tops);
                CollectionAssert.AreEqual(expectedSecondProfileLayersTops, layer2Tops, new DoubleWithToleranceComparer(1e-6));
            }
        }
    }
}