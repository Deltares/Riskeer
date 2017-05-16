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
using System.Drawing.Drawing2D;

namespace Core.Components.Charting.Styles
{
    /// <summary>
    /// This class represents styling of a line on a chart.
    /// </summary>
    public class ChartLineStyle
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartLineStyle"/>.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width of the line.</param>
        /// <param name="style">The <see cref="DashStyle"/> of the line.</param>
        public ChartLineStyle(Color color, int width, DashStyle style)
        {
            Color = color;
            Width = width;
            Style = style;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ChartLineStyle"/>.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width of the line.</param>
        /// <param name="style">The dash style definition of the line.</param>
        public ChartLineStyle(Color color, int width, double[] style)
        {
            Color = color;
            Width = width;
            Dashes = style;
        }

        /// <summary>
        /// Gets the line color.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Gets the line width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the line style.
        /// </summary>
        public DashStyle Style { get; private set; }

        /// <summary>
        /// Gets the line style.
        /// Overrides <see cref="Style"/>.
        /// </summary>
        public double[] Dashes { get; private set; }
    }
}