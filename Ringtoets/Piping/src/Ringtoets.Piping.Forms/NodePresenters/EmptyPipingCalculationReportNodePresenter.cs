using System.Drawing;

using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// A node presenter for <see cref="EmptyPipingCalculationReport"/> instances implemented 
    /// to mimicks the looks of the node presenter for <see cref="PipingCalculationData.CalculationReport"/> 
    /// to provide a uniform look and feel.
    /// </summary>
    public class EmptyPipingCalculationReportNodePresenter : RingtoetsNodePresenterBase<EmptyPipingCalculationReport>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, EmptyPipingCalculationReport nodeData)
        {
            node.Text = PipingDataResources.CalculationReport_DisplayName;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = PipingFormsResources.PipingCalculationReportIcon;
        }
    }
}