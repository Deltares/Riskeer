using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
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
        /// Sets the <see cref="IContextMenuBuilderProvider"/> to be used for creating the <see cref="ContextMenuStrip"/>.
        /// </summary>
        public IContextMenuBuilderProvider ContextMenuBuilderProvider { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingInputContext nodeData)
        {
            node.Text = "Invoer";
            node.Image = Resources.PipingInputIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingInputContext nodeData)
        {
            if (ContextMenuBuilderProvider == null)
            {
                return null;
            }
            return ContextMenuBuilderProvider
                .Get(sender)
                .AddExportItem()
                .AddImportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}