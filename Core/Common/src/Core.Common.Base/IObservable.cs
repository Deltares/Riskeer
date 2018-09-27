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

namespace Core.Common.Base
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that are supposed to be observable.
    /// Observables should notify their observers of the fact that their internal state has changed.
    /// </summary>
    /// <seealso cref="IObserver"/>
    public interface IObservable
    {
        /// <summary>
        /// Gets the observers that are attached to the <see cref="IObservable"/>.
        /// </summary>
        IEnumerable<IObserver> Observers { get; }

        /// <summary>
        /// This method attaches <paramref name="observer"/>. As a result, changes in the <see cref="IObservable"/> will now be notified to <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to notify on changes.</param>
        void Attach(IObserver observer);

        /// <summary>
        /// This method detaches <paramref name="observer"/>. As a result, changes in the <see cref="IObservable"/> will no longer be notified to <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to no longer notify on changes.</param>
        void Detach(IObserver observer);

        /// <summary>
        /// This method notifies all observers that have been currently attached to the <see cref="IObservable"/>.
        /// </summary>
        void NotifyObservers();
    }
}