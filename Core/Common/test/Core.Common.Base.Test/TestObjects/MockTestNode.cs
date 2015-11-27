using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Base.Test.TestObjects
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
    }
}