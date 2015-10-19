using DelftTools.Controls;

using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="PipingSurfaceLine"/> data nodes in the project tree view.
    /// </summary>
    public class PipingSurfaceLineNodePresenter : PipingNodePresenterBase<PipingSurfaceLine>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingSurfaceLine nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSurfaceLineIcon;
        }
    }
}