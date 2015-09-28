using System.ComponentModel;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public delegate void SelectionChangedEventHandler(object sender, PointEventArgs e);

    public class PointEventArgs : CancelEventArgs
    {
        private IChartSeries series;
        private int index;
        private double x;
        private double y;

        public PointEventArgs(IChartSeries series, int index, double x, double y)
        {
            this.series = series;
            this.index = index;
            this.x = x;
            this.y = y;
        }

        public IChartSeries Series
        {
            get { return series; }
        }

        public int Index
        {
            get { return index; }
        }

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }
    }

    public class HoverPointEventArgs : PointEventArgs
    {
        public HoverPointEventArgs(IChartSeries series, int index, double x, double y, bool entering) : base(series, index, x, y)
        {
        }

        public bool Entering { get; set; }
    }
        
}