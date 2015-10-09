using System;
using DelftTools.Controls.Swf.TreeViewControls;

namespace DelftTools.Tests.TestObjects
{
    public class MockTestNode : TreeNode
    {
        public event EventHandler Refreshed;

        public MockTestNode(TreeView treeView, bool loaded)
            : base(treeView)
        {
            isLoaded = loaded;
        }

        public void SetLoaded(bool value)
        {
            isLoaded = value;
        }
    }
}