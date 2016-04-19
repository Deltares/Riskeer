﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.Gis.Style;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// This class represents data in 2D space which is visible as points.
    /// </summary>
    public class MapPointData : FeatureBasedMapData
    {
        /// <summary>
        /// Create a new instance of <see cref="MapPointData"/>.
        /// </summary>
        /// <param name="features">A <see cref="IEnumerable{T}"/> of <see cref="MapFeature"/> 
        /// which describes a <see cref="IEnumerable{T}"/> of <see cref="MapGeometry"/>.</param>
        /// <param name="name">The name of the <see cref="MapData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when 
        /// <list type="bullet">
        /// <item><paramref name="features"/> is invalid.</item>
        /// <item><paramref name="name"/> is <c>null</c> or only whitespace.</item>
        /// </list>
        /// </exception>
        public MapPointData(IEnumerable<MapFeature> features, string name) : base(features, name) {}

        /// <summary>
        /// The style of the points.
        /// </summary>
        public PointStyle Style { get; set; }

        /// <summary>
        /// Validates the features.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <exception cref="System.ArgumentNullException">When <paramref name="features"/> is null
        /// or any feature contains multiple point-collections.</exception>
        protected override void ValidateFeatures(IEnumerable<MapFeature> features)
        {
            base.ValidateFeatures(features);

            if (HasFeatureWithMultiplePointCollections(features))
            {
                throw new ArgumentException("MapPointData only accept MapFeature instances whose MapGeometries contain a single point-collection.");
            }
        }

        private static bool HasFeatureWithMultiplePointCollections(IEnumerable<MapFeature> pointFeatures)
        {
            return pointFeatures.SelectMany(feature => feature.MapGeometries)
                                .Any(geometry => geometry.PointCollections.Count() != 1);
        }
    }
}