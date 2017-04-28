// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SurfaceLines;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Test.SurfaceLines
{
    [TestFixture]
    public class CharacteristicPointsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "CharacteristicPoints" + Path.DirectorySeparatorChar);

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new CharacteristicPointsCsvReader(path);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Path_must_be_specified);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InvalidPathCharactersInPath_ThrowsArgumentException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidLocations.csv");

            char[] invalidCharacters = Path.GetInvalidPathChars();

            string corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => new CharacteristicPointsCsvReader(corruptPath);

            // Assert
            const string innerErrorMessage = "Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            string expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(innerErrorMessage);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new CharacteristicPointsCsvReader(testDataPath);

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
            using (var reader = new CharacteristicPointsCsvReader(fakeFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        [TestCase("0locations.krp.csv", 0)]
        [TestCase("1location.krp.csv", 1)]
        [TestCase("2locations.krp.csv", 2)]
        [TestCase("2locations_with_white_line.krp.csv", 2)]
        public void GetLocationsCount_DifferentFiles_ShouldReturnNumberOfLocations(string fileName, int expectedCount)
        {
            // Setup
            string path = Path.Combine(testDataPath, fileName);
            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                int result = reader.GetLocationsCount();

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

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedError = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_File_does_not_exist);
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

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Directory_missing);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void GetLocationsCount_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "empty.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(UtilsResources.Error_File_empty);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [TestCase("start")]
        [TestCase("middle")]
        [TestCase("end")]
        [TestCase("start_order_number")]
        [TestCase("middle_order_number")]
        [TestCase("end_order_number")]
        public void GetLocationsCount_FileHasColumnsMissingInHeader_ThrowCriticalFileReadException(string malformattedVariableName)
        {
            // Setup
            string path = Path.Combine(testDataPath, string.Format("1location_column_missing_{0}.krp.csv", malformattedVariableName));

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(Resources.CharacteristicPointsCsvReader_File_invalid_header);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void GetLocationsCount_InvalidHeader_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "1location_invalid_header.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(Resources.CharacteristicPointsCsvReader_File_invalid_header);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void GetLocationsCount_DuplicateColumnsInHeader_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_header_duplicate_columns.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.GetLocationsCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(Resources.CharacteristicPointsCsvReader_File_invalid_header);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocationsWithCultureNL_ReturnCreatedCharacteristicPointsLocation()
        {
            ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation("2locations.krp.csv");
        }

        [Test]
        [SetCulture("en-US")]
        public void ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocationsWithCultureEN_ReturnCreatedCharacteristicPointsLocation()
        {
            ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation("2locations.krp.csv");
        }

        [Test]
        public void ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocationsWithWhiteLine_ReturnCreatedCharacteristicPointsLocation()
        {
            ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation("2locations_with_white_line.krp.csv");
        }

        [Test]
        public void ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoLocationsWhileAtTheEndOfFile_ReturnNull()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations.krp.csv");

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                int locationsCount = reader.GetLocationsCount();
                for (var i = 0; i < locationsCount; i++)
                {
                    CharacteristicPoints characteristicPointsLocation = reader.ReadCharacteristicPointsLocation();
                    Assert.IsNotInstanceOf<IDisposable>(characteristicPointsLocation,
                                                        "Fail Fast: Disposal logic required to be implemented in test.");
                    Assert.IsNotNull(characteristicPointsLocation);
                }

                // Call
                CharacteristicPoints result = reader.ReadCharacteristicPointsLocation();

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_FileCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_File_does_not_exist);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FileNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_DirectoryCannotBeFound_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "..", "this_folder_does_not_exist", "I_do_not_exist.csv");

            // Precondition
            Assert.IsFalse(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Directory_missing);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "empty.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(UtilsResources.Error_File_empty);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_InvalidHeader_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "1location_invalid_header.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(Resources.CharacteristicPointsCsvReader_File_invalid_header);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_DuplicateColumnsInHeader_ThrowCriticalFileReadException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_header_duplicate_columns.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(Resources.CharacteristicPointsCsvReader_File_invalid_header);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_FileHasInvalidCoordinate_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "1location_invalid_double.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("locatie 'Invalid'")
                    .Build(Resources.Error_CharacteristicPoint_has_not_double);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FormatException>(exception.InnerException);
            }
        }

        [Test]
        [TestCase("overflow_x")]
        [TestCase("overflow_y")]
        [TestCase("overflow_z")]
        [TestCase("underflow_x")]
        [TestCase("underflow_y")]
        [TestCase("underflow_z")]
        public void ReadCharacteristicPointsLocation_FileHasCoordinateCausingOverOrUnderflow_ThrowLineParseException(string malformattedVariableName)
        {
            // Setup
            string path = Path.Combine(testDataPath, string.Format("1location_{0}.krp.csv", malformattedVariableName));

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("locatie 'InvalidNumber'")
                    .Build(Resources.Error_CharacteristicPoint_parsing_causes_overflow);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<OverflowException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_FileLacksIds_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_each_missing_id.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_ID);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 3")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_ID);
                exception = Assert.Throws<LineParseException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_FileLacksIdsWithWhiteLine_ThrowLineParseExceptionOnCorrectLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_each_missing_id_with_white_line.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                // 1st line has no text at all:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_ID);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd line has only whitespace text:
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 4")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_ID);
                exception = Assert.Throws<LineParseException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_IncorrectValueSeparator_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_invalid_separator.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 1")
                    .Build(string.Format(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_separator_0_, ';'));
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_FileHasIncompleteCoordinateTriplets_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_each_missing_values.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                // 1st row lacks 1 coordinate value:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("locatie 'LacksOneCoordinate'")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_values_for_characteristic_points);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd row lacks 2 coordinate values:
                exception = Assert.Throws<LineParseException>(call);
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 3")
                    .WithSubject("locatie 'LacksTwoCoordinates'")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_values_for_characteristic_points);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadCharacteristicPointsLocation_FileHasTooManyValues_ThrowLineParseException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations_each_too_many_values.krp.csv");

            // Precondition
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                TestDelegate call = () => reader.ReadCharacteristicPointsLocation();

                // Assert
                // 1st row lacks 1 coordinate value:
                var exception = Assert.Throws<LineParseException>(call);
                string expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 2")
                    .WithSubject("locatie 'OneValueTooMuch'")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_values_for_characteristic_points);
                Assert.AreEqual(expectedMessage, exception.Message);

                // 2nd row lacks 2 coordinate values:
                exception = Assert.Throws<LineParseException>(call);
                expectedMessage = new FileReaderErrorMessageBuilder(path)
                    .WithLocation("op regel 3")
                    .WithSubject("locatie 'ThreeValuesTooMuch'")
                    .Build(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_values_for_characteristic_points);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void Dispose_HavingReadFile_CorrectlyReleaseFile()
        {
            // Setup
            string path = Path.Combine(testDataPath, "2locations.krp.csv");

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            // Call
            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                reader.ReadCharacteristicPointsLocation();
                reader.ReadCharacteristicPointsLocation();
            }

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(path));
        }

        private void ReadCharacteristicPointsLocation_OpenedValidFileWithHeaderAndTwoCharacteristicPointsLocations_ReturnCreatedCharacteristicPointsLocation(string fileName)
        {
            // Setup
            string path = Path.Combine(testDataPath, fileName);

            // Precondition:
            Assert.IsTrue(File.Exists(path));

            using (var reader = new CharacteristicPointsCsvReader(path))
            {
                // Call
                CharacteristicPoints location1 = reader.ReadCharacteristicPointsLocation();
                CharacteristicPoints location2 = reader.ReadCharacteristicPointsLocation();

                // Assert

                #region 1st location

                Assert.AreEqual("Rotterdam1", location1.Name);

                Assert.AreEqual(new Point3D(100, 0, -0.63), location1.SurfaceLevelInside);

                Assert.AreEqual(new Point3D(60.83, 0, -0.57), location1.DitchPolderSide);

                Assert.AreEqual(new Point3D(59.36, 0, -1.87), location1.BottomDitchPolderSide);

                Assert.AreEqual(new Point3D(57.99, 0, -1.9), location1.BottomDitchDikeSide);

                Assert.AreEqual(new Point3D(55.37, 0, -0.31), location1.DitchDikeSide);

                Assert.AreEqual(new Point3D(55.37, 0, -0.31), location1.DikeToeAtPolder);

                Assert.IsNull(location1.TopShoulderInside);

                Assert.IsNull(location1.ShoulderInside);

                Assert.AreEqual(new Point3D(40.17, 0, 2.63), location1.DikeTopAtPolder);

                Assert.AreEqual(new Point3D(40.85, 0, 2.44), location1.TrafficLoadInside);

                Assert.AreEqual(new Point3D(38.35, 0, 2.623), location1.TrafficLoadOutside);

                Assert.AreEqual(new Point3D(35.95, 0, 2.61), location1.DikeTopAtRiver);

                Assert.IsNull(location1.ShoulderOutisde);

                Assert.IsNull(location1.TopShoulderOutside);

                Assert.AreEqual(new Point3D(29.1, 0, -0.2), location1.DikeToeAtRiver);

                Assert.AreEqual(new Point3D(0, 0, -0.71), location1.SurfaceLevelOutside);

                Assert.AreEqual(new Point3D(23.703, 0, -1.5), location1.DikeTableHeight);

                Assert.IsNull(location1.InsertRiverChannel);
                Assert.IsNull(location1.BottomRiverChannel);

                #endregion

                #region 2nd location

                Assert.AreEqual("Amsterdam1", location2.Name);

                Assert.AreEqual(new Point3D(100, 0, -0.47), location2.SurfaceLevelInside);

                Assert.AreEqual(new Point3D(58.42, 0, -0.6), location2.DitchPolderSide);

                Assert.AreEqual(new Point3D(56.2, 0, -1.98), location2.BottomDitchPolderSide);

                Assert.AreEqual(new Point3D(56.2, 0, -1.98), location2.BottomDitchDikeSide);

                Assert.AreEqual(new Point3D(53.48, 0, -0.49), location2.DitchDikeSide);

                Assert.AreEqual(new Point3D(53.48, 0, -0.49), location2.DikeToeAtPolder);

                Assert.IsNull(location2.TopShoulderInside);

                Assert.IsNull(location2.ShoulderInside);

                Assert.AreEqual(new Point3D(38.17, 0, 3.04), location2.DikeTopAtPolder);

                Assert.AreEqual(new Point3D(37.73, 0, 3.13), location2.TrafficLoadInside);

                Assert.AreEqual(new Point3D(35.23, 0, 3.253), location2.TrafficLoadOutside);

                Assert.AreEqual(new Point3D(32.77, 0, 3.11), location2.DikeTopAtRiver);

                Assert.IsNull(location2.ShoulderOutisde);

                Assert.IsNull(location2.TopShoulderOutside);

                Assert.AreEqual(new Point3D(19.61, 0, -0.05), location2.DikeToeAtRiver);

                Assert.AreEqual(new Point3D(0, 0, -0.33), location2.SurfaceLevelOutside);

                Assert.AreEqual(new Point3D(17.32, 0, -1.52), location2.DikeTableHeight);

                Assert.IsNull(location2.InsertRiverChannel);
                Assert.IsNull(location2.BottomRiverChannel);

                #endregion
            }
        }
    }
}