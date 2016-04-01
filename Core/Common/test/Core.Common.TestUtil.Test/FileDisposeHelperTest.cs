using System;
using System.IO;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class FileDisposeHelperTest
    {
        [Test]
        public void Constructor_NotExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = "doesNotExist.tmp";
            Assert.IsFalse(File.Exists(filePath));

            try
            {
                // Call
                TestDelegate test = () => new FileDisposeHelper(filePath);

                // Assert
                Assert.DoesNotThrow(test);
                Assert.IsFalse(File.Exists(filePath));
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Constructor_ExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = "doesNotExist.tmp";

            try
            {
                using (File.Create(filePath)) {}
                Assert.IsTrue(File.Exists(filePath));

                // Call
                TestDelegate test = () => new FileDisposeHelper(filePath);

                // Assert
                Assert.DoesNotThrow(test);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void CreateFile_FileDoesNotExist_Createsfile()
        {
            // Setup
            string filePath = "doesExist.tmp";

            try
            {
                Assert.IsFalse(File.Exists(filePath));

                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                {
                    // Call
                    fileDisposeHelper.CreateFile();
                    // Assert
                Assert.IsTrue(File.Exists(filePath));
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Dispose_InvalidPath_DoesNotThrowException()
        {
            // Setup
            string filePath = String.Empty;

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Dispose_ExistingFile_DeletesFile()
        {
            // Setup
            string filePath = "doesExist.tmp";

            try
            {
                using (File.Create(filePath)) {}
                Assert.IsTrue(File.Exists(filePath));

                // Call
                using (new FileDisposeHelper(filePath)) {}

                // Assert
                Assert.IsFalse(File.Exists(filePath));
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Dispose_MultipleFiles_DeletesFiles()
        {
            // Setup
            var filePaths = new[]
            {
                "doesExist.tmp",
                "alsoDoesExist.tmp"
            };

            try
            {
                foreach (var filePath in filePaths)
                {
                    using (File.Create(filePath)) { }
                    Assert.IsTrue(File.Exists(filePath));
                }
                
                // Call
                using (new FileDisposeHelper(filePaths)) { }

                // Assert
                foreach (var filePath in filePaths)
                {
                    Assert.IsFalse(File.Exists(filePath));
                }
            }
            finally
            {
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
                
            }
        }
    }
}