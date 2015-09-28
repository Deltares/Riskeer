using System.IO;
using DelftTools.Controls.Swf;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
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