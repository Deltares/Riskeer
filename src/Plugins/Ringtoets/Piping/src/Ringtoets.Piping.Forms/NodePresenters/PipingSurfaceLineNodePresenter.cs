using DelftTools.Controls;

using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="RingtoetsPipingSurfaceLine"/> data nodes in the project tree view.
    /// </summary>
    public class PipingSurfaceLineNodePresenter : PipingNodePresenterBase<RingtoetsPipingSurfaceLine>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, RingtoetsPipingSurfaceLine nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSurfaceLineIcon;
        }
    }
}