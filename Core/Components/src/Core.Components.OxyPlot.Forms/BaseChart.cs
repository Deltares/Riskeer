using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
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
        private readonly List<Tuple<ChartData, Series>> series = new List<Tuple<ChartData, Series>>();
        private readonly ICollection<IObserver> observers = new Collection<IObserver>();

        private PlotView view;

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
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
                return series.Select(t => t.Item1).ToList();
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
        /// <param name="visibility">A boolean value representing the new visibility.</param>
        public void SetVisibility(ChartData serie, bool visibility)
        {
            if (serie == null)
            {
                throw new ArgumentNullException("serie", "Cannot set visibility of a null serie.");
            }
            serie.IsVisible = visibility;
            series.First(t => ReferenceEquals(t.Item1, serie)).Item2.IsVisible = visibility;
            view.Invalidate();
        }

        /// <summary>
        /// Sets the position of the <see cref="ChartData"/> amongst the other data of the <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to change the position for.</param>
        /// <param name="position">The new position.</param>
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
            var tuple = series.First(s => ReferenceEquals(s.Item1, data));

            series.Remove(tuple);
            series.Insert(position, tuple);

            view.Model.Series.Remove(tuple.Item2);
            view.Model.Series.Insert(position, tuple.Item2);
        }

        /// <summary>
        /// Sets the new data. When <paramref name="dataCollection"/> is <c>null</c> the <see cref="BaseChart"/> is
        /// cleared.
        /// </summary>
        /// <param name="dataCollection">The <see cref="ICollection{T}"/> of <see cref="ChartData"/> to set.</param>
        private void SetData(ICollection<ChartData> dataCollection)
        {
            series.Clear();

            if (dataCollection != null)
            {
                foreach (var data in dataCollection)
                {
                    AddDataAsSeries(data);
                }
            }

            UpdateTreeData();
        }

        /// <summary>
        /// Add <see cref="ChartData"/> to the <see cref="BaseChart"/> as a <see cref="Series"/>.
        /// </summary>
        /// <param name="data">The data to add to the <see cref="BaseChart"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="data"/> is of a non-supported <see cref="ChartData"/>
        /// type.</exception>
        private void AddDataAsSeries(ChartData data)
        {
            series.Add(new Tuple<ChartData, Series>(data, seriesFactory.Create(data)));
        }

        /// <summary>
        /// Initialize the <see cref="PlotView"/> for the <see cref="BaseChart"/>, disposing the old one if it existed.
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
        /// <returns>A new <see cref="LinearAxis"/> with given <paramref name="title"/> and <paramref name="position"/>.</returns>
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
        private void UpdateTreeData()
        {
            view.Model.Series.Clear();

            foreach (var data in series.Select(s => s.Item2))
            {
                view.Model.Series.Add(data);
            }
        }

        #region IObserver

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
    }
}