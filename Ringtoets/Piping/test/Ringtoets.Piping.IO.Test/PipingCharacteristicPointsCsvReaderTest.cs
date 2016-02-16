using System;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Test
{
    public class PipingCharacteristicPointsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "CharacteristicPoints" + Path.DirectorySeparatorChar);

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void ParameterdConstructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new PipingCharacteristicPointsCsvReader(path);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Path_must_be_specified);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ParameterdConstructor_InvalidPathCharactersInPath_ThrowsArgumentException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidLocations.csv");

            var invalidCharacters = Path.GetInvalidPathChars();

            var corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => new PipingCharacteristicPointsCsvReader(corruptPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(String.Format(UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                                                                                     String.Join(", ", Path.GetInvalidFileNameChars())));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ParametersConstructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new PipingCharacteristicPointsCsvReader(testDataPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = new FileReaderErrorMessageBuilder(testDataPath).Build(UtilsResources.Error_Path_must_not_point_to_folder);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ParameterdConstructor_AnyPath_ExpectedValues()
        {
            // Setup
            const string fakeFilePath = @"I\Dont\Really\Exist";

            // Call
            using (var reader = new PipingCharacteristicPointsCsvReader(fakeFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        [TestCase("0locations.krp.csv", 0)]
        [TestCase("1location.krp.csv", 1)]
        [TestCase("2locations.krp.csv", 2)]
        [TestCase("2locations_empty_line.krp.csv", 2)]
        public void GetLocationsCount_DifferentFiles_ShouldReturnNumberOfLocations(string fileName, int expectedCount)
        {
            // Setup
            string path = Path.Combine(testDataPath, fileName);
            using (var reader = new PipingCharacteristicPointsCsvReader(path))
            {
                // Call
                var result = reader.GetLocationsCount();

                // Assert
                Assert.AreEqual(expectedCount, result);
            }
        }

        [Test]
        public void GetLocationsCount_FileCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new PipingCharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedError = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_File_does_not_exist);
                Assert.AreEqual(expectedError, exception.Message);
                Assert.IsInstanceOf<FileNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void GetLocationsCount_DirectoryCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "..", "this_folder_does_not_exist", "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new PipingCharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Directory_missing);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void GetLocationsCount_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "empty.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingCharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(UtilsResources.Error_File_empty);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void GetLocationsCount_InvalidHeader1_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "1location_invalid_header.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingCharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(Resources.PipingCharacteristicPointsCsvReader_File_invalid_header);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ReadLine_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocationsWithCultureNL_ReturnCreatedCharacteristicPointsLocation()
        {
            ReadLine_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation();
        }

        [Test]
        [SetCulture("en-US")]
        public void ReadLine_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocationsWithCultureEN_ReturnCreatedCharacteristicPointsLocation()
        {
            ReadLine_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation();
        }

        private void ReadLine_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations.krp.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingCharacteristicPointsCsvReader(path))
            {
                // Call
                var location1 = reader.ReadLine();
                var location2 = reader.ReadLine();

                // Assert

                #region 1st location

                Assert.AreEqual("Rotterdam1", location1.Name);

                Assert.AreEqual(new Point3D
                {
                    X = 100,
                    Y = 0,
                    Z = -0.63
                }, location1.SurfaceLevelInside);

                Assert.AreEqual(new Point3D
                {
                    X = 60.83,
                    Y = 0,
                    Z = -0.57
                }, location1.DitchPolderSide);

                Assert.AreEqual(new Point3D
                {
                    X = 59.36,
                    Y = 0,
                    Z = -1.87
                }, location1.BottomDitchPolderSide);

                Assert.AreEqual(new Point3D
                {
                    X = 57.99,
                    Y = 0,
                    Z = -1.9
                }, location1.BottomDitchDikeSide);

                Assert.AreEqual(new Point3D
                {
                    X = 55.37,
                    Y = 0,
                    Z = -0.31
                }, location1.DitchDikeSide);

                Assert.AreEqual(new Point3D
                {
                    X = 55.37,
                    Y = 0,
                    Z = -0.31
                }, location1.DikeToeAtPolder);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location1.TopShoulderInside);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location1.ShoulderInside);

                Assert.AreEqual(new Point3D
                {
                    X = 40.17,
                    Y = 0,
                    Z = 2.63
                }, location1.DikeTopAtPolder);

                Assert.AreEqual(new Point3D
                {
                    X = 40.85,
                    Y = 0,
                    Z = 2.44
                }, location1.TrafficLoadInside);

                Assert.AreEqual(new Point3D
                {
                    X = 38.35,
                    Y = 0,
                    Z = 2.623
                }, location1.TrafficLoadOutside);

                Assert.AreEqual(new Point3D
                {
                    X = 35.95,
                    Y = 0,
                    Z = 2.61
                }, location1.DikeTopAtRiver);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location1.ShoulderOutisde);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location1.TopShoulderOutside);

                Assert.AreEqual(new Point3D
                {
                    X = 29.1,
                    Y = 0,
                    Z = -0.2
                }, location1.DikeToeAtRiver);

                Assert.AreEqual(new Point3D
                {
                    X = 0,
                    Y = 0,
                    Z = -0.71
                }, location1.SurfaceLevelOutside);

                Assert.AreEqual(new Point3D
                {
                    X = 23.703,
                    Y = 0,
                    Z = -1.5
                }, location1.DikeTableHeight);

                #endregion

                #region 2nd location

                Assert.AreEqual("Amsterdam1", location2.Name);

                Assert.AreEqual(new Point3D
                {
                    X = 100,
                    Y = 0,
                    Z = -0.47
                }, location2.SurfaceLevelInside);

                Assert.AreEqual(new Point3D
                {
                    X = 58.42,
                    Y = 0,
                    Z = -0.6
                }, location2.DitchPolderSide);

                Assert.AreEqual(new Point3D
                {
                    X = 56.2,
                    Y = 0,
                    Z = -1.98
                }, location2.BottomDitchPolderSide);

                Assert.AreEqual(new Point3D
                {
                    X = 56.2,
                    Y = 0,
                    Z = -1.98
                }, location2.BottomDitchDikeSide);

                Assert.AreEqual(new Point3D
                {
                    X = 53.48,
                    Y = 0,
                    Z = -0.49
                }, location2.DitchDikeSide);

                Assert.AreEqual(new Point3D
                {
                    X = 53.48,
                    Y = 0,
                    Z = -0.49
                }, location2.DikeToeAtPolder);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location2.TopShoulderInside);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location2.ShoulderInside);

                Assert.AreEqual(new Point3D
                {
                    X = 38.17,
                    Y = 0,
                    Z = 3.04
                }, location2.DikeTopAtPolder);

                Assert.AreEqual(new Point3D
                {
                    X = 37.73,
                    Y = 0,
                    Z = 3.13
                }, location2.TrafficLoadInside);

                Assert.AreEqual(new Point3D
                {
                    X = 35.23,
                    Y = 0,
                    Z = 3.253
                }, location2.TrafficLoadOutside);

                Assert.AreEqual(new Point3D
                {
                    X = 32.77,
                    Y = 0,
                    Z = 3.11
                }, location2.DikeTopAtRiver);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location2.ShoulderOutisde);

                Assert.AreEqual(new Point3D
                {
                    X = -1,
                    Y = -1,
                    Z = -1
                }, location2.TopShoulderOutside);

                Assert.AreEqual(new Point3D
                {
                    X = 19.61,
                    Y = 0,
                    Z = -0.05
                }, location2.DikeToeAtRiver);

                Assert.AreEqual(new Point3D
                {
                    X = 0,
                    Y = 0,
                    Z = -0.33
                }, location2.SurfaceLevelOutside);

                Assert.AreEqual(new Point3D
                {
                    X = 17.32,
                    Y = 0,
                    Z = -1.52
                }, location2.DikeTableHeight);

                #endregion
            }
        }
    }
}