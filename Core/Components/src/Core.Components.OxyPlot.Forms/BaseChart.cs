using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting;
using Core.Components.Charting.Collection;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Collection;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public sealed class BaseChart : Control, IObserver, IChart
    {
        private readonly ICollection<IObserver> observers = new Collection<IObserver>();
        private readonly ChartDataCollection series;

        private LinearPlotView view;
        private DynamicPlotController controller;

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
            series = new ChartDataCollection(view.Model.Series);
            series.OnChartDataRemoved += OnChartDataRemoved;
            series.OnChartDataAdded += OnChartDataAdded;
            MinimumSize = new Size(50, 75);
        }

        private void OnChartDataAdded(object sender, ChartDataCollectionEventArgs chartDataCollectionEventArgs)
        {
            chartDataCollectionEventArgs.Data.Attach(this);
        }

        private void OnChartDataRemoved(object sender, ChartDataCollectionEventArgs chartDataCollectionEventArgs)
        {
            chartDataCollectionEventArgs.Data.Detach(this);
        }

        public bool IsPanningEnabled
        {
            get
            {
                return controller.IsPanningEnabled;
            }
        }

        public bool IsRectangleZoomingEnabled
        {
            get
            {
                return controller.IsRectangleZoomingEnabled;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public ICollection<ChartData> Data
        {
            get
            {
                return series;
            }
        }

        public void SetPosition(ChartData data, int position)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", "Cannot set index of a null serie.");
            }
            if (position < 0 || position >= Data.Count)
            {
                throw new ArgumentException(string.Format("Cannot set index outside of range [0,{0})", Data.Count), "position");
            }
            series.Remove(data);
            series.Insert(position, data);
            view.InvalidatePlot(true);
        }

        public void TogglePanning()
        {
            controller.TogglePanning();
        }

        public void ToggleRectangleZooming()
        {
            controller.ToggleRectangleZooming();
        }

        public void ZoomToAll()
        {
            view.ZoomToAll();
        }

        /// <summary>
        /// Initialize the <see cref="PlotView"/> for the <see cref="BaseChart"/>.
        /// </summary>
        private void InitializePlotView()
        {
            view = new LinearPlotView();
            controller = new DynamicPlotController();
            view.Controller = controller;
            Controls.Add(view);
        }

        #region IObservable

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            // Iterate through a copy of the list of observers; an update of one observer might result in detaching
            // another observer (which will result in a "list modified" exception over here otherwise)
            foreach (var observer in observers.ToArray())
            {
                // Ensure the observer is still part of the original list of observers
                if (!observers.Contains(observer))
                {
                    continue;
                }

                observer.UpdateObserver();
            }
        }

        #endregion

        public void UpdateObserver()
        {
            series.Update();
            Refresh();
        }
    }
}