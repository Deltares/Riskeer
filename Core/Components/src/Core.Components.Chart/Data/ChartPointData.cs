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

using System;
using System.Drawing;
using Core.Components.Chart.Styles;

namespace Core.Components.Chart.Data
{
    /// <summary>
    /// This class represents data in 2D space which is visible as points.
    /// </summary>
    public class ChartPointData : PointBasedChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartPointData"/> with default styling.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartPointData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartPointData(string name) : this(name, CreateDefaultChartPointStyle()) {}

        /// <summary>
        /// Creates a new instance of <see cref="ChartPointData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartPointData"/>.</param>
        /// <param name="style">The style of the data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="style"/>
        /// is <c>null</c>.</exception>
        public ChartPointData(string name, ChartPointStyle style) : base(name)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
        }

        /// <summary>
        /// Gets the style of the points.
        /// </summary>
        public ChartPointStyle Style { get; }

        private static ChartPointStyle CreateDefaultChartPointStyle()
        {
            return new ChartPointStyle
            {
                Color = Color.Black,
                StrokeColor = Color.Black,
                Size = 2,
                StrokeThickness = 1,
                Symbol = ChartPointSymbol.Square
            };
        }
    }
}