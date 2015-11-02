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
using Ringtoets.Piping.IO.Test.TestHelpers;

namespace Ringtoets.Piping.IO.Test
{
    public class PipingSoilProfileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsFileNotFoundException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () => new PipingSoilProfileReader(testFile);

            // Assert
            var exception = Assert.Throws<FileNotFoundException>(test);
            Assert.AreEqual(String.Format(Resources.Error_File_0_does_not_exist, testFile), exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsArgumentException(string fileName)
        {
            // Call
            TestDelegate test = () => new PipingSoilProfileReader(fileName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(Resources.Error_Path_must_be_specified, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsPipingSoilProfileReadException(string dbName)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => new PipingSoilProfileReader(dbFile);

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual(String.Format(Resources.Error_SoilProfile_read_from_database, dbName), exception.Message);
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadProfile_AfterDatabaseHasBeenRead_ThrowsInvalidOperationException()
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

                // Call
                TestDelegate test = () => { pipingSoilProfileReader.ReadProfile(); };
                // Assert
                Assert.Throws<InvalidOperationException>(test);
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

            var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile);

            // Call
            pipingSoilProfilesReader.Dispose();

            // Assert
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenPreparedLayer_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile);

            // Call
            pipingSoilProfilesReader.Dispose();

            // Assert
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadProfile_DatabaseWith1DProfile3Layers_ReturnsProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);
            var reader = new PipingSoilProfileReader(dbFile);

            PipingSoilProfile profile;

            // Call
            profile = reader.ReadProfile();

            // Assert
            Assert.AreEqual(3,profile.Layers.Count());
        }

        [Test]
        public void Read_DatabaseWith1DAnd2DProfilesWithSameName_ReturnTwoProfilesWithSameName()
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
        public void Read_DatabaseProfileWithInvalid2dLayerGeometry_SkipsTheProfile()
        {
            // Setup
            var testFile = "invalid2dGeometry.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate profile = () => pipingSoilProfilesReader.ReadProfile();
                Func<PipingSoilProfile> profile2 = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(profile);
                var message = String.Format(Resources.PipingSoilProfileReader_CouldNotParseGeometryOfLayer_0_InProfile_1_, 1, "Profile");
                Assert.AreEqual(message, exception.Message);

                var pipingSoilProfile = profile2();
                Assert.AreEqual("Profile2", pipingSoilProfile.Name);
                Assert.AreEqual(3, pipingSoilProfile.Layers.Count());

                Assert.IsTrue(FileHelper.CanOpenFileForWrite(testFile));
            }
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

        public void GivenDatabaseWith1DProfile_WhenReadingTheCompleteDatabase_ReturnsCompleteSoilProfile()
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
                CollectionAssert.AreEquivalent(new[]
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
                }, result.Select(p => p.Name));

                CollectionAssert.AreEquivalent(new[]
                {
                    -39.999943172545208,
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

                CollectionAssert.AreEquivalent(new[]
                {
                    9,
                    8,
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
                }, result.Select(p => p.Layers.Count()));

                var firstProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D1");
                Assert.NotNull(firstProfile);
                var expectedFirstProfileLayersTops = new[]
                {
                    5.9075002357930027,
                    3.12499857931363,
                    2.3749957379408912,
                    1.1874992896568151,
                    0.12499005519541202,
                    -5.1250298344137661,
                    -14.000011365490959,
                    -19.000022730981915,
                    -30.000056827454785,
                };
                CollectionAssert.AllItemsAreUnique(firstProfile.Layers.Select(l => l.Top));
                CollectionAssert.AreEqual(expectedFirstProfileLayersTops, firstProfile.Layers.Select(l => l.Top));

                var secondProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D2");
                Assert.NotNull(secondProfile);
                var expectedSecondProfileLayersTops = new[]
                {
                    5.9075002357930053,
                    4.3475186908270276,
                    3.25,
                    -0.5,
                    -0.75,
                    -13,
                    -17,
                    -25,
                };
                CollectionAssert.AllItemsAreUnique(secondProfile.Layers.Select(l => l.Top));
                CollectionAssert.AreEqual(expectedSecondProfileLayersTops, secondProfile.Layers.Select(l => l.Top));
            }
        }
    }
}