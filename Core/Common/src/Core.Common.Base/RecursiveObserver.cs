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

namespace Core.Common.Base
{
    /// <summary>
    /// Class that implements <see cref="IObserver"/> in a way that a hierarchy of <typeparamref name="TObservable"/> objects can be observed.
    /// </summary>
    /// <remarks>
    /// The root container (<see cref="Observable"/>) being observed by instances of this class can be dynamically changed.
    /// </remarks>
    /// <typeparam name="TContainer">The type of the item containers that specify the object hierarchy.</typeparam>
    /// <typeparam name="TObservable">The type of items (in the containers) that should be observed.</typeparam>
    public class RecursiveObserver<TContainer, TObservable> : IObserver, IDisposable
        where TContainer : class, IObservable
        where TObservable : class, IObservable
    {
        private readonly Action updateObserverAction;
        private readonly Func<TContainer, IEnumerable<object>> getChildren;
        private readonly List<TContainer> observedContainers = new List<TContainer>();
        private readonly List<TObservable> observedChildren = new List<TObservable>();
        private readonly Observer containerObserver;
        private TContainer rootContainer;

        /// <summary>
        /// Creates a new instance of the <see cref="RecursiveObserver{TContainer,TObservable}"/> class.
        /// </summary>
        /// <param name="updateObserverAction">The <see cref="UpdateObserver"/> action to perform on notifications coming from one of the <typeparamref name="TObservable"/> items of the hierarchy.</param>
        /// <param name="getChildren">The method used for recursively obtaining the children of <typeparamref name="TContainer"/> objects in the hierarchy.</param>
        public RecursiveObserver(Action updateObserverAction, Func<TContainer, IEnumerable<object>> getChildren)
        {
            this.updateObserverAction = updateObserverAction;
            this.getChildren = getChildren;

            // Ensure subscriptions are updated (detach/attach) on changes in the hierarchy
            containerObserver = new Observer(UpdateObservedObjects);
        }

        /// <summary>
        /// Gets or sets the root container.
        /// </summary>
        public TContainer Observable
        {
            get
            {
                return rootContainer;
            }
            set
            {
                rootContainer = value;

                UpdateObservedObjects();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateObserver()
        {
            updateObserverAction();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Observable = null;
            }
        }

        private void UpdateObservedObjects()
        {
            // Detach from the currently observed containers
            foreach (TContainer observedObject in observedContainers)
            {
                observedObject.Detach(containerObserver);
            }

            // Detach from the currently observed children
            foreach (TObservable observedObject in observedChildren)
            {
                observedObject.Detach(this);
            }

            // Clear the lists of observed objects
            observedContainers.Clear();
            observedChildren.Clear();

            // If relevant, start observing objects again
            if (rootContainer != null)
            {
                ObserveObjectsRecursively(rootContainer);
            }
        }

        private void ObserveObjectsRecursively(TContainer container)
        {
            container.Attach(containerObserver);
            observedContainers.Add(container);

            var observable = container as TObservable;
            if (observable != null)
            {
                observable.Attach(this);
                observedChildren.Add(observable);
            }

            foreach (object child in getChildren(container))
            {
                var childContainer = child as TContainer;
                if (childContainer != null)
                {
                    ObserveObjectsRecursively(childContainer);
                }
                else if (child is TObservable)
                {
                    observable = (TObservable) child;
                    observable.Attach(this);
                    observedChildren.Add(observable);
                }
            }
        }
    }
}