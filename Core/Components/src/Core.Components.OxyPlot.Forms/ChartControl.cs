// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.Converter;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public sealed class ChartControl : Control, IObserver, IChartControl
    {
        private readonly ChartSeriesFactory seriesFactory = new ChartSeriesFactory();

        private LinearPlotView view;
        private DynamicPlotController controller;

        /// <summary>
        /// Creates a new instance of <see cref="ChartControl"/>.
        /// </summary>
        public ChartControl()
        {
            InitializePlotView();
            MinimumSize = new Size(50, 75);

            Data = new ChartDataCollection("Root");
            Data.Attach(this);

            DrawSeries();
        }

        /// <summary>
        /// Initialize the <see cref="PlotView"/> for the <see cref="ChartControl"/>.
        /// </summary>
        private void InitializePlotView()
        {
            view = new LinearPlotView
            {
                BackColor = Color.White
            };

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
            if (Data != null)
            {
                foreach (var series in seriesFactory.Create(Data))
                {
                    view.Model.Series.Add(series);
                }
            }
            view.InvalidatePlot(true);
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
        public ChartDataCollection Data { get; private set; }

        public string ChartTitle
        {
            get
            {
                return view.ModelTitle;
            }
            set
            {
                view.ModelTitle = value;
            }
        }

        public string BottomAxisTitle
        {
            get
            {
                return view.BottomAxisTitle;
            }
            set
            {
                view.BottomAxisTitle = value;
            }
        }

        public string LeftAxisTitle
        {
            get
            {
                return view.LeftAxisTitle;
            }
            set
            {
                view.LeftAxisTitle = value;
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
        }

        public void ResetChartData()
        {
            Data = null;
        }

        #endregion
    }
}