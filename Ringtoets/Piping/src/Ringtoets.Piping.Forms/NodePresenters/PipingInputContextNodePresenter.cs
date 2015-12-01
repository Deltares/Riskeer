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
        public PipingInputContextNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingInputContext nodeData)
        {
            node.Text = Resources.PipingInputContextNodePresenter_NodeDisplayName;
            node.Image = Resources.PipingInputIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PipingInputContext nodeData)
        {
            return contextMenuBuilderProvider
                .Get(sender)
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}