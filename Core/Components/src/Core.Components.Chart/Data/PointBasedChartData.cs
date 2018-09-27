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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Core.Components.Chart.Data
{
    /// <summary>
    /// Base class for <see cref="ChartData"/> that is based on an array of points in 2D space.
    /// </summary>
    public abstract class PointBasedChartData : ChartData
    {
        private IEnumerable<Point2D> points;

        /// <summary>
        /// Creates a new instance of <see cref="PointBasedChartData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="PointBasedChartData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        protected PointBasedChartData(string name) : base(name)
        {
            points = new Point2D[0];
        }

        public override bool HasData
        {
            get
            {
                return Points.Any();
            }
        }

        /// <summary>
        /// Gets or sets an array of points in 2D space.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public IEnumerable<Point2D> Points
        {
            get
            {
                return points;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), @"The array of points cannot be null.");
                }

                points = value;
            }
        }
    }
}