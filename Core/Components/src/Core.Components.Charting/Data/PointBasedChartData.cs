// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// Base class for <see cref="ChartData"/> which is based on a collection of points.
    /// </summary>
    public abstract class PointBasedChartData : ChartData {

        /// <summary>
        /// Creates a new instance of <see cref="PointData"/>.
        /// </summary>
        /// <param name="points">A <see cref="Collection{T}"/> of <see cref="Tuple{T1,T2}"/> as (X,Y) points.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        protected PointBasedChartData(IEnumerable<Tuple<double, double>> points)
        {
            if (points == null)
            {
                var message = String.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedChartData));
                throw new ArgumentNullException("points", message);
            }
            Points = points.ToArray();
            IsVisible = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChartData"/> is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets the collection of points in 2D space.
        /// </summary>
        public IEnumerable<Tuple<double, double>> Points { get; private set; }
    }
}