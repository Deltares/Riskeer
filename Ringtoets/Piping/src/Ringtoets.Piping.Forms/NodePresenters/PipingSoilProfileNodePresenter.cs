using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    public class PipingSoilProfileNodePresenter : RingtoetsNodePresenterBase<PipingSoilProfile>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingSoilProfile nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSoilProfileIcon;
        }
    }
}