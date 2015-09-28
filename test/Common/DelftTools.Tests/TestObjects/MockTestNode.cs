using System;
using DelftTools.Controls.Swf.TreeViewControls;

namespace DelftTools.Tests.TestObjects
{
    public class MockTestNode : TreeNode
    {
        public MockTestNode(TreeView treeView, bool loaded)
            : base(treeView)
        {
            isLoaded = loaded;
        }

        public void SetLoaded(bool value)
        {
            isLoaded = value;
        }

        public event EventHandler Refreshed;
    }
}