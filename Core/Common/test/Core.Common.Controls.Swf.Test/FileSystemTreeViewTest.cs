using System.IO;
using NUnit.Framework;

namespace Core.Common.Controls.Swf.Test
{
    [TestFixture]
    public class FileSystemTreeViewTest
    {
        [Test]
        public void ExpandTo()
        {
            var treeView = new FileSystemTreeView();
            Assert.IsTrue(treeView.ExpandTo(Path.GetTempPath()));
        }

        [Test]
        public void ExpandToWithEmptyString()
        {
            var treeView = new FileSystemTreeView();
            Assert.IsFalse(treeView.ExpandTo(""));
        }
    }
}