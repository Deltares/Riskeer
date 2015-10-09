using System.Collections;
using System.Drawing;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;

namespace DeltaShell.Plugins.ProjectExplorer.NodePresenters
{
    public class TreeFolderNodePresenter : TreeViewNodePresenterBaseForPluginGui<TreeFolder>
    {
        public TreeFolderNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin) {}

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, TreeFolder data)
        {
            node.Text = data.Text;
            node.Tag = data;
            node.Image = GetImage(data);
        }

        public override IEnumerable GetChildNodeObjects(TreeFolder parentNodeData, ITreeNode node)
        {
            return parentNodeData.ChildItems;
        }

        private static Image GetImage(TreeFolder data)
        {
            return data.Image;
        }
    }
}