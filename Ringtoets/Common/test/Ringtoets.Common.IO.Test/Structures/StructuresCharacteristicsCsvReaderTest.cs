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
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.Structures;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Test.Structures
{
    [TestFixture]
    public class StructuresCharacteristicsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO, string.Format("Characteristics{0}", Path.DirectorySeparatorChar));

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new StructuresCharacteristicsCsvReader(path);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Path_must_be_specified);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InvalidPathCharactersInPath_ThrowsArgumentException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "ValidCharacteristics.csv");

            char[] invalidCharacters = Path.GetInvalidPathChars();

            string corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => new StructuresCharacteristicsCsvReader(corruptPath);

            // Assert
            string innerExpectedMessage = string.Format((string) UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                                        string.Join(", ", Path.GetInvalidFileNameChars()));
            string expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(innerExpectedMessage);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new StructuresCharacteristicsCsvReader(testDataPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(call);
            string expectedMessage = new FileReaderErrorMessageBuilder(testDataPath).Build(UtilsResources.Error_Path_must_not_point_to_empty_file_name);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetLineCount_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            const string invalidFilePath = "I_do_not_exist.csv";

            using (var reader = new StructuresCharacteristicsCsvReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetLineCount();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual("Fout bij het lezen van bestand 'I_do_not_exist.csv': Het bestand bestaat niet.", message);
            }
        }

        [Test]
        public void GetLineCount_FolderDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine("I_do_not_exist", "I do not exist either.csv");

            using (var reader = new StructuresCharacteristicsCsvReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetLineCount();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestandspad verwijst naar een map die niet bestaat.",
                                                       invalidFilePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void GetLineCount_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "EmptyFile.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.GetLineCount();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: Het bestand is leeg.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("InvalidFile_LocationIdMissing.csv")]
        [TestCase("InvalidFile_ParameterIdMissing.csv")]
        [TestCase("InvalidFile_NumericValueMissing.csv")]
        [TestCase("InvalidFile_VarianceValueMissing.csv")]
        [TestCase("InvalidFile_BooleanMissing.csv")]
        public void GetLineCount_FileLackingLocationIdColumn_ThrowCriticalFileReadException(string fileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      fileName));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.GetLineCount();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedHeaderColumnsText = "identificatie, kunstwerken.identificatie, numeriekewaarde, standarddeviatie.variance, boolean";
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: Het bestand is niet geschikt om kunstwerken parameters uit te lezen (Verwachte koptekst moet de volgende kolommen bevatten: {1}.",
                                                       filePath, expectedHeaderColumnsText);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void GetLineCount_ValidFileWithTwoLocationsAndAllHeightStructureParameters_ReturnCount()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      "ValidFile_2Locations_AllHeightStructureParameters.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                int count = reader.GetLineCount();

                // Assert
                Assert.AreEqual(16, count);
            }
        }

        [Test]
        public void GetLineCount_ValidFileWithTwoLocationsAndAllHeightStructureParametersCondensed_ReturnCount()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      "ValidFile_2Locations_AllHeightStructureParameters_CondensedAndDifferentOrder.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                int count = reader.GetLineCount();

                // Assert
                Assert.AreEqual(16, count);
            }
        }

        [Test]
        public void GetLineCount_ValidFileWithOneLocationsAndExtraWhiteLines_ReturnCount()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      "ValidFile_1Location_AllHeightStructureParameters_ExtraWhiteLines.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                int count = reader.GetLineCount();

                // Assert
                Assert.AreEqual(8, count);
            }
        }

        [Test]
        [TestCase("InvalidFile_DuplicateLocationId.csv", "identificatie")]
        [TestCase("InvalidFile_DuplicateParameterId.csv", "kunstwerken.identificatie")]
        [TestCase("InvalidFile_DuplicateNumericalValue.csv", "numeriekewaarde")]
        [TestCase("InvalidFile_DuplicateVarianceValue.csv", "standarddeviatie.variance")]
        [TestCase("InvalidFile_DuplicateBoolean.csv", "boolean")]
        public void GetLineCount_DuplicateColumn_ThrowCriticalFileReadException(string fileName, string columnName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      fileName));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.GetLineCount();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: De kolom '{1}' mag maar 1x gedefinieerd zijn.",
                                                       filePath, columnName);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            const string invalidFilePath = "I_do_not_exist.csv";

            using (var reader = new StructuresCharacteristicsCsvReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual("Fout bij het lezen van bestand 'I_do_not_exist.csv': Het bestand bestaat niet.", message);
            }
        }

        [Test]
        public void ReadLine_FolderDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine("I_do_not_exist", "I do not exist either.csv");

            using (var reader = new StructuresCharacteristicsCsvReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestandspad verwijst naar een map die niet bestaat.",
                                                       invalidFilePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_EmptyFile_ThrowCriticalFileReadException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "EmptyFile.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: Het bestand is leeg.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("InvalidFile_LocationIdMissing.csv")]
        [TestCase("InvalidFile_ParameterIdMissing.csv")]
        [TestCase("InvalidFile_NumericValueMissing.csv")]
        [TestCase("InvalidFile_VarianceValueMissing.csv")]
        [TestCase("InvalidFile_BooleanMissing.csv")]
        public void ReadLine_FileLackingLocationIdColumn_ThrowCriticalFileReadException(string fileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      fileName));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedHeaderColumnsText = "identificatie, kunstwerken.identificatie, numeriekewaarde, standarddeviatie.variance, boolean";
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: Het bestand is niet geschikt om kunstwerken parameters uit te lezen (Verwachte koptekst moet de volgende kolommen bevatten: {1}.",
                                                       filePath, expectedHeaderColumnsText);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("InvalidFile_DuplicateLocationId.csv", "identificatie")]
        [TestCase("InvalidFile_DuplicateParameterId.csv", "kunstwerken.identificatie")]
        [TestCase("InvalidFile_DuplicateNumericalValue.csv", "numeriekewaarde")]
        [TestCase("InvalidFile_DuplicateVarianceValue.csv", "standarddeviatie.variance")]
        [TestCase("InvalidFile_DuplicateBoolean.csv", "boolean")]
        public void ReadLine_DuplicateColumn_ThrowCriticalFileReadException(string fileName, string columnName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures",
                                                                      "StructuresCharacteristicsCsvFiles",
                                                                      fileName));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 1: De kolom '{1}' mag maar 1x gedefinieerd zijn.",
                                                       filePath, columnName);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_NoLocations_ReturnNull()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "ValidFile_0Locations.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                StructuresParameterRow parameter = reader.ReadLine();

                // Assert
                Assert.IsNull(parameter);
            }
        }

        [Test]
        public void ReadLine_LineMissingSeparatorCharacter_ThrowLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineLackingSeparator.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Regel ontbreekt het verwachte scheidingsteken (het karakter: ;).",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineMissingValues_ThrowLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineMissingValues.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Regel verwacht van 20 elementen, maar het zijn er 19.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithExtraValues_ThrowLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineWithExtraValues.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Regel verwacht van 20 elementen, maar het zijn er 22.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithNoValueForLocationId_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineWithExtraValues.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Regel verwacht van 20 elementen, maar het zijn er 22.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("1")]
        [TestCase("2")]
        public void ReadLine_LineWithNoValueForLocationId_ThrowsLineParseException(string fileNamePostFix)
        {
            // Setup
            string fileName = string.Format("InvalidFile_1Location_SecondLineLocationIdEmpty{0}.csv",
                                            fileNamePostFix);
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", fileName));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: 'Identificatie' kolom mag geen lege waardes bevatten.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("1")]
        [TestCase("2")]
        public void ReadLine_LineWithNoValueForParameterId_ThrowsLineParseException(string fileNamePostFix)
        {
            // Setup
            string fileName = string.Format("InvalidFile_1Location_SecondLineParameterIdEmpty{0}.csv",
                                            fileNamePostFix);
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", fileName));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: 'Kunstwerken.identificatie' kolom mag geen lege waardes bevatten.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithPlainTextForNumericValue_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineNumericValueNotNumber.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Nummerieke waarde kan niet worden omgezet naar een getal.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithNumericValueTooLarge_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineNumericValueTooLarge.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Nummerieke waarde is te groot of te klein om ingelezen te worden.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithNumericValueTooSmall_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineNumericValueTooSmall.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Nummerieke waarde is te groot of te klein om ingelezen te worden.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithPlainTextForVarianceValue_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceValueNotNumber.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Variantie waarde kan niet worden omgezet naar een getal.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithVarianceValueTooLarge_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceValueTooLarge.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Variantie waarde is te groot of te klein om ingelezen te worden.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithVarianceValueTooSmall_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceValueTooSmall.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: Variantie waarde is te groot of te klein om ingelezen te worden.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithPlainTextForVarianceType_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceTypeNotNumber.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithVarianceTypeTooLarge_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceTypeTooLarge.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithVarianceTypeTooSmall_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceTypeTooSmall.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithVarianceTypeMinusOne_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceTypeValueMinusOne.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadLine_LineWithVarianceTypeTwo_ThrowsLineParseException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "InvalidFile_1Location_SecondLineVarianceTypeValueTwo.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.ReadLine();

                // Assert
                string message = Assert.Throws<LineParseException>(call).Message;
                string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' op regel 2: De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.",
                                                       filePath);
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase(1, "KUNST1", "KW_HOOGTE1", 45.0, 0.0, VarianceType.CoefficientOfVariation)]
        [TestCase(3, "KUNST1", "KW_HOOGTE3", 18.5, 0.05, VarianceType.StandardDeviation)]
        [TestCase(13, "KUNST2", "KW_HOOGTE5", double.NaN, 0.05, VarianceType.StandardDeviation)]
        public void ReadLine_ValidFile_ReturnDataFromLine(int lineNumber, string expectedLocationId, string paramterId,
                                                          double expectedNumericValue, double expectedVarianceValue, VarianceType expectedType)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "ValidFile_2Locations_AllHeightStructureParameters.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                StructuresParameterRow parameter = null;
                for (int i = 0; i < lineNumber; i++)
                {
                    parameter = reader.ReadLine();
                }

                // Assert
                Assert.IsNotNull(parameter);
                Assert.AreEqual(expectedLocationId, parameter.LocationId);
                Assert.AreEqual(paramterId, parameter.ParameterId);
                Assert.AreEqual(expectedNumericValue, parameter.NumericalValue);
                Assert.AreEqual(expectedVarianceValue, parameter.VarianceValue);
                Assert.AreEqual(expectedType, parameter.VarianceType);
            }
        }

        [Test]
        public void ReadLine_ValidFileWithEmptyVarianceValue_ReturnNaN()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "ValidFile_1Location_AllHeightStructureParameters_VarianceValueEmpty.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                StructuresParameterRow parameter = reader.ReadLine();

                // Assert
                Assert.IsNaN(parameter.VarianceValue);
            }
        }

        [Test]
        public void ReadLine_ValidFileWithEmptyVarianceType_ReturnNotSpecified()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresCharacteristicsCsvFiles", "ValidFile_1Location_AllHeightStructureParameters_VarianceTypeEmpty.csv"));

            using (var reader = new StructuresCharacteristicsCsvReader(filePath))
            {
                // Call
                StructuresParameterRow parameter = reader.ReadLine();

                // Assert
                Assert.AreEqual(VarianceType.NotSpecified, parameter.VarianceType);
            }
        }
    }
}