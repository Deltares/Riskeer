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
using Core.Components.Gis.Features;
using Core.Components.Gis.Style;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// This class represents features that form a closed area.
    /// </summary>
    public class MapPolygonData : FeatureBasedMapData
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapPolygonData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapPolygonData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public MapPolygonData(string name) : base(name) {}

        /// <summary>
        /// Gets or sets the style of the polygon.
        /// </summary>
        public PolygonStyle Style { get; set; }

        /// <summary>
        /// This method validates newly set features.
        /// </summary>
        /// <param name="featuresToValidate">The new features to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="featuresToValidate"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any feature in <paramref name="featuresToValidate"/> 
        /// contains no point-collections.</exception>
        /// <seealso cref="Features"/>
        protected override void ValidateFeatures(MapFeature[] featuresToValidate)
        {
            base.ValidateFeatures(featuresToValidate);

            if (HasFeatureWithEmptyPointCollections(featuresToValidate))
            {
                throw new ArgumentException("MapPolygonData only accepts MapFeature instances whose MapGeometries contain at least one single point-collection.");
            }
        }

        private static bool HasFeatureWithEmptyPointCollections(IEnumerable<MapFeature> lineFeatures)
        {
            return lineFeatures.SelectMany(feature => feature.MapGeometries)
                               .Any(geometry => !geometry.PointCollections.Any());
        }
    }
}