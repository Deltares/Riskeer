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
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Common.Util.Extensions;
using Ringtoets.Common.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv) containing
    /// data specifying characteristics of structures.
    /// </summary>
    public class StructuresCharacteristicsCsvReader : IDisposable
    {
        private const char separator = ';';
        private readonly string filePath;

        private readonly string[] requiredHeaderColumns =
        {
            StructureFilesKeywords.IdentificationColumnName,
            StructureFilesKeywords.StructureIdentificationColumnName,
            StructureFilesKeywords.AlphanumericalValueColumnName,
            StructureFilesKeywords.NumericalValueColumnName,
            StructureFilesKeywords.VariationValueColumnName,
            StructureFilesKeywords.VariationTypeColumnName
        };

        private int locationIdIndex, parameterIdIndex, alphanumericValueIndex, numericValueIndex, varianceValueIndex, varianceTypeIndex;
        private int headerLength;

        private int lineNumber;
        private StreamReader fileReader;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresCharacteristicsCsvReader"/>
        /// and opens a given file path.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        public StructuresCharacteristicsCsvReader(string path)
        {
            IOUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Counts the number of parameter definitions found in the file.
        /// </summary>
        /// <returns>An integer greater than or equal to 0, being the number of parameter rows.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the file/directory cannot be found, 
        /// some I/O related problem occurred or the header is not in the required format.</exception>
        public int GetLineCount()
        {
            using (StreamReader reader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                lineNumber = 1;
                ValidateHeader(reader);

                return CountNonEmptyLines(reader, 2);
            }
        }

        /// <summary>
        /// Reads the next structure parameter from file.
        /// </summary>
        /// <returns>The next <see cref="StructuresParameterRow"/> based on the read file,
        /// or <c>null</c> when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">
        /// Thrown when either:
        /// <list type="bullet">
        /// <item>The file or directory cannot be found.</item>
        /// <item>The file is empty.</item>
        /// <item>Some I/O related problem occurred.</item>
        /// <item>The header is not in the required format.</item>
        /// </list></exception>
        /// <exception cref="LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item>The line does not contain the separator character.</item>
        /// <item>Location id field is empty or consists only of white-space characters.</item>
        /// <item>Parameter id field is empty or consists only of white-space characters.</item>
        /// <item>Numeric value field is not a number or too large/small to be represented as <see cref="double"/>.</item>
        /// <item>Variance value field is not a number or too large/small to be represented as <see cref="double"/>.</item>
        /// <item>Boolean field is not a valid value.</item>
        /// </list></exception>
        public StructuresParameterRow ReadLine()
        {
            if (fileReader == null)
            {
                fileReader = StreamReaderHelper.InitializeStreamReader(filePath);

                lineNumber = 1;
                IndexFile(fileReader);
                lineNumber++;
            }

            string readText = ReadNextNonEmptyLine(fileReader);
            if (readText != null)
            {
                try
                {
                    return CreateStructuresParameterRow(readText);
                }
                finally
                {
                    lineNumber++;
                }
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (fileReader != null && disposing)
            {
                fileReader.Dispose();
                fileReader = null;
            }
        }

        /// <summary>
        /// Validates the header of the file.
        /// </summary>
        /// <param name="reader">The reader, which is currently at the header row.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the header is not in the required format.</exception>
        private void ValidateHeader(TextReader reader)
        {
            string[] tokenizedHeader = GetTokenizedHeader(reader);
            const int uninitializedValue = -999;
            int[] requiredHeaderColumnIndices = GetRequiredHeaderColumnIndices(uninitializedValue, tokenizedHeader);
            ValidateRequiredColumnIndices(requiredHeaderColumnIndices, uninitializedValue);
        }

        /// <summary>
        /// Counts the remaining non-empty lines.
        /// </summary>
        /// <param name="reader">The reader at the row from which counting should start.</param>
        /// <param name="currentLine">The current line, used for error messaging.</param>
        /// <returns>An integer greater than or equal to 0, being the number of parameter rows.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when an I/O exception occurred.</exception>
        private int CountNonEmptyLines(TextReader reader, int currentLine)
        {
            var count = 0;
            int lineNumberForMessage = currentLine;
            string line;
            while ((line = ReadLineAndHandleIOExceptions(reader, lineNumberForMessage)) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    count++;
                }

                lineNumberForMessage++;
            }

            return count;
        }

        /// <summary>
        /// Reads the next line and handles I/O exceptions.
        /// </summary>
        /// <param name="reader">The opened text file reader.</param>
        /// <param name="currentLine">Row number for error messaging.</param>
        /// <returns>The read line, or <c>null</c> when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a critical I/O exception occurred.</exception>
        private string ReadLineAndHandleIOExceptions(TextReader reader, int currentLine)
        {
            try
            {
                return reader.ReadLine();
            }
            catch (OutOfMemoryException e)
            {
                throw CreateCriticalFileReadException(currentLine, CoreCommonUtilResources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                string errorMessage = string.Format(CoreCommonUtilResources.Error_General_IO_ErrorMessage_0_,
                                                    e.Message);
                string fullErrorMessage = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);
                throw new CriticalFileReadException(fullErrorMessage, e);
            }
        }

        /// <summary>
        /// Reads the header and sets the internal indices of the required header columns.
        /// </summary>
        /// <param name="reader">The reader used to read the file.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the file is empty, some I/O exception
        /// occurred, or the header is not in the required format.</exception>
        private void IndexFile(TextReader reader)
        {
            string[] tokenizedHeader = GetTokenizedHeader(reader);

            headerLength = tokenizedHeader.Length;

            const int uninitializedValue = -999;
            int[] requiredHeaderColumnIndices = GetRequiredHeaderColumnIndices(uninitializedValue, tokenizedHeader);
            ValidateRequiredColumnIndices(requiredHeaderColumnIndices, uninitializedValue);

            SetColumnIndices(requiredHeaderColumnIndices);
        }

        /// <summary>
        /// Tokenizes the file header.
        /// </summary>
        /// <param name="reader">The reader used to read the file.</param>
        /// <returns>The header split based on <see cref="separator"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the file is empty or some I/O exception
        /// occurred.</exception>
        private string[] GetTokenizedHeader(TextReader reader)
        {
            string header = ReadLineAndHandleIOExceptions(reader, lineNumber);
            if (header == null)
            {
                throw CreateCriticalFileReadException(lineNumber, CoreCommonUtilResources.Error_File_empty);
            }

            return header.Split(separator)
                         .Select(s => s.Trim().ToLowerInvariant())
                         .ToArray();
        }

        private int[] GetRequiredHeaderColumnIndices(int initialColumnIndexValue, string[] tokenizedHeader)
        {
            int[] requiredHeaderColumnIndices = Enumerable.Repeat(initialColumnIndexValue, requiredHeaderColumns.Length)
                                                          .ToArray();
            for (var columnIndex = 0; columnIndex < tokenizedHeader.Length; columnIndex++)
            {
                string columnName = tokenizedHeader[columnIndex];
                int index = Array.IndexOf(requiredHeaderColumns, columnName);
                if (index != -1)
                {
                    if (requiredHeaderColumnIndices[index] == initialColumnIndexValue)
                    {
                        requiredHeaderColumnIndices[index] = columnIndex;
                    }
                    else
                    {
                        string message = string.Format(Resources.StructuresCharacteristicsCsvReader_Column_0_must_be_defined_only_once,
                                                       columnName.FirstToUpper());
                        throw CreateCriticalFileReadException(lineNumber, message);
                    }
                }
            }

            return requiredHeaderColumnIndices;
        }

        /// <summary>
        /// Checks if all required header columns have been matched.
        /// </summary>
        /// <param name="requiredHeaderColumnIndices">The array of matched column indices.</param>
        /// <param name="uninitializedValue">The initial index value put in <paramref name="requiredHeaderColumnIndices"/>.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the header is not in the required format.</exception>
        private void ValidateRequiredColumnIndices(int[] requiredHeaderColumnIndices, int uninitializedValue)
        {
            if (requiredHeaderColumnIndices.Any(i => i == uninitializedValue))
            {
                string message = string.Format(Resources.StructuresCharacteristicsCsvReader_ValidateRequiredColumnIndices_Invalid_header_Must_have_columns_0_,
                                               string.Join(Environment.NewLine, requiredHeaderColumns.Select(rh => "* " + rh.FirstToUpper())));
                throw CreateCriticalFileReadException(lineNumber, message);
            }
        }

        private void SetColumnIndices(int[] requiredHeaderColumnIndices)
        {
            locationIdIndex = requiredHeaderColumnIndices[0];
            parameterIdIndex = requiredHeaderColumnIndices[1];
            alphanumericValueIndex = requiredHeaderColumnIndices[2];
            numericValueIndex = requiredHeaderColumnIndices[3];
            varianceValueIndex = requiredHeaderColumnIndices[4];
            varianceTypeIndex = requiredHeaderColumnIndices[5];
        }

        /// <summary>
        /// Reads lines from file until the first non-white line is hit.
        /// </summary>
        /// <returns>The next line which is not a white line, or <c>null</c> when no non-white
        /// line could be found before the end of file.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a critical I/O exception occurred.</exception>
        private string ReadNextNonEmptyLine(StreamReader reader)
        {
            string readText;
            while ((readText = ReadLineAndHandleIOExceptions(reader, lineNumber)) != null)
            {
                if (string.IsNullOrWhiteSpace(readText))
                {
                    lineNumber++;
                }
                else
                {
                    break;
                }
            }

            return readText;
        }

        /// <summary>
        /// Creates the structures parameter row.
        /// </summary>
        /// <param name="readText">The read text.</param>
        /// <returns>The created structures parameter row.</returns>
        /// <exception cref="LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="readText"/> does not contain the separator character.</item>
        /// <item>Location id field is empty or consists only of white-space characters.</item>
        /// <item>Parameter id field is empty or consists only of white-space characters.</item>
        /// <item>Numeric value field is not a number or too large/small to be represented as <see cref="double"/>.</item>
        /// <item>Variance value field is not a number or too large/small to be represented as <see cref="double"/>.</item>
        /// <item>Boolean field is not a valid value.</item>
        /// </list></exception>
        private StructuresParameterRow CreateStructuresParameterRow(string readText)
        {
            string[] tokenizedText = TokenizeString(readText);

            if (tokenizedText.Length != headerLength)
            {
                string message = string.Format(Resources.StructuresCharacteristicsCsvReader_CreateStructuresParameterRow_Line_should_have_NumberOfExpectedElements_0_but_has_ActualNumberOfElements_1_,
                                               headerLength, tokenizedText.Length);
                throw CreateLineParseException(lineNumber, message);
            }

            string locationId = ParseLocationId(tokenizedText);
            string parameterId = ParseParameterId(tokenizedText);
            string alphanumericValue = ParseAlphanumericValue(tokenizedText);
            double numericValue = ParseNumericValue(tokenizedText);
            double varianceValue = ParseVarianceValue(tokenizedText);
            VarianceType varianceType = ParseVarianceType(tokenizedText);

            return new StructuresParameterRow
            {
                LocationId = locationId,
                ParameterId = parameterId,
                AlphanumericValue = alphanumericValue,
                NumericalValue = numericValue,
                VarianceValue = varianceValue,
                VarianceType = varianceType,
                LineNumber = lineNumber
            };
        }

        /// <summary>
        /// Tokenizes a string using a separator character up to the first empty field.
        /// </summary>
        /// <param name="readText">The read text.</param>
        /// <returns>The tokenized parts.</returns>
        /// <exception cref="LineParseException">Thrown when <paramref name="readText"/> lacks separator character.</exception>
        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                string message = string.Format(Resources.StructuresCharacteristicsCsvReader_TokenizeString_Line_lacks_SeparatorCharacter_0_,
                                               separator);
                throw CreateLineParseException(lineNumber, message);
            }

            return readText.Split(separator)
                           .ToArray();
        }

        /// <summary>
        /// Parses the location identifier from the read text.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The location ID.</returns>
        /// <exception cref="LineParseException">Thrown when the field Location ID is empty 
        /// or consists only of white-space characters.</exception>
        private string ParseLocationId(string[] tokenizedText)
        {
            string locationId = tokenizedText[locationIdIndex];
            return ParseIdString(locationId, StructureFilesKeywords.IdentificationColumnName.FirstToUpper());
        }

        /// <summary>
        /// Parses the parameter identifier from the read text.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The parameter ID.</returns>
        /// <exception cref="LineParseException">Thrown when the field Parameter ID is empty 
        /// or consists only of white-space characters.</exception>
        private string ParseParameterId(string[] tokenizedText)
        {
            string parameterId = tokenizedText[parameterIdIndex];
            return ParseIdString(parameterId, StructureFilesKeywords.StructureIdentificationColumnName.FirstToUpper());
        }

        private string ParseIdString(string parameterTextValue, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterTextValue))
            {
                string message = string.Format(Resources.StructuresCharacteristicsCsvReader_ParseIdString_ParameterName_0_cannot_be_empty,
                                               parameterName);
                throw CreateLineParseException(lineNumber, message);
            }

            return parameterTextValue;
        }

        private string ParseAlphanumericValue(string[] tokenizedText)
        {
            return tokenizedText[alphanumericValueIndex];
        }

        /// <summary>
        /// Parses the numeric value.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The numeric value (can be <see cref="double.NaN"/>).</returns>
        /// <exception cref="LineParseException">Thrown when the numeric value field is not a number
        /// or when it's too large or too small to be represented as <see cref="double"/>.</exception>
        private double ParseNumericValue(string[] tokenizedText)
        {
            string numericValueText = tokenizedText[numericValueIndex];
            return ParseDoubleValue(numericValueText, StructureFilesKeywords.NumericalValueColumnName.FirstToUpper());
        }

        /// <summary>
        /// Parses the standard deviation or coefficient of variation value.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The standard deviation or coefficient of variation value (can be <see cref="double.NaN"/>).</returns>
        /// <exception cref="LineParseException">Thrown when the standard deviation or coefficient
        /// of variation value field is not a number or when it's too large or too small
        /// to be represented as <see cref="double"/>.</exception>
        private double ParseVarianceValue(string[] tokenizedText)
        {
            string varianceValueText = tokenizedText[varianceValueIndex];
            return ParseDoubleValue(varianceValueText, StructureFilesKeywords.VariationValueColumnName.FirstToUpper());
        }

        /// <summary>
        /// Parses the double value.
        /// </summary>
        /// <param name="doubleValueText">The value text to be parsed.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns><see cref="double.NaN"/> when <paramref name="doubleValueText"/> is <c>null</c>
        /// or only whitespaces; otherwise the parsed value.</returns>
        /// <exception cref="LineParseException">Thrown when <paramref name="doubleValueText"/> is
        /// not a number or when it's too large or too small to be represented as <see cref="double"/>.</exception>
        private double ParseDoubleValue(string doubleValueText, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(doubleValueText))
            {
                return double.NaN;
            }

            try
            {
                return double.Parse(doubleValueText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw CreateLineParseException(lineNumber, string.Format(Resources.StructuresCharacteristicsCsvReader_ParseDoubleValue_ParameterName_0_not_number,
                                                                         parameterName), e);
            }
            catch (OverflowException e)
            {
                throw CreateLineParseException(lineNumber, string.Format(Resources.StructuresCharacteristicsCsvReader_ParseDoubleValue_ParameterName_0_overflow_error,
                                                                         parameterName), e);
            }
        }

        /// <summary>
        /// Parses the value that indicates how the variance field should be interpreted.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The <see cref="VarianceType"/> based on the text in the file.</returns>
        /// <exception cref="LineParseException">Thrown when the 'boolean' field is not a valid value.</exception>
        private VarianceType ParseVarianceType(string[] tokenizedText)
        {
            string varianceTypeText = tokenizedText[varianceTypeIndex];
            if (string.IsNullOrWhiteSpace(varianceTypeText))
            {
                return VarianceType.NotSpecified;
            }

            try
            {
                int typeValue = int.Parse(varianceTypeText, CultureInfo.InvariantCulture);
                if (typeValue == 0)
                {
                    return VarianceType.CoefficientOfVariation;
                }

                if (typeValue == 1)
                {
                    return VarianceType.StandardDeviation;
                }

                throw CreateLineParseException(lineNumber,
                                               string.Format(Resources.StructuresCharacteristicsCsvReader_ParseVarianceType_ParameterName_0_only_allows_certain_values,
                                                             StructureFilesKeywords.VariationTypeColumnName.FirstToUpper()));
            }
            catch (FormatException e)
            {
                throw CreateLineParseException(lineNumber,
                                               string.Format(Resources.StructuresCharacteristicsCsvReader_ParseVarianceType_ParameterName_0_only_allows_certain_values,
                                                             StructureFilesKeywords.VariationTypeColumnName.FirstToUpper()),
                                               e);
            }
            catch (OverflowException e)
            {
                throw CreateLineParseException(lineNumber,
                                               string.Format(Resources.StructuresCharacteristicsCsvReader_ParseVarianceType_ParameterName_0_only_allows_certain_values,
                                                             StructureFilesKeywords.VariationTypeColumnName.FirstToUpper()),
                                               e);
            }
        }

        /// <summary>
        /// Creates a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="LineParseException"/> with message set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string lineParseErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(CoreCommonUtilResources.TextFile_On_LineNumber_0_,
                                                       currentLine);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .Build(lineParseErrorMessage);
            return new LineParseException(message, innerException);
        }

        /// <summary>
        /// Creates a configured instance of <see cref="CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="CriticalFileReadException"/> with message set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(CoreCommonUtilResources.TextFile_On_LineNumber_0_,
                                                       currentLine);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }
    }
}