// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// This class represents a collection of <see cref="MapData"/>.
    /// </summary>
    public class MapDataCollection : MapData
    {
        private readonly List<MapData> mapDataList = new List<MapData>();

        /// <summary>
        /// Creates a new instance of <see cref="MapDataCollection"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapDataCollection"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public MapDataCollection(string name) : base(name) {}

        public override bool IsVisible
        {
            get
            {
                return GetVisibility() != MapDataCollectionVisibility.NotVisible;
            }
            set
            {
                foreach (MapData mapData in mapDataList)
                {
                    mapData.IsVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="MapData"/> of the <see cref="MapDataCollection"/>.
        /// </summary>
        public IEnumerable<MapData> Collection
        {
            get
            {
                return mapDataList;
            }
        }

        /// <summary>
        /// Gets the visibility of the map data collection.
        /// </summary>
        /// <returns>A <see cref="MapDataCollectionVisibility"/>.</returns>
        public MapDataCollectionVisibility GetVisibility()
        {
            IEnumerable<MapData> collectionsToIgnore = mapDataList.Where(md =>
            {
                var collection = md as MapDataCollection;
                return collection != null && !collection.Collection.Any();
            });

            MapData[] children = mapDataList.Except(collectionsToIgnore).ToArray();
            MapDataCollection[] nestedCollections = children.OfType<MapDataCollection>()
                                                            .ToArray();

            if (nestedCollections.Any(c => c.GetVisibility() == MapDataCollectionVisibility.Mixed))
            {
                return MapDataCollectionVisibility.Mixed;
            }

            if (children.All(md => !md.IsVisible))
            {
                return MapDataCollectionVisibility.NotVisible;
            }

            if (children.All(md => md.IsVisible))
            {
                return MapDataCollectionVisibility.Visible;
            }

            return MapDataCollectionVisibility.Mixed;
        }

        /// <summary>
        /// Adds an item to the collection of <see cref="MapData"/>.
        /// </summary>
        /// <param name="item">The <see cref="MapData"/> item to add to the collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <c>null</c>.</exception>
        public void Add(MapData item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), @"An item cannot be null when adding it to the collection.");
            }

            mapDataList.Add(item);
        }

        /// <summary>
        /// Inserts the given item into the collection of <see cref="MapData"/> on the given index.
        /// </summary>
        /// <param name="index">The position to insert the item on.</param>
        /// <param name="item">The <see cref="MapData"/> item to insert.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than the size of <see cref="Collection"/>.</exception>
        public void Insert(int index, MapData item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), @"An item cannot be null when adding it to the collection.");
            }

            mapDataList.Insert(index, item);
        }

        /// <summary>
        /// Removes the given item from the collection of <see cref="MapData"/>.
        /// </summary>
        /// <param name="item">The <see cref="MapData"/> item to remove.</param>
        public void Remove(MapData item)
        {
            mapDataList.Remove(item);
        }

        /// <summary>
        /// Removes all items from the collection of <see cref="MapData"/>.
        /// </summary>
        public void Clear()
        {
            mapDataList.Clear();
        }
    }
}