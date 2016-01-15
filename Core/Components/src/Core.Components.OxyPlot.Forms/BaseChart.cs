using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public sealed class BaseChart : Control, IObservable
    {
        private readonly SeriesFactory seriesFactory = new SeriesFactory();
        private readonly IDictionary<ChartData, Series> series = new Dictionary<ChartData, Series>(new ReferenceEqualityComparer<ChartData>());
        private readonly List<ChartData> order = new List<ChartData>();

        private readonly ICollection<IObserver> observers = new Collection<IObserver>();

        private PlotView view;

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
            Data = new List<ChartData>();
            MinimumSize = new Size(50, 75);
        }

        /// <summary>
        /// Gets or sets the data to show in the <see cref="BaseChart"/>.
        /// </summary>
        /// <remarks>The returned collection is a copy of the previously set data.</remarks>
        public ICollection<ChartData> Data
        {
            get
            {
                return order.ToList();
            }
            set
            {
                SetData(value);
            }
        }

        /// <summary>
        /// Sets the visibility of a series in this <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="serie">The <see cref="ChartData"/> to set the visibility for.</param>
        /// <param name="visibility">A boolean value representing the new visibility of the <paramref name="serie"/>.</param>
        public void SetVisibility(ChartData serie, bool visibility)
        {
            if (serie == null)
            {
                throw new ArgumentNullException("serie", "Cannot set visibility of a null serie.");
            }
            serie.IsVisible = visibility;
            series[serie].IsVisible = visibility;
            view.Invalidate();
        }

        public void SetIndex(ChartData serie, int index)
        {
            if (serie == null)
            {
                throw new ArgumentNullException("serie", "Cannot set visibility of a null serie.");
            }
            if (index < 0 || index >= Data.Count)
            {
                throw new ArgumentNullException("index", string.Format("Cannot set index outside of range [0,{0})", Data.Count));
            }
            var newOrder = order.ToList();
            newOrder.Remove(serie);
            newOrder.Insert(index, serie);
        }

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

        /// <summary>
        /// Sets the new data. When <paramref name="dataCollection"/> is <c>null</c> the <see cref="BaseChart"/> is
        /// cleared.
        /// </summary>
        /// <param name="dataCollection">The <see cref="ICollection{T}"/> of <see cref="ChartData"/> to set.</param>
        private void SetData(ICollection<ChartData> dataCollection)
        {
            series.Clear();
            order.Clear();

            if (dataCollection != null)
            {
                foreach (var data in dataCollection)
                {
                    AddDataAsSeries(data);
                }
            }

            UpdateTree();
        }

        /// <summary>
        /// Add <see cref="ChartData"/> to the <see cref="BaseChart"/> as a <see cref="Series"/>.
        /// </summary>
        /// <param name="data">The data to add to the <see cref="BaseChart"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="data"/> is of a non-supported <see cref="ChartData"/>
        /// type.</exception>
        private void AddDataAsSeries(ChartData data)
        {
            series.Add(data, seriesFactory.Create(data));
            order.Add(data);
        }

        /// <summary>
        /// Initialize the <see cref="PlotView"/> for the <see cref="BaseChart"/>.
        /// </summary>
        private void InitializePlotView()
        {
            view = new PlotView
            {
                Dock = DockStyle.Fill,
                Model = new PlotModel
                {
                    Axes =
                    {
                        CreateAxis(Resources.BaseChart_XAxisTitle, AxisPosition.Bottom),
                        CreateAxis(Resources.BaseChart_YAxisTitle, AxisPosition.Left)
                    }
                }
            };
            Controls.Add(view);
        }

        /// <summary>
        /// Creates an axis with default style set.
        /// </summary>
        /// <param name="title">The title of the <see cref="LinearAxis"/>.</param>
        /// <param name="position">The <see cref="AxisPosition"/> of the <see cref="LinearAxis"/>.</param>
        /// <returns></returns>
        private static LinearAxis CreateAxis(string title, AxisPosition position)
        {
            return new LinearAxis
            {
                Title = title,
                Position = position,
                TickStyle = TickStyle.None,
                ExtraGridlines = new[]
                {
                    0.0
                },
                ExtraGridlineThickness = 1,
                Layer = AxisLayer.AboveSeries,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
        }

        /// <summary>
        /// Updates the tree with the currently known <see cref="Data"/>.
        /// </summary>
        private void UpdateTree()
        {
            foreach (var data in order)
            {
                view.Model.Series.Add(series[data]);
            }
        }
    }
}