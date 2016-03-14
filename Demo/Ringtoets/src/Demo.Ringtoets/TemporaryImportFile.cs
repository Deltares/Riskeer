using System;
using System.IO;
using Core.Common.Utils.Reflection;

namespace Demo.Ringtoets
{
    /// <summary>
    /// Class for creating a temporary file in the windows Temp directory based on a file 
    /// stored in Embedded Resources.
    /// </summary>
    internal class TemporaryImportFile : IDisposable
    {
        private readonly string targetFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryImportFile"/> class.
        /// </summary>
        /// <param name="embeddedResourceFileNames">The names of the 'Embedded Resource' files to temporary copy to the file system.</param>
        public TemporaryImportFile(params string[] embeddedResourceFileNames)
        {
            targetFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(targetFolderPath);

            foreach (string embeddedResourceFileName in embeddedResourceFileNames)
            {
                WriteEmbeddedResourceToTemporaryFile(embeddedResourceFileName, Path.Combine(targetFolderPath, embeddedResourceFileName));
            }
        }

        public string TargetFolderPath
        {
            get
            {
                return targetFolderPath;
            }
        }

        public void Dispose()
        {
            Directory.Delete(targetFolderPath, true);
        }

        private void WriteEmbeddedResourceToTemporaryFile(string embeddedResourceFileName, string filePath)
        {
            var stream = GetStreamToFileInResource(embeddedResourceFileName);

            var bytes = GetBinaryDataOfStream(stream);

            File.WriteAllBytes(filePath, bytes);
        }

        private Stream GetStreamToFileInResource(string embeddedResourceFileName)
        {
            return AssemblyUtils.GetAssemblyResourceStream(GetType().Assembly, embeddedResourceFileName);
        }

        private static byte[] GetBinaryDataOfStream(Stream stream)
        {
            var bytes = new byte[stream.Length];
            var reader = new BinaryReader(stream);
            reader.Read(bytes, 0, (int) stream.Length);
            return bytes;
        }
    }
}