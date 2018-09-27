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
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial.Layer
{
    /// <summary>
    /// A <see cref="MapPolygonLayer"/> based on and updated according to the wrapped <see cref="MapPolygonData"/>.
    /// </summary>
    public class MapPolygonDataLayer : MapPolygonLayer, IFeatureBasedMapDataLayer
    {
        private readonly MapPolygonData mapPolygonData;
        private readonly MapPolygonDataConverter converter = new MapPolygonDataConverter();

        private IEnumerable<MapFeature> drawnFeatures;

        /// <summary>
        /// Creates a new instance of <see cref="MapPolygonDataLayer"/>.
        /// </summary>
        /// <param name="mapPolygonData">The <see cref="MapPolygonData"/> which the map polygon data layer is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapPolygonData"/> is <c>null</c>.</exception>
        public MapPolygonDataLayer(MapPolygonData mapPolygonData)
        {
            if (mapPolygonData == null)
            {
                throw new ArgumentNullException(nameof(mapPolygonData));
            }

            this.mapPolygonData = mapPolygonData;
            Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(mapPolygonData.Features, drawnFeatures))
            {
                converter.ConvertLayerFeatures(mapPolygonData, this);

                drawnFeatures = mapPolygonData.Features;
            }

            converter.ConvertLayerProperties(mapPolygonData, this);
        }
    }
}