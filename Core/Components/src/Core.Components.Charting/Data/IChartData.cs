using System;
using System.Collections.Generic;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This interface describes the chart data.
    /// </summary>
    public interface IChartData {
        bool IsVisible { get; set; }
        IEnumerable<Tuple<double, double>> Points { get; }
    }
}