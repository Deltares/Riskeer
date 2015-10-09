using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public abstract class ToolBase
    {
        internal readonly ToolWrapper tool;
        private readonly IChart chart;

        protected ToolBase(IChart chart)
        {
            this.chart = chart;
            tool = new ToolWrapper(chart);
        }

        public IChart Chart
        {
            get
            {
                return chart;
            }
        }

        public bool Active
        {
            get
            {
                return tool.Active;
            }
            set
            {
                tool.Active = value;
            }
        }

        public Action<ChartMouseEvent, MouseEventArgs, Cursor> OnMouseEvent
        {
            get
            {
                return tool.OnMouseEvent;
            }
            set
            {
                tool.OnMouseEvent = value;
            }
        }

        public void Invalidate()
        {
            tool.Invalidate();
        }

        protected Action<EventArgs> OnAfterDraw
        {
            get
            {
                return tool.OnAfterDraw;
            }
            set
            {
                tool.OnAfterDraw = value;
            }
        }
    }
}