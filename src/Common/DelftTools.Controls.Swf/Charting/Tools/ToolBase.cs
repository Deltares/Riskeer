using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public abstract class ToolBase
    {
        private readonly IChart chart;
        internal readonly ToolWrapper tool;

        protected ToolBase(IChart chart)
        {
            this.chart = chart;
            tool = new ToolWrapper(chart);
        }

        public IChart Chart { get { return chart; } }

        protected Action<EventArgs> OnAfterDraw
        {
            get { return tool.OnAfterDraw; }
            set { tool.OnAfterDraw = value; }
        }

        public bool Active
        {
            get { return tool.Active; }
            set { tool.Active = value; }
        }

        public Action<ChartMouseEvent, MouseEventArgs, Cursor> OnMouseEvent
        {
            get { return tool.OnMouseEvent; }
            set { tool.OnMouseEvent = value; }
        }

        public void Invalidate()
        {
            tool.Invalidate();
        }
    }
}