using System.Collections;
using System.Drawing;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui.Swf;

namespace Core.Plugins.ProjectExplorer.NodePresenters
{
    public class TreeFolderNodePresenter : TreeViewNodePresenterBase<TreeFolder>
    {
        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, TreeFolder data)
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