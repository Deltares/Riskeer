using NUnit.Framework;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class TreeNodeTest
    {
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