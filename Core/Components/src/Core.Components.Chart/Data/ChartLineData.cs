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
    /// This class represents data in 2D space which is visible as a line.
    /// </summary>
    public class ChartLineData : PointBasedChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartLineData"/> with default styling.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartLineData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartLineData(string name) : this(name, CreateDefaultChartLineStyle()) {}

        /// <summary>
        /// Creates a new instance of <see cref="ChartLineData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartLineData"/>.</param>
        /// <param name="style">The style of the data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="style"/>
        /// is <c>null</c>.</exception>
        public ChartLineData(string name, ChartLineStyle style) : base(name)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
        }

        /// <summary>
        /// Gets the style of the line.
        /// </summary>
        public ChartLineStyle Style { get; }

        private static ChartLineStyle CreateDefaultChartLineStyle()
        {
            return new ChartLineStyle
            {
                Color = Color.Black,
                Width = 2,
                DashStyle = ChartLineDashStyle.Solid
            };
        }
    }
}