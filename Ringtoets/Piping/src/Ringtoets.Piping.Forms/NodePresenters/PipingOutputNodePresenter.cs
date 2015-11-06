using Core.Common.Controls;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    public class PipingOutputNodePresenter : PipingNodePresenterBase<PipingOutput>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingOutput nodeData)
        {
            node.Text = Resources.PipingOutput_DisplayName;
            node.Image = Resources.PipingOutputIcon;
        }

        protected override bool CanRemove(object parentNodeData, PipingOutput nodeData)
        {
            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingOutput nodeData)
        {
            var pipingCalculationInputs = (PipingCalculationInputs)parentNodeData;

            pipingCalculationInputs.ClearOutput();
            pipingCalculationInputs.NotifyObservers();

            return true;
        }
    }
}