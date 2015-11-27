using System.Drawing;

using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="PipingOutput"/> instances.
    /// </summary>
    public class PipingOutputNodePresenter : RingtoetsNodePresenterBase<PipingOutput>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingOutput nodeData)
        {
            node.Text = Resources.PipingOutput_DisplayName;
            node.Image = Resources.PipingOutputIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override bool CanRemove(object parentNodeData, PipingOutput nodeData)
        {
            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingOutput nodeData)
        {
            var pipingCalculationContext = (PipingCalculationContext)parentNodeData;

            pipingCalculationContext.ClearOutput();
            pipingCalculationContext.NotifyObservers();

            return true;
        }
    }
}