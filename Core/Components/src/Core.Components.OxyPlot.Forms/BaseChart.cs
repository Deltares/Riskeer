using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Components.OxyPlot.Data;
using Core.Components.OxyPlot.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public sealed class BaseChart : Control
    {
        private PlotView view;

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
            Series = new List<IChartData>();
            MinimumSize = new Size(50, 75);
        }

        public ICollection<IChartData> Series { get; private set; }

        /// <summary>
        /// Add <see cref="IChartData"/> to the <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="data">The data to add to the <see cref="BaseChart"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public void AddData(IChartData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", "Cannot add null data to the chart.");
            }
            Series.Add(data);
            UpdateTree();
        }

        /// <summary>
        /// Remove all the <see cref="IChartData"/> that has been added to the <see cref="BaseChart"/>.
        /// </summary>
        public void ClearData()
        {
            Series.Clear();
            UpdateTree();
        }

        /// <summary>
        /// Sets the visibility of a series in this <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="series">The <see cref="IChartData"/> to set the visibility for.</param>
        /// <param name="visibility">A boolean value representing the new visibility of the <paramref name="series"/>.</param>
        public void SetVisibility(IChartData series, bool visibility)
        {
            if (series != null)
            {
                series.IsVisible = visibility;
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
            view.Model.Series.Clear();
            foreach (var data in Series)
            {
                view.Model.Series.Add(((ISeries) data).Series);
            }
        }
    }
}