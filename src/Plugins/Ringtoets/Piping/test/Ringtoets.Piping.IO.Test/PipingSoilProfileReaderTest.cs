using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DelftTools.TestUtils;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Calculation;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Test.TestHelpers;

namespace Ringtoets.Piping.IO.Test
{
    public class PipingSoilProfileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Plugins.Wti.WtiIOPath, "PipingSoilProfilesReader");

        [Test]
        public void Constructor_CorrectPath_ReturnsNewInstance()
        {
            // Setup
            var testFile = "empty.soil";

            // Call
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Assert
                Assert.NotNull(pipingSoilProfilesReader);
            }
        }

        [Test]
        public void Dispose_AfterConstruction_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "empty.soil";
            var dbFile = Path.Combine(testDataPath, testFile);
            var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile);

            // Call
            pipingSoilProfilesReader.Dispose();

            // Assert
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenRead_CorrectlyReleasesFile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            var dbFile = Path.Combine(testDataPath, testFile);

            // Precondition
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            var pipingSoilProfilesReader = new PipingSoilProfileReader(dbFile);
            pipingSoilProfilesReader.Read();

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
        public void Read_NullOrEmpty_ThrowsArgumentException(string fileName)
        {
            // Call
            TestDelegate test = () => new PipingSoilProfileReader(fileName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(Resources.Error_PathMustBeSpecified, exception.Message);
        }

        [Test]
        public void Read_IncorrectFormatFile_ThrowsSqLiteException()
        {
            // Setup
            var dbName = "text";
            var testFile = Path.Combine(testDataPath, dbName + ".txt");

            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(testFile))
            {
                // Call
                TestDelegate test = () => pipingSoilProfilesReader.Read();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(test);
                Assert.AreEqual(String.Format(Resources.Error_SoilProfileReadFromDatabase, dbName), exception.Message);
            }
        }

        [Test]
        public void Read_DatabaseWith1DAnd2DProfilesWithSameName_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var testFile = "combined1d2d.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate test = () => pipingSoilProfilesReader.Read();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(test);
                Assert.AreEqual(Resources.Error_CannotCombine2DAnd1DLayersInProfile, exception.Message);
            }
        }

        [Test]
        public void Read_DatabaseWithNullValueForBottom_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var testFile = "invalidbottom.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                TestDelegate test = () => pipingSoilProfilesReader.Read();

                // Assert
                var exception = Assert.Throws<PipingSoilProfileReadException>(test);
                var message = String.Format(Resources.PipingSoilProfileReader_InvalidValueOnColumn, "Bottom");
                Assert.AreEqual(message, exception.Message);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Read_NLDatabaseWith1DProfile_ReturnsCompleteSoilProfile()
        {
            Read_DatabaseWith1DProfile_ReturnsCompleteSoilProfile();
        }

        [Test]
        [SetCulture("en-US")]
        public void Read_ENDatabaseWith1DProfile_ReturnsCompleteSoilProfile()
        {
            Read_DatabaseWith1DProfile_ReturnsCompleteSoilProfile();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ReadSoilProfiles_NLCompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            ReadSoilProfiles_CompleteDatabase_Returns2ProfilesWithLayersAndGeometries();
        }

        [Test]
        [SetCulture("en-US")]
        public void ReadSoilProfiles_ENCompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            ReadSoilProfiles_CompleteDatabase_Returns2ProfilesWithLayersAndGeometries();
        }

        public void Read_DatabaseWith1DProfile_ReturnsCompleteSoilProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                PipingSoilProfile[] result = pipingSoilProfilesReader.Read().ToArray();

                // Assert
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(-2.1, result[0].Bottom);
                CollectionAssert.AreEqual(new[]
                {
                    3.3,
                    2.2,
                    1.1
                }, result[0].Layers.Select(l => l.Top));
            }
        }

        private void ReadSoilProfiles_CompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            // Setup
            var testFile = "complete.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                PipingSoilProfile[] result = pipingSoilProfilesReader.Read().ToArray();

                // Assert
                Assert.AreEqual(26, result.Length);
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
                    8,8,8,6,6,5,5,6,4,4,3,3,7,7,7,5,5,5,6,6,4,5,4,4,2,3
                }, result.Select(p => p.Layers.Count()));

                var firstProfile = result.FirstOrDefault(l => l.Name == "AD640M00_Segment_36005_1D1");
                Assert.NotNull(firstProfile);
                var expectedFirstProfileLayersTops = new[]
                {
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