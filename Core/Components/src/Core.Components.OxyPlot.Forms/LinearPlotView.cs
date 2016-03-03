﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using System.Windows.Forms;
using Core.Components.OxyPlot.Forms.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with two linear axes.
    /// </summary>
    internal sealed class LinearPlotView : PlotView
    {
        /// <summary>
        /// Creates a new instance of <see cref="LinearPlotView"/>.
        /// </summary>
        public LinearPlotView()
        {
            Dock = DockStyle.Fill;
            Model = new PlotModel
            {
                Axes =
                {
                    CreateAxis(Resources.ChartControl_XAxisTitle, AxisPosition.Bottom),
                    CreateAxis(Resources.ChartControl_YAxisTitle, AxisPosition.Left)
                }
            };

            Invalidated += OnInvalidated;
        }

        private void OnInvalidated(object sender, EventArgs e)
        {
            FixateZoom();
        }

        /// <summary>
        /// Performs a 'fake' zoom, so that the view is not updated when series are hidden or shown.
        /// </summary>
        private void FixateZoom()
        {
            ActualModel.ZoomAllAxes(1.0);
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
                MinorGridlineStyle = LineStyle.Dot,
                MinimumRange = 0.1
            };
        }

        /// <summary>
        /// Zooms to a level so that everything is in view.
        /// </summary>
        public void ZoomToAll()
        {
            ActualModel.ResetAllAxes();
            InvalidatePlot(false);
        }
    }
}