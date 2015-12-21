using System.IO;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class TestHelperTest
    {
        [Test]
        public void CanOpenFileForWrite_InvalidPath_DoesNotThrowAnyExceptions()
        {
            const string invalidPath = @".\DirectoryDoesNotExist\fileDoesNotExist";
            var canOpenForWrite = true;

            // Call
            TestDelegate call = () => canOpenForWrite = TestHelper.CanOpenFileForWrite(invalidPath);

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsFalse(canOpenForWrite);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanOpenFileForWrite_InvalidPath_ThrowsException(string invalidPath)
        {
            // Call
            TestDelegate call = () => TestHelper.CanOpenFileForWrite(invalidPath);
         
            // Assert
            Assert.Catch(call);
        }

        [Test]
        public void CanOpenFileForWrite_ValidPath_DoesNotThrowAnyExceptions()
        {
            const string validPath = @".\fileDoesNotExist";
            var canOpenForWrite = false;

            // Call
            TestDelegate call = () => canOpenForWrite = TestHelper.CanOpenFileForWrite(validPath);

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsTrue(canOpenForWrite);

            // Cleanup
            Assert.IsTrue(File.Exists(validPath));
            File.Delete(validPath);
        }
    }
}