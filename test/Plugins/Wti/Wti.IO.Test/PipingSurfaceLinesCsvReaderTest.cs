using System;
using System.IO;
using System.Linq;

using DelftTools.TestUtils;

using NUnit.Framework;

using Wti.IO.Exceptions;

using IOResources = Wti.IO.Properties.Resources;

namespace Wti.IO.Test
{
    [TestFixture]
    public class PipingSurfaceLinesCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Plugins.Wti.WtiIOPath, "PipingSurfaceLinesCsvReader" + Path.DirectorySeparatorChar);

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void ParameterdConstructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(path);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual(IOResources.Error_PathMustBeSpecified, exception.Message);
        }

        [Test]
        public void ParameterdConstructor_InvalidPathCharactersInPath_ThrowsArgumentException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            var invalidCharacters = Path.GetInvalidPathChars();

            var corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(corruptPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = String.Format(IOResources.Error_PathCannotContainCharacters_0_,
                String.Join(", ", Path.GetInvalidFileNameChars()));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ParametersConstructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(testDataPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual(IOResources.Error_PathMustNotPointToFolder, exception.Message);
        }

        [Test]
        public void ParameterdConstructor_AnyPath_ExpectedValues()
        {
            // Setup
            const string fakeFilePath = @"I\Dont\Really\Exist";

            // Call
            using (var reader = new PipingSurfaceLinesCsvReader(fakeFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnNumberOfSurfaceLines()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                int linesCount = reader.GetSurfaceLinesCount();

                // Assert
                Assert.AreEqual(2, linesCount);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_OpenedValidFileWithHeaderAndNoSurfaceLines_ReturnZero()
        {
            // Setup
            string path = Path.Combine(testDataPath, "ValidFileWithoutSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                int linesCount = reader.GetSurfaceLinesCount();

                // Assert
                Assert.AreEqual(0, linesCount);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_FileCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.Error_File_0_does_not_exist, path);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FileNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_DirectoryCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "..", "this_folder_does_not_exist", "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.Error_Directory_in_path_0_missing, path);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "empty.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.Error_File_0_empty, path);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_InvalidHeader1_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "InvalidHeader_UnsupportedId.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.PipingSurfaceLinesCsvReader_File_0_invalid_header, path);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [TestCase("X")]
        [TestCase("Y")]
        [TestCase("Z")]
        public void GetSurfaceLinesCount_InvalidHeader2_ThrowCriticalFileReadException(string missingVariableName)
        {
            // Setup
            var filename = string.Format("InvalidHeader_Lacks{0}1.csv", missingVariableName);
            string path = Path.Combine(testDataPath, filename);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format("Het bestand op '{0}' is niet geschikt om dwarsdoorsneden uit te lezen (Verwachte header: locationid;X1;Y1;Z1).", path);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithCultureNL_ReturnCreatedSurfaceLine()
        {
            DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine();
        }

        [Test]
        [SetCulture("en-US")]
        public void ReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithCultureEN_ReturnCreatedSurfaceLine()
        {
            DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine();
        }

        [Test]
        public void ReadLine_OpenedValidFileWithoutHeaderAndTwoSurfaceLinesWhileAtTheEndOfFile_ReturnNull()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                int surfaceLinesCount = reader.GetSurfaceLinesCount();
                for (int i = 0; i < surfaceLinesCount; i++)
                {
                    var pipingSurfaceLine = reader.ReadLine();
                    Assert.IsNotInstanceOf<IDisposable>(pipingSurfaceLine,
                                                        "Fail Fast: Disposal logic required to be implemented in test.");
                    Assert.IsNotNull(pipingSurfaceLine);
                }

                // Call
                var result = reader.ReadLine();

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        public void ReadLine_FileCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.Error_File_0_does_not_exist, path);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FileNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadLine_DirectoryCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "..", "this_folder_does_not_exist", "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.Error_Directory_in_path_0_missing, path);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadLine_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "empty.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.Error_File_0_empty, path);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_InvalidHeader1_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "InvalidHeader_UnsupportedId.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format(IOResources.PipingSurfaceLinesCsvReader_File_0_invalid_header, path);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [TestCase("X")]
        [TestCase("Y")]
        [TestCase("Z")]
        public void ReadLine_InvalidHeader2_ThrowCriticalFileReadException(string missingVariableName)
        {
            // Setup
            var filename = string.Format("InvalidHeader_Lacks{0}1.csv", missingVariableName);
            string path = Path.Combine(testDataPath, filename);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                var expectedMessage = string.Format("Het bestand op '{0}' is niet geschikt om dwarsdoorsneden uit te lezen (Verwachte header: locationid;X1;Y1;Z1).", path);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        private void DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                var surfaceLine1 = reader.ReadLine();
                var surfaceLine2 = reader.ReadLine();

                // Assert

                #region 1st surfaceline

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Count());
                Assert.AreEqual(94263.0026213, surfaceLine1.StartingWorldPoint.X);
                Assert.AreEqual(427776.654093, surfaceLine1.StartingWorldPoint.Y);
                Assert.AreEqual(-1.02, surfaceLine1.StartingWorldPoint.Z);
                Assert.AreEqual(94331.1767309, surfaceLine1.EndingWorldPoint.X);
                Assert.AreEqual(427960.112661, surfaceLine1.EndingWorldPoint.Y);
                Assert.AreEqual(1.44, surfaceLine1.EndingWorldPoint.Z);
                Assert.AreEqual(surfaceLine1.StartingWorldPoint, surfaceLine1.Points.First());
                Assert.AreEqual(surfaceLine1.EndingWorldPoint, surfaceLine1.Points.Last());

                #endregion

                #region 2nd surfaceline

                Assert.AreEqual("ArtificalLocal", surfaceLine2.Name);
                Assert.AreEqual(3, surfaceLine2.Points.Count());
                Assert.AreEqual(2.3, surfaceLine2.StartingWorldPoint.X);
                Assert.AreEqual(0, surfaceLine2.StartingWorldPoint.Y);
                Assert.AreEqual(1, surfaceLine2.StartingWorldPoint.Z);
                Assert.AreEqual(4.4, surfaceLine2.EndingWorldPoint.X);
                Assert.AreEqual(0, surfaceLine2.EndingWorldPoint.Y);
                Assert.AreEqual(1.1, surfaceLine2.EndingWorldPoint.Z);
                Assert.AreEqual(surfaceLine2.StartingWorldPoint, surfaceLine2.Points.First());
                Assert.AreEqual(surfaceLine2.EndingWorldPoint, surfaceLine2.Points.Last());

                #endregion
            }
        }
    }
}