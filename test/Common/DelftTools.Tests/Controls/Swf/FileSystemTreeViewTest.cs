using System.IO;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class FileSystemTreeViewTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Show()
        {
            var treeView = new FileSystemTreeView();
            WindowsFormsTestHelper.ShowModal(treeView);
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ExpandTo()
        {
            var treeView = new FileSystemTreeView();
            Assert.IsTrue(treeView.ExpandTo(Path.GetTempPath()));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ExpandToWithEmptyString()
        {
            var treeView = new FileSystemTreeView();
            Assert.IsFalse(treeView.ExpandTo(""));
        }
    }
}