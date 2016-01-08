﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using RingtoestFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="PipingOutput"/> instances.
    /// </summary>
    public class PipingOutputNodePresenter : RingtoetsNodePresenterBase<PipingOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingOutputNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingOutputNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(TreeNode parentNode, TreeNode node, PipingOutput nodeData)
        {
            UpdateNode(node);
        }

        protected static void UpdateNode(TreeNode node)
        {
            node.Text = Resources.PipingOutput_DisplayName;
            node.Image = Resources.PipingOutputIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override ContextMenuStrip GetContextMenu(TreeNode node, PipingOutput nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddExportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}