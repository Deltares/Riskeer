using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
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
        /// <summary>
        /// Creates a new instance of <see cref="PipingInputContextNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingInputContextNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingInputContext nodeData)
        {
            node.Text = Resources.PipingInputContextNodePresenter_NodeDisplayName;
            node.Image = Resources.PipingInputIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, PipingInputContext nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}