using System;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Properties;

namespace Core.Common.Controls.Swf.Charting.Tools
{
    public class ExportChartAsImageChartTool : IChartViewContextMenuTool
    {
        public event EventHandler<EventArgs> ActiveChanged;
        private bool active;

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

            menu.Items.Add(new ToolStripMenuItem(Resources.KeyEvent_Deletion_not_implemented_for_this_type_of_datasource, null, ExportChartEventHandler));
        }

        private void ExportChartEventHandler(object sender, EventArgs e)
        {
            ChartView.ExportAsImage();
        }
    }
}