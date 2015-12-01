using System.Drawing;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="RingtoetsPipingSurfaceLine"/> data nodes in the project tree view.
    /// </summary>
    public class PipingSurfaceLineNodePresenter : RingtoetsNodePresenterBase<RingtoetsPipingSurfaceLine>
    {
        public PipingSurfaceLineNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, RingtoetsPipingSurfaceLine nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSurfaceLineIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }
    }
}