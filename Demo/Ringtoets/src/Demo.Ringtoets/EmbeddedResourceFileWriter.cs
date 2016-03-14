using System;
using System.IO;
using Core.Common.Utils.Reflection;

namespace Demo.Ringtoets
{
    /// <summary>
    /// Class for writing Embedded Resources to the Windows Temp directory.
    /// </summary>
    internal class EmbeddedResourceFileWriter : IDisposable
    {
        private readonly bool removeFilesOnDispose;
        private readonly string targetFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceFileWriter"/> class.
        /// </summary>
        /// <param name="removeFilesOnDispose">Whether or not the files should be removed after disposing the created <see cref="EmbeddedResourceFileWriter"/> instance.</param>
        /// <param name="embeddedResourceFileNames">The names of the Embedded Resource files to (temporary) write to the Windows Temp directory.</param>
        public EmbeddedResourceFileWriter(bool removeFilesOnDispose, params string[] embeddedResourceFileNames)
        {
            this.removeFilesOnDispose = removeFilesOnDispose;

            targetFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(targetFolderPath);

            foreach (string embeddedResourceFileName in embeddedResourceFileNames)
            {
                WriteEmbeddedResourceToTemporaryFile(embeddedResourceFileName, Path.Combine(targetFolderPath, embeddedResourceFileName));
            }
        }

        /// <summary>
        /// Gets the target folder path.
        /// </summary>
        public string TargetFolderPath
        {
            get
            {
                return targetFolderPath;
            }
        }

        /// <summary>
        /// Disposes the <see cref="EmbeddedResourceFileWriter"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (removeFilesOnDispose)
            {
                Directory.Delete(targetFolderPath, true);
            }
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