using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv), containing
    /// data specifying surfacelines. 
    /// Expects data to be specified in the following format:
    /// <para><c>{ID};X1;Y1;Z1...;(Xn;Yn;Zn)</c></para>
    /// Where {ID} has to be a particular accepted text, and n triplets of doubles form the
    /// 3D coordinates defining the geometric shape of the surfaceline.
    /// </summary>
    public class PipingSurfaceLinesCsvReader : IDisposable
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSurfaceLinesCsvReader"/> class
        /// and opens a given file path.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid.</exception>
        public PipingSurfaceLinesCsvReader(string path)
        {
            FileUtils.ValidateFilePath(path);

            filePath = path;
        }

        /// <summary>
        /// Reads the file to determine the number of available <see cref="RingtoetsPipingSurfaceLine"/>
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
        /// <item>File incompatible for importing surface lines.</item>
        /// </list>
        /// </exception>
        public int GetSurfaceLinesCount()
        {
            using (var reader = InitializeStreamReader(filePath))
            {
                ValidateHeader(reader, 1);

                return CountNonEmptyLines(reader, 2);
            }
        }

        /// <summary>
        /// Reads and consumes the next data row, parsing the data to create an instance 
        /// of <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <returns>Return the parse surfaceline, or null when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">A critical error has occurred, which may be caused by:
        /// <list type="bullet">
        /// <item>File cannot be found at specified path.</item>
        /// <item>The specified path is invalid, such as being on an unmapped drive.</item>
        /// <item>Some other I/O related issue occurred, such as: path includes an incorrect 
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>There is insufficient memory to allocate a buffer for the returned string.</item>
        /// <item>File incompatible for importing surface lines.</item>
        /// </list>
        /// </exception>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row doesn't contain any supported separator character.</item>
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// <item>The row is missing an identifier value.</item>
        /// <item>The row is missing values to form a surface line point.</item>
        /// </list>
        /// </exception>
        public RingtoetsPipingSurfaceLine ReadLine()
        {
            if (fileReader == null)
            {
                fileReader = InitializeStreamReader(filePath);

                ValidateHeader(fileReader, 1);
                lineNumber = 2;
            }

            var readText = ReadLineAndHandleIOExceptions(fileReader, lineNumber);
            if (readText != null)
            {
                try
                {
                    var tokenizedString = TokenizeString(readText);

                    var surfaceLineName = GetSurfaceLineName(tokenizedString);
                    var points = GetSurfaceLinePoints(tokenizedString);

                    var surfaceLine = new RingtoetsPipingSurfaceLine
                    {
                        Name = surfaceLineName
                    };
                    surfaceLine.SetGeometry(points);

                    CheckIfGeometryIsValid(surfaceLine);

                    return surfaceLine;
                }
                finally
                {
                    lineNumber++;
                }
            }

            return null;
        }

        private void CheckIfGeometryIsValid(RingtoetsPipingSurfaceLine surfaceLine)
        {
            double[] lCoordinates = surfaceLine.ProjectGeometryToLZ().Select(p => p.X).ToArray();
            for (int i = 1; i < lCoordinates.Length; i++)
            {
                if (lCoordinates[i - 1] > lCoordinates[i])
                {
                    var message = string.Format(Resources.PipingSurfaceLinesCsvReader_ReadLine_File_0_Line_1_has_reclining_geometry,
                                                filePath, 2);
                    throw new LineParseException(message);
                }
            }
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
        /// Tokenizes a string using a separator character up to the first empty field.
        /// </summary>
        /// <param name="readText">The text.</param>
        /// <returns>The tokenized parts.</returns>
        /// <exception cref="LineParseException"><paramref name="readText"/> lacks separator character.</exception>
        private string[] TokenizeString(string readText)
        {
            if (!readText.Contains(separator))
            {
                var message = string.Format(Resources.PipingSurfaceLinesCsvReader_ReadLine_File_0_Line_1_lacks_separator_2_,
                                            filePath, lineNumber, separator);
                throw new LineParseException(message);
            }
            return readText.Split(separator)
                           .TakeWhile(text => !String.IsNullOrEmpty(text))
                           .ToArray();
        }

        /// <summary>
        /// Gets the 3D surface line points.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string.</param>
        /// <returns>Set of all 3D world coordinate points.</returns>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item><paramref name="tokenizedString"/> contains a coordinate value that cannot be parsed as a double.</item>
        /// <item><paramref name="tokenizedString"/> contains a number that is too big or too small to be represented with a double.</item>
        /// <item><paramref name="tokenizedString"/> is missing coordinate values to define a proper surface line point.</item>
        /// </list>
        /// </exception>
        private IEnumerable<Point3D> GetSurfaceLinePoints(string[] tokenizedString)
        {
            const int expectedValuesForPoint = 3;

            var worldCoordinateValues = ParseWorldCoordinateValuesAndHandleParseErrors(tokenizedString);
            if (worldCoordinateValues.Length % expectedValuesForPoint != 0)
            {
                var message = string.Format(Resources.PipingSurfaceLinesCsvReader_ReadLine_File_0_Line_1_lacks_values_for_coordinate_triplet,
                                            filePath, lineNumber);
                throw new LineParseException(message);
            }

            int coordinateCount = worldCoordinateValues.Length / expectedValuesForPoint;
            var points = new Point3D[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                points[i] = new Point3D
                {
                    X = worldCoordinateValues[i * expectedValuesForPoint],
                    Y = worldCoordinateValues[i * expectedValuesForPoint + 1],
                    Z = worldCoordinateValues[i * expectedValuesForPoint + 2]
                };
            }
            return points;
        }

        /// <summary>
        /// Gets the name of the surface line.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string from which the name should be extrated.</param>
        /// <returns>The name of the surface line.</returns>
        /// <exception cref="LineParseException">Id value is null or empty.</exception>
        private string GetSurfaceLineName(IList<string> tokenizedString)
        {
            var name = tokenizedString.Any() ? tokenizedString[0].Trim() : string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                var message = string.Format(Resources.PipingSurfaceLinesCsvReader_ReadLine_File_0_Line_1_no_ID,
                                            filePath, lineNumber);
                throw new LineParseException(message);
            }
            return name;
        }

        /// <summary>
        /// Parses the world coordinate values and handles parse errors.
        /// </summary>
        /// <param name="tokenizedString">The tokenized string.</param>
        /// <returns></returns>
        /// <exception cref="LineParseException">A parse error has occurred for the current row, which may be caused by:
        /// <list type="bullet">
        /// <item>The row contains a coordinate value that cannot be parsed as a double.</item>
        /// <item>The row contains a number that is too big or too small to be represented with a double.</item>
        /// </list>
        /// </exception>
        private double[] ParseWorldCoordinateValuesAndHandleParseErrors(string[] tokenizedString)
        {
            try
            {
                return tokenizedString.Skip(1)
                                      .Select(ts => Double.Parse(ts, CultureInfo.InvariantCulture))
                                      .ToArray();
            }
            catch (FormatException e)
            {
                var message = string.Format(Resources.Error_File_0_has_not_double_Line_1_,
                                            filePath, lineNumber);
                throw new LineParseException(message, e);
            }
            catch (OverflowException e)
            {
                var message = string.Format(Resources.Error_File_0_parsing_causes_overflow_Line_1_,
                                            filePath, lineNumber);
                throw new LineParseException(message, e);
            }
        }

        /// <summary>
        /// Initializes the stream reader for a UTF8 encoded file.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <returns>A UTF8 encoding configured stream reader opened on <paramref name="path"/>.</returns>
        /// <exception cref="CriticalFileReadException">File/directory cannot be found or 
        /// some other I/O related problem occurred.</exception>
        private static StreamReader InitializeStreamReader(string path)
        {
            try
            {
                return new StreamReader(path);
            }
            catch (FileNotFoundException e)
            {
                var message = string.Format(Resources.Error_File_0_does_not_exist, path);
                throw new CriticalFileReadException(message, e);
            }
            catch (DirectoryNotFoundException e)
            {
                var message = string.Format(Resources.Error_Directory_in_path_0_missing, path);
                throw new CriticalFileReadException(message, e);
            }
            catch (IOException e)
            {
                var message = string.Format(Resources.Error_General_IO_File_0_ErrorMessage_1_, path, e.Message);
                throw new CriticalFileReadException(message, e);
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
            var header = ReadLineAndHandleIOExceptions(reader, currentLine);
            if (header != null)
            {
                if (!IsHeaderValid(header))
                {
                    var message = string.Format(Resources.PipingSurfaceLinesCsvReader_File_0_invalid_header, filePath);
                    throw new CriticalFileReadException(message);
                }
            }
            else
            {
                var message = string.Format(Resources.Error_File_0_empty, filePath);
                throw new CriticalFileReadException(message);
            }
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
                var message = string.Format(Resources.Error_File_0_contains_Line_1_too_big, filePath, currentLine);
                throw new CriticalFileReadException(message, e);
            }
            catch (IOException e)
            {
                var message = string.Format(Resources.Error_General_IO_File_0_ErrorMessage_1_, filePath, e.Message);
                throw new CriticalFileReadException(message, e);
            }
        }

        private bool IsHeaderValid(string header)
        {
            var tokenizedHeader = header.Split(separator).Select(s => s.Trim().ToLowerInvariant()).ToArray();

            // Check for valid id:
            if (!acceptableLowerCaseIdNames.Contains(tokenizedHeader[0]))
            {
                return false;
            }

            // CHeck for valid 1st coordinate in header:
            bool valid = true;
            for (int i = 0; i < expectedFirstCoordinateHeader.Length && valid; i++)
            {
                valid = tokenizedHeader[1 + i].Equals(expectedFirstCoordinateHeader[i]);
            }
            return valid;
        }
    }
}