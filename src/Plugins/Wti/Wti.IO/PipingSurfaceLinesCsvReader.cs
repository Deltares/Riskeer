using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Wti.Data;
using Wti.IO.Exceptions;
using Wti.IO.Properties;

namespace Wti.IO
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv), containing
    /// data specifying surfacelines. 
    /// Expects data to be specified in the following format:
    /// <para><c>{ID};X1;Y1;Z1...;(Xn;Yn;Zn)</c></para>
    /// Where {ID} has to be a particular accepted text, and n triplets of doubles form the
    /// 3D coordinates defining the geometric shape of the surfaceline..
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSurfaceLinesCsvReader"/> class
        /// and opens a given file path.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid.</exception>
        public PipingSurfaceLinesCsvReader(string path)
        {
            CheckIfPathIsValid(path);

            filePath = path;
        }

        /// <summary>
        /// Reads the file to determine the number of available <see cref="PipingSurfaceLine"/>
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
            int count = 0, lineNumber = 0;
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filePath);

                ValidateHeader(reader);
                lineNumber++;

                // Count SurfaceLines:
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        count++;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                var message = string.Format(Resources.Error_File_0_does_not_exist, filePath);
                throw new CriticalFileReadException(message, e);
            }
            catch (DirectoryNotFoundException e)
            {
                var message = string.Format(Resources.Error_Directory_in_path_0_missing, filePath);
                throw new CriticalFileReadException(message, e);
            }
            catch (OutOfMemoryException e)
            {
                var message = string.Format(Resources.Error_File_0_contains_Line_1_too_big, filePath, lineNumber);
                throw new CriticalFileReadException(message, e);
            }
            catch (IOException e)
            {
                var message = string.Format(Resources.Error_General_IO_File_0_ErrorMessage_1_, filePath, e.Message);
                throw new CriticalFileReadException(message, e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return count;
        }

        /// <summary>
        /// Reads and consumes the next data row, parsing the data to create an instance 
        /// of <see cref="PipingSurfaceLine"/>.
        /// </summary>
        /// <returns>Return the parse surfaceline, or null when at the end of the file.</returns>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">Filepath includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="FormatException">A coordinate value does not represent a number in a valid format or the line is incorrectly formatted.</exception>
        /// <exception cref="OverflowException">A coordinate value represents a number that is less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public PipingSurfaceLine ReadLine()
        {
            if (fileReader == null)
            {
                fileReader = new StreamReader(filePath);
                // Skip Header:
                fileReader.ReadLine();
            }
            var readText = fileReader.ReadLine();
            if (readText != null)
            {
                var tokenizedString = readText.Split(separator);
                var worldCoordinateValues = tokenizedString.Skip(1)
                                                           .Select(ts => Double.Parse(ts, CultureInfo.InvariantCulture))
                                                           .ToArray();
                int coordinateCount = worldCoordinateValues.Length / 3;
                var points = new Point3D[coordinateCount];
                for (int i = 0; i < coordinateCount; i++)
                {
                    points[i] = new Point3D
                    {
                        X = worldCoordinateValues[i * 3],
                        Y = worldCoordinateValues[i * 3 + 1],
                        Z = worldCoordinateValues[i * 3 + 2]
                    };
                }

                var surfaceLine = new PipingSurfaceLine
                {
                    Name = tokenizedString.First()
                };
                surfaceLine.SetGeometry(points);
                return surfaceLine;
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

        private void ValidateHeader(StreamReader reader)
        {
            var header = reader.ReadLine();
            if (header != null)
            {
                if (!IsHeaderValid(header))
                {
                    var expectedMessage = string.Format(Resources.PipingSurfaceLinesCsvReader_File_0_invalid_header, filePath);
                    throw new CriticalFileReadException(expectedMessage);
                }
            }
            else
            {
                var expectedMessage = string.Format(Resources.Error_File_0_empty, filePath);
                throw new CriticalFileReadException(expectedMessage);
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

        private void CheckIfPathIsValid(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(Resources.Error_PathMustBeSpecified);
            }

            string name;
            try
            {
                name = Path.GetFileName(path);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(String.Format(Resources.Error_PathCannotContainCharacters_0_,
                                                          String.Join(", ", Path.GetInvalidFileNameChars())), e);
            }
            if (string.Empty == name)
            {
                throw new ArgumentException(Resources.Error_PathMustNotPointToFolder);
            }
        }
    }
}