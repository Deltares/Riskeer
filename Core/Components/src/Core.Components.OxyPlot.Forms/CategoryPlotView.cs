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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// This class describes a plot view with a category axis.
    /// </summary>
    internal sealed class CategoryPlotView : PlotView
    {
        private readonly CategoryAxis categoryAxis;
        private readonly LinearAxis linearAxis;

        /// <summary>
        /// Creates a new <see cref="CategoryPlotView"/>.
        /// </summary>
        public CategoryPlotView()
        {
            Dock = DockStyle.Fill;

            categoryAxis = new CategoryAxis
            {
                MinorStep = 1,
                Angle = 90,
                AbsoluteMinimum = -0.5,
                IsPanEnabled = false,
                IsZoomEnabled = false
            };

            linearAxis = new LinearAxis
            {
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1,
                MaximumPadding = 0.06,
                MinimumPadding = 0,
                IsPanEnabled = false,
                IsZoomEnabled = false
            };

            Model = new PlotModel
            {
                Axes =
                {
                    categoryAxis,
                    linearAxis
                },
                LegendBorderThickness = 0,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopCenter
            };
        }

        /// <summary>
        /// Gets or sets the title of the plot view.
        /// </summary>
        public string ModelTitle
        {
            get
            {
                return Model.Title;
            }
            set
            {
                Model.Title = value;
                InvalidatePlot(false);
            }
        }

        /// <summary>
        /// Gets or sets the title of the vertical axis of the plot view.
        /// </summary>
        public string VerticalAxisTitle
        {
            get
            {
                return linearAxis.Title;
            }
            set
            {
                linearAxis.Title = value;
                InvalidatePlot(false);
            }
        }

        /// <summary>
        /// Adds labels to the <see cref="CategoryAxis"/>
        /// </summary>
        /// <param name="labels">The labels to add.</param>
        /// <exception cref="ArgumentNullException">Thrown
        /// when <paramref name="labels"/> is <c>null</c>.</exception>
        public void AddLabels(IEnumerable<string> labels)
        {
            if (labels == null)
            {
                throw new ArgumentNullException(nameof(labels));
            }

            categoryAxis.Labels.AddRange(labels);

            categoryAxis.AbsoluteMaximum = categoryAxis.Labels.Count > 0
                                               ? categoryAxis.Labels.Count - 0.5
                                               : 0;
        }

        /// <summary>
        /// Clears the labels of the <see cref="CategoryAxis"/>.
        /// </summary>
        public void ClearLabels()
        {
            categoryAxis.Labels.Clear();
        }
    }
}