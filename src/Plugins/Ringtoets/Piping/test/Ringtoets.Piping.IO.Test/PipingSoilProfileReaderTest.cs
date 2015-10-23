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
            var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile));
             
            // Assert
            Assert.NotNull(pipingSoilProfilesReader);
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
            var pipingSoilProfilesReader = new PipingSoilProfileReader(testFile);

            // Call
            TestDelegate test = () => pipingSoilProfilesReader.Read();

            // Assert
            var exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual(String.Format(Resources.Error_SoilProfileReadFromDatabase, dbName), exception.Message);
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

        private void ReadSoilProfiles_CompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            // Setup
            var testFile = "complete.soil";
            var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile));

            // Call
            PipingSoilProfile[] result = pipingSoilProfilesReader.Read().ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            var firstProfile = result.SingleOrDefault(psp => psp.Name == "10Y_005_STBI");
            Assert.NotNull(firstProfile);

            Assert.AreEqual(-10, firstProfile.Bottom);
            Assert.AreEqual(6, firstProfile.Layers.Count());
            var expected = new[]
            {
                -3.5,
                -1.2,
                0.63,
                1.088434916,
                1.947578092,
                2.473341176
            };
            CollectionAssert.AllItemsAreUnique(firstProfile.Layers.Select(l => l.Top));
            Assert.AreEqual(6, firstProfile.Layers.Count(l => expected.Contains(l.Top, new DoubleComparer())));
        }
    }

    internal class DoubleComparer : IEqualityComparer<double> {
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