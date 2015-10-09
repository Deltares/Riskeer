using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface ICursorLineTool : IChartViewTool
    {
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<EventArgs> Drop;
        event EventHandler<EventArgs> ValueChanged;
        bool ToolTipEnabled { get; set; }
        double XValue { get; set; }
        double YValue { get; set; }

        //Pen
        Color LinePenColor { get; set; }
        float[] LinePenDashPattern { get; set; }
        int LinePenWidth { get; set; }
        DashStyle LinePenStyle { get; set; }
        //event EventHandler<TeeChartMouseEventArgs> CursorToolMouseEvent;
        //event EventHandler<CursorChangeEventArgs> CursorToolChange;
        //void DoMouseEvents(TeeChartMouseEventArgs mouseEventArgs);
        //CursorClicked Clicked(int X, int Y);
        //bool LineClicked(int X, int Y);
        //void AddToChart(TChart chart);
        //// TODO : move to IChart

        //Steema.TeeChart.Chart Chart { get; set; }
    }
}