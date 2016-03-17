using System;
using System.IO;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to set a temporary file while testing. 
    /// Disposing an instance of this class will delete the file.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new FileDisposeHelper("pathToFile")) {
    ///     // Perform tests with file
    /// }
    /// </code>
    /// </example>
    public class FileDisposeHelper : IDisposable
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePath">Path of the file that will be used.</param>
        public FileDisposeHelper(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Creates the temporary file.
        /// </summary>
        public void CreateFile()
        {
            using (File.Create(filePath)) {}
        }

        /// <summary>
        /// Disposes the <see cref="FileDisposeHelper"/> instance.
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}