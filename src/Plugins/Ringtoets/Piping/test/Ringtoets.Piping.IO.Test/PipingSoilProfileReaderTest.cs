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
        public void ReadSoilProfiles_NullOrEmpty_ThrowsArgumentException(string fileName)
        {
            // Call
            TestDelegate test = () => new PipingSoilProfileReader(fileName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(Resources.Error_PathMustBeSpecified, exception.Message);
        }

        [Test]
        public void ReadSoilProfiles_IncorrectFormatFile_ThrowsSqLiteException()
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
        public void ReadProfiles_DatabaseWith1DAnd2DProfilesWithSameName_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var testFile = "Combined1dAnd2d.soil";
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
        [SetCulture("nl-NL")]
        public void ReadProfiles_NLDatabaseWith1DProfile_ReturnsCompleteSoilProfile()
        {
            ReadProfiles_DatabaseWith1DProfile_ReturnsCompleteSoilProfile();
        }

        [Test]
        [SetCulture("en-US")]
        public void ReadProfiles_ENDatabaseWith1DProfile_ReturnsCompleteSoilProfile()
        {
            ReadProfiles_DatabaseWith1DProfile_ReturnsCompleteSoilProfile();
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

        public void ReadProfiles_DatabaseWith1DProfile_ReturnsCompleteSoilProfile()
        {
            // Setup
            var testFile = "1dprofile.soil";
            using (var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile)))
            {
                // Call
                PipingSoilProfile[] result = pipingSoilProfilesReader.Read().ToArray();

                // Assert
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(-12, result[0].Bottom);
                CollectionAssert.AreEquivalent(new[] { 60, 0.63, -7.0 }, result[0].Layers.Select(l => l.Top));
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
                Assert.AreEqual(36, result.Length);
                CollectionAssert.AreEquivalent(new[]
                {
                    "10Y_005_STBI_p",
                    "10Y_024_STBI_p",
                    "10Y_041_STBI_p",
                    "10Y_042_STBI_p",
                    "10Y_043_STBI_p",
                    "10Y_054_STBI_p",
                    "10Y_066_STBI_p",
                    "10Y_090_STBI_p",
                    "10Y_102_STBI_p",
                    "10Y_117_STBI_p",
                    "10Y_130_STBI_p",
                    "10Y_149_STBI_p",
                    "10Z_157_STBI_p",
                    "10Z_165_STBI_p",
                    "10Z_181_STBI_p",
                    "10Z_186_STBI_p",
                    "10Z_199_STBI_p",
                    "10Z_228_STBI_p",
                    "10Z_258_STBI_p",
                    "10Z_275_STBI_p",
                    "10Z_282_STBI_p",
                    "10Z_286_STBI_p",
                    "10Z_311_STBI_p",
                    "10Z_327_STBI_p",
                    "10Z_352_STBI_p",
                    "10Z_358_STBI_p",
                    "10Z_369_STBI_p",
                    "10Z_380_STBI_p",
                    "10Z_390_STBI_p",
                    "10Z_400_STBI_p",
                    "10Z_421_STBI_p",
                    "10Z_432_STBI_p",
                    "10Z_460_STBI_p",
                    "10Y_091_STBI_p",
                    "10Y_005_STBI",
                    "10Y_041_STBI"
                }, result.Select(p => p.Name));

                CollectionAssert.AreEquivalent(new[]
                {
                    -10,
                    -12.0,
                    -10.8,
                    -10,
                    -21.5,
                    -21.5,
                    -21.5,
                    -22.0,
                    -24.0,
                    -21.5,
                    -21.5,
                    -22.0,
                    -24.17,
                    -24.8,
                    -25.0,
                    -24.0,
                    -22.8,
                    -23.8,
                    -24.2,
                    -24.5,
                    -24.0,
                    -23.7,
                    -24.4,
                    -24.0,
                    -31.5,
                    -23.2,
                    -23.9,
                    -23.1,
                    -23.5,
                    -23.125,
                    -22.5,
                    -21.88,
                    -22.0,
                    40.0,
                    40.0,
                    40.0
                }, result.Select(p => p.Bottom));

                CollectionAssert.AreEquivalent(new[]
                {
                    6,
                    3,
                    2,
                    3,
                    2,
                    2,
                    2,
                    2,
                    3,
                    2,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                    2,
                    2,
                    2,
                    2,
                    3,
                    2,
                    3,
                    3,
                    2,
                    2,
                    3,
                    2,
                    2,
                    2,
                    2,
                    2,
                    3,
                    1,
                    1,
                    1
                }, result.Select(p => p.Layers.Count()));

                var firstProfile = result.FirstOrDefault(l => l.Name == "10Y_005_STBI");
                Assert.NotNull(firstProfile);
                var expectedFirstProfileLayersTops = new[]
                {
                    -3.5,
                    -1.2,
                    0.63,
                    1.088434916,
                    1.947578092,
                    2.473341176
                };
                CollectionAssert.AllItemsAreUnique(firstProfile.Layers.Select(l => l.Top));
                Assert.AreEqual(6, firstProfile.Layers.Count(l => expectedFirstProfileLayersTops.Contains(l.Top, new DoubleComparer())));

                var secondProfile = result.FirstOrDefault(l => l.Name == "10Y_041_STBI");
                Assert.NotNull(secondProfile);
                var expectedSecondProfileLayersTops = new[]
                {
                    -1.5,
                    -1.180438596,
                    0.333203067
                };
                CollectionAssert.AllItemsAreUnique(secondProfile.Layers.Select(l => l.Top));
                Assert.AreEqual(3, secondProfile.Layers.Count(l => expectedSecondProfileLayersTops.Contains(l.Top, new DoubleComparer())));
            }
        }
    }

    internal class DoubleComparer : IEqualityComparer<double>
    {
        public bool Equals(double x, double y)
        {
            return Math.Abs(x - y) < Math2D.EpsilonForComparisons;
        }

        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }
    }
}