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

            node
                .Should().Be.EqualTo(node1);
        }
    }
}