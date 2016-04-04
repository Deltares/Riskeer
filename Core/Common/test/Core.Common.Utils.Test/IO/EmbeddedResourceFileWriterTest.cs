using System;
using System.IO;
using Core.Common.Utils.IO;
using NUnit.Framework;

namespace Core.Common.Utils.Test.IO
{
    [TestFixture]
    public class EmbeddedResourceFileWriterTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void EmbeddedResourceFileWriter_ValidEmbeddedResources_FilesCorrectlyWritten(bool removeFilesOnDispose)
        {
            // Call
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly, removeFilesOnDispose, "EmbeddedResource1.txt", "EmbeddedResource2.txt"))
            {
                // Assert
                Assert.IsTrue(File.Exists(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "EmbeddedResource1.txt")));
                Assert.IsTrue(File.Exists(Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "EmbeddedResource2.txt")));
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
            Assert.AreEqual(!removeFilesOnDispose, File.Exists(Path.Combine(targetFolderPath, "EmbeddedResource1.txt")));
            Assert.AreEqual(!removeFilesOnDispose, File.Exists(Path.Combine(targetFolderPath, "EmbeddedResource2.txt")));

            // Cleanup
            if (!removeFilesOnDispose)
            {
                Directory.Delete(targetFolderPath, true);
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
