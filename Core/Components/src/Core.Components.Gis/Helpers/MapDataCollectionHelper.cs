﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Components.Gis.Data;

namespace Core.Components.Gis.Helpers
{
    /// <summary>
    /// Helper class for <see cref="MapDataCollection"/>.
    /// </summary>
    public static class MapDataCollectionHelper
    {
        /// <summary>
        /// Gets the visibility states of the child <see cref="MapDataCollection"/> of <paramref name="mapDataCollection"/>.
        /// </summary>
        /// <param name="mapDataCollection">The collection to get the child states from.</param>
        /// <returns>A dictionary with the child map data collection and visibility states.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapDataCollection"/> is <c>null</c>.</exception>
        public static Dictionary<MapDataCollection, MapDataCollectionVisibility> GetNestedCollectionVisibilityStates(MapDataCollection mapDataCollection)
        {
            if (mapDataCollection == null)
            {
                throw new ArgumentNullException(nameof(mapDataCollection));
            }

            return GetMapDataCollectionRecursively(mapDataCollection).ToDictionary(child => child, child => child.GetVisibility());
        }

        private static IEnumerable<MapDataCollection> GetMapDataCollectionRecursively(MapDataCollection mapDataCollection)
        {
            var mapDataCollectionList = new List<MapDataCollection>();

            foreach (MapData mapData in mapDataCollection.Collection)
            {
                var nestedMapDataCollection = mapData as MapDataCollection;
                if (nestedMapDataCollection != null)
                {
                    mapDataCollectionList.AddRange(GetMapDataCollectionRecursively(nestedMapDataCollection));
                    mapDataCollectionList.Add(nestedMapDataCollection);
                }
            }

            return mapDataCollectionList;
        }
    }
}