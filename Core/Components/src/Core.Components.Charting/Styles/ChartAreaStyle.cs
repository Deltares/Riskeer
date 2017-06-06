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

using System.Drawing;

namespace Core.Components.Charting.Styles
{
    /// <summary>
    /// This class represents styling of a area on a chart.
    /// </summary>
    public class ChartAreaStyle
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartAreaStyle"/>.
        /// </summary>
        /// <param name="fillColor">The fill color of the area.</param>
        /// <param name="strokeColor">The stroke color of the area.</param>
        /// <param name="strokeThickness">The stroke thickness of the area border.</param>
        public ChartAreaStyle(Color fillColor, Color strokeColor, int strokeThickness)
        {
            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = strokeThickness;
        }

        /// <summary>
        /// Gets or sets the area fill color.
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// Gets or sets the area stroke color.
        /// </summary>
        public Color StrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the area border stroke thickness.
        /// </summary>
        public int StrokeThickness { get; set; }
    }
}