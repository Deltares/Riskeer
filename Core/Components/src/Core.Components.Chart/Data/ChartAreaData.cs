// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    /// This class represents data in 2D space which forms a closed area.
    /// </summary>
    public class ChartAreaData : PointBasedChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartAreaData"/> with default styling.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartAreaData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartAreaData(string name) : this(name, CreateDefaultChartAreaStyle()) {}

        /// <summary>
        /// Creates a new instance of <see cref="ChartAreaData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartAreaData"/>.</param>
        /// <param name="style">The style of the data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="style"/>
        /// is <c>null</c>.</exception>
        public ChartAreaData(string name, ChartAreaStyle style) : base(name)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
        }

        /// <summary>
        /// Gets the style of the chart area.
        /// </summary>
        public ChartAreaStyle Style { get; }

        private static ChartAreaStyle CreateDefaultChartAreaStyle()
        {
            return new ChartAreaStyle
            {
                FillColor = Color.Gray,
                StrokeColor = Color.Black,
                StrokeThickness = 2
            };
        }
    }
}