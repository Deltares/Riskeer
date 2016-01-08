using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of <see cref="RingtoetsPipingSurfaceLine"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSurfaceLineCollectionNodePresenter : RingtoetsNodePresenterBase<IEnumerable<RingtoetsPipingSurfaceLine>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineCollectionNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingSurfaceLineCollectionNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(TreeNode parentNode, TreeNode node, IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            node.Text = Resources.PipingSurfaceLinesCollection_DisplayName;
            node.ForegroundColor = nodeData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FolderIcon;
        }

        protected override IEnumerable GetChildNodeObjects(IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            return nodeData;
        }

        protected override ContextMenuStrip GetContextMenu(TreeNode node, IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .Build();
        }
    }
}