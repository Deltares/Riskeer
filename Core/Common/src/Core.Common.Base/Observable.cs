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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Common.Base
{
    /// <summary>
    /// Class that implements the <see cref="IObservable"/> pattern.
    /// </summary>
    public abstract class Observable : IObservable
    {
        /// <summary>
        /// The observers that are attached.
        /// </summary>
        protected Collection<IObserver> ObserverCollection = new Collection<IObserver>();

        public IEnumerable<IObserver> Observers
        {
            get
            {
                return ObserverCollection;
            }
        }

        public void Attach(IObserver observer)
        {
            ObserverCollection.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            ObserverCollection.Remove(observer);
        }

        public virtual void NotifyObservers()
        {
            // Iterate through a copy of the list of observers; an update of one observer might result in detaching
            // another observer (which will result in a "list modified" exception over here otherwise)
            foreach (IObserver observer in ObserverCollection.ToArray())
            {
                // Ensure the observer is still part of the original list of observers
                if (!ObserverCollection.Contains(observer))
                {
                    continue;
                }

                observer.UpdateObserver();
            }
        }
    }
}