using System;
using System.Collections;
using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;

namespace DelftTools.Tests.TestObjects
{
    public class ChildNodePresenter : TreeViewNodePresenterBase<Child>
    {
        public event EventHandler AfterUpdate;

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Child nodeData)
        {
            node.Text = nodeData.Name;

            if (AfterUpdate != null)
            {
                AfterUpdate(this, null);
            }
        }

        public override IEnumerable GetChildNodeObjects(Child parentNodeData, ITreeNode node)
        {
            return parentNodeData.Children;
        }
    }
}