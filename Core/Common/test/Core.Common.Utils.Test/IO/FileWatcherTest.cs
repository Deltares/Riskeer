using Core.Common.Utils.IO;
using NUnit.Framework;

namespace Core.Common.Utils.Test.IO
{
    [TestFixture]
    public class FileWatcherTest
    {
        [Test]
        public void RelativePathTest()
        {
            FileWatcher fileWatcher = new FileWatcher
            {
                FilePath = @"c:\temp\RelativePathTest\Test\somefile.ext"
            };
            FileWatcher.ProjectLocation = @"c:\temp";
            Assert.IsTrue(FileUtils.CompareDirectories(@"RelativePathTest\Test\somefile.ext", fileWatcher.RelativePath));
        }
    }
}