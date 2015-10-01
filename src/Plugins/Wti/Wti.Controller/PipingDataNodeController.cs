using System;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using Wti.Calculation.Piping;
using Wti.Data;
using Wti.Forms.NodePresenters;

namespace Wti.Controller
{
    public class PipingDataNodeController : IContextMenuProvider
    {
        private PipingDataNodePresenter nodePresenter = new PipingDataNodePresenter();

        public PipingDataNodePresenter NodePresenter
        {
            get
            {
                return nodePresenter;
            }
        }

        public PipingDataNodeController()
        {
            nodePresenter.ContextMenu = GetContextMenu;
        }

        public IMenuItem GetContextMenu(object pipingData)
        {
            var contextMenu = new PipingContextMenuStrip((PipingData) pipingData);
            var contextMenuAdapter = new MenuItemContextMenuStripAdapter(contextMenu);
            return contextMenuAdapter;
        }
    }

    public class PipingContextMenuStrip : ContextMenuStrip
    {
        private PipingData pipingData;

        public PipingContextMenuStrip(PipingData pipingData)
        {
            this.pipingData = pipingData;
            var menuItem = new ToolStripButton("Bereken");
            menuItem.Click += menuItem_Click;
            Items.Add(menuItem);
        }

        public sealed override ToolStripItemCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            var input = new PipingCalculationInput(
                0.0,
                0.0,
                0.0,
                pipingData.AssessmentLevel,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0
            );
            var pipingCalculation = new PipingCalculation(input);
            pipingCalculation.Calculate();
        }
    }
}