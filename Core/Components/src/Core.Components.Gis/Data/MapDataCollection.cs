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
using System.Collections.Generic;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// This class represents a collection of <see cref="MapData"/>.
    /// </summary>
    public class MapDataCollection : MapData
    {
        private readonly IList<MapData> mapDataList;

        /// <summary>
        /// Creates a new instance of <see cref="MapDataCollection"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapDataCollection"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public MapDataCollection(string name) : base(name)
        {
            mapDataList = new List<MapData>();
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
        /// Adds an element to the collection of <see cref="MapData"/>.
        /// </summary>
        /// <param name="elementToAdd">The <see cref="MapData"/> element to add to the collection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="elementToAdd"/> is <c>null</c>.</exception>
        public void Add(MapData elementToAdd)
        {
            if (elementToAdd == null)
            {
                throw new ArgumentNullException("elementToAdd", @"An element cannot be null when adding it to the collection.");
            }
            mapDataList.Add(elementToAdd);
        }

        /// <summary>
        /// Inserts the given element into the collection of <see cref="MapData"/> on the given position.
        /// </summary>
        /// <param name="position">The position to insert the element on.</param>
        /// <param name="elementToInsert">The <see cref="MapData"/> element to insert.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="elementToInsert"/> is <c>null</c>.</exception>
        public void Insert(int position, MapData elementToInsert)
        {
            if (elementToInsert == null)
            {
                throw new ArgumentNullException("elementToInsert", @"An element cannot be null when adding it to the collection.");
            }
            mapDataList.Insert(position, elementToInsert);
        }

        /// <summary>
        /// Removes the given element from the collection of <see cref="MapData"/>.
        /// </summary>
        /// <param name="elementToRemove">The <see cref="MapData"/> element to remove.</param>
        public void Remove(MapData elementToRemove)
        {
            mapDataList.Remove(elementToRemove);
        }

        /// <summary>
        /// Removes all elements from the collection of <see cref="MapData"/>.
        /// </summary>
        public void Clear()
        {
            mapDataList.Clear();
        }
    }
}