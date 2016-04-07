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
            FileDisposeHelper disposeHelper = null;

            // Precondition
            Assert.IsFalse(File.Exists(filePath), String.Format("Precondition failed: File '{0}' should not exist", filePath));

            // Call
            TestDelegate test = () => disposeHelper = new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
            disposeHelper.Dispose();
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Constructor_ExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = "doesExist.tmp";
            FileDisposeHelper disposeHelper = null;

            try
            {
                using (File.Create(filePath)) {}

                // Precondition
                Assert.IsTrue(File.Exists(filePath), String.Format("Precondition failed: File '{0}' should exist", filePath));

                // Call
                TestDelegate test = () => disposeHelper = new FileDisposeHelper(filePath);

                // Assert
                Assert.DoesNotThrow(test);
                disposeHelper.Dispose();
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Create_FileDoesNotExist_Createsfile()
        {
            // Setup
            string filePath = "willExist.tmp";

            try
            {
                // Precondition
                Assert.IsFalse(File.Exists(filePath));

                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                {
                    // Call
                    fileDisposeHelper.Create();

                    // Assert
                    Assert.IsTrue(File.Exists(filePath));
                }
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Create_InvalidPath_DoesNotThrowException()
        {
            // Setup
            var filePath = String.Empty;
            var fileDisposeHelper = new FileDisposeHelper(filePath);

            // Call
            TestDelegate test = () => fileDisposeHelper.Create();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Create_MultipleFiles_CreatesFiles()
        {
            // Setup
            var filePaths = new[]
            {
                "willExist.tmp",
                "alsoWillExist.tmp"
            };

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePaths))
                {
                    // Call
                    fileDisposeHelper.Create();

                    // Assert
                    foreach (var filePath in filePaths)
                    {
                        Assert.IsTrue(File.Exists(filePath));
                    }
                }
            }
            catch (Exception exception)
            {
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
                Assert.Fail(exception.Message);
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

                // Precondition
                Assert.IsTrue(File.Exists(filePath));

                // Call
                using (new FileDisposeHelper(filePath)) {}
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }

            // Assert
            Assert.IsFalse(File.Exists(filePath));
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
                    using (File.Create(filePath)) {}

                    // Precondition
                    Assert.IsTrue(File.Exists(filePath));
                }

                // Call
                using (new FileDisposeHelper(filePaths)) {}
            }
            catch (Exception exception)
            {
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
                Assert.Fail(exception.Message);
            }

            // Assert
            foreach (var filePath in filePaths)
            {
                Assert.IsFalse(File.Exists(filePath));
            }
        }
    }
}