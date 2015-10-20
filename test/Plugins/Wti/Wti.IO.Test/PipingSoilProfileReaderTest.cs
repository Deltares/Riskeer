using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DelftTools.TestUtils;
using NUnit.Framework;
using Wti.Data;
using Wti.IO.Exceptions;
using Wti.IO.Properties;

namespace Wti.IO.Test
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
        public void ReadSoilProfiles_CompleteDatabase_Returns2ProfilesWithLayersAndGeometries()
        {
            // Setup
            var testFile = "complete.soil";
            var pipingSoilProfilesReader = new PipingSoilProfileReader(Path.Combine(testDataPath, testFile));

            var firstProfileFirstLayerOuterLoop = new HashSet<Point3D>
            {
                new Point3D{X=0,Y=0,Z=-10},
                new Point3D{X=0,Y=0,Z=-3.5},
                new Point3D{X=70.208,Y=0,Z=-3.5},
                new Point3D{X=73.91,Y=0,Z=-3.5},
                new Point3D{X=80.78,Y=0,Z=-3.5},
                new Point3D{X=80.78,Y=0,Z=-10},
            };

            var secondProfileSecondLayerOuterLoop = new HashSet<Point3D>
            {
                new Point3D{X=87.516,Y=0,Z=-1.5},
                new Point3D{X=87.567000000000007,Y=0,Z=-0.70000000000000007},
                new Point3D{X=101.4,Y=0,Z=-0.70000000000000007},
                new Point3D{X=101.4,Y=0,Z=-1.5},
            };

            // Call
            IEnumerable<PipingSoilProfile> result = pipingSoilProfilesReader.Read().ToArray();

            // Assert
            Assert.AreEqual(2, result.Count());
            var firstProfile = result.SingleOrDefault(psp => psp.Name == "10Y_005_STBI");
            Assert.NotNull(firstProfile);
            Assert.AreEqual(13, firstProfile.Count());
            CollectionAssert.AreEqual(firstProfileFirstLayerOuterLoop, firstProfile.Layers.ToArray()[0].OuterLoop);
            Assert.IsNull(firstProfile.Layers.ToArray()[0].InnerLoop);

            var secondProfile = result.SingleOrDefault(psp => psp.Name == "10Y_041_STBI");
            Assert.NotNull(secondProfile);
            Assert.AreEqual(14, secondProfile.Count());
            CollectionAssert.AreEqual(secondProfileSecondLayerOuterLoop, secondProfile.Layers.ToArray()[1].OuterLoop);
            Assert.IsNull(secondProfile.Layers.ToArray()[1].InnerLoop);
        }
    }
}