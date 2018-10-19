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

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Extension methods for <see cref="MapDataCollection"/>.
    /// </summary>
    public static class MapDataCollectionExtensions
    {
        /// <summary>
        /// Gets all the <see cref="FeatureBasedMapData"/> recursively in the given <paramref name="mapDataCollection"/>.
        /// </summary>
        /// <param name="mapDataCollection">The collection to get all <see cref="FeatureBasedMapData"/> from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="FeatureBasedMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapDataCollection"/> is <c>null</c>.</exception>
        public static IEnumerable<FeatureBasedMapData> GetFeatureBasedMapDataRecursively(this MapDataCollection mapDataCollection)
        {
            if (mapDataCollection == null)
            {
                throw new ArgumentNullException(nameof(mapDataCollection));
            }

            var featureBasedMapDataList = new List<FeatureBasedMapData>();

            foreach (MapData mapData in mapDataCollection.Collection)
            {
                var nestedMapDataCollection = mapData as MapDataCollection;
                if (nestedMapDataCollection != null)
                {
                    featureBasedMapDataList.AddRange(GetFeatureBasedMapDataRecursively(nestedMapDataCollection));
                    continue;
                }

                featureBasedMapDataList.Add((FeatureBasedMapData) mapData);
            }

            return featureBasedMapDataList;
        }
    }
}