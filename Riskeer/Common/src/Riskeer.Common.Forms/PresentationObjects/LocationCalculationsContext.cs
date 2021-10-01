// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Base class for location calculation related presentation objects that should be uniquely identifiable.
    /// </summary>
    /// <typeparam name="TWrappedData">The object type of the wrapped instance.</typeparam>
    /// <typeparam name="TObservable">The object type of the instances that effect the unique identification.</typeparam>
    public abstract class LocationCalculationsContext<TWrappedData, TObservable> : WrappedObjectContextBase<TWrappedData>, IObservable
        where TObservable : class, IObservable
    {
        private readonly Collection<IObserver> observers = new Collection<IObserver>();

        private Observer locationCalculationsListObserver;

        private RecursiveObserver<ObservableList<TObservable>, TObservable> locationCalculationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="LocationCalculationsContext{TWrappedData, TObservable}"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/> is <c>null</c>.</exception>
        protected LocationCalculationsContext(TWrappedData wrappedData)
            : base(wrappedData) {}

        public IEnumerable<IObserver> Observers => observers;

        public virtual void Attach(IObserver observer)
        {
            if (!observers.Any())
            {
                locationCalculationsListObserver = new Observer(NotifyObservers)
                {
                    Observable = LocationCalculationsListToObserve
                };

                locationCalculationsObserver = new RecursiveObserver<ObservableList<TObservable>, TObservable>(NotifyObservers, list => list)
                {
                    Observable = LocationCalculationsListToObserve
                };
            }

            observers.Add(observer);
        }

        public virtual void Detach(IObserver observer)
        {
            observers.Remove(observer);

            if (!observers.Any())
            {
                locationCalculationsListObserver.Dispose();
                locationCalculationsObserver.Dispose();
            }
        }

        public void NotifyObservers()
        {
            foreach (IObserver observer in observers)
            {
                try
                {
                    observer.UpdateObserver();
                }
                catch (InvalidOperationException)
                {
                    // Catch any exception due to inevitably updating the unique identification of data that was already removed 
                }
            }
        }

        /// <summary>
        /// Gets the list of instances that effect the unique identification.
        /// </summary>
        protected abstract ObservableList<TObservable> LocationCalculationsListToObserve { get; }
    }
}