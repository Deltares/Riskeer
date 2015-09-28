using System.Collections;
using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;

namespace DelftTools.Tests.TestObjects
{
    public class ParentNodePresenter : TreeViewNodePresenterBase<Parent>
    {
        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Parent nodeData)
        {
            node.Text = nodeData.Name;
        }

        public override IEnumerable GetChildNodeObjects(Parent parentNodeData, ITreeNode node)
        {
            return parentNodeData.Children;
        }
    }
}