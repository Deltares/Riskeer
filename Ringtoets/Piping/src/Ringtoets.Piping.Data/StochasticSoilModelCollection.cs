// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// A collection to store the <see cref="StochasticSoilModel"/> and the source path
    /// they were imported from.
    /// </summary>
    public class StochasticSoilModelCollection : IObservable, IEnumerable<StochasticSoilModel>
    {
        private readonly List<StochasticSoilModel> stochasticSoilModels = new List<StochasticSoilModel>();
        private readonly ICollection<IObserver> observers = new Collection<IObserver>();

        /// <summary>
        /// Gets or sets the element at index <paramref name="i"/> in the collection.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The element at index <paramref name="i"/> in the collection.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="i"/> is not 
        /// between [0, <see cref="Count"/>)</exception>
        public StochasticSoilModel this[int i]
        {
            get
            {
                return stochasticSoilModels[i];
            }
            set
            {
                stochasticSoilModels[i] = value;
            }
        }

        /// <summary>
        /// Removes the first occurence of <paramref name="model"/> in the collection.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to be removed.</param>
        /// <returns><c>True</c> if the <paramref name="model"/> was successfully removed from the collection;
        /// <c>False</c> if otherwise or if the <paramref name="model"/> was not found in the collection. </returns>
        public bool Remove(StochasticSoilModel model)
        {
            return stochasticSoilModels.Remove(model);
        }

        /// <summary>
        /// Clears the imported items in the collection and 
        /// the <see cref="SourcePath"/>.
        /// </summary>
        public void Clear()
        {
            SourcePath = null;
            stochasticSoilModels.Clear();
        }

        /// <summary>
        /// Adds a <see cref="StochasticSoilModel"/> item to the end of the collection.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to be added.</param>
        public void Add(StochasticSoilModel model)
        {
            stochasticSoilModels.Add(model);
        }

        /// <summary>
        /// Gets the amount of items stored in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return stochasticSoilModels.Count;
            }
        }

        /// <summary>
        /// Gets or sets the last known file path from which the <see cref="StochasticSoilModel"/>
        /// elements were imported.
        /// </summary>
        public string SourcePath { get; set; }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            // Iterate through a copy of the list of observers; an update of one observer might result in detaching another observer (which will result in a "list modified" exception over here otherwise)
            foreach (var observer in observers.ToArray())
            {
                // Ensure the observer is still part of the original list of observers
                if (!observers.Contains(observer))
                {
                    continue;
                }

                observer.UpdateObserver();
            }
        }

        public IEnumerator<StochasticSoilModel> GetEnumerator()
        {
            return stochasticSoilModels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}