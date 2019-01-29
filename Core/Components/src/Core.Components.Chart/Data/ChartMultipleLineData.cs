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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Styles;

namespace Core.Components.Chart.Data
{
    /// <summary>
    /// This class represents data in 2D space which forms multiple lines.
    /// </summary>
    public class ChartMultipleLineData : ChartData
    {
        private IEnumerable<IEnumerable<Point2D>> lines;

        /// <summary>
        /// Creates a new instance of <see cref="ChartMultipleLineData"/> with default styling.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartMultipleLineData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartMultipleLineData(string name) : this(name, CreateDefaultChartLineStyle()) {}

        /// <summary>
        /// Creates a new instance of <see cref="ChartMultipleLineData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartMultipleLineData"/>.</param>
        /// <param name="style">The style of the data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="style"/>
        /// is <c>null</c>.</exception>
        public ChartMultipleLineData(string name, ChartLineStyle style) : base(name)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
            Lines = new List<Point2D[]>();
        }

        public override bool HasData
        {
            get
            {
                return lines.Any(l => l.Any());
            }
        }

        /// <summary>
        /// Gets or sets the lines that are described by the <see cref="ChartMultipleLineData"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> contains
        /// any <c>null</c> value.</exception>
        public IEnumerable<IEnumerable<Point2D>> Lines
        {
            get
            {
                return lines;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), @"The collection of point arrays cannot be null.");
                }

                if (value.Any(array => array == null))
                {
                    throw new ArgumentException(@"The collection of point arrays cannot contain null values.", nameof(value));
                }

                lines = value;
            }
        }

        /// <summary>
        /// Gets the style of the <see cref="ChartMultipleLineData"/>.
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