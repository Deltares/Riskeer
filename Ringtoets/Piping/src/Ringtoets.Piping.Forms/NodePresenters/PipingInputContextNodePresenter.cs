using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="PipingInputContext"/> instances.
    /// </summary>
    public class PipingInputContextNodePresenter : RingtoetsNodePresenterBase<PipingInputContext>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingInputContext nodeData)
        {
            node.Text = "Invoer";
            node.Image = Resources.PipingInputIcon;
        }
    }
}