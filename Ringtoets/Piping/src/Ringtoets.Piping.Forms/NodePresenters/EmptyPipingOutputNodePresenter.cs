using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// A node presenter for <see cref="EmptyPipingOutput"/> instances implemented to mimics 
    /// the looks of <see cref="PipingOutputNodePresenter"/> to provide a uniform look and feel.
    /// </summary>
    public class EmptyPipingOutputNodePresenter : RingtoetsNodePresenterBase<EmptyPipingOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmptyPipingCalculationReportNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public EmptyPipingOutputNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, EmptyPipingOutput nodeData)
        {
            var dummyOutput = new PipingOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            new PipingOutputNodePresenter(contextMenuBuilderProvider).UpdateNode(parentNode, node, dummyOutput);
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, EmptyPipingOutput nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddDeleteItem()
                .AddSeparator()
                .AddExportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}