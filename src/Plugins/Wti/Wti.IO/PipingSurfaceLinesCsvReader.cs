using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Wti.Data;
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
        private readonly string[] acceptableIdNames = { "Profielnaam, LocationID" };
        private readonly string filePath;
        private StreamReader fileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSurfaceLinesCsvReader"/> class
        /// and opens a given file path.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        public PipingSurfaceLinesCsvReader(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(Resources.Error_PathMustBeSpecified);
            }
            filePath = path;
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
        /// Reads the file to determine the number of available <see cref="PipingSurfaceLine"/>
        /// data rows.
        /// </summary>
        /// <returns>A value greater than or equal to 0.</returns>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">Filepath includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        public int GetSurfaceLinesCount()
        {
            var count = 0;
            using (var reader = new StreamReader(filePath))
            {
                // Skip header:
                reader.ReadLine();

                // Count SurfaceLines:
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        count++;
                    }
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
                var tokenizedString = readText.Split(';');
                var worldCoordinateValues = tokenizedString.Skip(1)
                                                           .Select(ts => Double.Parse(ts, CultureInfo.InvariantCulture))
                                                           .ToArray();
                int coordinateCount = worldCoordinateValues.Length/3;
                var points = new Point3D[coordinateCount];
                for (int i = 0; i < coordinateCount; i++)
                {
                    points[i] = new Point3D
                    {
                        X = worldCoordinateValues[i*3],
                        Y = worldCoordinateValues[i*3+1],
                        Z = worldCoordinateValues[i*3+2]
                    };
                }

                var surfaceLine = new PipingSurfaceLine();
                surfaceLine.Name = tokenizedString.First();
                surfaceLine.SetGeometry(points);
                return surfaceLine;
            }

            return null;
        }
    }
}