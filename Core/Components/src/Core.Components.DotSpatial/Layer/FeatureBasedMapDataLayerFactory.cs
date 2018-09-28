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
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.Layer
{
    /// <summary>
    /// A factory to create <see cref="IFeatureBasedMapDataLayer"/> based on <see cref="FeatureBasedMapData"/>.
    /// </summary>
    public static class FeatureBasedMapDataLayerFactory
    {
        /// <summary>
        /// Creates a <see cref="IFeatureBasedMapDataLayer"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="FeatureBasedMapData"/> to create a <see cref="IFeatureBasedMapDataLayer"/> from.</param>
        /// <returns>A <see cref="IFeatureBasedMapDataLayer"/> instance.</returns>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="data"/> type is not supported.</exception>
        public static IFeatureBasedMapDataLayer Create(FeatureBasedMapData data)
        {
            var mapPointData = data as MapPointData;
            if (mapPointData != null)
            {
                return new MapPointDataLayer(mapPointData);
            }

            var mapLineData = data as MapLineData;
            if (mapLineData != null)
            {
                return new MapLineDataLayer(mapLineData);
            }

            var mapPolygonData = data as MapPolygonData;
            if (mapPolygonData != null)
            {
                return new MapPolygonDataLayer(mapPolygonData);
            }

            throw new NotSupportedException($"FeatureBasedMapData of type {data.GetType().Name} is not supported.");
        }
    }
}