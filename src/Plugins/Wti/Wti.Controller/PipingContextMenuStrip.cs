using System;
using System.Windows.Forms;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Controller
{
    internal class PipingContextMenuStrip : ContextMenuStrip
    {
        private PipingData pipingData;

        public Action<PipingData> OnCalculationClick;

        public PipingContextMenuStrip(PipingData pipingData)
        {
            this.pipingData = pipingData;
            var calculateItem = new ToolStripMenuItem(Resources.PipingDataContextMenuCalculate);
            calculateItem.Click += CalculateItemClick;
            Items.Add(calculateItem);
        }

        public sealed override ToolStripItemCollection Items
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