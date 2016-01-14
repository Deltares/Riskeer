using System;
using System.Collections.ObjectModel;
using Core.Components.Charting.Data;

namespace Core.Components.Charting.TestUtil
{
    /// <summary>
    /// A class representing a ChartData type which is not in the regular codebase.
    /// </summary>
    public class TestChartData : ChartData {
        public TestChartData() : base(new Collection<Tuple<double, double>>()) {}
    }
}