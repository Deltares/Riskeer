using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core.Common.TestUtils;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.IO.Test.TestHelpers;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    public class PipingSoilProfileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => new PipingSoilProfileReader(testFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(Resources.Error_File_does_not_exist, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () => new PipingSoilProfileReader(fileName).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(Resources.Error_Path_must_be_specified, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsPipingCriticalFileReadException(string dbName)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => new PipingSoilProfileReader(dbFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(String.Format(Resources.Error_SoilProfile_read_from_database, dbName), exception.Message);
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_EmptyDatabase_HasNextFalse()
        {
            // Setup
            var dbName = "emptyschema.soil";
            var dbFile = Path.Combine(testDataPath, dbName);


            // Call
            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                // Assert
                Assert.IsFalse(pipingSoilProfileReader.HasNext);
            }
        }

        [Test]
        public void Constructor_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            var version = "15.0.5.0";
            var dbName = "incorrectversion.soil";
            var dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => new PipingSoilProfileReader(dbFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(String.Format(Resources.PipingSoilProfileReader_Database_incorrect_version_requires_Version_0_, version), exception.Message);
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadProfile_AfterDatabaseHasBeenRead_ReturnsNull()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

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
            var dbFile = Path.Combine(testDataPath, testFile);

            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile))
            {
                var result = new Collection<PipingSoilProfile>();

                // Call
                while (pipingSoilProfilesReader.HasNext)
                {
                    result.Add(pipingSoilProfilesReader.ReadProfile());
                }

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(result[0].Name, result[1].Name);
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithInvalid2dLayerGeometry_SkipsTheProfile()
        {
            // Setup
            var testFile = "invalid2dGeometry.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(profile);
                Assert.AreEqual(Resources.SoilLayer2DReader_Geometry_contains_no_valid_xml, exception.Message);

                // Call
                var pipingSoilProfile = pipingSoilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile2", pipingSoilProfile.Name);
                Assert.AreEqual(3, pipingSoilProfile.Layers.Count());

                Assert.IsTrue(FileHelper.CanOpenFileForWrite(testFile));
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithVerticalSegmentAtX_SkipsTheProfile()
        {
            // Setup
            var testFile = "vertical2dGeometry.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(profile);
                var message = String.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_, 85.2);
                Assert.AreEqual(message, exception.Message);

                // Call
                var pipingSoilProfile = pipingSoilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile2", pipingSoilProfile.Name);
                Assert.AreEqual(3, pipingSoilProfile.Layers.Count());

                Assert.IsTrue(FileHelper.CanOpenFileForWrite(testFile));
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
                var profile = pipingSoilProfilesReader.ReadProfile();

                // Assert
                Assert.AreEqual("Profile", profile.Name);
                Assert.AreEqual(3, profile.Layers.Count());
                CollectionAssert.AreEqual(Enumerable.Repeat(false, 3), profile.Layers.Select(l => l.IsAquifer));
                CollectionAssert.AreEqual(Enumerable.Repeat((double?)null, 3), profile.Layers.Select(l => l.AbovePhreaticLevel));
                CollectionAssert.AreEqual(Enumerable.Repeat((double?)null, 3), profile.Layers.Select(l => l.BelowPhreaticLevel));
                CollectionAssert.AreEqual(Enumerable.Repeat((double?)null, 3), profile.Layers.Select(l => l.DryUnitWeight));
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithInvalidBottom_ReturnsNoProfile()
        {
            // Setup
            var testFile = "invalidBottom1dProfile.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exceptionMessage = Assert.Throws<PipingSoilProfileReadException>(profile).Message;
                var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_Column_1_, "Profile", "Bottom");
                Assert.AreEqual(message, exceptionMessage);
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithLayerWithInvalidTop_ReturnsNoProfile()
        {
            // Setup
            var testFile = "invalidTop1dProfile.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exceptionMessage = Assert.Throws<PipingSoilProfileReadException>(profile).Message;
                var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_Column_1_, "Profile", "Top");
                Assert.AreEqual(message, exceptionMessage);
            }
        }

        [Test]
        public void ReadProfile_DatabaseProfileWithLayerWithInvalidLayerProperty_ReturnsNoProfile()
        {
            // Setup
            var testFile = "incorrectValue2dProperty.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exceptionMessage = Assert.Throws<PipingSoilProfileReadException>(profile).Message;
                var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_Column_1_, "Profile", "DryUnitWeight");
                Assert.AreEqual(message, exceptionMessage);
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DProfile3Layers_ReturnsProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);
            using (var reader = new PipingSoilProfileReader(dbFile))
            {
                // Call
                var profile = reader.ReadProfile();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    false,
                    false,
                    true
                }, profile.Layers.Select(l => l.IsAquifer));
                CollectionAssert.AreEqual(new[]
                {
                    0.001,
                    0.001,
                    0.001
                }, profile.Layers.Select(l => l.AbovePhreaticLevel));
                CollectionAssert.AreEqual(new[]
                {
                    0.001,
                    0.001,
                    0.001
                }, profile.Layers.Select(l => l.BelowPhreaticLevel));
                CollectionAssert.AreEqual(new double?[]
                {
                    null,
                    null,
                    null
                }, profile.Layers.Select(l => l.DryUnitWeight));
            }
        }

        [Test]
        public void Dispose_AfterConstruction_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            // Call
            new PipingSoilProfileReader(dbFile).Dispose();

            // Assert
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenReadProfile_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            PipingSoilProfileReader pipingSoilProfilesReader = null;
            PipingSoilProfile profile = null;
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
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
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
            GivenACompleteDatabase_WhenReadingTheCompleteDatabase_Returns2ProfilesWithLayersAndGeometries();
        }

        [Test]
        [SetCulture("en-US")]
        public void GivenACompleteDatabaseAndEnglishLocale_WhenReadingTheCompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            GivenACompleteDatabase_WhenReadingTheCompleteDatabase_Returns2ProfilesWithLayersAndGeometries();
        }

        private void GivenDatabaseWith1DProfile_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
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

                // Assert
                Assert.AreEqual(0, skipped);
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

        private void GivenACompleteDatabase_WhenReadingTheCompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            // Setup
            var testFile = "complete.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
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

                // Assert
                Assert.AreEqual(0, skipped);
                Assert.AreEqual(26, result.Count);
                CollectionAssert.AreEqual(new[]
                {
                    "AD640M00_Segment_36005_1D1",
                    "AD640M00_Segment_36005_1D2",
                    "Segment_36005_1D1",
                    "Segment_36005_1D10",
                    "Segment_36005_1D2",
                    "Segment_36005_1D3",
                    "Segment_36005_1D4",
                    "Segment_36005_1D5",
                    "Segment_36005_1D6",
                    "Segment_36005_1D7",
                    "Segment_36005_1D8",
                    "Segment_36005_1D9",
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
                }, result.Select(p => p.Name));

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
                }, result.Select(p => p.Bottom));

                CollectionAssert.AreEqual(new[]
                {
                    9,
                    7,
                    8,
                    3,
                    6,
                    6,
                    5,
                    5,
                    6,
                    4,
                    4,
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
                }, result.Select(p => p.Layers.Count()));

                var firstProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D1");
                Assert.NotNull(firstProfile);
                var expectedFirstProfileLayersTops = new[]
                {
                    5.9075,
                    3.250,
                    2.750,
                    1.250,
                    1.0,
                    -2.5,
                    -13,
                    -17,
                    -25,
                };
                CollectionAssert.AllItemsAreUnique(firstProfile.Layers.Select(l => l.Top));
                CollectionAssert.AreEqual(expectedFirstProfileLayersTops, firstProfile.Layers.Select(l => l.Top), new DoubleWithToleranceComparer(1e-6));

                var secondProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D2");
                Assert.NotNull(secondProfile);
                var expectedSecondProfileLayersTops = new[]
                {
                    5.9075,
                    3.25,
                    -0.5,
                    -0.75,
                    -13,
                    -17,
                    -25,
                };
                CollectionAssert.AllItemsAreUnique(secondProfile.Layers.Select(l => l.Top));
                CollectionAssert.AreEqual(expectedSecondProfileLayersTops, secondProfile.Layers.Select(l => l.Top), new DoubleWithToleranceComparer(1e-6));
            }
        }
    }
}