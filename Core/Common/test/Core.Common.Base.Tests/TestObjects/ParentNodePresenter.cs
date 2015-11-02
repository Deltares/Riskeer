using System.Collections;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Base.Tests.TestObjects
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