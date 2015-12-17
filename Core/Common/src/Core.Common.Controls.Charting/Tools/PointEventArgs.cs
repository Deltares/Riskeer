using System.ComponentModel;

namespace Core.Common.Controls.Charting.Tools
{
    public delegate void SelectionChangedEventHandler(object sender, PointEventArgs e);

    public class PointEventArgs : CancelEventArgs
    {
        public PointEventArgs(IChartSeries series, int index, double x, double y)
        {
            Series = series;
            Index = index;
            X = x;
            Y = y;
        }

        public IChartSeries Series { get; private set; }

        public int Index { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }
    }

    public class HoverPointEventArgs : PointEventArgs
    {
        public HoverPointEventArgs(IChartSeries series, int index, double x, double y, bool entering) : base(series, index, x, y) {}

        public bool Entering { get; set; }
    }
}