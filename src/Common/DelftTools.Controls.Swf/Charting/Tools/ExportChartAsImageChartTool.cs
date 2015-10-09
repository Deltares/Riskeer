using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public class ExportChartAsImageChartTool : IChartViewContextMenuTool
    {
        public event EventHandler<EventArgs> ActiveChanged;
        private bool active;

        public ExportChartAsImageChartTool(IChartView chartView)
        {
            ChartView = chartView;
        }

        public IChartView ChartView { get; set; }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public bool Enabled { get; set; }

        public void OnBeforeContextMenu(ContextMenuStrip menu)
        {
            if (menu.Items.Count > 0)
            {
                menu.Items.Add(new ToolStripSeparator());
            }

            menu.Items.Add(new ToolStripMenuItem("Export as Image...", null, ExportChartEventHandler));
        }

        private void ExportChartEventHandler(object sender, EventArgs e)
        {
            ChartView.ExportAsImage();
        }
    }
}