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
        private readonly string tempTargetFolderPath;
        private readonly string fullFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryImportFile"/> class.
        /// </summary>
        /// <param name="embeddedResourceFileName">Name of the file with build action 'Embedded Resource' within this project.</param>
        /// <param name="supportFiles">Names of extra files required for importing the <paramref name="embeddedResourceFileName"/>.</param>
        public TemporaryImportFile(string embeddedResourceFileName, params string[] supportFiles)
        {
            tempTargetFolderPath = Path.Combine(Path.GetTempPath(), "demo_traject");
            Directory.CreateDirectory(tempTargetFolderPath);

            fullFilePath = Path.Combine(tempTargetFolderPath, embeddedResourceFileName);

            WriteEmbeddedResourceToTemporaryFile(embeddedResourceFileName, fullFilePath);

            foreach (string supportFile in supportFiles)
            {
                var filePath = Path.Combine(tempTargetFolderPath, supportFile);
                WriteEmbeddedResourceToTemporaryFile(supportFile, filePath);
            }
        }

        private void WriteEmbeddedResourceToTemporaryFile(string embeddedResourceFileName, string filePath)
        {
            var stream = GetStreamToFileInResource(embeddedResourceFileName);

            var bytes = GetBinaryDataOfStream(stream);

            File.WriteAllBytes(filePath, bytes);
        }

        public string FilePath
        {
            get
            {
                return fullFilePath;
            }
        }

        public void Dispose()
        {
            Directory.Delete(tempTargetFolderPath, true);
        }

        private Stream GetStreamToFileInResource(string embeddedResourceFileName)
        {
            return AssemblyUtils.GetAssemblyResourceStream(GetType().Assembly, embeddedResourceFileName);
        }

        private static byte[] GetBinaryDataOfStream(Stream stream)
        {
            var bytes = new byte[stream.Length];
            var reader = new BinaryReader(stream);
            reader.Read(bytes, 0, (int)stream.Length);
            return bytes;
        }
    }
}