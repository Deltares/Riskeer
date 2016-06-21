﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents a collection of <see cref="ChartData"/>.
    /// </summary>
    public class ChartDataCollection : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartDataCollection"/>.
        /// </summary>
        /// <param name="list">A <see cref="IList{T}"/> of <see cref="ChartData"/>.</param>
        /// <param name="name">The name of the <see cref="ChartDataCollection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is  <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartDataCollection(IList<ChartData> list, string name) : base(name)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "A list collection is required when creating ChartDataCollection.");
            }
            List = list;
        }

        /// <summary>
        /// Gets the list of <see cref="ChartData"/> of the <see cref="ChartDataCollection"/>.
        /// </summary>
        public IList<ChartData> List { get; private set; }

        /// <summary>
        /// Adds an element to the list of <see cref="ChartData"/>.
        /// </summary>
        /// <param name="elementToAdd">The <see cref="ChartData"/> element to add to the list.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="elementToAdd"/> is <c>null</c>.</exception>
        public void Add(ChartData elementToAdd)
        {
            if (elementToAdd == null)
            {
                throw new ArgumentNullException("elementToAdd", "An element cannot be null when adding it to the collection.");
            }
            List.Add(elementToAdd);
        }

        /// <summary>
        /// Replaces an element in the list of <see cref="ChartData"/>.
        /// </summary>
        /// <param name="oldElement">The <see cref="ChartData"/> element to replace.</param>
        /// <param name="newElement">The <see cref="ChartData"/> element to replace with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldElement"/> or 
        /// <paramref name="newElement"/> is <c>null</c>.</exception>
        public void Replace(ChartData oldElement, ChartData newElement)
        {
            if (newElement == null)
            {
                throw new ArgumentNullException("newElement", "An element cannot be replaced with null. Use Remove instead.");
            }

            if (oldElement == null)
            {
                throw new ArgumentNullException("oldElement", "A null element cannot be replaced. Use Add instead.");
            }

            for (var i = 0; i < List.Count; i++)
            {
                if (List[i].Equals(oldElement))
                {
                    List[i] = newElement;
                }
            }
        }

        /// <summary>
        /// Removes the given element from the list of <see cref="ChartData"/>.
        /// </summary>
        /// <param name="elementToRemove">The <see cref="ChartData"/> element to remove.</param>
        public void Remove(ChartData elementToRemove)
        {
            List.Remove(elementToRemove);
        }
    }
}