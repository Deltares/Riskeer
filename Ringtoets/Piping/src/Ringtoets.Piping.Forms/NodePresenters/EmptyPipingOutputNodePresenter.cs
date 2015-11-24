using System.Drawing;

using Core.Common.Controls;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// A node presenter for <see cref="EmptyPipingOutput"/> instances implemented to mimicks 
    /// the looks of <see cref="PipingOutputNodePresenter"/> to provide a uniform look and feel.
    /// </summary>
    public class EmptyPipingOutputNodePresenter : RingtoetsNodePresenterBase<EmptyPipingOutput>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, EmptyPipingOutput nodeData)
        {
            var dummyOutput = new PipingOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            new PipingOutputNodePresenter().UpdateNode(parentNode, node, dummyOutput);
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
        }
    }
}