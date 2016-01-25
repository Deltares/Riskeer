using System;
using System.Collections.Generic;
using System.IO;
using Core.Components.DotSpatial.Properties;

namespace Core.Components.DotSpatial.Data
{
    /// <summary>
    /// This class describes the data for the map.
    /// </summary>
    public class MapData
    {
        private const string extension = ".shp";

        private readonly HashSet<string> filePaths;

        /// <summary>
        /// Creates a new instance of <see cref="MapData"/>.
        /// </summary>
        public MapData()
        {
            filePaths = new HashSet<string>();
        }

        /// <summary>
        /// The collection of the file paths.
        /// </summary>
        public IEnumerable<string> FilePaths
        {
            get
            {
                return filePaths;
            }
        }

        /// <summary>
        /// Adds the shape file to the list. Each <paramref name="filePath"/> should be unique.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="filePath"/> does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> has an unaccepted extension.</exception>
        /// <returns>True when added to the list. False when it is not added, for example when <paramref name="filePath"/> is not unique.</returns>
        public bool AddShapeFile(string filePath)
        {
            IsPathValid(filePath);

            return filePaths.Add(filePath);
        }

        /// <summary>
        /// Checks if the given paths are valid.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throwns when value in <see cref="FilePaths"/> is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when value in <see cref="FilePaths"/> does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown when value <see cref="FilePaths"/> has an unaccepted extension.</exception>
        /// <returns></returns>
        public bool IsValid()
        {
            foreach (var path in filePaths)
            {
                IsPathValid(path);
            }

            return true;
        }

        private void IsPathValid(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "A path is required when adding shape files");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format(Resources.MapData_IsPathValid_File_on_path__0__does_not_exist, path));
            }

            if (!CheckExtension(path))
            {
                throw new ArgumentException(string.Format(Resources.MapData_IsPathValid_File_on_path__0__does_not_have_the_shp_extension, path));
            }
        }

        private bool CheckExtension(string path)
        {
            return Path.GetExtension(path).Equals(extension);
        }
    }
}