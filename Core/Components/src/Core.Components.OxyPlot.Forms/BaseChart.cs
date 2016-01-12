using System;
using Core.Components.OxyPlot.Data;
using Core.Components.OxyPlot.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms
{
    public sealed class BaseChart : PlotView
    {
        private LinearAxis xAxis;
        private LinearAxis yAxis;
        
        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            Model = new PlotModel();
            InitializeAxes();
            InitializeDefaultStyle();

            MinimumSize = new System.Drawing.Size(50, 75);
        }

        /// <summary>
        /// Sets the default look and feel of the <see cref="BaseChart"/>
        /// </summary>
        private void InitializeDefaultStyle()
        {
            xAxis.MajorGridlineStyle = LineStyle.Solid;
            xAxis.MinorGridlineStyle = LineStyle.Dot;
            yAxis.MajorGridlineStyle = LineStyle.Solid;
            yAxis.MinorGridlineStyle = LineStyle.Dot;
        }

        /// <summary>
        /// Sets up default axes representations.
        /// </summary>
        private void InitializeAxes()
        {
            xAxis = new LinearAxis
            {
                Title = Resources.BaseChart_XAxisTitle,
                Position = AxisPosition.Bottom,
                TickStyle = TickStyle.None,
                ExtraGridlines = new[] { 0.0 },
                ExtraGridlineThickness = 1,
                Layer = AxisLayer.AboveSeries
            };
            yAxis = new LinearAxis
            {
                Title = Resources.BaseChart_YAxisTitle,
                TickStyle = TickStyle.None,
                ExtraGridlines = new[] { 0.0 },
                ExtraGridlineThickness = 1,
                Layer = AxisLayer.AboveSeries
            };
            Model.Axes.Add(xAxis);
            Model.Axes.Add(yAxis);
        }

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
            data.AddTo(Model);
        }

        /// <summary>
        /// Remove all the <see cref="LineData"/> that has been added to the <see cref="BaseChart"/>.
        /// </summary>
        public void ClearData()
        {
            Model.Series.Clear();
        }

        /// <summary>
        /// Sets the visibility of a series in this <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="series">The <see cref="IChartData"/> to set the visibility for.</param>
        /// <param name="visibility">A boolean value representing the new visibility of the <paramref name="series"/>.</param>
        public void SetVisibility(IChartData series, bool visibility)
        {
            var chartData = series as Series;
            if (chartData != null)
            {
                chartData.IsVisible = visibility;
                Invalidate();
            }
            else
            {
                throw new ArgumentException("Visibility set for IChartData which was not of type Series.");
            }
        }
    }
}