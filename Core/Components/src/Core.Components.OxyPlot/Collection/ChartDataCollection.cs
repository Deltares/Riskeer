using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Components.Charting.Collection;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Converter;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Collection
{
    public class ChartDataCollection : IList<ChartData>
    {
        private readonly IList<ChartData> list = new List<ChartData>();
        private readonly SeriesFactory seriesFactory = new SeriesFactory();
        private readonly ElementCollection<Series> series;

        public EventHandler<ChartDataCollectionEventArgs> OnChartDataAdded;
        public EventHandler<ChartDataCollectionEventArgs> OnChartDataRemoved;

        internal ChartDataCollection(ElementCollection<Series> associatedSeries)
        {
            series = associatedSeries;
        }

        public ChartData this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return list.IsReadOnly;
            }
        }

        public void Update()
        {
            for (var i = 0; i < list.Count; i++)
            {
                series[i].IsVisible = list[i].IsVisible;
            }
        }

        public IEnumerator<ChartData> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ChartData item)
        {
            Insert(Count, item);
        }

        public void Clear()
        {
            var removedItems = list.ToList();
            list.Clear();
            series.Clear();
            foreach (var item in removedItems)
            {
                TriggerOnChartDataRemoved(item);
            }
        }

        public bool Contains(ChartData item)
        {
            return list.Contains(item);
        }

        public void CopyTo(ChartData[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ChartData item)
        {
            var index = IndexOf(item);
            if (list.Remove(item))
            {
                series.RemoveAt(index);
                TriggerOnChartDataRemoved(item);
                return true;
            }
            return false;
        }

        public int IndexOf(ChartData item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, ChartData item)
        {
            list.Insert(index, item);
            series.Insert(index, seriesFactory.Create(item));
            TriggerOnDataAdded(item);
        }

        public void RemoveAt(int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            series.RemoveAt(index);
            TriggerOnChartDataRemoved(item);
        }

        private void TriggerOnChartDataRemoved(ChartData item)
        {
            if (OnChartDataRemoved != null)
            {
                OnChartDataRemoved(this, new ChartDataCollectionEventArgs(item));
            }
        }

        private void TriggerOnDataAdded(ChartData item)
        {
            if (OnChartDataAdded != null)
            {
                OnChartDataAdded(this, new ChartDataCollectionEventArgs(item));
            }
        }
    }
}