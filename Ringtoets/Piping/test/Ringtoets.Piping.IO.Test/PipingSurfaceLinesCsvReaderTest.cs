using System;
using System.IO;
using System.Linq;

using Core.Common.TestUtils;

using NUnit.Framework;

using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Test.TestHelpers;

using IOResources = Ringtoets.Piping.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Test
{
    [TestFixture]
    public class PipingSurfaceLinesCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SurfaceLines" + Path.DirectorySeparatorChar);

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
            var expectedMessage = new FileReaderErrorMessageBuilder(path).Build(IOResources.Error_Path_must_be_specified);
            Assert.AreEqual(expectedMessage, exception.Message);
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
            var expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(String.Format(IOResources.Error_Path_cannot_contain_Characters_0_,
                                                                                              String.Join(", ", Path.GetInvalidFileNameChars())));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ParametersConstructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(testDataPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = new FileReaderErrorMessageBuilder(testDataPath).Build(IOResources.Error_Path_must_not_point_to_folder);
            Assert.AreEqual(expectedMessage, exception.Message);
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
                var expectedError = new FileReaderErrorMessageBuilder(path).Build(IOResources.Error_File_does_not_exist);
                Assert.AreEqual(expectedError, exception.Message);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).Build(IOResources.Error_Directory_missing);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(IOResources.Error_File_empty);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(IOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build("Het bestand is niet geschikt om profielmetingen uit te lezen (Verwachte header: locationid;X1;Y1;Z1).");
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
        [SetCulture("en-US")]
        public void ReadLine_OpenedValidFileWithHeaderAndSurfaceLinesWithDuplicatePointsWithCultureEN_ReturnCreatedSurfaceLineWithDuplicatePoints()
        {
            // Setup
            // File: First 3 points are identical
            string path = Path.Combine(testDataPath, "ValidSurfaceLine_HasConsecutiveDuplicatePoints.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                var surfaceLine1 = reader.ReadLine();

                // Assert
                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                var geometryPoints = surfaceLine1.Points.ToArray();
                Assert.AreNotEqual(geometryPoints.Length, geometryPoints.Distinct().Count(),
                    "Duplicate points should be parsed.");
                Assert.AreEqual(geometryPoints[0].X, geometryPoints[1].X,
                    "Consecutive duplicate points are still parsed.");
                Assert.AreEqual(surfaceLine1.StartingWorldPoint, geometryPoints.First());
                Assert.AreEqual(surfaceLine1.EndingWorldPoint, geometryPoints.Last());
            }
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).Build(IOResources.Error_File_does_not_exist);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).Build(IOResources.Error_Directory_missing);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(IOResources.Error_File_empty);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(IOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
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
                var expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build("Het bestand is niet geschikt om profielmetingen uit te lezen (Verwachte header: locationid;X1;Y1;Z1).");
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [TestCase("X")]
        [TestCase("Y")]
        [TestCase("Z")]
        public void ReadLine_FileHasInvalidCoordinate_ThrowLineParseException(string malformattedVariableName)
        {
            // Setup
            string path = Path.Combine(testDataPath, string.Format("InvalidRow_{0}NotAValidNumber.csv", malformattedVariableName));

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielmeting 'InvalidSurfaceLine'")
                    .Build(IOResources.Error_SurfaceLine_has_not_double);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FormatException>(exception.InnerException);
            }
        }

        [Test]
        [TestCase("XOver")]
        [TestCase("YOver")]
        [TestCase("ZOver")]
        [TestCase("XUnder")]
        [TestCase("YUnder")]
        [TestCase("ZUnder")]
        public void ReadLine_FileHasCoordinateCausingOverOrUnderflow_ThrowLineParseException(string malformattedVariableName)
        {
            // Setup
            string path = Path.Combine(testDataPath, string.Format("InvalidRow_{0}flowingNumber.csv", malformattedVariableName));

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielmeting 'InvalidSurfaceLine'")
                    .Build(IOResources.Error_SurfaceLine_parsing_causes_overflow);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<OverflowException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadLine_FileLacksIds_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoInvalidRows_LacksId.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 2").Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 3").Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
                exception = Assert.Throws<LineParseException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_IncorrectValueSeparator_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "InvalidRow_IncorrectValueSeparator.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 2").Build(string.Format(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_separator_0_, ';'));
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_FileLacksCoordinateValues_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "InvalidRow_LacksCoordinateValues.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .Build(string.Format(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_separator_0_, ';'));
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_FileHasIncompleteCoordinateTriplets_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoInvalidRows_IncompleteCoordinateTriplets.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                // 1st row lacks 1 coordinate value:
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielmeting 'LacksOneCoordinate'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_triplet);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd row lacks 2 coordinate values:
                exception = Assert.Throws<LineParseException>(call);
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 3")
                    .WithSubject("profielmeting 'LacksTwoCoordinates'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_triplet);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_LineWithoutMonotonicallyIncreasingLCoordinates_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "InvalidRow_NotMonotinocallyIncreasingLCoordinates.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                var expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielmeting 'ArtificialLocal'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void Dispose_HavingReadFile_CorrectlyReleaseFile()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            // Call
            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                reader.ReadLine();
                reader.ReadLine();
            }

            // Assert
            Assert.IsTrue(FileHelper.CanOpenFileForWrite(path));
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

                Assert.AreEqual("ArtifcialLocal", surfaceLine2.Name);
                Assert.AreEqual(3, surfaceLine2.Points.Count());
                Assert.AreEqual(2.3, surfaceLine2.StartingWorldPoint.X);
                Assert.AreEqual(0, surfaceLine2.StartingWorldPoint.Y);
                Assert.AreEqual(1, surfaceLine2.StartingWorldPoint.Z);
                Assert.AreEqual(5.7, surfaceLine2.EndingWorldPoint.X);
                Assert.AreEqual(0, surfaceLine2.EndingWorldPoint.Y);
                Assert.AreEqual(1.1, surfaceLine2.EndingWorldPoint.Z);
                Assert.AreEqual(surfaceLine2.StartingWorldPoint, surfaceLine2.Points.First());
                Assert.AreEqual(surfaceLine2.EndingWorldPoint, surfaceLine2.Points.Last());

                #endregion
            }
        }
    }
}