using System.IO;
using DelftTools.Utils.IO;

namespace DelftTools.Shell.Core.Extensions
{
    public class FilePathHandler
    {
        private string CurrentDirectory { get; set; }

        public string FilePath { get; private set; }

        public static FilePathHandler Create(string path)
        {
            return new FilePathHandler {FilePath = path};
        }

        public static FilePathHandler Create(string currentDirectory, string path)
        {
            if (FileUtils.PathIsRelative(path))
            {
                return new FilePathHandler {FilePath = path, CurrentDirectory = currentDirectory};
            }
            var modelSubFile = Create(path);
            if (currentDirectory != null)
            {
                modelSubFile.SwitchTo(currentDirectory);
            }
            return modelSubFile;
        }

        public bool FileExists
        {
            get { return FullPath != null && File.Exists(FullPath); }
        }

        public string FullPath
        {
            get
            {
                if (FileUtils.PathIsRelative(FilePath))
                {
                    return CurrentDirectory == null ? null : Path.Combine(CurrentDirectory, FilePath);
                }

                return FilePath;
            }
        }

        public bool PathIsRelative
        {
            get { return FilePath != null && FileUtils.PathIsRelative(FilePath); }
        }

        public void SwitchTo(string newDirectory, bool makeRelative = true)
        {
            CopyTo(newDirectory, makeRelative);

            if (makeRelative)
            {
                FilePath = Path.GetFileName(FilePath);
            }

            CurrentDirectory = newDirectory;
        }

        public void CopyTo(string newDirectory, bool makeRelative = true)
        {
            if (FileExists && newDirectory != null && (PathIsRelative || makeRelative))
            {
                var newRelativePath = PathIsRelative ? FilePath : Path.GetFileName(FilePath);
                if (newRelativePath != null)
                {
                    var newPath = Path.Combine(newDirectory, newRelativePath);
                    var directory = Path.GetDirectoryName(newPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (!Equals(FullPath, newPath))
                    {
                        File.Copy(FullPath, newPath, true);
                    }
                }
            }
        }
    }
}
