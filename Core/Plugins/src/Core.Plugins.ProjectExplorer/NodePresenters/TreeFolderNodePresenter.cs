using System.Collections;
using System.Drawing;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Swf;

namespace Core.Plugins.ProjectExplorer.NodePresenters
{
    public class TreeFolderNodePresenter : TreeViewNodePresenterBase<TreeFolder>
    {
        public override void UpdateNode(TreeNode parentNode, TreeNode node, TreeFolder data)
        {
            node.Text = data.Text;
            node.Tag = data;
            node.Image = GetImage(data);
        }

        public override IEnumerable GetChildNodeObjects(TreeFolder parentNodeData)
        {
            return parentNodeData.ChildItems;
        }

        private static Image GetImage(TreeFolder data)
        {
            return data.Image;
        }
    }
}