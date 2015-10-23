using DelftTools.Controls.Swf.TreeViewControls;
using NUnit.Framework;
using SharpTestsEx;

namespace DelftTools.Tests.Controls.Swf.TreeViewControls
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

            node.Should().Be.EqualTo(node1);
        }
    
        [Test]
        public void TestTextLengthLimit()
        {
            var node = treeView.NewNode();
            
            treeView.Nodes.Add(node);
            
            var hugeText = "";
            for (var i = 0; i <= 99; i++) 
            {
                hugeText +=  "1834567890"; // hundred times 10 symbols = 1000 symbols
            }
            Assert.AreEqual(hugeText.Length, 1000);

            node.Text = hugeText;
            Assert.AreEqual(node.Text.Length, hugeText.Length);

            node.Text = hugeText + "123";

            Assert.AreEqual(node.Text.Length, hugeText.Length);
        }

        [Test]
        public void TestTextSetToNull()
        {
            var node = treeView.NewNode();

            treeView.Nodes.Add(node);

            var originalText = "tst";
            node.Text = originalText;
            Assert.AreEqual(originalText, node.Text);

            node.Text = null;
            Assert.AreEqual("", node.Text);
        }
    }
}