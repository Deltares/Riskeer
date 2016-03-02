// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Base class for <see cref="MapData"/> which is based on a collection of points.
    /// </summary>
    public abstract class PointBasedMapData : MapData
    {
        /// <summary>
        /// Create a new instance of <see cref="PointBasedMapData"/>.
        /// </summary>
        /// <param name="points">A <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> which describe locations in 2D space.</param>
        /// <param name="name">The name of the <see cref="MapData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <list type="bullet">
        /// <item><paramref name="points"/> is <c>null</c>.</item>
        /// <item><paramref name="name"/> is <c>null</c> or only whitespace.</item>
        /// </list>
        /// </exception>
        protected PointBasedMapData(IEnumerable<Point2D> points, string name) : base(name)
        {
            if (points == null)
            {
                var message = String.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedMapData));
                throw new ArgumentNullException("points", message);
            }
            Points = points.ToArray();
            IsVisible = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PointBasedMapData"/> is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets the collection of points in 2D space.
        /// </summary>
        public IEnumerable<Point2D> Points { get; private set; }
    }
}