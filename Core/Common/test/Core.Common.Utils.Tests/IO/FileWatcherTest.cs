using Core.Common.Utils.IO;
using NUnit.Framework;

namespace Core.Common.Utils.Tests.IO
{
    [TestFixture]
    public class FileWatcherTest
    {
        [Test]
        public void RelativePathTest()
        {
            FileWatcher fileWatcher = new FileWatcher();
            fileWatcher.FilePath = @"c:\temp\RelativePathTest\Test\somefile.ext";
            FileWatcher.ProjectLocation = @"c:\temp";
            Assert.IsTrue(FileUtils.CompareDirectories(@"RelativePathTest\Test\somefile.ext", fileWatcher.RelativePath));
        }

        [Test]
        public void RelativePathTest2()
        {
            FileWatcher fileWatcher = new FileWatcher();
            FileWatcher.ProjectLocation = @"c:\temp";
            fileWatcher.FilePath = @"c:\temp\RelativePathTest\Test\somefile.ext";

            Assert.IsTrue(FileUtils.CompareDirectories(@"RelativePathTest\Test\somefile.ext", fileWatcher.RelativePath));
        }
    }
}