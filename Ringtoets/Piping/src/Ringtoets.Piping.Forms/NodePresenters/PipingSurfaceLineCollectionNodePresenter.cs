using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of <see cref="RingtoetsPipingSurfaceLine"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSurfaceLineCollectionNodePresenter : RingtoetsNodePresenterBase<IEnumerable<RingtoetsPipingSurfaceLine>>
    {
        public PipingSurfaceLineCollectionNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            node.Text = Resources.PipingSurfaceLinesCollection_DisplayName;
            node.ForegroundColor = nodeData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FolderIcon;
        }

        protected override IEnumerable GetChildNodeObjects(IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            return nodeData;
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            return contextMenuBuilderProvider
                .Get(sender)
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddImportItem()
                .AddExportItem()
                .Build();
        }
    }
}