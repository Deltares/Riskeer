using System.Drawing;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    public class PipingSoilProfileNodePresenter : RingtoetsNodePresenterBase<PipingSoilProfile>
    {
        public PipingSoilProfileNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingSoilProfile nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSoilProfileIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }
    }
}