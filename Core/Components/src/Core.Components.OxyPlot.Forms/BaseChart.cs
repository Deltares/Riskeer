﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
        private readonly SeriesFactory seriesFactory = new SeriesFactory();

        private ChartData data;

        private LinearPlotView view;
        private DynamicPlotController controller;

        /// <summary>
        /// Creates a new instance of <see cref="BaseChart"/>.
        /// </summary>
        public BaseChart()
        {
            InitializePlotView();
            MinimumSize = new Size(50, 75);
        }

        /// <summary>
        /// Attaches the <see cref="BaseChart"/> to the currently set <see cref="Data"/>, if there is any.
        /// </summary>
        private void AttachToData()
        {
            if (data != null)
            {
                data.Attach(this);
            }
        }

        /// <summary>
        /// Detaches the <see cref="BaseChart"/> to the currently set <see cref="Data"/>, if there is any.
        /// </summary>
        private void DetachFromData()
        {
            if (data != null)
            {
                data.Detach(this);
            }
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

        /// <summary>
        /// Draws series based on the currently set <see cref="Data"/>.
        /// </summary>
        private void DrawSeries()
        {
            view.Model.Series.Clear();
            if (data != null)
            {
                foreach (var series in seriesFactory.Create(data))
                {
                    view.Model.Series.Add(series);
                }
            }
        }

        #region IChart

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

        public void UpdateObserver()
        {
            DrawSeries();
            view.InvalidatePlot(true);
        }

        #endregion
    }
}