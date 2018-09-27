// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Util.IO;
using NUnit.Framework;

namespace Core.Common.Util.Test.IO
{
    [TestFixture]
    public class EmbeddedResourceFileWriterTest
    {
        [Test]
        public void Constructor_AssemblyNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new EmbeddedResourceFileWriter(null, true, "A");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assembly", paramName);
        }

        [Test]
        public void Constructor_FilePathsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new EmbeddedResourceFileWriter(GetType().Assembly, true, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("embeddedResourceFileNames", paramName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EmbeddedResourceFileWriter_ValidEmbeddedResources_FilesCorrectlyWritten(bool removeFilesOnDispose)
        {
            // Setup
            const string fileName1 = "EmbeddedResource1.txt";
            const string fileName2 = "EmbeddedResource2.txt";

            // Call
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, removeFilesOnDispose, fileName1, fileName2))
            {
                // Assert
                string filePath1 = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, fileName1);
                string filePath2 = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, fileName2);

                try
                {
                    Assert.IsTrue(File.Exists(filePath1));
                    Assert.IsTrue(File.Exists(filePath2));
                }
                finally
                {
                    if (File.Exists(filePath1))
                    {
                        File.Delete(filePath1);
                    }

                    if (File.Exists(filePath2))
                    {
                        File.Delete(filePath2);
                    }
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EmbeddedResourceFileWriter_ValidEmbeddedResources_FilesPreservedAccordingToFlagRemoveFilesOnDispose(bool removeFilesOnDispose)
        {
            // Setup
            string targetFolderPath;

            // Call
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, removeFilesOnDispose, "EmbeddedResource1.txt", "EmbeddedResource2.txt"))
            {
                targetFolderPath = embeddedResourceFileWriter.TargetFolderPath;
            }

            // Assert
            try
            {
                Assert.AreEqual(!removeFilesOnDispose, File.Exists(Path.Combine(targetFolderPath, "EmbeddedResource1.txt")));
                Assert.AreEqual(!removeFilesOnDispose, File.Exists(Path.Combine(targetFolderPath, "EmbeddedResource2.txt")));
            }
            finally
            {
                // Cleanup
                if (!removeFilesOnDispose)
                {
                    Directory.Delete(targetFolderPath, true);
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EmbeddedResourceFileWriter_ResourceNotEmbedded_ArgumentExceptionThrown(bool removeFilesOnDispose)
        {
            // Setup & Call
            TestDelegate test = () => new EmbeddedResourceFileWriter(GetType().Assembly, removeFilesOnDispose, "NonEmbeddedResource.txt");

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.IsTrue(exception.Message.Contains("Cannot find embedded resource file 'NonEmbeddedResource.txt'"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EmbeddedResourceFileWriter_NonExistingResource_ArgumentExceptionThrown(bool removeFilesOnDispose)
        {
            // Setup & Call
            TestDelegate test = () => new EmbeddedResourceFileWriter(GetType().Assembly, removeFilesOnDispose, "NonExistingResource.txt");

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.IsTrue(exception.Message.Contains("Cannot find embedded resource file 'NonExistingResource.txt'"));
        }
    }
}