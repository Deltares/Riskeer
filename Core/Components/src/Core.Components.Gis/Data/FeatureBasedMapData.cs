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
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Base class for <see cref="MapData"/> which is based on a collection of points.
    /// </summary>
    public abstract class FeatureBasedMapData : MapData
    {
        /// <summary>
        /// Create a new instance of <see cref="FeatureBasedMapData"/>.
        /// </summary>
        /// <param name="features">A <see cref="IEnumerable{T}"/> of <see cref="MapFeature"/> which describes a <see cref="IEnumerable{T}"/> of <see cref="MapGeometry"/>.</param>
        /// <param name="name">The name of the <see cref="MapData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="features"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        protected FeatureBasedMapData(IEnumerable<MapFeature> features, string name) : base(name)
        {
            if (features == null)
            {
                var message = String.Format("A feature collection is required when creating a subclass of {0}.", typeof(FeatureBasedMapData));
                throw new ArgumentNullException("features", message);
            }

            Features = features.ToArray();
            IsVisible = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="FeatureBasedMapData"/> is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets the collection of features.
        /// </summary>
        public IEnumerable<MapFeature> Features { get; private set; }
    }
}