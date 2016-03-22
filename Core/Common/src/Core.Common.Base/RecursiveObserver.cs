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

namespace Core.Common.Base
{
    /// <summary>
    /// Class that implements <see cref="IObserver"/> in a way that a hierarchy of <see cref="IObservable"/> objects can be observed recursively.
    /// </summary>
    /// <remarks>
    /// The root <see cref="Observable"/> being observed by instances of this class can be dynamically changed.
    /// </remarks>
    /// <typeparam name="T">The type of objects to observe recursively.</typeparam>
    public class RecursiveObserver<T> : IObserver, IDisposable where T : class, IObservable
    {
        private T rootObservable;
        private readonly Action updateObserverAction;
        private readonly Func<T, IEnumerable<T>> getChildObservables;
        private readonly IList<T> observedObjects = new List<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="RecursiveObserver{T}"/> class.
        /// </summary>
        /// <param name="updateObserverAction">The <see cref="UpdateObserver"/> action to perform on notifications coming from one of the items of the hierarchy of observed objects.</param>
        /// <param name="getChildObservables">The method used for recursively obtaining the objects to observe.</param>
        public RecursiveObserver(Action updateObserverAction, Func<T, IEnumerable<T>> getChildObservables)
        {
            this.updateObserverAction = updateObserverAction;
            this.getChildObservables = getChildObservables;
        }

        /// <summary>
        /// Gets or sets the root object to observe.
        /// </summary>
        public T Observable
        {
            get
            {
                return rootObservable;
            }
            set
            {
                rootObservable = value;

                UpdateObservedObjects();
            }
        }

        public void UpdateObserver()
        {
            updateObserverAction();

            // Update the list of observed objects as observables might have been added/removed
            UpdateObservedObjects();
        }

        private void UpdateObservedObjects()
        {
            // Detach from the currently observed objects
            foreach (var observedObject in observedObjects)
            {
                observedObject.Detach(this);
            }

            // Clear the list of observed objects
            observedObjects.Clear();

            // If relevant, start observing objects again
            if (rootObservable != null)
            {
                foreach (var objectToObserve in GetObservablesRecursive(rootObservable))
                {
                    objectToObserve.Attach(this);
                    observedObjects.Add(objectToObserve);
                }
            }
        }

        private IEnumerable<T> GetObservablesRecursive(T observable)
        {
            var observables = new List<T>
            {
                observable
            };

            foreach (var childObservable in getChildObservables(observable))
            {
                observables.AddRange(GetObservablesRecursive(childObservable));
            }

            return observables;
        }

        public void Dispose()
        {
            Observable = null;
        }
    }
}
