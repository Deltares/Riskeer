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

namespace Core.Components.Gis.Data.Removable
{
    /// <summary>
    /// Converts <see cref="FeatureBasedMapData"/> to <see cref="FeatureBasedMapData"/>
    /// implementing <see cref="IRemovable"/>.
    /// </summary>
    public static class RemovableMapDataConverter
    {
        /// <summary>
        /// Converts the given <paramref name="mapData"/> to <see cref="FeatureBasedMapData"/> 
        /// implementing <see cref="IRemovable"/>.
        /// </summary>
        /// <param name="mapData">The map data to convert.</param>
        /// <returns>A new <see cref="FeatureBasedMapData"/> implementing <see cref="IRemovable"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the type <paramref name="mapData"/>
        /// could not be converted.</exception>
        public static FeatureBasedMapData FromFeatureBasedMapData(FeatureBasedMapData mapData)
        {
            var mapPointData = mapData as MapPointData;
            if (mapPointData != null)
            {
                return new RemovableMapPointData(mapPointData.Name, mapPointData.Style)
                {
                    Features = mapPointData.Features
                };
            }

            var mapLineData = mapData as MapLineData;
            if (mapLineData != null)
            {
                return new RemovableMapLineData(mapLineData.Name, mapLineData.Style)
                {
                    Features = mapLineData.Features
                };
            }

            var mapPolygonData = mapData as MapPolygonData;
            if (mapPolygonData != null)
            {
                return new RemovableMapPolygonData(mapPolygonData.Name, mapPolygonData.Style)
                {
                    Features = mapPolygonData.Features
                };
            }

            throw new NotSupportedException($"The given {nameof(mapData)} was not convertible to {typeof(IRemovable).Name} data.");
        }
    }
}