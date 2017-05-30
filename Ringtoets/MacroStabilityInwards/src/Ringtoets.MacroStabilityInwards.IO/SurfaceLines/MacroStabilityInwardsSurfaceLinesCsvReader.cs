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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.SurfaceLines
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv), containing
    /// data specifying surfacelines. 
    /// Expects data to be specified in the following format:
    /// <para><c>{ID};X1;Y1;Z1...;(Xn;Yn;Zn)</c></para>
    /// Where {ID} has to be a particular accepted text, and n triplets of doubles form the
    /// 3D coordinates defining the geometric shape of the surfaceline.
    /// </summary>
    public class MacroStabilityInwardsSurfaceLinesCsvReader : IDisposable
    {
        private const char separator = ';';

        private readonly string[] acceptableLowerCaseIdNames =
        {
            "profielnaam",
            "locationid"
        };

        private readonly string filePath;

        private readonly string[] expectedFirstCoordinateHeader =
        {
            "x1",
            "y1",
            "z1"
        };

        private StreamReader fileReader;
        private int lineNumber;

        private int idNameColumnIndex;
        private int startGeometryColumnIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsSurfaceLinesCsvReader"/> class
        /// and opens a given file path.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid.</exception>
        public MacroStabilityInwardsSurfaceLinesCsvReader(string path)
        {
            IOUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Reads the file to determine the number of available <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>
        /// data rows.
        /// </summary>
        /// <returns>A value greater than or equal to 0.</returns>
        /// <exception cref="CriticalFileReadException">A critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>The file cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>The file incompatible for importing surface lines.</item>
        /// </list>
        /// </exception>
        public int GetSurfaceLinesCount()
        {
            using (StreamReader reader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                ValidateHeader(reader, 1);

                return CountNonEmptyLines(reader, 2);
            }
        }

        /// <summary>
        /// Reads and consumes the next data row which contains a surface line, parsing the data to create an instance 
        /// of <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <returns>Return the parsed surfaceline, or null when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">A critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>The file cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>The file incompatible for importing surface lines.</item>
        /// </list>
        /// </exception>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row doesn't use ';' as separator character.</item>
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// <item>The row is missing an identifier value.</item>
        /// <item>The row is missing values to form a surface line point.</item>
        /// </list>
        /// </exception>
        public RingtoetsMacroStabilityInwardsSurfaceLine ReadSurfaceLine()
        {
            if (fileReader == null)
            {
                fileReader = StreamReaderHelper.InitializeStreamReader(filePath);

                ValidateHeader(fileReader, 1);
                lineNumber = 2;
            }

            string readText = ReadNextNonEmptyLine();

            if (readText != null)
            {
                try
                {
                    return CreateRingtoetsMacroStabilityInwardsSurfaceLine(readText);
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
        /// Reads lines from file until the first non-white line is hit.
        /// </summary>
        /// <returns>The next line which is not a white line, or <c>null</c> when no non-white line could be found before the
        /// end of file.</returns>
        private string ReadNextNonEmptyLine()
        {
            string readText;
            while ((readText = ReadLineAndHandleIOExceptions(fileReader, lineNumber)) != null)
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

        private RingtoetsMacroStabilityInwardsSurfaceLine CreateRingtoetsMacroStabilityInwardsSurfaceLine(string readText)
        {
            string[] tokenizedString = TokenizeString(readText);

            string surfaceLineName = GetSurfaceLineName(tokenizedString);
            IEnumerable<Point3D> points = GetSurfaceLinePoints(tokenizedString, surfaceLineName);

            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(points);

            CheckIfGeometryIsValid(surfaceLine);

            return surfaceLine;
        }

        /// <summary>
        /// Checks if the geometry defining the surface line is valid.
        /// </summary>
        /// <param name="surfaceLine">The surface line to be checked.</param>
        /// <exception cref="LineParseException">Surface line geometry is invalid.</exception>
        private void CheckIfGeometryIsValid(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            CheckZeroLength(surfaceLine);
            CheckReclinging(surfaceLine);
        }

        private void CheckZeroLength(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            Point3D lastPoint = null;
            foreach (Point3D point in surfaceLine.Points)
            {
                if (lastPoint != null)
                {
                    if (!Equals(lastPoint, point))
                    {
                        return;
                    }
                }
                lastPoint = point;
            }
            throw CreateLineParseException(lineNumber, surfaceLine.Name, Resources.MacroStabilityInwardsSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_zero_length);
        }

        private void CheckReclinging(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            double[] lCoordinates = surfaceLine.ProjectGeometryToLZ().Select(p => p.X).ToArray();
            for (var i = 1; i < lCoordinates.Length; i++)
            {
                if (lCoordinates[i - 1] > lCoordinates[i])
                {
                    throw CreateLineParseException(lineNumber, surfaceLine.Name, Resources.MacroStabilityInwardsSurfaceLinesCsvReader_ReadLine_SurfaceLine_has_reclining_geometry);
                }
            }
        }

        /// <summary>
        /// Tokenizes a string using a separator character up to the first empty field.
        /// </summary>
        /// <param name="readText">The text.</param>
        /// <returns>The tokenized parts.</returns>
        /// <exception cref="LineParseException"><paramref name="readText"/> lacks separator character.</exception>
        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                throw CreateLineParseException(lineNumber, string.Format(Resources.MacroStabilityInwardsSurfaceLinesCsvReader_ReadLine_Line_lacks_separator_0_,
                                                                         separator));
            }
            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }

        /// <summary>
        /// Gets the 3D surface line points.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string.</param>
        /// <param name="surfaceLineName">The name of the surface line for which geometry points are retrieved.</param>
        /// <returns>Set of all 3D world coordinate points.</returns>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item><paramref name="tokenizedString"/> contains a coordinate value that cannot be parsed as a double.</item>
        /// <item><paramref name="tokenizedString"/> contains a number that is too big or too small to be represented with a double.</item>
        /// <item><paramref name="tokenizedString"/> is missing coordinate values to define a proper surface line point.</item>
        /// </list>
        /// </exception>
        private IEnumerable<Point3D> GetSurfaceLinePoints(string[] tokenizedString, string surfaceLineName)
        {
            const int expectedValuesForPoint = 3;

            double[] worldCoordinateValues = ParseWorldCoordinateValuesAndHandleParseErrors(tokenizedString, surfaceLineName);
            if (worldCoordinateValues.Length % expectedValuesForPoint != 0)
            {
                throw CreateLineParseException(lineNumber, surfaceLineName, Resources.MacroStabilityInwardsSurfaceLinesCsvReader_ReadLine_SurfaceLine_lacks_values_for_coordinate_triplet);
            }

            int coordinateCount = worldCoordinateValues.Length / expectedValuesForPoint;
            var points = new Point3D[coordinateCount];
            for (var i = 0; i < coordinateCount; i++)
            {
                double x = worldCoordinateValues[i * expectedValuesForPoint];
                double y = worldCoordinateValues[i * expectedValuesForPoint + 1];
                double z = worldCoordinateValues[i * expectedValuesForPoint + 2];
                points[i] = new Point3D(x, y, z);
            }
            return points;
        }

        /// <summary>
        /// Gets the name of the surface line.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string from which the name should be extracted.</param>
        /// <returns>The name of the surface line.</returns>
        /// <exception cref="LineParseException">Id value is null or empty.</exception>
        private string GetSurfaceLineName(IList<string> tokenizedString)
        {
            string name = tokenizedString.Any() ? tokenizedString[idNameColumnIndex].Trim() : string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                throw CreateLineParseException(lineNumber, Resources.MacroStabilityInwardsSurfaceLinesCsvReader_ReadLine_Line_lacks_ID);
            }
            return name;
        }

        /// <summary>
        /// Parses the world coordinate values and handles parse errors.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string.</param>
        /// <param name="surfaceLineName">The name of the surface line whose coordinate values are being parsed.</param>
        /// <returns></returns>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// </list>
        /// </exception>
        private double[] ParseWorldCoordinateValuesAndHandleParseErrors(string[] tokenizedString, string surfaceLineName)
        {
            try
            {
                return tokenizedString.Skip(startGeometryColumnIndex)
                                      .Select(ts => double.Parse(ts, CultureInfo.InvariantCulture))
                                      .ToArray();
            }
            catch (FormatException e)
            {
                throw CreateLineParseException(lineNumber, surfaceLineName, Resources.Error_SurfaceLine_has_not_double, e);
            }
            catch (OverflowException e)
            {
                throw CreateLineParseException(lineNumber, surfaceLineName, Resources.Error_SurfaceLine_parsing_causes_overflow, e);
            }
        }

        /// <summary>
        /// Validates the header of the file.
        /// </summary>
        /// <param name="reader">The reader, which is currently at the header row.</param>
        /// <param name="currentLine">Row index used in error messaging.</param>
        /// <exception cref="CriticalFileReadException">The header is not in the required format.</exception>
        private void ValidateHeader(TextReader reader, int currentLine)
        {
            string header = ReadLineAndHandleIOExceptions(reader, currentLine);
            if (header != null)
            {
                if (!IsHeaderValid(header))
                {
                    throw CreateCriticalFileReadException(currentLine, Resources.MacroStabilityInwardsSurfaceLinesCsvReader_File_invalid_header);
                }
            }
            else
            {
                throw CreateCriticalFileReadException(currentLine, UtilsResources.Error_File_empty);
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="CriticalFileReadException"/> with message and inner exception set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(UtilsResources.TextFile_On_LineNumber_0_, currentLine);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <returns>New <see cref="LineParseException"/> with message set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string lineParseErrorMessage)
        {
            string locationDescription = string.Format(UtilsResources.TextFile_On_LineNumber_0_, currentLine);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .Build(lineParseErrorMessage);
            return new LineParseException(message);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="surfaceLineName">The name of the surfaceline being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="LineParseException"/> with message and inner exceptions set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string surfaceLineName, string lineParseErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(UtilsResources.TextFile_On_LineNumber_0_, currentLine);
            string subjectDescription = string.Format(Resources.MacroStabilityInwardsSurfaceLinesCsvReader_SurfaceLineName_0_, surfaceLineName);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .WithSubject(subjectDescription)
                                                                        .Build(lineParseErrorMessage);
            return new LineParseException(message, innerException);
        }

        /// <summary>
        /// Counts the remaining non-empty lines.
        /// </summary>
        /// <param name="reader">The reader at the row from which counting should start.</param>
        /// <param name="currentLine">The current line, used for error messaging.</param>
        /// <returns>An integer greater than or equal to 0, being the number of surfaceline rows.</returns>
        /// <exception cref="CriticalFileReadException">An I/O exception occurred.</exception>
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
        /// <returns>The read line, or null when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">An critical I/O exception occurred.</exception>
        private string ReadLineAndHandleIOExceptions(TextReader reader, int currentLine)
        {
            try
            {
                return reader.ReadLine();
            }
            catch (OutOfMemoryException e)
            {
                throw CreateCriticalFileReadException(currentLine, UtilsResources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(string.Format(UtilsResources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }

        private bool IsHeaderValid(string header)
        {
            string[] tokenizedHeader = header.Split(separator).Select(s => s.Trim().ToLowerInvariant()).ToArray();

            // Check for valid id:
            DetermineIdNameColumnIndex(tokenizedHeader);

            if (idNameColumnIndex == -1)
            {
                return false;
            }

            // Check for valid 1st coordinate in header:
            DetermineStartGeometryColumnIndex(tokenizedHeader);

            if (startGeometryColumnIndex == -1)
            {
                return false;
            }

            var valid = true;
            for (var i = 0; i < expectedFirstCoordinateHeader.Length && valid; i++)
            {
                valid = tokenizedHeader[startGeometryColumnIndex + i].Equals(expectedFirstCoordinateHeader[i]);
            }
            return valid;
        }

        private void DetermineStartGeometryColumnIndex(string[] tokenizedHeader)
        {
            startGeometryColumnIndex = Array.IndexOf(tokenizedHeader, expectedFirstCoordinateHeader[0]);
        }

        private void DetermineIdNameColumnIndex(string[] tokenizedHeader)
        {
            idNameColumnIndex = -1;
            foreach (string name in acceptableLowerCaseIdNames)
            {
                idNameColumnIndex = Array.IndexOf(tokenizedHeader, name);
                if (idNameColumnIndex > -1)
                {
                    break;
                }
            }
        }
    }
}