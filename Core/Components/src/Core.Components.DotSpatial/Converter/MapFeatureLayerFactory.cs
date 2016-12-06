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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Components.DotSpatial.Layer;
using Core.Components.Gis.Data;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// A factory to create <see cref="IMapFeatureLayer"/> data from <see cref="FeatureBasedMapData"/> which can be used on the map.
    /// </summary>
    public static class MapFeatureLayerFactory
    {
        private static readonly Collection<IFeatureBasedMapDataConverter> converters = new Collection<IFeatureBasedMapDataConverter>
        {
            new MapPointDataConverter(),
            new MapLineDataConverter(),
            new MapPolygonDataConverter()
        };

        public static IFeatureBasedMapDataLayer CreateLayer(FeatureBasedMapData data)
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

            throw new NotSupportedException(string.Format("FeatureBasedMapData of type {0} is not supported.", data.GetType().Name));
        }

        /// <summary>
        /// Creates a <see cref="IMapFeatureLayer"/> from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="FeatureBasedMapData"/> to base the creation of <see cref="IMapFeatureLayer"/> upon.</param>
        /// <returns>A new layer based on <see cref="FeatureBasedMapData"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="data"/> type is not supported.</exception>
        public static IMapFeatureLayer Create(FeatureBasedMapData data)
        {
            var converter = converters.FirstOrDefault(c => c.CanConvertMapData(data));
            if (converter != null)
            {
                return converter.Convert(data);
            }

            throw new NotSupportedException(string.Format("FeatureBasedMapData of type {0} is not supported.", data.GetType().Name));
        }
    }
}