// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv) containing
    /// data specifying characteristics of height structures.
    /// </summary>
    public class StructuresCharacteristicsCsvReader : IDisposable
    {
        private const char separator = ';';
        private readonly string filePath;

        private readonly string[] requiredHeaderColumns =
        {
            "identificatie",
            "kunstwerken.identificatie",
            "numeriekeWaarde",
            "standarddeviatie.variance",
            "boolean"
        };

        private int locationIdIndex, parameterIdIndex, numericValueIndex, varianceValueIndex, varianceTypeIndex;

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
            FileUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Counts the number of parameter definitions found in the file.
        /// </summary>
        /// <returns>An integer greater than or equal to 0, being the number of parameter rows.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">File/directory cannot be found or 
        /// some other I/O related problem occurred or the header is not in the required format.</exception>
        public int GetLineCount()
        {
            using (var reader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                ValidateHeader(reader, 1);

                return CountNonEmptyLines(reader, 2);
            }
        }

        /// <summary>
        /// Reads the next structure parameter from file.
        /// </summary>
        /// <returns>The next <see cref="Ringtoets.Common.IO.Structures.StructuresParameterRow"/> based on the read file,
        /// or null when at the end of the file.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">
        /// Thrown when either:
        /// <list type="bullet">
        /// <item>The file or directory cannot be found.</item>
        /// <item>The file is empty.</item>
        /// <item>Some I/O related problem occurred.</item>
        /// <item>The header is not in the required format.</item>
        /// </list></exception>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item>The line does not contain the separator character.</item>
        /// <item>Location id field is empty or consists out of only white spaces.</item>
        /// <item>Parameter id field is empty or consists out of only white spaces.</item>
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
            if (fileReader != null)
            {
                fileReader.Dispose();
                fileReader = null;
            }
        }

        /// <summary>
        /// Validates the header of the file.
        /// </summary>
        /// <param name="reader">The reader, which is currently at the header row.</param>
        /// <param name="currentLine">Row index used in error messaging.</param>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">The header is not in the required format.</exception>
        private void ValidateHeader(TextReader reader, int currentLine)
        {
            string[] tokenizedHeader = GetTokenizedHeader(reader);
            const int uninitializedValue = -999;
            int[] requiredHeaderColumnIndices = GetRequiredHeaderColumnIndices(uninitializedValue, tokenizedHeader);
            ValidateRequiredColumnIndices(currentLine, requiredHeaderColumnIndices, uninitializedValue);
        }

        /// <summary>
        /// Counts the remaining non-empty lines.
        /// </summary>
        /// <param name="reader">The reader at the row from which counting should start.</param>
        /// <param name="currentLine">The current line, used for error messaging.</param>
        /// <returns>An integer greater than or equal to 0, being the number of parameter rows.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">An I/O exception occurred.</exception>
        private int CountNonEmptyLines(TextReader reader, int currentLine)
        {
            int count = 0, lineNumberForMessage = currentLine;
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
        /// <returns>The read line, or null when at the end of the file.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">An critical I/O exception occurred.</exception>
        private string ReadLineAndHandleIOExceptions(TextReader reader, int currentLine)
        {
            try
            {
                return reader.ReadLine();
            }
            catch (OutOfMemoryException e)
            {
                throw CreateCriticalFileReadException(currentLine, CoreCommonUtilsResources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                string errorMessage = string.Format((string)CoreCommonUtilsResources.Error_General_IO_ErrorMessage_0_,
                                                    e.Message);
                var fullErrorMessage = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);
                throw new CriticalFileReadException(fullErrorMessage, e);
            }
        }

        /// <summary>
        /// Reads the header and sets the internal indices of the required header columns.
        /// </summary>
        /// <param name="reader">The reader used to read the file.</param>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">The file is empty or some I/O exception
        /// occurred or the header is not in the required format.</exception>
        private void IndexFile(TextReader reader)
        {
            string[] tokenizedHeader = GetTokenizedHeader(reader);

            const int uninitializedValue = -999;
            int[] requiredHeaderColumnIndices = GetRequiredHeaderColumnIndices(uninitializedValue, tokenizedHeader);
            ValidateRequiredColumnIndices(lineNumber, requiredHeaderColumnIndices, uninitializedValue);

            SetColumnIndices(requiredHeaderColumnIndices);
        }

        /// <summary>
        /// Tokenizes the file header.
        /// </summary>
        /// <param name="reader">The reader used to read the file.</param>
        /// <returns>The header split based on <see cref="separator"/>.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">The file is empty or some I/O exception
        /// occurred.</exception>
        private string[] GetTokenizedHeader(TextReader reader)
        {
            var header = ReadLineAndHandleIOExceptions(reader, lineNumber);
            if (header == null)
            {
                throw CreateCriticalFileReadException(lineNumber, CoreCommonUtilsResources.Error_File_empty);
            }

            return header.Split(separator)
                         .Select(s => s.Trim().ToLowerInvariant())
                         .ToArray();
        }

        private int[] GetRequiredHeaderColumnIndices(int initialColumnIndexValue, string[] tokenizedHeader)
        {
            int[] requiredHeaderColumnIndices = Enumerable.Repeat(initialColumnIndexValue, requiredHeaderColumns.Length)
                                                          .ToArray();
            for (int columnIndex = 0; columnIndex < tokenizedHeader.Length; columnIndex++)
            {
                string columnName = tokenizedHeader[columnIndex];
                int index = Array.IndexOf(requiredHeaderColumns, columnName);
                if (index != -1)
                {
                    // TODO: same column multiple times!
                    requiredHeaderColumnIndices[index] = columnIndex;
                }
            }
            return requiredHeaderColumnIndices;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentLine"></param>
        /// <param name="requiredHeaderColumnIndices"></param>
        /// <param name="uninitializedValue"></param>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">The header is not in the required format.</exception>
        private void ValidateRequiredColumnIndices(int currentLine, int[] requiredHeaderColumnIndices, int uninitializedValue)
        {
            if (requiredHeaderColumnIndices.Any(i => i == uninitializedValue))
            {
                throw CreateCriticalFileReadException(currentLine, string.Format("Het bestand is niet geschikt om kunstwerken parameters uit te lezen (Verwachte koptekst moet de volgende kolommen bevatten: {0}.",
                                                                                 string.Join(", ", requiredHeaderColumns)));
            }
        }

        private void SetColumnIndices(int[] requiredHeaderColumnIndices)
        {
            locationIdIndex = requiredHeaderColumnIndices[0];
            parameterIdIndex = requiredHeaderColumnIndices[1];
            numericValueIndex = requiredHeaderColumnIndices[2];
            varianceValueIndex = requiredHeaderColumnIndices[3];
            varianceTypeIndex = requiredHeaderColumnIndices[4];
        }

        /// <summary>
        /// Reads lines from file until the first non-white line is hit.
        /// </summary>
        /// <returns>The next line which is not a white line, or <c>null</c> when no non-white
        /// line could be found before the end of file.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">An critical I/O exception occurred.</exception>
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
        /// <returns></returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="readText"/> does not contain the separator character.</item>
        /// <item>Location id field is empty or consists out of only white spaces.</item>
        /// <item>Parameter id field is empty or consists out of only white spaces.</item>
        /// <item>Numeric value field is not a number or too large/small to be represented as <see cref="double"/>.</item>
        /// <item>Variance value field is not a number or too large/small to be represented as <see cref="double"/>.</item>
        /// <item>Boolean field is not a valid value.</item>
        /// </list></exception>
        private StructuresParameterRow CreateStructuresParameterRow(string readText)
        {
            string[] tokenizedText = TokenizeString(readText);

            // TODO: tokenizedText.Length smaller than largest index value => LineParseException

            string locationId = ParseLocationId(tokenizedText);
            string parameterId = ParseParameterId(tokenizedText);
            double numbericValue = ParseNumericValue(tokenizedText);
            double varianceValue = ParseVarianceValue(tokenizedText);
            VarianceType varianceType = ParseVarianceType(tokenizedText);

            return new StructuresParameterRow
            {
                LocationId = locationId,
                ParameterId = parameterId,
                NumericalValue = numbericValue,
                VarianceValue = varianceValue,
                VarianceType = varianceType
            };
        }

        /// <summary>
        /// Tokenizes a string using a separator character up to the first empty field.
        /// </summary>
        /// <param name="readText">The text.</param>
        /// <returns>The tokenized parts.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException"><paramref name="readText"/> lacks separator character.</exception>
        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                throw CreateLineParseException(lineNumber, string.Format("Regel ontbreekt het verwachte scheidingsteken (het karakter: {0}).",
                                                                         separator));
            }
            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }

        /// <summary>
        /// Parses the location identifier from the read text.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The location ID.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">Location ID field is empty or only has whitespaces.</exception>
        private string ParseLocationId(string[] tokenizedText)
        {
            string locationId = tokenizedText[locationIdIndex];
            if (string.IsNullOrWhiteSpace(locationId))
            {
                throw CreateLineParseException(lineNumber, "'Identificatie' kolom mag geen lege waardes bevatten.");
            }
            return locationId;
        }

        /// <summary>
        /// Parses the parameter identifier from the read text.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns></returns>
        /// <returns>The parameter ID.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">Parameter ID field is empty or only has whitespaces.</exception>
        private string ParseParameterId(string[] tokenizedText)
        {
            string parameterId = tokenizedText[parameterIdIndex];
            if (string.IsNullOrWhiteSpace(parameterId))
            {
                throw CreateLineParseException(lineNumber, "'Kunstwerken.identificatie' kolom mag geen lege waardes bevatten.");
            }
            return parameterId;
        }

        /// <summary>
        /// Parses the numeric value.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The numeric value (can be <see cref="double.NaN"/>).</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">When the numeric value field is not a number
        /// or when it's too large or too small to be represented as <see cref="double"/>.</exception>
        private double ParseNumericValue(string[] tokenizedText)
        {
            string numericValueText = tokenizedText[numericValueIndex];
            return ParseDoubleValue(numericValueText, "Nummerieke waarde");
        }

        /// <summary>
        /// Parses the standard deviation or coefficient of variation value.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The standard deviation or coefficient of variation value (can be <see cref="double.NaN"/>).</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">When the standard deviation or coefficient
        /// of variation value field is not a number or when it's too large or too small
        /// to be represented as <see cref="double"/>.</exception>
        private double ParseVarianceValue(string[] tokenizedText)
        {
            string varianceValueText = tokenizedText[varianceValueIndex];
            return ParseDoubleValue(varianceValueText, "Variantie waarde");
        }

        /// <summary>
        /// Parses the double value.
        /// </summary>
        /// <param name="doubleValueText">The value text to be parsed.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns><see cref="double.NaN"/> when <paramref name="doubleValueText"/> is null
        /// or only whitespaces; otherwise the parsed number.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">When <paramref name="doubleValueText"/> is
        /// not a number or when it's too large or too small to be represented as <see cref="double"/>.</exception>
        private double ParseDoubleValue(string doubleValueText, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(doubleValueText))
            {
                return double.NaN;
            }
            try
            {
                return double.Parse(doubleValueText);
            }
            catch (FormatException e)
            {
                throw CreateLineParseException(lineNumber, string.Format("{0} kan niet worden omgezet naar een getal.",
                                                                         parameterName), e);
            }
            catch (OverflowException e)
            {
                throw CreateLineParseException(lineNumber, string.Format("{0} is te groot of te klein om ingelezen te worden.",
                                                                         parameterName), e);
            }
        }

        /// <summary>
        /// Parses the value that indicates how the variance field should be interpreted.
        /// </summary>
        /// <param name="tokenizedText">The tokenized text.</param>
        /// <returns>The <see cref="Ringtoets.Common.IO.Structures.VarianceType"/> based on the text in the file.</returns>
        /// <exception cref="Core.Common.IO.Exceptions.LineParseException">When the 'boolean' field is not a valid value.</exception>
        private VarianceType ParseVarianceType(string[] tokenizedText)
        {
            string varianceTypeText = tokenizedText[varianceTypeIndex];
            if (string.IsNullOrWhiteSpace(varianceTypeText))
            {
                return VarianceType.NotSpecified;
            }
            try
            {
                int typeValue = int.Parse(varianceTypeText);
                if (typeValue == 0)
                {
                    return VarianceType.CoefficientOfVariation;
                }
                if (typeValue == 1)
                {
                    return VarianceType.StandardDeviation;
                }
                throw CreateLineParseException(lineNumber, "De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.");
            }
            catch (FormatException e)
            {
                throw CreateLineParseException(lineNumber, "De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.", e);
            }
            catch (OverflowException e)
            {
                throw CreateLineParseException(lineNumber, "De 'Boolean' kolom mag uitsluitend de waardes '0' of '1' bevatten, of mag leeg zijn.", e);
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="Core.Common.IO.Exceptions.LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="Core.Common.IO.Exceptions.LineParseException"/> with message set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string lineParseErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(CoreCommonUtilsResources.TextFile_On_LineNumber_0_, currentLine);
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                     .Build(lineParseErrorMessage);
            return new LineParseException(message, innerException);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="Core.Common.IO.Exceptions.CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="Core.Common.IO.Exceptions.CriticalFileReadException"/> with message and inner exception set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(CoreCommonUtilsResources.TextFile_On_LineNumber_0_,
                                                       currentLine);
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                     .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }
    }
}