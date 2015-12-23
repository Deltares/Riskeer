﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Placeholder;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// A node presenter for <see cref="EmptyPipingCalculationReport"/> instances implemented 
    /// to mimics the looks of the node presenter for <see cref="PipingCalculation.CalculationReport"/> 
    /// to provide a uniform look and feel.
    /// </summary>
    public class EmptyPipingCalculationReportNodePresenter : RingtoetsNodePresenterBase<EmptyPipingCalculationReport>
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmptyPipingCalculationReportNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public EmptyPipingCalculationReportNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, EmptyPipingCalculationReport nodeData)
        {
            node.Text = PipingDataResources.CalculationReport_DisplayName;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = PipingFormsResources.PipingCalculationReportIcon;
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, EmptyPipingCalculationReport nodeData)
        {
            return contextMenuBuilderProvider
                .Get(node)
                .AddOpenItem()
                .AddSeparator()
                .AddDeleteItem()
                .AddSeparator()
                .AddExportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }
    }
}