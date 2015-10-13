using System;
using System.Windows.Forms;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Class for creating a <seealso cref="ContextMenuStrip"/> for a <see cref="PipingDataNodePresenter"/>.
    /// </summary>
    internal sealed class PipingContextMenuStrip : ContextMenuStrip
    {
        private readonly PipingData pipingData;

        /// <summary>
        /// Gets or sets the <see cref="Action"/> which is performed when the calculate item on a <see cref="PipingContextMenuStrip"/> is called.
        /// </summary>
        public Action<PipingData> OnCalculationClick { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PipingContextMenuStrip"/> and assigns the <see cref="PipingData"/> to it.
        /// </summary>
        /// <param name="pipingData"></param>
        public PipingContextMenuStrip(PipingData pipingData)
        {
            this.pipingData = pipingData;
            var calculateItem = new ToolStripMenuItem(Resources.PipingDataContextMenuCalculate);
            calculateItem.Click += CalculateItemClick;
            Items.Add(calculateItem);
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