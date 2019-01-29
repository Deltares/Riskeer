// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Riskeer.Common.IO.SurfaceLines;

namespace Riskeer.Common.IO.Test.SurfaceLines
{
    [TestFixture]
    public class SurfaceLinesCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "SurfaceLines" + Path.DirectorySeparatorChar);

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new SurfaceLinesCsvReader(path);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(path).Build("Bestandspad mag niet leeg of ongedefinieerd zijn.");
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InvalidPathCharactersInPath_ThrowsArgumentException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            char[] invalidCharacters = Path.GetInvalidPathChars();

            string corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => new SurfaceLinesCsvReader(corruptPath);

            // Assert
            const string innerExpectedMessage = "Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            string expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(innerExpectedMessage);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new SurfaceLinesCsvReader(testDataPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            string expectedMessage = new FileReaderErrorMessageBuilder(testDataPath).Build("Bestandspad mag niet verwijzen naar een lege bestandsnaam.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_AnyPath_ExpectedValues()
        {
            // Setup
            const string fakeFilePath = @"I\Dont\Really\Exist";

            // Call
            using (var reader = new SurfaceLinesCsvReader(fakeFilePath))
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                int linesCount = reader.GetSurfaceLinesCount();

                // Assert
                Assert.AreEqual(2, linesCount);
            }
        }

        [Test]
        public void GetSurfaceLinesCount_OpenedValidFileWithHeaderWithOptionalHeadersAndTwoSurfaceLines_ReturnNumberOfSurfaceLines()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines_WithOptionalHeaders.csv");

            using (var reader = new SurfaceLinesCsvReader(path))
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

            using (var reader = new SurfaceLinesCsvReader(path))
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedError = new FileReaderErrorMessageBuilder(path).Build("Het bestand bestaat niet.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build("Het bestandspad verwijst naar een map die niet bestaat.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build("Het bestand is leeg.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 1")
                                         .Build("Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1).");
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
            string filename = $"InvalidHeader_Lacks{missingVariableName}1.csv";
            string path = Path.Combine(testDataPath, filename);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetSurfaceLinesCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 1")
                                         .Build("Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1).");
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
        public void DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithWhiteLine_ReturnCreatedSurfaceLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines_WithWhiteLine.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                SurfaceLine surfaceLine1 = reader.ReadSurfaceLine();
                SurfaceLine surfaceLine2 = reader.ReadSurfaceLine();

                // Assert

                #region 1st surface line

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Length);
                Assert.AreEqual(94263.0026213, surfaceLine1.Points.First().X);
                Assert.AreEqual(427776.654093, surfaceLine1.Points.First().Y);
                Assert.AreEqual(-1.02, surfaceLine1.Points.First().Z);
                Assert.AreEqual(94331.1767309, surfaceLine1.Points.Last().X);
                Assert.AreEqual(427960.112661, surfaceLine1.Points.Last().Y);
                Assert.AreEqual(1.44, surfaceLine1.Points.Last().Z);
                Assert.AreEqual(surfaceLine1.Points.First(), surfaceLine1.Points.First());
                Assert.AreEqual(surfaceLine1.Points.Last(), surfaceLine1.Points.Last());

                #endregion

                #region 2nd surface line

                Assert.AreEqual("ArtifcialLocal", surfaceLine2.Name);
                Assert.AreEqual(3, surfaceLine2.Points.Length);
                Assert.AreEqual(2.3, surfaceLine2.Points.First().X);
                Assert.AreEqual(0, surfaceLine2.Points.First().Y);
                Assert.AreEqual(1, surfaceLine2.Points.First().Z);
                Assert.AreEqual(5.7, surfaceLine2.Points.Last().X);
                Assert.AreEqual(0, surfaceLine2.Points.Last().Y);
                Assert.AreEqual(1.1, surfaceLine2.Points.Last().Z);
                Assert.AreEqual(surfaceLine2.Points.First(), surfaceLine2.Points.First());
                Assert.AreEqual(surfaceLine2.Points.Last(), surfaceLine2.Points.Last());

                #endregion
            }
        }

        [Test]
        public void DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithOptionalHeaders_ReturnCreatedSurfaceLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines_WithOptionalHeaders.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                SurfaceLine surfaceLine1 = reader.ReadSurfaceLine();
                SurfaceLine surfaceLine2 = reader.ReadSurfaceLine();

                // Assert

                #region 1st surface line

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Length);
                Assert.AreEqual(94263.0026213, surfaceLine1.Points.First().X);
                Assert.AreEqual(427776.654093, surfaceLine1.Points.First().Y);
                Assert.AreEqual(-1.02, surfaceLine1.Points.First().Z);
                Assert.AreEqual(94331.1767309, surfaceLine1.Points.Last().X);
                Assert.AreEqual(427960.112661, surfaceLine1.Points.Last().Y);
                Assert.AreEqual(1.44, surfaceLine1.Points.Last().Z);
                Assert.AreEqual(surfaceLine1.Points.First(), surfaceLine1.Points.First());
                Assert.AreEqual(surfaceLine1.Points.Last(), surfaceLine1.Points.Last());

                #endregion

                #region 2nd surface line

                Assert.AreEqual("ArtifcialLocal", surfaceLine2.Name);
                Assert.AreEqual(3, surfaceLine2.Points.Length);
                Assert.AreEqual(2.3, surfaceLine2.Points.First().X);
                Assert.AreEqual(0, surfaceLine2.Points.First().Y);
                Assert.AreEqual(1, surfaceLine2.Points.First().Z);
                Assert.AreEqual(5.7, surfaceLine2.Points.Last().X);
                Assert.AreEqual(0, surfaceLine2.Points.Last().Y);
                Assert.AreEqual(1.1, surfaceLine2.Points.Last().Z);
                Assert.AreEqual(surfaceLine2.Points.First(), surfaceLine2.Points.First());
                Assert.AreEqual(surfaceLine2.Points.Last(), surfaceLine2.Points.Last());

                #endregion
            }
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                SurfaceLine surfaceLine1 = reader.ReadSurfaceLine();

                // Assert
                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Point3D[] geometryPoints = surfaceLine1.Points.ToArray();
                Assert.AreNotEqual(geometryPoints.Length, geometryPoints.Distinct().Count(),
                                   "Duplicate points should be parsed.");
                Assert.AreEqual(geometryPoints.First().X, geometryPoints[1].X,
                                "Consecutive duplicate points are still parsed.");
                Assert.AreEqual(surfaceLine1.Points.First(), geometryPoints.First());
                Assert.AreEqual(surfaceLine1.Points.Last(), geometryPoints.Last());
            }
        }

        [Test]
        public void ReadLine_OpenedValidFileWithoutHeaderAndTwoSurfaceLinesWhileAtTheEndOfFile_ReturnNull()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                int surfaceLinesCount = reader.GetSurfaceLinesCount();
                for (var i = 0; i < surfaceLinesCount; i++)
                {
                    SurfaceLine surfaceLine = reader.ReadSurfaceLine();
                    Assert.IsNotInstanceOf<IDisposable>(surfaceLine,
                                                        "Fail Fast: Disposal logic required to be implemented in test.");
                    Assert.IsNotNull(surfaceLine);
                }

                // Call
                SurfaceLine result = reader.ReadSurfaceLine();

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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build("Het bestand bestaat niet.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build("Het bestandspad verwijst naar een map die niet bestaat.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 1")
                                         .Build("Het bestand is leeg.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 1")
                                         .Build("Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1).");
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
            string filename = $"InvalidHeader_Lacks{missingVariableName}1.csv";
            string path = Path.Combine(testDataPath, filename);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 1")
                                         .Build("Het bestand is niet geschikt om profielschematisaties uit te lezen (Verwachte koptekst: locationid;X1;Y1;Z1).");
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
            string path = Path.Combine(testDataPath, $"InvalidRow_{malformattedVariableName}NotAValidNumber.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .WithSubject("profielschematisatie 'InvalidSurfaceLine'")
                                         .Build("Profielschematisatie heeft een coördinaatwaarde die niet omgezet kan worden naar een getal.");
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
            string path = Path.Combine(testDataPath, $"InvalidRow_{malformattedVariableName}flowingNumber.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .WithSubject("profielschematisatie 'InvalidSurfaceLine'")
                                         .Build("Profielschematisatie heeft een coördinaatwaarde die te groot of te klein is om ingelezen te worden.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .Build("Regel heeft geen ID.");
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                                  .WithLocation("op regel 3")
                                  .Build("Regel heeft geen ID.");
                exception = Assert.Throws<LineParseException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_FileLacksIdsWithWhiteLine_ThrowLineParseExceptionOnCorrectLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoInvalidRows_LacksIdWithWhiteLine.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .Build("Regel heeft geen ID.");
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                                  .WithLocation("op regel 4")
                                  .Build("Regel heeft geen ID.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .Build($"Ontbrekend scheidingsteken '{';'}'.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .Build($"Ontbrekend scheidingsteken '{';'}'.");
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

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                // 1st row lacks 1 coordinate value:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .WithSubject("profielschematisatie 'LacksOneCoordinate'")
                                         .Build("Voor de profielschematisatie ontbreken er waardes om een 3D (X,Y,Z) punt aan te maken.");
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd row lacks 2 coordinate values:
                exception = Assert.Throws<LineParseException>(call);
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                                  .WithLocation("op regel 3")
                                  .WithSubject("profielschematisatie 'LacksTwoCoordinates'")
                                  .Build("Voor de profielschematisatie ontbreken er waardes om een 3D (X,Y,Z) punt aan te maken.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadLine_LineWithoutMonotonicallyIncreasingLCoordinates_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "InvalidRow_NotMonotonicallyIncreasingLCoordinates.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .WithSubject("profielschematisatie 'ArtificialLocal'")
                                         .Build("Profielschematisatie heeft een teruglopende geometrie (punten behoren een oplopende set L-coördinaten te hebben in het lokale coördinatenstelsel).");
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [TestCase("InvalidRow_DuplicatePointsCausingZeroLength.csv")]
        [TestCase("InvalidRow_SinglePoint.csv")]
        public void ReadLine_LineWithZeroLength_ThrowLineParseException(string file)
        {
            // Setup
            string path = Path.Combine(testDataPath, file);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                         .WithLocation("op regel 2")
                                         .WithSubject("profielschematisatie 'Rotterdam1'")
                                         .Build("Profielschematisatie heeft een geometrie die een lijn met lengte 0 beschrijft.");
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
            using (var reader = new SurfaceLinesCsvReader(path))
            {
                reader.ReadSurfaceLine();
                reader.ReadSurfaceLine();
            }

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        private void DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new SurfaceLinesCsvReader(path))
            {
                // Call
                SurfaceLine surfaceLine1 = reader.ReadSurfaceLine();
                SurfaceLine surfaceLine2 = reader.ReadSurfaceLine();

                // Assert

                #region 1st surface line

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Length);
                Assert.AreEqual(94263.0026213, surfaceLine1.Points.First().X);
                Assert.AreEqual(427776.654093, surfaceLine1.Points.First().Y);
                Assert.AreEqual(-1.02, surfaceLine1.Points.First().Z);
                Assert.AreEqual(94331.1767309, surfaceLine1.Points.Last().X);
                Assert.AreEqual(427960.112661, surfaceLine1.Points.Last().Y);
                Assert.AreEqual(1.44, surfaceLine1.Points.Last().Z);
                Assert.AreEqual(surfaceLine1.Points.First(), surfaceLine1.Points.First());
                Assert.AreEqual(surfaceLine1.Points.Last(), surfaceLine1.Points.Last());

                #endregion

                #region 2nd surface line

                Assert.AreEqual("ArtifcialLocal", surfaceLine2.Name);
                Assert.AreEqual(3, surfaceLine2.Points.Length);
                Assert.AreEqual(2.3, surfaceLine2.Points.First().X);
                Assert.AreEqual(0, surfaceLine2.Points.First().Y);
                Assert.AreEqual(1, surfaceLine2.Points.First().Z);
                Assert.AreEqual(5.7, surfaceLine2.Points.Last().X);
                Assert.AreEqual(0, surfaceLine2.Points.Last().Y);
                Assert.AreEqual(1.1, surfaceLine2.Points.Last().Z);
                Assert.AreEqual(surfaceLine2.Points.First(), surfaceLine2.Points.First());
                Assert.AreEqual(surfaceLine2.Points.Last(), surfaceLine2.Points.Last());

                #endregion
            }
        }
    }
}