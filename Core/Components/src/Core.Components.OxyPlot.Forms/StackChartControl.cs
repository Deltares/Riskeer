// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.OxyPlot.DataSeries.Stack;
using Core.Components.Stack.Data;
using Core.Components.Stack.Forms;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with configured representation of axes.
    /// </summary>
    public class StackChartControl : Control, IStackChartControl
    {
        private readonly CategoryPlotView plotView;
        private readonly Observer stackChartDataObserver;

        private StackChartData data;

        /// <summary>
        /// Creates a new <see cref="StackChartControl"/>.
        /// </summary>
        public StackChartControl()
        {
            plotView = new CategoryPlotView
            {
                BackColor = Color.White,
                Model =
                {
                    IsLegendVisible = true
                }
            };

            Controls.Add(plotView);

            stackChartDataObserver = new Observer(HandleStackChartDataChange);
        }

        public string VerticalAxisTitle
        {
            get
            {
                return plotView.VerticalAxisTitle;
            }
            set
            {
                plotView.VerticalAxisTitle = value;
            }
        }

        public StackChartData Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != null)
                {
                    ClearPlot();
                }

                data = value;
                stackChartDataObserver.Observable = data;

                if (data != null)
                {
                    DrawPlot();
                }
            }
        }

        public string ChartTitle
        {
            get
            {
                return plotView.ModelTitle;
            }
            set
            {
                plotView.ModelTitle = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            plotView.Dispose();
            stackChartDataObserver.Dispose();

            base.Dispose(disposing);
        }

        private void HandleStackChartDataChange()
        {
            ClearPlot();
            DrawPlot();
        }

        private void DrawPlot()
        {
            DrawLabels();
            DrawColumns();

            plotView.InvalidatePlot(true);
        }

        private void ClearPlot()
        {
            plotView.ClearLabels();
            plotView.Model.Series.Clear();
        }

        private void DrawLabels()
        {
            plotView.AddLabels(data.Columns);
        }

        private void DrawColumns()
        {
            IEnumerable<RowChartDataSeries> series = RowChartDataSeriesFactory.Create(data);

            foreach (RowChartDataSeries rowChartDataSeries in series)
            {
                plotView.Model.Series.Add(rowChartDataSeries);
            }
        }
    }
}