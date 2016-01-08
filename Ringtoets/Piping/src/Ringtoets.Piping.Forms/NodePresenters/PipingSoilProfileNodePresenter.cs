using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    public class PipingSoilProfileNodePresenter : RingtoetsNodePresenterBase<PipingSoilProfile>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfileNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingSoilProfileNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(TreeNode parentNode, TreeNode node, PipingSoilProfile nodeData)
        {
            node.Text = nodeData.Name;
            node.Image = Resources.PipingSoilProfileIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override ContextMenuStrip GetContextMenu(TreeNode node, PipingSoilProfile nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddPropertiesItem()
                .Build();
        }
    }
}