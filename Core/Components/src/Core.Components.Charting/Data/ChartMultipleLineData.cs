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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Styles;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data in 2D space which forms multiple lines.
    /// </summary>
    public class ChartMultipleLineData : ChartData
    {
        private IEnumerable<Point2D[]> lines;

        /// <summary>
        /// Creates a new instance of <see cref="ChartMultipleLineData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartMultipleLineData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartMultipleLineData(string name) : base(name)
        {
            Lines = new List<Point2D[]>();
        }

        /// <summary>
        /// Gets or sets the lines that are described by the <see cref="ChartMultipleLineData"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> contains
        /// any <c>null</c> value.</exception>
        public IEnumerable<Point2D[]> Lines
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
        /// Gets or sets the style of the <see cref="ChartMultipleLineData"/>.
        /// </summary>
        public ChartLineStyle Style { get; set; }

        public override bool HasData
        {
            get
            {
                return lines.Any(l => l.Any());
            }
        }
    }
}
