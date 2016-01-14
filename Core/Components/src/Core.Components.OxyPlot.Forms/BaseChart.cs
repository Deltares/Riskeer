using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
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
        private readonly IDictionary<IChartData, Series> series = new Dictionary<IChartData, Series>(new ReferenceEqualityComparer());
        private PlotView view;

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
            Data = new List<IChartData>();
            MinimumSize = new Size(50, 75);
        }

        public ICollection<IChartData> Data
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

        private void SetData(ICollection<IChartData> value)
        {
            series.Clear();

            if (value != null)
            {
                foreach (var serie in value)
                {
                    AddSeries(serie);
                }
            }

            UpdateTree();
        }

        /// <summary>
        /// Add <see cref="Core.Components.Charting.Data.IChartData"/> to the <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="data">The data to add to the <see cref="BaseChart"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        private void AddSeries(IChartData data)
        {
            series.Add(data, seriesFactory.Create(data));
        }

        /// <summary>
        /// Sets the visibility of a series in this <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="serie">The <see cref="Core.Components.Charting.Data.IChartData"/> to set the visibility for.</param>
        /// <param name="visibility">A boolean value representing the new visibility of the <paramref name="serie"/>.</param>
        public void SetVisibility(IChartData serie, bool visibility)
        {
            if (serie != null)
            {
                serie.IsVisible = visibility;
                series[serie].IsVisible = visibility;
                view.Invalidate();
            }
            else
            {
                throw new ArgumentException("Visibility set for IChartData which was not of type Series.");
            }
        }

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

        private void UpdateTree()
        {
            foreach (var data in series.Values)
            {
                view.Model.Series.Add(data);
            }
        }
    }

    internal class ReferenceEqualityComparer : IEqualityComparer<IChartData>
    {
        public bool Equals(IChartData x, IChartData y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(IChartData obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}