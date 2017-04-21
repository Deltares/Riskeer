﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.IO.SurfaceLines;
using Ringtoets.Piping.Primitives;
using IOResources = Ringtoets.Piping.IO.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Test.SurfaceLines
{
    [TestFixture]
    public class PipingSurfaceLinesCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "SurfaceLines" + Path.DirectorySeparatorChar);

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(path);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Path_must_be_specified);
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
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(corruptPath);

            // Assert
            var innerExpectedMessage = "Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            string expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(innerExpectedMessage);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(testDataPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            string expectedMessage = new FileReaderErrorMessageBuilder(testDataPath).Build(UtilsResources.Error_Path_must_not_point_to_empty_file_name);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_AnyPath_ExpectedValues()
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
        public void GetSurfaceLinesCount_OpenedValidFileWithHeaderWithOptionalHeadersAndTwoSurfaceLines_ReturnNumberOfSurfaceLines()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines_WithOptionalHeaders.csv");

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
                string expectedError = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_File_does_not_exist);
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
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Directory_missing);
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
                string expectedMessage = new FileReaderErrorMessageBuilder(path).WithLocation("op regel 1").Build(UtilsResources.Error_File_empty);
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
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
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
            string filename = string.Format("InvalidHeader_Lacks{0}1.csv", missingVariableName);
            string path = Path.Combine(testDataPath, filename);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
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

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                RingtoetsPipingSurfaceLine surfaceLine1 = reader.ReadSurfaceLine();
                RingtoetsPipingSurfaceLine surfaceLine2 = reader.ReadSurfaceLine();

                // Assert

                #region 1st surfaceline

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Length);
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
                Assert.AreEqual(3, surfaceLine2.Points.Length);
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

        [Test]
        public void DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithOptionalHeaders_ReturnCreatedSurfaceLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines_WithOptionalHeaders.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                RingtoetsPipingSurfaceLine surfaceLine1 = reader.ReadSurfaceLine();
                RingtoetsPipingSurfaceLine surfaceLine2 = reader.ReadSurfaceLine();

                // Assert

                #region 1st surfaceline

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Length);
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
                Assert.AreEqual(3, surfaceLine2.Points.Length);
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
                RingtoetsPipingSurfaceLine surfaceLine1 = reader.ReadSurfaceLine();

                // Assert
                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Point3D[] geometryPoints = surfaceLine1.Points.ToArray();
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
                for (var i = 0; i < surfaceLinesCount; i++)
                {
                    RingtoetsPipingSurfaceLine pipingSurfaceLine = reader.ReadSurfaceLine();
                    Assert.IsNotInstanceOf<IDisposable>(pipingSurfaceLine,
                                                        "Fail Fast: Disposal logic required to be implemented in test.");
                    Assert.IsNotNull(pipingSurfaceLine);
                }

                // Call
                RingtoetsPipingSurfaceLine result = reader.ReadSurfaceLine();

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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_File_does_not_exist);
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Directory_missing);
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(UtilsResources.Error_File_empty);
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_File_invalid_header);
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
            string filename = string.Format("InvalidHeader_Lacks{0}1.csv", missingVariableName);
            string path = Path.Combine(testDataPath, filename);

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
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
            string path = Path.Combine(testDataPath, string.Format("InvalidRow_{0}NotAValidNumber.csv", malformattedVariableName));

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielschematisatie 'InvalidSurfaceLine'")
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielschematisatie 'InvalidSurfaceLine'")
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 3")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
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

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 4")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .Build(string.Format(IOResources.PipingSurfaceLinesCsvReader_ReadLine_Line_lacks_separator_0_, ';'));
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
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
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                // 1st row lacks 1 coordinate value:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielschematisatie 'LacksOneCoordinate'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_triplet);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd row lacks 2 coordinate values:
                exception = Assert.Throws<LineParseException>(call);
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 3")
                    .WithSubject("profielschematisatie 'LacksTwoCoordinates'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_triplet);
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

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielschematisatie 'ArtificialLocal'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
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

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadSurfaceLine();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("profielschematisatie 'Rotterdam1'")
                    .Build(IOResources.PipingSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length);
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

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                RingtoetsPipingSurfaceLine surfaceLine1 = reader.ReadSurfaceLine();
                RingtoetsPipingSurfaceLine surfaceLine2 = reader.ReadSurfaceLine();

                // Assert

                #region 1st surfaceline

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Length);
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
                Assert.AreEqual(3, surfaceLine2.Points.Length);
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