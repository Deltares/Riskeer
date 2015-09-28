using System;
using DelftTools.Controls.Swf.Charting.Series;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public class SeriesBandTool : Steema.TeeChart.Tools.SeriesBandTool, ISeriesBandTool
    {
        private ChartSeries chartSeries1;
        private ChartSeries chartSeries2;
        public IChartView ChartView { get; set; }

        public bool Enabled { get; set; }

        public event EventHandler<EventArgs> ActiveChanged;

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                base.Active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public new IChartSeries Series
        {
            get { return chartSeries1; }
            set
            {
                chartSeries1 = (ChartSeries) value;
                base.Series = chartSeries1.series;
            }
        }

        public new IChartSeries Series2
        {
            get { return chartSeries2; }
            set
            {
                chartSeries2 = (ChartSeries) value;
                base.Series2 = chartSeries2.series;
            }
        }
    }
}
