using System;
using System.Windows.Forms;
using Wti.Calculation.Piping;
using Wti.Data;

namespace Wti.Controller
{
    internal class PipingContextMenuStrip : ContextMenuStrip
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