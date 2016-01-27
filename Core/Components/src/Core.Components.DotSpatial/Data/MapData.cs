using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Components.DotSpatial.Exceptions;
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
        /// Adds the shape file to the list. Duplicates are ignored.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="MapDataException">Thrown when
        /// <list type="bullet">
        /// <item>The <paramref name="filePath"/> is null.</item>
        /// <item>The file at <paramref name="filePath"/> does not exist.</item>
        /// <item>The extension of <paramref name="filePath"/> is not accepted.</item>
        /// </list>
        /// </exception>
        /// <returns>True when added to the list. False when it is not added, for example when <paramref name="filePath"/> is not unique.</returns>
        public bool AddShapeFile(string filePath)
        {
            string validationMessage = ValidateFilePath(filePath);

            if (validationMessage != string.Empty)
            {
                throw new MapDataException(validationMessage);
            }

            return filePaths.Add(filePath);
        }

        /// <summary>
        /// Checks if the given paths are valid.
        /// </summary>
        /// <returns><c>True</c> when all filePaths are valid. <c>False</c> otherwise.</returns>
        public bool IsValid()
        {
            return filePaths.Count != 0 && filePaths.All(fp => ValidateFilePath(fp) == string.Empty);
        }

        private string ValidateFilePath(string path)
        {
            if (path == null)
            {
                return "A path is required when adding shape files";
            }

            if (!File.Exists(path))
            {
                return string.Format(Resources.MapData_IsPathValid_File_on_path__0__does_not_exist, path);
            }

            if (!CheckExtension(path))
            {
                return string.Format(Resources.MapData_IsPathValid_File_on_path__0__does_not_have_the_shp_extension, path);
            }

            return string.Empty;
        }

        private bool CheckExtension(string path)
        {
            return Path.GetExtension(path).Equals(extension);
        }
    }
}