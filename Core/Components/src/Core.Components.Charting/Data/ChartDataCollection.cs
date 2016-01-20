using System;
using System.Collections.Generic;
using Core.Components.Charting.Data;

namespace Core.Components.OxyPlot.Collection
{
    public class ChartDataCollection : ChartData
    {
        public ChartDataCollection(IList<ChartData> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "A list collection is required when creating ChartDataCollection.");
            }
            List = list;
        }

        public IList<ChartData> List { get; private set; }
    }
}