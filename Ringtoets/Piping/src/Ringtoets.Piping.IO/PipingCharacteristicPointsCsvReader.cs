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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO
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
    public class PipingCharacteristicPointsCsvReader : IDisposable
    {
        private const string locationHeader = "locationid";

        private const char separator = ';';

        private const string xPrefix = "x_";
        private const string yPrefix = "y_";
        private const string zPrefix = "z_";

        private const string surfaceLevelInsideKey = "maaiveld binnenwaarts";
        private const string ditchPolderSideKey = "insteek sloot polderzijde";
        private const string bottomDitchPolderSideKey = "slootbodem polderzijde";
        private const string bottomDitchDikeSideKey = "slootbodem dijkzijde";
        private const string ditchDikeSideKey = "insteek sloot dijkzijde";
        private const string dikeToeAtPolderKey = "teen dijk binnenwaarts";
        private const string topShoulderInsideKey = "kruin binnenberm";
        private const string shoulderInsideKey = "insteek binnenberm";
        private const string dikeTopAtPolderKey = "kruin binnentalud";
        private const string trafficLoadInsideKey = "verkeersbelasting kant binnenwaarts";
        private const string trafficLoadOutsideKey = "verkeersbelasting kant buitenwaarts";
        private const string dikeTopAtRiverKey = "kruin buitentalud";
        private const string shoulderOutsideKey = "insteek buitenberm";
        private const string topShoulderOutsideKey = "kruin buitenberm";
        private const string dikeToeAtRiverKey = "teen dijk buitenwaarts";
        private const string surfaceLevelOutsideKey = "maaiveld buitenwaarts";
        private const string dikeTableHeightKey = "dijktafelhoogte";
        private const string insertRiverChannelKey = "insteek geul";
        private const string bottomRiverChannelKey = "teen geul";

        private readonly string filePath;
        private readonly string orderNumberHeader = "volgnummer";

        /// <summary>
        /// Lower case string representations of the known characteristic point types.
        /// </summary>
        private IList<string> columnsInFile;

        private StreamReader fileReader;

        /// <summary>
        /// The next line number to be read by this reader.
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// Initializes a new instance of <see cref="PipingCharacteristicPointsCsvReader"/> using
        /// the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to use for reading characteristic points.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid.</exception>
        public PipingCharacteristicPointsCsvReader(string path)
        {
            FileUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Reads the file to determine the number of available <see cref="PipingCharacteristicPointsLocation"/>
        /// data rows.
        /// </summary>
        /// <returns>A value greater than or equal to 0.</returns>
        /// <exception cref="CriticalFileReadException">A critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>File cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>File incompatible for importing characteristic points locations.</item>
        /// </list>
        /// </exception>
        public int GetLocationsCount()
        {
            using (var reader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                ValidateHeader(reader);

                return CountNonEmptyLines(reader, 2);
            }
        }

        /// <summary>
        /// Reads and consumes the next data row, parsing the data to create an instance 
        /// of <see cref="PipingCharacteristicPointsLocation"/>.
        /// </summary>
        /// <returns>Return the parsed characteristic points location, or null when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">A critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>File cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>File incompatible for importing characteristic points.</item>
        /// </list>
        /// </exception>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row doesn't contain any supported separator character.</item>
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// <item>The row is missing an identifier value.</item>
        /// <item>The row is missing values to form a characteristic point.</item>
        /// </list>
        /// </exception>
        public PipingCharacteristicPointsLocation ReadLine()
        {
            if (fileReader == null)
            {
                fileReader = StreamReaderHelper.InitializeStreamReader(filePath);

                ValidateHeader(fileReader);
                lineNumber = 2;
            }

            var readText = ReadLineAndHandleIOExceptions(fileReader, lineNumber);
            if (readText != null)
            {
                try
                {
                    return CreatePipingCharacteristicPointsLocation(readText);
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
        /// <exception cref="CriticalFileReadException">The header is not in the required format or the file is empty.</exception>
        private void ValidateHeader(TextReader reader)
        {
            var currentLine = 1;
            var header = ReadLineAndHandleIOExceptions(reader, currentLine);
            if (header != null)
            {
                if (!IsHeaderValid(header))
                {
                    throw CreateCriticalFileReadException(currentLine, Resources.PipingCharacteristicPointsCsvReader_File_invalid_header);
                }
            }
            else
            {
                throw CreateCriticalFileReadException(currentLine, Core.Common.Utils.Properties.Resources.Error_File_empty);
            }
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
                throw CreateCriticalFileReadException(currentLine, Core.Common.Utils.Properties.Resources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                var message = new FileReaderErrorMessageBuilder(filePath).Build(string.Format(Core.Common.Utils.Properties.Resources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
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
            string locationDescription = string.Format(Resources.TextFile_On_LineNumber_0_, currentLine);
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                     .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }

        /// <summary>
        /// Checks whether the given <paramref name="header"/> is valid. A valid header has a locationid column and is followed by triplets
        /// of x_{characteristic_point_type};y_{characteristic_point_type};z_{characteristic_point_type} triplets.
        /// </summary>
        /// <param name="header">The line which should represent the header of a characteristic point file.</param>
        /// <returns><c>true</c> if the <paramref name="header"/> is valid, <c>false</c> otherwise.</returns>
        private bool IsHeaderValid(string header)
        {
            var hasLocationColumn = header.ToLowerInvariant().StartsWith(locationHeader);
            if (!hasLocationColumn)
            {
                return false;
            }

            var columns = GetCharacteristicPointColumnsFromHeader(header);
            if (columns.Count % 3 > 0)
            {
                return false;
            }

            var columnsValid = true;
            var currentColumn = 0;
            while (columnsValid && currentColumn < columns.Count)
            {
                var key = columns.ElementAt(currentColumn).Substring(2);
                columnsValid &= columns.ElementAt(currentColumn) == xPrefix + key;
                columnsValid &= columns.ElementAt(currentColumn + 1) == yPrefix + key;
                columnsValid &= columns.ElementAt(currentColumn + 2) == zPrefix + key;

                currentColumn += 3;
            }

            columnsInFile = columns;

            return columnsValid;
        }

        /// <summary>
        /// Obtains the columns from the header by stripping of the location and order number columns.
        /// </summary>
        /// <param name="header">The header to obtain characteristic point columns from.</param>
        /// <returns>A <see cref="IList{T}"/> of column names which should represent triplets of x,y,z for characteristic
        /// points.</returns>
        private IList<string> GetCharacteristicPointColumnsFromHeader(string header)
        {
            IList<string> tokenizedHeader = TokenizeString(header.ToLowerInvariant()).ToList();

            tokenizedHeader.RemoveAt(0);

            var hasOrderNumberColumn = tokenizedHeader.Last() == orderNumberHeader;
            if (hasOrderNumberColumn)
            {
                tokenizedHeader.RemoveAt(tokenizedHeader.Count - 1);
            }

            return tokenizedHeader;
        }

        /// <summary>
        /// Counts the remaining non-empty lines.
        /// </summary>
        /// <param name="reader">The reader at the row from which counting should start.</param>
        /// <param name="currentLine">The current line, used for error messaging.</param>
        /// <returns>An integer greater than or equal to 0, being the number of characteristic points location rows.</returns>
        /// <exception cref="CriticalFileReadException">An I/O exception occurred.</exception>
        private int CountNonEmptyLines(TextReader reader, int currentLine)
        {
            int count = 0, lineNumberForMessage = currentLine;
            string line;
            while ((line = ReadLineAndHandleIOExceptions(reader, lineNumberForMessage)) != null)
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    count++;
                }
                lineNumberForMessage++;
            }
            return count;
        }

        /// <summary>
        /// Creates a new <see cref="PipingCharacteristicPointsLocation"/> from the <paramref name="readText"/>.
        /// </summary>
        /// <param name="readText">A single line read from file.</param>
        /// <returns>A new <see cref="PipingCharacteristicPointsLocation"/> with name and characteristic points set.</returns>
        private PipingCharacteristicPointsLocation CreatePipingCharacteristicPointsLocation(string readText)
        {
            var tokenizedString = TokenizeString(readText);

            var locationName = GetLocationName(tokenizedString);
            var location = new PipingCharacteristicPointsLocation(locationName);

            SetCharacteristicPoints(tokenizedString, location);

            return location;
        }

        /// <summary>
        /// Sets the characteristic points from the given <paramref name="tokenizedString"/> to the given <paramref name="location"/>.
        /// </summary>
        /// <param name="tokenizedString">The string read from file.</param>
        /// <param name="location">The <see cref="PipingCharacteristicPointsLocation"/> to set the characteristic points for.</param>
        private void SetCharacteristicPoints(string[] tokenizedString, PipingCharacteristicPointsLocation location)
        {
            int expectedValuesForPoint = 3;
            int expectedValuesCount = columnsInFile.Count;
            var locationName = location.Name;

            var worldCoordinateValues = ParseWorldCoordinateValuesAndHandleParseErrors(tokenizedString, locationName);
            if (worldCoordinateValues.Length != expectedValuesCount)
            {
                throw CreateLineParseException(lineNumber, locationName, Resources.PipingCharacteristicPointsCsvReader_ReadLine_Location_lacks_values_for_characteristic_points);
            }

            int coordinateCount = worldCoordinateValues.Length/expectedValuesForPoint;
            var points = new Point3D[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                points[i] = new Point3D
                {
                    X = worldCoordinateValues[i*expectedValuesForPoint],
                    Y = worldCoordinateValues[i*expectedValuesForPoint + 1],
                    Z = worldCoordinateValues[i*expectedValuesForPoint + 2]
                };
            }
            MapPointsToCharacteristicPoints(points, location);
        }

        /// <summary>
        /// Sets the locations for the characteristic points for the <paramref name="location"/> based on the 
        /// values read from file.
        /// </summary>
        /// <param name="points">The <see cref="Core.Common.Base.Geometry.Point3D"/> collection from file.</param>
        /// <param name="location">The <see cref="PipingCharacteristicPointsLocation"/> to set the locations of
        /// the characteristic points for.</param>
        private void MapPointsToCharacteristicPoints(Point3D[] points, PipingCharacteristicPointsLocation location)
        {
            location.SurfaceLevelInside = GetPoint3D(points, surfaceLevelInsideKey);
            location.DitchPolderSide = GetPoint3D(points, ditchPolderSideKey);
            location.BottomDitchPolderSide = GetPoint3D(points, bottomDitchPolderSideKey);
            location.BottomDitchDikeSide = GetPoint3D(points, bottomDitchDikeSideKey);
            location.DitchDikeSide = GetPoint3D(points, ditchDikeSideKey);
            location.DikeToeAtPolder = GetPoint3D(points, dikeToeAtPolderKey);
            location.TopShoulderInside = GetPoint3D(points, topShoulderInsideKey);
            location.ShoulderInside = GetPoint3D(points, shoulderInsideKey);
            location.DikeTopAtPolder = GetPoint3D(points, dikeTopAtPolderKey);
            location.TrafficLoadInside = GetPoint3D(points, trafficLoadInsideKey);
            location.TrafficLoadOutside = GetPoint3D(points, trafficLoadOutsideKey);
            location.DikeTopAtRiver = GetPoint3D(points, dikeTopAtRiverKey);
            location.ShoulderOutisde = GetPoint3D(points, shoulderOutsideKey);
            location.TopShoulderOutside = GetPoint3D(points, topShoulderOutsideKey);
            location.DikeToeAtRiver = GetPoint3D(points, dikeToeAtRiverKey);
            location.SurfaceLevelOutside = GetPoint3D(points, surfaceLevelOutsideKey);
            location.DikeTableHeight = GetPoint3D(points, dikeTableHeightKey);
            location.InsertRiverChannel = GetPoint3D(points, insertRiverChannelKey);
            location.BottomRiverChannel = GetPoint3D(points, bottomRiverChannelKey);
        }

        /// <summary>
        /// Obtains the <see cref="Point3D"/> from <paramref name="points"/> for a <paramref name="characteristicPointType"/>.
        /// </summary>
        /// <param name="points">The collection of characteristic points that were read.</param>
        /// <param name="characteristicPointType">The type of characteristic point to obtain.</param>
        /// <returns>Returns the <see cref="Point3D"/> from the <paramref name="points"/> collections 
        /// for the <paramref name="characteristicPointType"/>.</returns>
        private Point3D GetPoint3D(Point3D[] points, string characteristicPointType)
        {
            var columnName = xPrefix + characteristicPointType;
            var columnIndex = columnsInFile.IndexOf(columnName);
            if (columnIndex > -1)
            {
                var indexOfPoint = columnIndex/3;
                return points[indexOfPoint];
            }
            return null;
        }

        /// <summary>
        /// Parses the world coordinate values and handles parse errors.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string.</param>
        /// <param name="locationName">The name of the characteristic points location whose coordinate values are being parsed.</param>
        /// <returns></returns>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// </list>
        /// </exception>
        private double[] ParseWorldCoordinateValuesAndHandleParseErrors(string[] tokenizedString, string locationName)
        {
            try
            {
                return tokenizedString
                    .Take(tokenizedString.Length - 1)
                    .Skip(1)
                    .Select(ts => double.Parse(ts, CultureInfo.InvariantCulture))
                    .ToArray();
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
        /// Gets the name of the characteristic points location.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string from which the name should be extracted.</param>
        /// <returns>The name of the location.</returns>
        /// <exception cref="LineParseException">Id value is null or empty.</exception>
        private string GetLocationName(IList<string> tokenizedString)
        {
            var name = tokenizedString.Any() ? tokenizedString[0].Trim() : string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                throw CreateLineParseException(lineNumber, Resources.PipingCharacteristicPointsCsvReader_ReadLine_Line_lacks_ID);
            }
            return name;
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
                throw CreateLineParseException(lineNumber, string.Format(Resources.PipingCharacteristicPointsCsvReader_ReadLine_Line_lacks_separator_0_,
                                                                         separator));
            }
            return readText.Split(separator)
                           .TakeWhile(text => !string.IsNullOrEmpty(text))
                           .ToArray();
        }

        /// <summary>
        /// Throws a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <returns>New <see cref="LineParseException"/> with message set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string lineParseErrorMessage)
        {
            string locationDescription = string.Format(Resources.TextFile_On_LineNumber_0_, currentLine);
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                     .Build(lineParseErrorMessage);
            return new LineParseException(message);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="LineParseException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="locationName">The name of the location being read.</param>
        /// <param name="lineParseErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="LineParseException"/> with message and inner exceptions set.</returns>
        private LineParseException CreateLineParseException(int currentLine, string locationName, string lineParseErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(Resources.TextFile_On_LineNumber_0_, currentLine);
            string subjectDescription = string.Format(Resources.PipingCharacteristicPointsCsvReader_LocationName_0_, locationName);
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                     .WithSubject(subjectDescription)
                                                                     .Build(lineParseErrorMessage);
            return new LineParseException(message, innerException);
        }
    }
}