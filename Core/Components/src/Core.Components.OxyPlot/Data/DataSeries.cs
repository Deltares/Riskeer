using System;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Data
{
    public abstract class DataSeries : ISeries
    {
        public Series Series { get; protected set; }

        public bool IsVisible
        {
            get
            {
                return Series.IsVisible;
            }
            set
            {
                Series.IsVisible = value;
            }
        }

        protected static DataPoint TupleToDataPoint(object obj)
        {
            var point = (Tuple<double, double>)obj;
            return new DataPoint(point.Item1, point.Item2);
        }
    }
}