using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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
    public sealed class BaseChart : Control
    {
        private readonly SeriesFactory seriesFactory = new SeriesFactory();
        private readonly IDictionary<ChartData, Series> series = new Dictionary<ChartData, Series>(new ReferenceEqualityComparer<ChartData>());
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
        public ICollection<ChartData> Data
        {
            get
            {
                return series.Keys;
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

        /// <summary>
        /// Sets the new data. When <paramref name="data"/> is <c>null</c> the <see cref="BaseChart"/> is
        /// cleared.
        /// </summary>
        /// <param name="data">The <see cref="ICollection{T}"/> of <see cref="ChartData"/> to set.</param>
        private void SetData(ICollection<ChartData> data)
        {
            series.Clear();

            if (data != null)
            {
                foreach (var serie in data)
                {
                    AddSeries(serie);
                }
            }

            UpdateTree();
        }

        /// <summary>
        /// Add <see cref="ChartData"/> to the <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="data">The data to add to the <see cref="BaseChart"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="data"/> is of a non-supported <see cref="ChartData"/>
        /// type.</exception>
        private void AddSeries(ChartData data)
        {
            series.Add(data, seriesFactory.Create(data));
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
            foreach (var data in series.Values)
            {
                view.Model.Series.Add(data);
            }
        }
    }
}