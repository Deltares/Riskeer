using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary files while testing. 
    /// Disposing an instance of this class will delete the files.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new FileDisposeHelper(new[]{"pathToFile"})) {
    ///     // Perform tests with files
    /// }
    /// </code>
    /// </example>
    public class FileDisposeHelper : IDisposable
    {
        private readonly IEnumerable<string> files;

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePaths">Path of the files that will be used.</param>
        public FileDisposeHelper(IEnumerable<string> filePaths)
        {
            files = filePaths;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePath">Path of the single file that will be used.</param>
        public FileDisposeHelper(string filePath)
        {
            files = new[]
            {
                filePath
            };
        }

        /// <summary>
        /// Creates the temporary files.
        /// </summary>
        public void CreateFile()
        {
            foreach (var file in files)
            {
                using (File.Create(file)) {}
            }
        }

        /// <summary>
        /// Disposes the <see cref="FileDisposeHelper"/> instance.
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (var file in files)
            {
                Dispose(file);
            }
        }

        private static void Dispose(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                File.Delete(filename);
            }
        }
    }
}