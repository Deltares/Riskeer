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
using Core.Common.Util;
using Core.Common.Util.Builders;
using Ringtoets.Common.IO.Properties;
using UtilResources = Core.Common.Util.Properties.Resources;

namespace Ringtoets.Common.IO.SurfaceLines
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv), containing
    /// data specifying characteristic points.
    /// Expects data to be specified in the following format:
    /// <para><c>
    /// LocationID;X_Maaiveld binnenwaarts;Y_Maaiveld binnenwaarts;Z_Maaiveld binnenwaarts;X_Insteek sloot polderzijde;Y_Insteek sloot polderzijde;Z_Insteek sloot polderzijde;X_Slootbodem polderzijde;Y_Slootbodem polderzijde;Z_Slootbodem polderzijde;X_Slootbodem dijkzijde;Y_Slootbodem dijkzijde;Z_Slootbodem dijkzijde;X_Insteek sloot dijkzijde;Y_Insteek_sloot dijkzijde;Z_Insteek sloot dijkzijde;X_Teen dijk binnenwaarts;Y_Teen dijk binnenwaarts;Z_Teen dijk binnenwaarts;X_Kruin binnenberm;Y_Kruin binnenberm;Z_Kruin binnenberm;X_Insteek binnenberm;Y_Insteek binnenberm;Z_Insteek binnenberm;X_Kruin binnentalud;Y_Kruin binnentalud;Z_Kruin binnentalud;X_Verkeersbelasting kant binnenwaarts;Y_Verkeersbelasting kant binnenwaarts;Z_Verkeersbelasting kant binnenwaarts;X_Verkeersbelasting kant buitenwaarts;Y_Verkeersbelasting kant buitenwaarts;Z_Verkeersbelasting kant buitenwaarts;X_Kruin buitentalud;Y_Kruin buitentalud;Z_Kruin buitentalud;X_Insteek buitenberm;Y_Insteek buitenberm;Z_Insteek buitenberm;X_Kruin buitenberm;Y_Kruin buitenberm;Z_Kruin buitenberm;X_Teen dijk buitenwaarts;Y_Teen dijk buitenwaarts;Z_Teen dijk buitenwaarts;X_Maaiveld buitenwaarts;Y_Maaiveld buitenwaarts;Z_Maaiveld buitenwaarts;X_Dijktafelhoogte;Y_Dijktafelhoogte;Z_Dijktafelhoogte;Volgnummer
    /// </c></para>
    /// Where {LocationID} has to be a particular accepted text, {Volgnummer} is ignored, and n triplets of doubles form the
    /// 3D coordinates defining each characteristic point.
    /// </summary>
    public class CharacteristicPointsCsvReader : IDisposable
    {
        private const char separator = ';';
        private const string xPrefix = "x_";
        private const string yPrefix = "y_";
        private const string zPrefix = "z_";

        private static readonly Point3D undefinedPoint = new Point3D(-1, -1, -1);

        private readonly string filePath;

        /// <summary>
        /// Lower case string representations of the known characteristic point types.
        /// </summary>
        private readonly Dictionary<string, int> columnsInFile = new Dictionary<string, int>();

        private StreamReader fileReader;

        private int lineNumber;

        /// <summary>
        /// Initializes a new instance of <see cref="CharacteristicPointsCsvReader"/> using
        /// the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to use for reading characteristic points.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        public CharacteristicPointsCsvReader(string path)
        {
            IOUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Reads the file to determine the number of available <see cref="CharacteristicPoints"/>
        /// data rows.
        /// </summary>
        /// <returns>A value greater than or equal to 0.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>The file cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>The file is incompatible for importing characteristic points locations.</item>
        /// </list>
        /// </exception>
        public int GetLocationsCount()
        {
            using (StreamReader reader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                ValidateHeader(reader);

                return CountNonEmptyLines(reader, 2);
            }
        }

        /// <summary>
        /// Reads and parses the next data row to create a new instance of <see cref="CharacteristicPoints"/>,
        /// which will contain all the declared characteristic points for some surface line.
        /// </summary>
        /// <returns>Returns the parsed characteristic points location, or <c>null</c> when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>The file cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>The file is incompatible for importing characteristic points.</item>
        /// </list>
        /// </exception>
        /// <exception cref="LineParseException">Thrown when a parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row doesn't use ';' as separator character.</item>
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// <item>The row is missing an identifier value.</item>
        /// <item>The row is missing values to form a characteristic point.</item>
        /// </list>
        /// </exception>
        public CharacteristicPoints ReadCharacteristicPointsLocation()
        {
            if (fileReader == null)
            {
                fileReader = StreamReaderHelper.InitializeStreamReader(filePath);

                ValidateHeader(fileReader);
                lineNumber = 2;
            }

            string readText = ReadNextNonEmptyLine();

            if (readText != null)
            {
                try
                {
                    return CreateCharacteristicPointsLocation(readText);
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
        /// Reads lines from file until the first non-white line is hit.
        /// </summary>
        /// <returns>The next line which is not a white line, or <c>null</c> when no non-white 
        /// line could be found before the end of file.</returns>
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

        /// <summary>
        /// Validates the header of the file.
        /// </summary>
        /// <param name="reader">The reader, which is currently at the header row.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the header is not in the 
        /// required format or the file is empty.</exception>
        private void ValidateHeader(TextReader reader)
        {
            const int currentLine = 1;
            string header = ReadLineAndHandleIOExceptions(reader, currentLine);
            if (header != null)
            {
                if (!IsHeaderValid(header))
                {
                    throw CreateCriticalFileReadException(currentLine, Resources.CharacteristicPointsCsvReader_File_invalid_header);
                }
            }
            else
            {
                throw CreateCriticalFileReadException(currentLine, UtilResources.Error_File_empty);
            }
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
                throw CreateCriticalFileReadException(currentLine, UtilResources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(string.Format(UtilResources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Checks whether the given <paramref name="header"/> is valid. A valid header has a locationid column and is followed by
        /// x_{characteristic_point_type};y_{characteristic_point_type};z_{characteristic_point_type} triplets.
        /// </summary>
        /// <param name="header">The line which should represent the header of a characteristic point file.</param>
        /// <returns><c>true</c> if the <paramref name="header"/> is valid, <c>false</c> otherwise.</returns>
        private bool IsHeaderValid(string header)
        {
            columnsInFile.Clear();

            lineNumber = 1;

            string[] tokenizedHeader = TokenizeString(header.ToLowerInvariant().Replace(' ', '_'));

            if (!DetermineIdColumn(tokenizedHeader))
            {
                return false;
            }

            if (tokenizedHeader.Where(c => c.StartsWith(xPrefix))
                               .Any(c => !DetermineRequiredYZColumns(c, tokenizedHeader)))
            {
                return false;
            }

            DetermineOrderColumn(tokenizedHeader);

            return tokenizedHeader.Length == columnsInFile.Count;
        }

        private void DetermineOrderColumn(string[] tokenizedHeader)
        {
            int orderColumnIndex = Array.IndexOf(tokenizedHeader, orderNumberKey);
            if (orderColumnIndex > -1)
            {
                columnsInFile[orderNumberKey] = orderColumnIndex;
            }
        }

        private bool DetermineRequiredYZColumns(string column, string[] tokenizedHeader)
        {
            string key = column.Substring(2);

            string xColumnIdentifier = xPrefix + key;
            string yColumnIdentifier = yPrefix + key;
            string zColumnIdentifier = zPrefix + key;

            int xColumnIndex = Array.IndexOf(tokenizedHeader, xColumnIdentifier);
            int yColumnIndex = Array.IndexOf(tokenizedHeader, yColumnIdentifier);
            int zColumnIndex = Array.IndexOf(tokenizedHeader, zColumnIdentifier);

            if (yColumnIndex == -1 || zColumnIndex == -1)
            {
                return false;
            }

            columnsInFile[xColumnIdentifier] = xColumnIndex;
            columnsInFile[yColumnIdentifier] = yColumnIndex;
            columnsInFile[zColumnIdentifier] = zColumnIndex;

            return true;
        }

        private bool DetermineIdColumn(string[] tokenizedHeader)
        {
            int locationIdColumnIndex = Array.IndexOf(tokenizedHeader, locationIdKey);
            if (locationIdColumnIndex == -1)
            {
                locationIdColumnIndex = Array.IndexOf(tokenizedHeader, surfaceLineKey);
            }

            if (locationIdColumnIndex > -1)
            {
                columnsInFile[locationIdKey] = locationIdColumnIndex;
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Counts the remaining non-empty lines.
        /// </summary>
        /// <param name="reader">The reader at the row from which counting should start.</param>
        /// <param name="currentLine">The current line, used for error messaging.</param>
        /// <returns>An integer greater than or equal to 0, being the number of characteristic points location rows.</returns>
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
        /// Creates a new <see cref="CharacteristicPoints"/> from the <paramref name="readText"/>.
        /// </summary>
        /// <param name="readText">A single line read from file.</param>
        /// <returns>A new <see cref="CharacteristicPoints"/> with name and characteristic points set.</returns>
        /// <exception cref="LineParseException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="readText"/> has too many or few columns.</item>
        /// <item><paramref name="readText"/> contains a coordinate value which could not be parsed to <see cref="double"/>.</item>
        /// </list></exception>
        private CharacteristicPoints CreateCharacteristicPointsLocation(string readText)
        {
            string[] tokenizedString = TokenizeString(readText);
            string locationName = GetLocationName(tokenizedString);
            if (tokenizedString.Length != columnsInFile.Count)
            {
                throw CreateLineParseException(lineNumber, locationName, Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Location_lacks_values_for_characteristic_points);
            }

            var location = new CharacteristicPoints(locationName);

            SetCharacteristicPoints(tokenizedString, location);

            return location;
        }

        /// <summary>
        /// Sets the characteristic points from the given <paramref name="tokenizedString"/> to the given <paramref name="location"/>.
        /// </summary>
        /// <param name="tokenizedString">The string read from file.</param>
        /// <param name="location">The <see cref="CharacteristicPoints"/> to set the characteristic points for.</param>
        /// <exception cref="LineParseException">Thrown when <paramref name="tokenizedString"/> 
        /// contains a coordinate value which could not be parsed to <see cref="double"/>.</exception>
        private void SetCharacteristicPoints(string[] tokenizedString, CharacteristicPoints location)
        {
            location.SurfaceLevelInside = GetPoint3D(tokenizedString, surfaceLevelInsideKey, location.Name);
            location.DitchPolderSide = GetPoint3D(tokenizedString, ditchPolderSideKey, location.Name);
            location.BottomDitchPolderSide = GetPoint3D(tokenizedString, bottomDitchPolderSideKey, location.Name);
            location.BottomDitchDikeSide = GetPoint3D(tokenizedString, bottomDitchDikeSideKey, location.Name);
            location.DitchDikeSide = GetPoint3D(tokenizedString, ditchDikeSideKey, location.Name);
            location.DikeToeAtPolder = GetPoint3D(tokenizedString, dikeToeAtPolderKey, location.Name);
            location.ShoulderTopInside = GetPoint3D(tokenizedString, shoulderTopInsideKey, location.Name);
            location.ShoulderBaseInside = GetPoint3D(tokenizedString, shoulderBaseInsideKey, location.Name);
            location.DikeTopAtPolder = GetPoint3D(tokenizedString, dikeTopAtPolderKey, location.Name);
            location.TrafficLoadInside = GetPoint3D(tokenizedString, trafficLoadInsideKey, location.Name);
            location.TrafficLoadOutside = GetPoint3D(tokenizedString, trafficLoadOutsideKey, location.Name);
            location.DikeTopAtRiver = GetPoint3D(tokenizedString, dikeTopAtRiverKey, location.Name);
            location.ShoulderBaseOutside = GetPoint3D(tokenizedString, shoulderBaseOutsideKey, location.Name);
            location.ShoulderTopOutside = GetPoint3D(tokenizedString, shoulderTopOutsideKey, location.Name);
            location.DikeToeAtRiver = GetPoint3D(tokenizedString, dikeToeAtRiverKey, location.Name);
            location.SurfaceLevelOutside = GetPoint3D(tokenizedString, surfaceLevelOutsideKey, location.Name);
            location.DikeTableHeight = GetPoint3D(tokenizedString, dikeTableHeightKey, location.Name);
            location.InsertRiverChannel = GetPoint3D(tokenizedString, insertRiverChannelKey, location.Name);
            location.BottomRiverChannel = GetPoint3D(tokenizedString, bottomRiverChannelKey, location.Name);
        }

        /// <summary>
        /// Creates a new <see cref="Point3D"/> from the collection of <paramref name="valuesRead"/>, by using
        /// the <paramref name="typeKey"/> to create the column identifiers to read from.
        /// </summary>
        /// <param name="valuesRead">The collection of read data.</param>
        /// <param name="typeKey">The key for the type of characteristic point.</param>
        /// <param name="locationName">The name of the location used for creating descriptive errors.</param>
        /// <returns>A new <see cref="Point3D"/> with values for x,y,z set.</returns>
        /// <exception cref="LineParseException">Thrown when <paramref name="valuesRead"/> 
        /// contains a value which could not be parsed to a double in the column that had to be read for creating
        /// the <see cref="Point3D"/>.</exception>
        private Point3D GetPoint3D(string[] valuesRead, string typeKey, string locationName)
        {
            try
            {
                Point3D point = null;
                string xColumnKey = xPrefix + typeKey;
                if (columnsInFile.ContainsKey(xColumnKey))
                {
                    int xColumnIndex = columnsInFile[xColumnKey];
                    int yColumnIndex = columnsInFile[yPrefix + typeKey];
                    int zColumnIndex = columnsInFile[zPrefix + typeKey];

                    point = new Point3D(
                        double.Parse(valuesRead[xColumnIndex], CultureInfo.InvariantCulture),
                        double.Parse(valuesRead[yColumnIndex], CultureInfo.InvariantCulture),
                        double.Parse(valuesRead[zColumnIndex], CultureInfo.InvariantCulture)
                    );

                    if (point.Equals(undefinedPoint))
                    {
                        point = null;
                    }
                }

                return point;
            }
            catch (FormatException e)
            {
                throw CreateLineParseException(lineNumber, locationName, Resources.Error_CharacteristicPoint_has_not_double, e);
            }
            catch (OverflowException e)
            {
                throw CreateLineParseException(lineNumber, locationName, Resources.Error_CharacteristicPoint_parsing_causes_overflow, e);
            }
        }

        /// <summary>
        /// Gets the name of the location for which the <paramref name="tokenizedString"/> defines the
        /// characteristic points.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string from which the name should be extracted.</param>
        /// <returns>The name of the location.</returns>
        /// <exception cref="LineParseException">Thrown when id value is <c>null</c> or empty.</exception>
        private string GetLocationName(IEnumerable<string> tokenizedString)
        {
            string name = tokenizedString.Any() ? tokenizedString.ElementAt(columnsInFile[locationIdKey]).Trim() : null;
            if (string.IsNullOrEmpty(name))
            {
                throw CreateLineParseException(lineNumber, Resources.CharacteristicPointsCsvReader_ReadLine_Line_lacks_ID);
            }

            return name;
        }

        /// <summary>
        /// Tokenizes a string using a separator character up to the first empty field.
        /// </summary>
        /// <param name="readText">The text.</param>
        /// <returns>The tokenized parts.</returns>
        /// <exception cref="LineParseException">Thrown when <paramref name="readText"/> lacks the separator character.</exception>
        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                throw CreateLineParseException(lineNumber, string.Format(Resources.CharacteristicPointsCsvReader_ReadCharacteristicPointsLocation_Line_lacks_separator_0_,
                                                                         separator));
            }

            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }

        /// <summary>
        /// Creates a configured instance of <see cref="CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="CriticalFileReadException"/> with message and inner exception set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(UtilResources.TextFile_On_LineNumber_0_, currentLine);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }

        /// <summary>
        /// Creates a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <returns>New <see cref="LineParseException"/> with message set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string lineParseErrorMessage)
        {
            string locationDescription = string.Format(UtilResources.TextFile_On_LineNumber_0_, currentLine);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .Build(lineParseErrorMessage);
            return new LineParseException(message);
        }

        /// <summary>
        /// Creates a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="locationName">The name of the location being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="LineParseException"/> with message and inner exceptions set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string locationName, string lineParseErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(UtilResources.TextFile_On_LineNumber_0_, currentLine);
            string subjectDescription = string.Format(Resources.CharacteristicPointsCsvReader_LocationName_0_, locationName);
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                        .WithSubject(subjectDescription)
                                                                        .Build(lineParseErrorMessage);
            return new LineParseException(message, innerException);
        }

        #region Csv column names

        private const string locationIdKey = "locationid";
        private const string surfaceLineKey = "profielnaam";
        private const string surfaceLevelInsideKey = "maaiveld_binnenwaarts";
        private const string ditchPolderSideKey = "insteek_sloot_polderzijde";
        private const string bottomDitchPolderSideKey = "slootbodem_polderzijde";
        private const string bottomDitchDikeSideKey = "slootbodem_dijkzijde";
        private const string ditchDikeSideKey = "insteek_sloot_dijkzijde";
        private const string dikeToeAtPolderKey = "teen_dijk_binnenwaarts";
        private const string shoulderTopInsideKey = "kruin_binnenberm";
        private const string shoulderBaseInsideKey = "insteek_binnenberm";
        private const string dikeTopAtPolderKey = "kruin_binnentalud";
        private const string trafficLoadInsideKey = "verkeersbelasting_kant_binnenwaarts";
        private const string trafficLoadOutsideKey = "verkeersbelasting_kant_buitenwaarts";
        private const string dikeTopAtRiverKey = "kruin_buitentalud";
        private const string shoulderBaseOutsideKey = "insteek_buitenberm";
        private const string shoulderTopOutsideKey = "kruin_buitenberm";
        private const string dikeToeAtRiverKey = "teen_dijk_buitenwaarts";
        private const string surfaceLevelOutsideKey = "maaiveld_buitenwaarts";
        private const string dikeTableHeightKey = "dijktafelhoogte";
        private const string insertRiverChannelKey = "insteek_geul";
        private const string bottomRiverChannelKey = "teen_geul";
        private const string orderNumberKey = "volgnummer";

        #endregion
    }
}