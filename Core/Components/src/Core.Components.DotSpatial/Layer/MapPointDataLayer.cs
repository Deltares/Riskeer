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
    /// A <see cref="MapPointLayer"/> based on and updated according to the wrapped <see cref="MapPointData"/>.
    /// </summary>
    public class MapPointDataLayer : MapPointLayer, IFeatureBasedMapDataLayer
    {
        private readonly MapPointData mapPointData;
        private readonly MapPointDataConverter converter = new MapPointDataConverter();

        private IEnumerable<MapFeature> drawnFeatures;

        /// <summary>
        /// Creates a new instance of <see cref="MapPointDataLayer"/>.
        /// </summary>
        /// <param name="mapPointData">The <see cref="MapPointData"/> which the map point data layer is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapPointData"/> is <c>null</c>.</exception>
        public MapPointDataLayer(MapPointData mapPointData)
        {
            if (mapPointData == null)
            {
                throw new ArgumentNullException(nameof(mapPointData));
            }

            this.mapPointData = mapPointData;
            Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(mapPointData.Features, drawnFeatures))
            {
                converter.ConvertLayerFeatures(mapPointData, this);

                drawnFeatures = mapPointData.Features;
            }

            converter.ConvertLayerProperties(mapPointData, this);
        }
    }
}