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
