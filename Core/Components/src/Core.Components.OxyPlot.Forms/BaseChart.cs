using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Converter;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public sealed class BaseChart : Control, IObserver, IChart
    {
        private readonly ICollection<IObserver> observers = new Collection<IObserver>();

        private ChartData data;

        private LinearPlotView view;
        private DynamicPlotController controller;
        private SeriesFactory seriesFactory = new SeriesFactory();

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
            MinimumSize = new Size(50, 75);
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
        public ChartData Data
        {
            get
            {
                return data;
            }
            set
            {
                DetachFromData();
                data = value;
                AttachToData();
                DrawSeries();
            }
        }

        private void AttachToData()
        {
            if (data != null)
            {
                data.Attach(this);
            }
        }

        private void DetachFromData()
        {
            if (data != null)
            {
                data.Detach(this);
            }
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
            DrawSeries();
            view.InvalidatePlot(true);
        }

        private void DrawSeries()
        {
            view.Model.Series.Clear();
            foreach (var series in seriesFactory.Create(data))
            {
                view.Model.Series.Add(series);
            }
        }
    }
}