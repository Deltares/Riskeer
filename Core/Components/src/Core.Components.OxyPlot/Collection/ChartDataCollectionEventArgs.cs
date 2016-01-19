using System;
using Core.Components.Charting.Data;

namespace Core.Components.Charting.Collection
{
    public class ChartDataCollectionEventArgs : EventArgs
    {
        internal ChartDataCollectionEventArgs(ChartData item)
        {
            this.Data = item;
        }

        public ChartData Data { get; private set; }
    }
}