using Core.Common.Controls;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
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