using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface ICursorLineTool : IChartViewTool
    {
        bool ToolTipEnabled { get; set; }
        double XValue { get; set; }
        double YValue { get; set; }
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<EventArgs> Drop;
        event EventHandler<EventArgs> ValueChanged;

        //Pen
        Color LinePenColor { get; set; }
        float[] LinePenDashPattern { get; set; }
        int LinePenWidth { get; set; }
        DashStyle LinePenStyle { get; set; }
        
        //Steema.TeeChart.Chart Chart { get; set; }
        //// TODO : move to IChart
        //void AddToChart(TChart chart);
        //bool LineClicked(int X, int Y);
        //CursorClicked Clicked(int X, int Y);
        //void DoMouseEvents(TeeChartMouseEventArgs mouseEventArgs);
        //event EventHandler<CursorChangeEventArgs> CursorToolChange;
        //event EventHandler<TeeChartMouseEventArgs> CursorToolMouseEvent;
    }
}