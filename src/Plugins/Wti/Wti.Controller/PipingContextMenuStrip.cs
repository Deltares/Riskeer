using System;
using System.Windows.Forms;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Controller
{
    internal class PipingContextMenuStrip : ContextMenuStrip
    {
        public Action<PipingData> OnCalculationClick;
        private readonly PipingData pipingData;

        public PipingContextMenuStrip(PipingData pipingData)
        {
            this.pipingData = pipingData;
            var calculateItem = new ToolStripMenuItem(Resources.PipingDataContextMenuCalculate);
            calculateItem.Click += CalculateItemClick;
            Items.Add(calculateItem);
        }

        public override sealed ToolStripItemCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        private void CalculateItemClick(object sender, EventArgs e)
        {
            if (OnCalculationClick != null)
            {
                OnCalculationClick(pipingData);
            }
        }
    }
}