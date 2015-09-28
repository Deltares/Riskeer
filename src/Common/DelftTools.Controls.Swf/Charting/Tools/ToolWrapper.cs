using System;
using System.Windows.Forms;
using Steema.TeeChart;
using Steema.TeeChart.Tools;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    internal class ToolWrapper : Tool
    {
        public ToolWrapper(IChart chart) : base(((Chart)chart).chart)
        {
        }

        protected override void ChartEvent(EventArgs e)
        {
            base.ChartEvent(e);

            if (e is AfterDrawEventArgs && OnAfterDraw != null)
            {
                OnAfterDraw(e);
            }
        }

        protected override void MouseEvent(MouseEventKinds kind, MouseEventArgs e, ref Cursor c)
        {
            if (OnMouseEvent != null)
            {
                OnMouseEvent((ChartMouseEvent) ((int) kind), e, c);
            }
        }

        public Action<EventArgs> OnAfterDraw;

        public Action<ChartMouseEvent, MouseEventArgs, Cursor> OnMouseEvent;
    }
}