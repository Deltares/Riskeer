using Core.Common.Controls.Swf.TreeViewControls;
using NUnit.Framework;

namespace Core.Common.Base.Test.Controls.Swf.TreeViewControls
{
    [TestFixture]
    public class TreeNodeTest
    {
        private TreeView treeView;

        [SetUp]
        public void SetUp()
        {
            treeView = new TreeView();
        }

        [Test]
        public void GetParentOfLevel()
        {
            var node1 = treeView.NewNode();
            var node11 = treeView.NewNode();
            var node12 = treeView.NewNode();

            treeView.Nodes.Add(node1);
            node1.Nodes.Add(node11);
            node1.Nodes.Add(node12);

            var node = node12.GetParentOfLevel(0);

            Assert.AreEqual(node1, node);
        }
    
        [Test]
        public void TestTextLengthLimit()
        {
            var node = new TreeNode(null);

            var largeText = "";
            for (var i = 0; i <= 24; i++) 
            {
                largeText += "1234567890";
            }

            node.Text = largeText;
            Assert.AreEqual(250, node.Text.Length);

            node.Text = largeText + "123456789";
            Assert.AreEqual(259, node.Text.Length);

            node.Text = largeText + "1234567890";
            Assert.AreEqual(259, node.Text.Length, "Text length limit should be 259");
        }

        [Test]
        public void TestTextSetToNull()
        {
            var node = new TreeNode(null);

            var originalText = "test";
            node.Text = originalText;
            Assert.AreEqual(originalText, node.Text);

            node.Text = null;
            Assert.AreEqual("", node.Text);
        }
    }
}