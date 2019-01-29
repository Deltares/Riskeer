// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Reflection;
using Core.Common.Util.Reflection;

namespace Core.Common.Util.IO
{
    /// <summary>
    /// Class for writing Embedded Resources to the Windows Temp directory.
    /// </summary>
    public class EmbeddedResourceFileWriter : IDisposable
    {
        private readonly bool removeFilesOnDispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceFileWriter"/> class.
        /// </summary>
        /// <param name="assembly">The assembly that embeds the resources to write.</param>
        /// <param name="removeFilesOnDispose">Whether or not the files should be removed after 
        /// disposing the created <see cref="EmbeddedResourceFileWriter"/> instance.</param>
        /// <param name="embeddedResourceFileNames">The names of the Embedded Resource files to 
        /// (temporarily) write to the Windows Temp directory.</param>
        /// <exception cref="ArgumentException">Thrown when an embedded resource file in <paramref name="embeddedResourceFileNames"/> 
        /// cannot be found in <paramref name="assembly"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> 
        /// or <paramref name="embeddedResourceFileNames"/> is <c>null</c>.</exception>
        public EmbeddedResourceFileWriter(Assembly assembly, bool removeFilesOnDispose, params string[] embeddedResourceFileNames)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (embeddedResourceFileNames == null)
            {
                throw new ArgumentNullException(nameof(embeddedResourceFileNames));
            }

            this.removeFilesOnDispose = removeFilesOnDispose;

            TargetFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(TargetFolderPath);

            foreach (string embeddedResourceFileName in embeddedResourceFileNames)
            {
                WriteEmbeddedResourceToTemporaryFile(assembly, embeddedResourceFileName, Path.Combine(TargetFolderPath, embeddedResourceFileName));
            }
        }

        /// <summary>
        /// Gets the (automatically generated) target folder path.
        /// </summary>
        public string TargetFolderPath { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (removeFilesOnDispose && disposing)
            {
                Directory.Delete(TargetFolderPath, true);
            }
        }

        private static void WriteEmbeddedResourceToTemporaryFile(Assembly assembly, string embeddedResourceFileName, string filePath)
        {
            Stream stream = GetStreamToFileInResource(assembly, embeddedResourceFileName);
            byte[] bytes = GetBinaryDataOfStream(stream);

            File.WriteAllBytes(filePath, bytes);
        }

        private static Stream GetStreamToFileInResource(Assembly assembly, string embeddedResourceFileName)
        {
            return AssemblyUtils.GetAssemblyResourceStream(assembly, embeddedResourceFileName);
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