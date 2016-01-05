using System;
using Core.Components.OxyPlot.Data;
using Core.Components.OxyPlot.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot
{
    public class BaseChart : PlotView
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
                ExtraGridlineThickness = 1
            };
            yAxis = new LinearAxis
            {
                Title = Resources.BaseChart_YAxisTitle,
                TickStyle = TickStyle.None,
                ExtraGridlines = new[] { 0.0 },
                ExtraGridlineThickness = 1
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
    }
}