using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="PipingInputParametersContext"/> instances.
    /// </summary>
    public class PipingInputParametersContextNodePresenter : RingtoetsNodePresenterBase<PipingInputParametersContext>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingInputParametersContext nodeData)
        {
            node.Text = "Invoer";
            node.Image = Resources.PipingInputIcon;
        }
    }
}