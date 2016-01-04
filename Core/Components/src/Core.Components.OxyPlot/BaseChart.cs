﻿using Core.Components.OxyPlot.Properties;
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
                TickStyle = TickStyle.None
            };
            yAxis = new LinearAxis
            {
                Title = Resources.BaseChart_YAxisTitle,
                TickStyle = TickStyle.None
            };
            Model.Axes.Add(xAxis);
            Model.Axes.Add(yAxis);
        }

        /// <summary>
        /// Add <see cref="ChartData"/> to the <see cref="BaseChart"/>.
        /// </summary>
        /// <param name="data"></param>
        public void AddData(ChartData data)
        {
            data.AddTo(Model);
        }

        /// <summary>
        /// Remove all the <see cref="ChartData"/> that has been added to the <see cref="BaseChart"/>.
        /// </summary>
        public void ClearData()
        {
            Model.Series.Clear();
        }
    }
}