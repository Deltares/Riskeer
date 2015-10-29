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
        public void ReadProfile_NextNotCalled_ThrowsInvalidOperationException()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                // Call
                TestDelegate test = () =>
                {
                    pipingSoilProfileReader.ReadProfile();
                };
                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }
        }

        [Test]
        public void ReadProfile_AfterDatabaseHasBeenRead_ThrowsInvalidOperationException()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                while (pipingSoilProfileReader.Next())
                {
                    pipingSoilProfileReader.ReadProfile();
                }

                // Call
                TestDelegate test = () =>
                {
                    pipingSoilProfileReader.ReadProfile();
                };
                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }
        }

        [Test]
        public void Next_InvalidSchema_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var testFile = "empty.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                // Call
                TestDelegate test = () =>
                {
                    pipingSoilProfileReader.Next();
                };
                // Assert
                Assert.Throws<PipingSoilProfileReadException>(test);
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
            Assert.AreEqual(Resources.Error_PathMustBeSpecified, exception.Message);
        }

        [Test]
        public void Next_IncorrectFormatFile_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var dbName = "text";
            var dbFile = Path.Combine(testDataPath, dbName + ".txt");

            using (var pipingSoilProfileReader = new PipingSoilProfileReader(dbFile))
            {
                // Call
                TestDelegate test = () => pipingSoilProfileReader.Next();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(test);
                Assert.AreEqual(String.Format(Resources.Error_SoilProfileReadFromDatabase, dbName), exception.Message);
            }
        }

        [Test]
        public void ReadProfile_DatabaseWith1DProfile3Layers_ReturnsProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);
            var reader = new PipingSoilProfileReader(dbFile);
            PipingSoilProfile profile;
            var skipped = 0;
            // Call
            while (reader.Next())
            {
                try
                {
                    profile = reader.ReadProfile();
                }
                catch (PipingSoilProfileReadException)
                {
                    skipped++;
                }
            } 

            // Assert
            Assert.AreEqual(0, skipped);
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
                while (pipingSoilProfilesReader.Next())
                {
                    result.Add(pipingSoilProfilesReader.ReadProfile());
                }

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(result[0].Name, result[1].Name);
            }
        }

        [Test]
        public void Read_DatabaseWithNullValueForBottom_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var testFile = "invalidbottom.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                pipingSoilProfilesReader.Next();

                // Call
                TestDelegate test = () => pipingSoilProfilesReader.ReadProfile();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(test);
                var message = String.Format(Resources.PipingSoilProfileReader_InvalidValueOnColumn, "Bottom");
                Assert.AreEqual(message, exception.Message);
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
                var skippedProfiles = 0;

                while (pipingSoilProfilesReader.Next())
                {
                    try
                    {
                        result.Add(pipingSoilProfilesReader.ReadProfile());
                    }
                    catch
                    {
                        skippedProfiles++;
                    }
                }

                // Assert
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

                while (pipingSoilProfilesReader.Next())
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
                Assert.AreEqual(15, skipped);
                Assert.AreEqual(11, result.Count);
                CollectionAssert.AreEquivalent(new[]
                {
                    "AD640M00_Segment_36005_1D2",
                    "Segment_36005_1D10",
                    "Segment_36005_1D2",
                    "Segment_36005_1D3",
                    "Segment_36005_1D5",
                    "Segment_36005_1D6",
                    "Segment_36005_1D7",
                    "Segment_36005_1D8",
                    "Segment_36005_1D9",
                    "Segment_36006_1D5",
                    "Segment_36006_1D6"
                }, result.Select(p => p.Name));

                CollectionAssert.AreEquivalent(new[]
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
                    -45
                }, result.Select(p => p.Bottom));

                CollectionAssert.AreEquivalent(new[]
                {
                    8,
                    3,
                    6,
                    6,
                    5,
                    6,
                    4,
                    4,
                    3,
                    5,
                    5
                }, result.Select(p => p.Layers.Count()));

                var profile2D = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D2");
                Assert.NotNull(profile2D);
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
                CollectionAssert.AllItemsAreUnique(profile2D.Layers.Select(l => l.Top));
                CollectionAssert.AreEqual(expectedSecondProfileLayersTops, profile2D.Layers.Select(l => l.Top));
            }
        }
    }
}