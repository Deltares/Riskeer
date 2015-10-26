using DelftTools.Controls;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    public class PipingSoilProfileNodePresenter : PipingNodePresenterBase<PipingSoilProfile>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingSoilProfile nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSoilProfileIcon;
        }
    }
}