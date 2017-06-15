// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        /// <summary>
        /// Creates a new <see cref="CategoryPlotView"/>.
        /// </summary>
        public CategoryPlotView()
        {
            Dock = DockStyle.Fill;
            Model = new PlotModel
            {
                Axes =
                {
                    new CategoryAxis()
                },
                LegendBorderThickness = 0,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopCenter
            };
        }

        /// <summary>
        /// Sets the title of the plot view.
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
    }
}