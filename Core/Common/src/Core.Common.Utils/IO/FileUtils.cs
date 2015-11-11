using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using log4net;
using NDepend.Helpers.FileDirectoryPath;

namespace Core.Common.Utils.IO
{
    /// <summary>
    /// File manipulations
    /// </summary>
    public static class FileUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileUtils));

        /// <summary>
        /// Copy all files and folders in a directory to another directory
        /// 
        /// TODO: make it copy directory or file, change sourceDirectory, targetDirectory to sourcePath, targetPath
        /// or rename this method to Copy
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="ignorePath"></param>
        public static void CopyDirectory(string sourceDirectory, string targetDirectory, string ignorePath = "")
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget, ignorePath);
        }

        /// <summary>
        /// Copies the source file to the target destination; if the file
        /// already exists, it will be overwritten
        /// </summary>
        public static void CopyFile(string sourcePath, string targetPath, bool overwrite = true)
        {
            var sourceFullPath = Path.GetFullPath(sourcePath);
            var targetFullPath = Path.GetFullPath(targetPath);
            if (sourceFullPath == targetFullPath)
            {
                return;
            }

            if (File.Exists(targetPath))
            {
                if (!overwrite)
                {
                    return;
                }

                File.Delete(targetPath);
            }

            File.Copy(sourcePath, targetPath);
        }

        /// <summary>
        /// Copy files in a directory and its subdirectories to another directory.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignorePath"></param>
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, string ignorePath)
        {
            foreach (var diSourceSubDir in source.GetDirectories().Where(diSourceSubDir => diSourceSubDir.Name != ignorePath))
            {
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);
                }
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, ignorePath);
            }
            foreach (var fi in source.GetFiles())
            {
                Log.DebugFormat(@"Copying {0}\{1}", target.FullName, fi.Name);

                if (!target.Exists)
                {
                    target.Create();
                }

                var path = Path.Combine(target.ToString(), fi.Name);
                fi.CopyTo(path, true);
            }
        }

        ///<summary>
        /// Make all files and sub-directories writable.
        ///</summary>
        ///<param name="path"></param>
        public static void MakeWritable(string path)
        {
            if (Directory.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
            }

            var di = new DirectoryInfo(path);
            foreach (DirectoryInfo di2 in di.GetDirectories())
            {
                File.SetAttributes(path, FileAttributes.Normal);
                MakeWritable(Path.Combine(path, di2.Name));
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in di.GetFiles())
            {
                String filePath = Path.Combine(path, fi.Name);
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
        }

        /// <summary>
        /// check if the search item path is a subdirectory of the rootDir
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        public static bool IsSubdirectory(string rootDir, string searchItem)
        {
            if (rootDir.StartsWith("\\")) //network disk?
            {
                return searchItem.StartsWith(rootDir, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                var root = new DirectoryPathAbsolute(rootDir);
                var search = new DirectoryPathAbsolute(searchItem);
                return search.IsChildDirectoryOf(root);
            }
        }

        /// <summary>
        /// Check if the searchItem is part of the rootDir, or equal to.
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        public static bool IsSubdirectoryOrEquals(string rootDir, string searchItem)
        {
            DirectoryInfo di1 = new DirectoryInfo(rootDir);
            DirectoryInfo di2 = new DirectoryInfo(searchItem);

            return di2.FullName == di1.FullName ||
                   IsSubdirectory(rootDir, searchItem);
        }

        /// <summary>
        /// Returns if the supplied path is a directory
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Path is directory</returns>
        public static bool IsDirectory(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        /// Compares two directory strings
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        public static bool CompareDirectories(string rootDir, string searchItem)
        {
            var root = new FilePathAbsolute(Path.GetFullPath(rootDir));
            var search = new FilePathAbsolute(Path.GetFullPath(searchItem));
            return (root == search);
        }

        /// <summary>
        /// Returns a relative path string from a full path.
        /// </summary>
        /// <returns></returns>
        public static string GetRelativePath(string rootDir, string filePath)
        {
            if (rootDir == null || filePath == null)
            {
                return filePath;
            }

            if (rootDir.StartsWith("\\") && IsSubdirectory(rootDir, filePath)) //network disk?
            {
                return "." + filePath.Substring(rootDir.Length);
            }

            try
            {
                var filePathAbsolute = new FilePathAbsolute(filePath);
                var directoryPathAbsolute = new DirectoryPathAbsolute(rootDir);
                FilePathRelative filePathRelative = filePathAbsolute.GetPathRelativeFrom(directoryPathAbsolute);
                return filePathRelative.Path;
            }
            catch (Exception)
            {
                // gulp: return original filepath
            }
            return filePath;
        }

        /// <summary>
        /// Create dir if not exists
        /// </summary>
        /// <param name="path">File path to a directory</param>
        /// <param name="deleteIfExists">Optional flag to delete the specified directory if it does exist when set to true</param>
        /// <exception cref="IOException"> When:
        ///   The directory specified by <paramref name="path"/> is read-only
        /// Furthermore when <paramref name="deleteIfExists"/> is true:
        ///   A file with the same name and location specified by <paramref name="path"/> exists. -or- 
        ///   The directory is the application's current working directory.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException"> When: The caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>. -or- 
        ///   <paramref name="path"/> is prefixed with, or contains only a colon character (:).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="PathTooLongException">
        ///   The specified path, file name, or both exceed the system-defined maximum length. 
        ///   For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        public static void CreateDirectoryIfNotExists(string path, bool deleteIfExists = false)
        {
            if (Directory.Exists(path))
            {
                //replace the directory with a one
                if (deleteIfExists)
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Initializes filesystemwatcher
        /// </summary>
        /// <returns></returns>
        public static FileSystemWatcher CreateWatcher()
        {
            // use built-in filesystemwatcher class to monitor creation/modification/deletion of files
            // example http://www.codeguru.com/csharp/csharp/cs_network/article.php/c6043/
            var watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Size
            };
            return watcher;
        }

        /// <summary>
        /// Checks wether the path is a valid relative path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool PathIsRelative(string path)
        {
            return !Path.IsPathRooted(path);
        }

        /// <summary>
        /// Replaces text in a file.
        /// </summary>
        /// <param name="filePath">Path of the text file.</param>
        /// <param name="searchText">Text to search for.</param>
        /// <param name="replaceText">Text to replace the search text.</param>
        public static void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            var reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            var writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        /// <summary>
        /// Creates a temporary directory
        /// </summary>
        /// <returns>path to temporary directory</returns>
        public static string CreateTempDirectory()
        {
            string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), path));
            return Path.Combine(Path.GetTempPath(), path);
        }

        /// <summary>
        /// Check if one or more files can be copied
        /// 
        /// Todo: Is this function necessary? Probably better design to just copy and handle errors
        /// </summary>
        /// <param name="files"></param>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static bool CanCopy(IEnumerable<string> files, string targetDir)
        {
            //todo check if targetdir is readonly

            //check if targetdrive has enough space
            long spaceRequired = 0;

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                spaceRequired += fi.Length;
            }
            string driveName = targetDir.Substring(0, 1);
            DriveInfo di = new DriveInfo(driveName);
            if (spaceRequired > di.AvailableFreeSpace)
            {
                return false;
            }

            //check if files exist with the same name
            foreach (string file in files)
            {
                var info = new FileInfo(Path.Combine(targetDir, Path.GetFileName(file)));
                if (info.Exists)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// check if file can be copied to another folder
        /// 
        /// Todo: Is this function necessary? Probably better design to just copy and handle errors
        /// </summary>
        /// <param name="name"></param>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static bool CanCopy(string name, string targetDir)
        {
            string[] files = new string[]
            {
                name
            };
            return CanCopy(files, targetDir);
        }

        /// <summary>
        /// Check if the content of two files are equal.
        /// </summary>
        /// <param name="file1Path">first file path</param>
        /// <param name="file2Path">second file path</param>
        /// <returns></returns>
        public static bool FilesAreEqual(string file1Path, string file2Path)
        {
            var first = new FileInfo(file1Path);
            var second = new FileInfo(file2Path);

            if (first.Length != second.Length)
            {
                return false;
            }

            const int bytesToRead = sizeof(Int64);
            var iterations = (int) Math.Ceiling((double) first.Length/bytesToRead);
            using (FileStream fs1 = first.OpenRead())
            {
                using (FileStream fs2 = second.OpenRead())
                {
                    var one = new byte[bytesToRead];
                    var two = new byte[bytesToRead];
                    for (int i = 0; i < iterations; i++)
                    {
                        fs1.Read(one, 0, bytesToRead);
                        fs2.Read(two, 0, bytesToRead);
                        if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the extension of a file belongs to the filefilter
        /// </summary>
        /// <param name="fileFilter">"My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"</param>
        /// <param name="path">Path to a file or filename including extension</param>
        /// <returns></returns>
        public static bool FileMatchesFileFilterByExtension(string fileFilter, string path)
        {
            /* if (!fileFilterRegex.Match(fileFilter).Success)
            {
                throw new ArgumentException(string.Format("Invalid filefilter: {0}", fileFilter));
            }*/
            if (String.IsNullOrEmpty(Path.GetExtension(path)))
            {
                return false;
            }
            return Regex.Match(fileFilter, String.Format(@"(\||\;)+\s*\*\{0}", Path.GetExtension(path))).Success;
        }

        /// <summary>
        /// Deletes the given file or directory if it exists
        /// </summary>
        public static void DeleteIfExists(string path)
        {
            if (!File.Exists(path) & !Directory.Exists(path))
            {
                return;
            }

            var attributes = File.GetAttributes(path);

            // if file is readonly - make it non-readonly
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(path, attributes ^ FileAttributes.ReadOnly);
            }

            // now delete everything
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (var path2 in Directory.GetDirectories(path).Union(Directory.GetFiles(path)))
                {
                    DeleteIfExists(path2);
                }
                Directory.Delete(path);
            }
        }

        public static IList<string> GetDirectoriesRelative(string path)
        {
            var q = from subdir in Directory.GetDirectories(path)
                    select GetRelativePath(Path.GetFullPath(path), Path.GetFullPath(subdir));

            return q.ToList();
        }

        public static bool IsValidFileName(string fileName)
        {
            return fileName != null
                   && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
                   && fileName.Trim().Length > 0;
        }

        public static IEnumerable<char> GetReducedInvalidFileNameChars()
        {
            // reduce by excluding 'exotic' characters with ASCII code < 32
            return Path.GetInvalidFileNameChars().Where(c => c > 31);
        }

        /// <summary>
        /// Checks if directory is empty
        /// </summary>
        /// <param name="path">The path to the directory</param>
        /// <returns>true is directory exists and is empty, false otherwise</returns>
        public static bool IsDirectoryEmpty(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        /// Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="maxTries"></param>
        /// <param name="waitTimePerTryInMillis"></param>
        public static bool WaitForFile(string fullPath, int maxTries = 5, int waitTimePerTryInMillis = 500)
        {
            // from: http://stackoverflow.com/questions/50744/wait-until-file-is-unlocked-in-net
            var numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    // Attempt to open the file exclusively.
                    using (var fs = new FileStream(fullPath, FileMode.Open,
                                                   FileAccess.ReadWrite,
                                                   FileShare.None, 100))
                    {
                        fs.ReadByte();
                        break; // If we got this far the file is ready
                    }
                }
                catch (Exception)
                {
                    if (numTries > maxTries)
                    {
                        return false;
                    }

                    // Wait for the lock to be released
                    Thread.Sleep(waitTimePerTryInMillis);
                }
            }
            return true;
        }

        /// <summary>
        /// Computes the checksum for a given file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The </returns>
        /// <remarks>Uses the MD5 checksum algorithm.</remarks>
        public static string GetChecksum(string filePath)
        {
            using (var md5Algorithm = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var computedByteHash = md5Algorithm.ComputeHash(stream);
                    return BitConverter.ToString(computedByteHash).Replace("-", "").ToLower();
                }
            }
        }

        /// <summary>
        /// Verifies that a given file matches with a checksum.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="checksumToBeMatched">The checksum to be matched.</param>
        /// <returns>True if the checksum matches; false otherwise.</returns>
        /// <seealso cref="GetChecksum"/>
        public static bool VerifyChecksum(string filePath, string checksumToBeMatched)
        {
            var checksum = GetChecksum(filePath);
            return Equals(checksumToBeMatched, checksum);
        }

        /// <summary>
        /// Check if two paths are equal.
        /// This prevents failure of forward and backward slashes.
        /// It checks on full path names via <see cref="FileInfo"/>.
        /// </summary>
        public static bool PathsAreEqual(string path1, string path2)
        {
            var info1 = new FileInfo(path1);
            var info2 = new FileInfo(path2);
            return info1.FullName == info2.FullName;
        }

        private class FileName 
        {
            public string Name { get; set; }
        }
    }
}